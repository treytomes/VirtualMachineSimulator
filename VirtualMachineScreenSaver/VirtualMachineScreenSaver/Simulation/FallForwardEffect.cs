using VirtualMachineScreenSaver.Rendering;

namespace VirtualMachineScreenSaver.Simulation
{
	public class FallForwardEffect : EffectBase
	{
		public FallForwardEffect(int ip)
			: base(1000, ip, 2)
		{
		}

		public override void Apply(Simulator sim, ITessellator tessellator, MemoryTileSet tiles)
		{
			var tileIndex = MemoryTileSet.GetTileIndex(sim[InstructionPointer].op, 1);
			var blend = 1.0f - (Progress / (float)LifeSpan);

			var x = InstructionPointer % sim.Columns;
			var y = InstructionPointer / sim.Columns;

			tessellator.BindColor(1.0f, 1.0f, 1.0f, blend);
			tessellator.PushTransform();

			tessellator.Translate(x * tiles.Width + 0.5f + 256.0f * Progress / LifeSpan, y * tiles.Height + 0.5f + 768.0f / 4.0f * (Progress * Progress) / (LifeSpan * LifeSpan));

			tiles.Render(tessellator, tileIndex);
			tessellator.PopTransform();
		}
	}
}
