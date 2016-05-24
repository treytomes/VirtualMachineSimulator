using OpenTK;
using System.Runtime.InteropServices;

namespace VirtualMachineScreenSaver.Rendering
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VertexBufferElement
	{
		public Vector3 Position;
		public int Color;
		public Vector2 TextureUV;
		public Vector3 Normal;

		public VertexBufferElement(Vector3 position, int color, float u, float v, Vector3 normal)
		{
			Position = position;
			Color = color;
			TextureUV = new Vector2(u, v);
			Normal = normal;
		}

		public VertexBufferElement(Vector3 position, int color, Vector3 normal)
			: this(position, color, 0, 0, normal)
		{
		}
	}
}