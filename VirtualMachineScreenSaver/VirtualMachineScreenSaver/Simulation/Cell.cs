namespace VirtualMachineScreenSaver.Simulation
{
	/// <summary>
	/// A single cell of memory.
	/// </summary>
	public class Cell
	{
		/// <summary>
		/// The operation code.
		/// </summary>
		public byte Op;

		/// <summary>
		/// Was this cell modified?  0 = not modified; higher numbers will leave the cell highlighted longer.
		/// </summary>
		public int Modified;

		/// <summary>
		/// For SPAWN operations, how many cycles until the cell reverts to a HALT.
		/// </summary>
		public int Lifespan;
	}
}