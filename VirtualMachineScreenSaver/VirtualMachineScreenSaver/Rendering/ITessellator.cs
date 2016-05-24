using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace VirtualMachineScreenSaver.Rendering
{
	public interface ITessellator : ICamera
	{
		PrimitiveType PrimitiveType { get; }
		Color CurrentColor { get; }

		void Begin(PrimitiveType primitiveType);
		void End();

		void BindTexture(Texture2D texture);
		void BindColor(Color color);

		/// <summary>
		/// Bind an integer color in the format of 0xRRGGBB.
		/// </summary>
		void BindColor(int color);

		void BindColor(byte red, byte green, byte blue, byte alpha = 255);
		void BindColor(float red, float green, float blue, float alpha = 1.0f);

		void AddPoint(float x, float y, float z, float u, float v);
		void AddPoint(float x, float y, float z);

		void AddPoint(float x, float y, float u, float v);
		void AddPoint(float x, float y);
	}
}