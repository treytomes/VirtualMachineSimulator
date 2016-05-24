namespace VirtualMachineScreenSaver.Simulation
{
	public class Thread
	{
		public Thread(int ip, bool isReversed)
		{
			InstructionPointer = ip;
			Stack = new int[AppSettings.Instance.MaxStackSize];
			StackTop = 0;
			Age = 0;
			IsReversed = isReversed;
		}

		public int[] Stack { get; private set; }

		public bool IsReversed { get; private set; }

		public int InstructionPointer { get; set; }

		public int Age { get; set; }

		public int StackTop { get; set; }
	}
}