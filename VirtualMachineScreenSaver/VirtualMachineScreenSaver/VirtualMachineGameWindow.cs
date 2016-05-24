using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using VirtualMachineScreenSaver.Properties;
using VirtualMachineScreenSaver.Simulation;

namespace VirtualMachineScreenSaver
{
	public class VirtualMachineGameWindow : GameWindow
	{
		#region Fields

		private Simulator _simulator;
		private SimulatorView _view;

		#endregion

		#region Constructors

		public VirtualMachineGameWindow(int width = 1287, int height = 726)
			: base(width, height, new GraphicsMode(new ColorFormat(32), 1, 0, 4, new ColorFormat(32), 2), "Virtual Machine")
		{
			Icon = Resources.Icon;

			Load += VirtualMachineGameWindow_Load;
			Resize += VirtualMachineGameWindow_Resize;
			UpdateFrame += VirtualMachineGameWindow_UpdateFrame;
			RenderFrame += VirtualMachineGameWindow_RenderFrame;

			_simulator = new Simulator(height / 22, width / 11);
			_view = new SimulatorView(width, height);
		}

		#endregion

		#region Event Handlers

		private void VirtualMachineGameWindow_Load(object sender, EventArgs e)
		{
			VSync = VSyncMode.Adaptive;

			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Lequal);

			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);

			GL.Enable(EnableCap.Texture2D);

			GL.ClearColor(Color.Black);

			_view.LoadContent();
		}

		private void VirtualMachineGameWindow_Resize(object sender, EventArgs e)
		{
			_view.Resize(Width, Height);
		}

		private void VirtualMachineGameWindow_UpdateFrame(object sender, FrameEventArgs e)
		{
			if (Focused)
			{
				_simulator.Behave((int)(e.Time * 1000));
			}
		}

		private void VirtualMachineGameWindow_RenderFrame(object sender, FrameEventArgs e)
		{
			_view.Render(_simulator);

			SwapBuffers();
		}

		#endregion
	}
}
