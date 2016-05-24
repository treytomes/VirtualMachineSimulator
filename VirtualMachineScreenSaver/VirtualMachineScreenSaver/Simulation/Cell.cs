namespace VirtualMachineScreenSaver.Simulation
{
	/// <summary>
	/// A single cell of memory.
	/// </summary>
	public struct Cell
	{
		/// <summary>
		/// The operation code.
		/// </summary>
		public byte Op;

		public int Modified;

		/// <summary>
		/// For SPAWN operations, how many cycles until the cell reverts to a HALT.
		/// </summary>
		public int Lifespan;
	}
}