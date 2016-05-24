using VirtualMachineScreenSaver.Properties;
using VirtualMachineScreenSaver.Rendering;

namespace VirtualMachineScreenSaver.Simulation
{
	public class MemoryTileSet : TileSet
	{
		private const int ROWS = 17;
		private const int COLUMN = 4;

		public MemoryTileSet()
			: base(new Texture2D(Resources.wvm), ROWS, COLUMN)
		{
			IsNormalized = false;
		}

		public static int GetTileIndex(int op, int modified)
		{
			var row = 0;
			var column = 0;

			switch (op)
			{
				case (int)Operations.COPY:
					row = 12;
					break;
				case (int)Operations.SPAWN:
					row = 14;
					break;
				case (int)Operations.HALT:
					row = 15;
					break;
				default:
					// Push operations are rendered using the number tiles.
					// The first column is positive number; the second column is negative.

					row = op - (int)Operations.PUSH;
					if (row < 0)
					{
						row *= -1;
						column = 1;
					}
					break;
			}

			if (modified == 0)
			{
				// Use the darker tiles if the cell has not been modified.
				column += 2;
			}

			return (row * 4) + column;
		}
	}
}
