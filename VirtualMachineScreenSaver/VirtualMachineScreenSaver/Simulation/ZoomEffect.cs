using VirtualMachineScreenSaver.Rendering;

namespace VirtualMachineScreenSaver.Simulation
{
	public class ZoomEffect : EffectBase
	{
		public ZoomEffect(int index)
			: base(500, index, 3)
		{
		}

		public override void Apply(Simulator sim, ITessellator tessellator, MemoryTileSet tiles)
		{
			var tileIndex = MemoryTileSet.GetTileIndex(sim[InstructionPointer].Op, 1);
			var blend = 1.0f - (Progress / (float)LifeSpan); // + 0.5f;
			var zoom = 1.5f * (1.0f - (Progress / (LifeSpan * 2.0f)));

			var x = InstructionPointer % sim.Columns;
			var y = InstructionPointer / sim.Columns;

			tessellator.BindColor(1.0f, 1.0f, 1.0f, blend);
			tessellator.PushTransform();
			tessellator.Scale(zoom, zoom);
			tessellator.Translate(tiles.Width * (x + (1 - zoom) / 2.0f), tiles.Height * (y + (1 - zoom) / 2.0f));

			tiles.Render(tessellator, tileIndex);
			tessellator.PopTransform();
		}
	}
}
