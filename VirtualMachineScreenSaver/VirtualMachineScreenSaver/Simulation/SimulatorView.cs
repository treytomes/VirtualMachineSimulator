using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Linq;
using VirtualMachineScreenSaver.Rendering;

namespace VirtualMachineScreenSaver.Simulation
{

	public class SimulatorView
	{
		#region Fields

		private Camera<OrthographicProjection> _camera;
		private ITessellator _tessellator;
		private MemoryTileSet _tiles;

		#endregion

		#region Constructors

		public SimulatorView(int width, int height)
		{
			var viewport = new Viewport(0, 0, width, height);

			_camera = Camera.CreateOrthographicCamera(viewport);
			_camera.Projection.OrthographicSize = viewport.Height / 2;
			_camera.MoveBy(new Vector2(width, height) / 2);

			_tessellator = new VertexBufferTessellator();
		}

		#endregion

		#region Methods

		public void LoadContent()
		{
			_tiles = new MemoryTileSet();
		}

		public void Resize(int width, int height)
		{
			_camera.Resize(new Viewport(0, 0, width, height));
		}

		public void Render(Simulator sim)
		{
			if (sim.IsDirty)
			{
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				_camera.Apply();

				_tessellator.LoadIdentity();
				_tessellator.BindColor(Color.White);
				_tessellator.Begin(PrimitiveType.Quads);

				_tiles.Render(_tessellator, 5);
				for (int y = 0, index = 0; y < sim.Rows; y++)
				{
					for (var x = 0; x < sim.Columns; x++, index++)
					{
						var tileIndex = MemoryTileSet.GetTileIndex(sim[index].Op, sim[index].Modified);

						_tessellator.PushTransform();
						_tessellator.Translate(x * _tiles.Width, y * _tiles.Height);
						_tiles.Render(_tessellator, tileIndex);
						_tessellator.PopTransform();

						if (sim[index].Modified > 0)
						{
							var cell = sim[index];
							cell.Modified--;
							sim[index] = cell;
						}
					}
				}

				foreach (var effect in sim.Effects.OrderBy(x => x.RenderOrder))
				{
					effect.Apply(sim, _tessellator, _tiles);
				}

				_tessellator.End();

				sim.IsDirty = false;
			}
		}

		#endregion
	}
}
