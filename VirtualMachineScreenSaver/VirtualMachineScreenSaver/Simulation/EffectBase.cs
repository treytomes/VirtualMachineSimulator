using System;
using VirtualMachineScreenSaver.Rendering;

namespace VirtualMachineScreenSaver.Simulation
{
	public abstract class EffectBase : IEquatable<EffectBase>
	{
		#region Constructors

		protected EffectBase(int lifespan, int ip, int renderOrder)
		{
			Progress = 0;
			LifeSpan = lifespan;
			InstructionPointer = ip;
			RenderOrder = renderOrder;
		}

		#endregion

		#region Properties

		public int Progress { get; set; }

		public int LifeSpan { get; private set; }

		public int InstructionPointer { get; private set; }

		public int RenderOrder { get; private set; }

		#endregion

		#region Methods

		public abstract void Apply(Simulator sim, ITessellator tessellator, MemoryTileSet tiles);

		public bool Approximates(EffectBase other)
		{
			return (other != null) && (other.InstructionPointer == InstructionPointer) && other.GetType().Equals(GetType()) && (other.Progress < (other.LifeSpan / 2));
		}

		public bool Equals(EffectBase other)
		{
			return (other != null) && other.GetType().Equals(GetType()) && (other.InstructionPointer == InstructionPointer) && (other.Progress == Progress);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as EffectBase);
		}

		public override int GetHashCode()
		{
			return InstructionPointer;
		}

		#endregion
	}
}
