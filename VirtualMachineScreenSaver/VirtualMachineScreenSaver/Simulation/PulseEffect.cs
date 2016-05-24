using VirtualMachineScreenSaver.Rendering;

namespace VirtualMachineScreenSaver.Simulation
{
	public class PulseEffect : EffectBase
	{
		public PulseEffect(int ip)
			: base(1000, ip, 1)
		{
		}

		public override void Apply(Simulator sim, ITessellator tessellator, MemoryTileSet tiles)
		{
			var tileIndex = MemoryTileSet.GetTileIndex(sim[InstructionPointer].Op, sim[InstructionPointer].Modified);
			var blend = Progress / (float)LifeSpan;

			var x = InstructionPointer % sim.Columns;
			var y = InstructionPointer / sim.Columns;

			tessellator.BindColor(1.0f, 1.0f, 1.0f, blend);
			tessellator.PushTransform();
			tessellator.Translate(x * tiles.Width, y * tiles.Height);
			tiles.Render(tessellator, tileIndex);
			tessellator.PopTransform();
		}
	}
}
