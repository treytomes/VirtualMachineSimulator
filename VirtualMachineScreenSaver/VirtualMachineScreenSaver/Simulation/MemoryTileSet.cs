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
				case (int)Operations.OP_COPY:
					row = 12;
					break;
				case (int)Operations.OP_SPAWN:
					row = 14;
					break;
				case (int)Operations.OP_HALT:
					row = 15;
					break;
				default:
					row = op - (int)Operations.OP_PUSH;
					if (row < 0)
					{
						row *= -1;
						column = 1;
					}
					break;
			}

			if (modified == 0)
			{
				column += 2;
			}

			return (row * 4) + column;
		}
	}
}
