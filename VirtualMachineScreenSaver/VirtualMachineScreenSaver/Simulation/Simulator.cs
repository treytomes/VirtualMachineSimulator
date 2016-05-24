using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualMachineScreenSaver.Simulation
{


	public class Simulator
	{
		#region Constants

		private const int ERROR_VALUE = int.MaxValue;

		#endregion

		#region Fields

		private static Random _random;

		private Cell[] _memory;

		private List<EffectBase> _effects;
		private List<Thread> _threads;
		private int _progress;

		#endregion

		#region Constructors

		static Simulator()
		{
			_random = new Random();
		}

		public Simulator(int rows, int columns)
		{
			Rows = rows;
			Columns = columns;
			_memory = new Cell[rows * columns];

			_threads = new List<Thread>();
			_effects = new List<EffectBase>();
			_progress = 0;

			if (AppSettings.Instance.RandomizeMemoryOnInitialize)
			{
				RandomizeMemory();
			}
			else
			{
				ClearMemory();
			}

			IsDirty = true;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Does the simulator need to be re-rendered?
		/// </summary>
		/// <remarks>
		/// This is marked true when a cell is modified, and false when rendering is complete.
		/// </remarks>
		public bool IsDirty { get; set; }

		public Cell this[int index]
		{
			get
			{
				return _memory[index];
			}
			set
			{
				_memory[index] = value;
			}
		}

		public int Rows { get; private set; }

		public int Columns { get; private set; }

		public IEnumerable<EffectBase> Effects
		{
			get
			{
				foreach (var effect in _effects)
				{
					yield return effect;
				}
				yield break;
			}
		}

		private bool AllowReverse
		{
			get
			{
				if (AppSettings.Instance.ReverseEnabled)
				{
					return _random.Next(2) == 1;
				}
				else
				{
					return false;
				}
			}
		}

		private int TotalSlice
		{
			get
			{
				return AppSettings.Instance.CopySlice + AppSettings.Instance.HaltSlice + AppSettings.Instance.PushSlice + AppSettings.Instance.SpawnSlice;
			}
		}

		#endregion

		#region Methods

		public void Behave(int delta)
		{
			_progress += delta;
			while (_progress >= AppSettings.Instance.MsPerCycle)
			{
				_progress -= AppSettings.Instance.MsPerCycle;
				DoCycle();
			}

			// Remove effects whose lifespan is complete.
			for (var index = 0; index < _effects.Count; index++)
			{
				var effect = _effects[index];

				effect.Progress += delta;
				if (effect.Progress > effect.LifeSpan)
				{
					_effects.Remove(effect);
					index--;
				}
			}
		}

		private void DoCycle()
		{
			// Add random noise.
			for (var index = 0; index < AppSettings.Instance.RandomNoisePerCycle; index++)
			{
				SetRandomOperator(_random.Next(_memory.Length));
			}

			// Start random threads.
			for (var index = 0; index < AppSettings.Instance.NewThreadsPerCycle; index++)
			{
				Spawn(_random.Next(_memory.Length));
			}

			for (var index = 0; index < _threads.Count; index++)
			{
				var thread = _threads[index];
				thread.Age++;

				// Get current operator.
				byte op = _memory[thread.InstructionPointer].Op;

				// Move instruction pointer.
				thread.InstructionPointer += thread.IsReversed ? -1 : 1;

				// Wrap around the edge of memory.
				thread.InstructionPointer = (thread.InstructionPointer + _memory.Length) % _memory.Length;

				// Handle the operator.
				switch (op)
				{
					// Performs a copy.
					case (byte)Operations.COPY:
						if (!ExecuteCOPY(thread))
						{
							index--;
						}
						break;

					// Halt the current thread.
					case (byte)Operations.HALT:
						KillThread(thread);
						index--;
						break;

					// Spawn a new thread.
					case (byte)Operations.SPAWN:
						if (!ExecuteSPAWN(thread))
						{
							index--;
						}
						break;

					// Push a value onto the stack.
					default:
						var pushValue = op - (byte)Operations.PUSH;
						// Push the argument on the stack, halt if the stack is full.
						if (Push(thread, pushValue) == ERROR_VALUE)
						{
							KillThread(thread);
							index--;
						}
						break;
				}
			}

			while (_threads.Count > AppSettings.Instance.MaxThreadCount)
			{
				if (AppSettings.Instance.ThreadReductionEnabled) // kill a random thread
				{
					KillThread(_threads[_random.Next(_threads.Count)]);
				}
				else // kill the oldest thread
				{
					KillThread(_threads.First(x => x.Age == _threads.Max(y => y.Age)));
				}
			}

			foreach (var thread in _threads)
			{
				_memory[thread.InstructionPointer].Modified = 2;
			}
		}

		/// <remarks>
		/// If the operation fails, the executing thread should halt.
		/// </remarks>
		/// <returns>Did the operation succeed?</returns>
		private bool ExecuteCOPY(Thread thread)
		{
			var sourceIP = 0;
			var destinationIP = 0;
			var copyLength = 0;

			if ((sourceIP = Pop(thread)) == ERROR_VALUE) // Pop the first argument off the stack, halt if the stack is empty.
			{
				KillThread(thread);
				return false;
			}
			else if ((destinationIP = Pop(thread)) == ERROR_VALUE) // Pop the second argument off the stack, halt if the stack is empty.
			{
				KillThread(thread);
				return false;
			}
			else if ((copyLength = Pop(thread)) == ERROR_VALUE) // Pop the third argument off the stack, halt if the stack is empty.
			{
				KillThread(thread);
				return false;
			}
			else
			{
				// Calculate locations
				sourceIP = (thread.InstructionPointer + sourceIP + _memory.Length) % _memory.Length;
				destinationIP = (thread.InstructionPointer + destinationIP + _memory.Length) % _memory.Length;

				var direction = thread.IsReversed ? -1 : 1;

				// Copy copyLength operators from arg to destinationIP.
				for (var copyIndex = 0; copyIndex < copyLength; copyIndex++)
				{
					var offset = copyIndex * direction + _memory.Length;
					var destinationOp = _memory[(destinationIP + offset) % _memory.Length].Op;
					var sourceOp = _memory[(sourceIP + offset) % _memory.Length].Op;

					SetOperator((destinationIP + offset) % _memory.Length, sourceOp);

					if (AppSettings.Instance.UseCopyEffect)
					{
						AddEffect(new ZoomEffect((destinationIP + offset) % _memory.Length));
					}

					if (destinationOp != sourceOp)
					{
						_memory[(destinationIP + offset) % _memory.Length].Modified = 1;
						IsDirty = true;
					}
				}
			}

			return true;
		}

		/// <remarks>
		/// If the operation fails, the executing thread should halt.
		/// </remarks>
		/// <returns>Did the operation succeed?</returns>
		private bool ExecuteSPAWN(Thread thread)
		{
			// Pop the first argument off the stack, halt if the stack is empty.
			var offset = Pop(thread);
			if (offset == ERROR_VALUE)
			{
				KillThread(thread);
				return false;
			}
			else
			{
				// Calculate where the new thread will start.
				offset = (thread.InstructionPointer + offset + _memory.Length) % _memory.Length;

				// Begin execution of the new thread.
				if (_memory[thread.InstructionPointer].Lifespan > 0)
				{
					_memory[thread.InstructionPointer].Lifespan--;
					if (_memory[thread.InstructionPointer].Lifespan <= 0)
					{
						SetOperator(thread.InstructionPointer, (byte)Operations.HALT);
						if (AppSettings.Instance.UseSpawnExpireEffect)
						{
							AddEffect(new ZoomEffect(thread.InstructionPointer));
						}
					}
				}
			}

			return true;
		}

		private void RandomizeMemory()
		{
			for (int cellIndex = 0; cellIndex < _memory.Length; cellIndex++)
			{
				SetRandomOperator(cellIndex);
			}
		}

		private void ClearMemory()
		{
			for (var index = 0; index < _memory.Length; index++)
			{
				SetOperator(index, (byte)Operations.HALT);
			}
		}

		private void SetRandomOperator(int cellIndex)
		{
			if (TotalSlice == 0)
			{
				return;
			}

			var slice = _random.Next(TotalSlice);
			if (slice < AppSettings.Instance.CopySlice)
			{
				SetOperator(cellIndex, (byte)Operations.COPY);
			}
			else if ((slice -= AppSettings.Instance.CopySlice) < AppSettings.Instance.HaltSlice)
			{
				SetOperator(cellIndex, (byte)Operations.HALT);
			}
			else if (slice - AppSettings.Instance.HaltSlice < AppSettings.Instance.PushSlice)
			{
				SetOperator(cellIndex, (byte)(Operations.PUSH + 11 - _random.Next(23)));
			}
			else
			{
				SetOperator(cellIndex, (byte)Operations.SPAWN);
			}
		}

		private void SetOperator(int index, byte op)
		{
			_memory[index].Op = op;
			_memory[index].Modified = 1;
			if (op == (byte)Operations.SPAWN)
			{
				_memory[index].Lifespan = AppSettings.Instance.SpawnLifeSpan;
			}
			else
			{
				_memory[index].Lifespan = 0;
			}

			IsDirty = true;
		}

		private int Push(Thread thread, int value)
		{
			if (thread.StackTop == AppSettings.Instance.MaxStackSize)
			{
				return ERROR_VALUE;
			}
			else
			{
				thread.Stack[thread.StackTop++] = value;
				return 1;
			}
		}

		private int Pop(Thread thread)
		{
			if (thread.StackTop == 0)
			{
				return ERROR_VALUE;
			}
			else
			{
				return thread.Stack[--thread.StackTop];
			}
		}

		private void Spawn(int ip)
		{
			_threads.Insert(0, new Thread(ip, AllowReverse));
		}

		private void AddEffect(EffectBase effect)
		{
			if (_effects.Any(x => x.Equals(effect)))
			{
				return;
			}

			_effects.Insert(0, effect);
		}

		private void KillThread(Thread thread)
		{
			if (AppSettings.Instance.UseThreadDeathEffect)
			{
				if (thread.IsReversed)
				{
					AddEffect(new FallReverseEffect(thread.InstructionPointer));
				}
				else
				{
					AddEffect(new FallForwardEffect(thread.InstructionPointer));
				}

				AddEffect(new PulseEffect(thread.InstructionPointer));
			}

			_threads.Remove(thread);
		}

		#endregion
	}
}
