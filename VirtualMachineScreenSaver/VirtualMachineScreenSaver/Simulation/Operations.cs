namespace VirtualMachineScreenSaver.Simulation
{
	/// <summary>
	/// Define the operation code values.
	/// </summary>
	public enum Operations
	{
		COPY = 1,
		HALT = 2,
		SPAWN = 3,
		PUSH = byte.MaxValue / 2
	}
}