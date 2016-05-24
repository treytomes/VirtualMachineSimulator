using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace VirtualMachineScreenSaver.Rendering
{
	/// <remarks>
	/// To create a VBO:
	/// 1) Generate the buffer handles for the vertex and element buffers.
	/// 2) Bind the vertex buffer handle and upload your vertex data. Check that the buffer was uploaded correctly.
	/// 3) Bind the element buffer handle and upload your element data. Check that the buffer was uploaded correctly.
	/// 
	/// To draw a VBO:
	/// 1) Ensure that the VertexArray client state is enabled.
	/// 2) Bind the vertex and element buffer handles.
	/// 3) Set up the data pointers (vertex, normal, color) according to your vertex format.
	/// 4) Call DrawElements. (Note: the last parameter is an offset into the element buffer and will usually be IntPtr.Zero).
	/// </remarks>
	public class VertexBufferObject : IDisposable
	{
		#region Constants

		private const int SIZEOF_VECTOR3 = sizeof(float) * 3;
		private const int SIZEOF_VECTOR2 = sizeof(float) * 2;

		private const int OFFSET_POSITION = 0;
		private const int OFFSET_COLOR = OFFSET_POSITION + SIZEOF_VECTOR3;
		private const int OFFSET_TEXTUREUV = OFFSET_COLOR + sizeof(uint);
		private const int OFFSET_NORMAL = OFFSET_TEXTUREUV + sizeof(float) * 2;

		#endregion

		#region Fields

		private int _vboID;
		private int _eboID;
		private int _textureID;
		private int _numElements;

		private bool _disposed;

		private VertexBufferElement[] _vertices;
		private short[] _elements;

		private int _stride;

		#endregion

		#region Constructors

		public VertexBufferObject(int textureID, VertexBufferElement[] vertices, short[] elements)
		{
			_disposed = false;

			EnableStates();

			_textureID = textureID;
			_vertices = vertices;
			_elements = elements;
			_stride = BlittableValueType.StrideOf(vertices);

			LoadData();
		}

		~VertexBufferObject()
		{
			Dispose(false);
		}

		#endregion

		#region Methods

		public static void EnableStates()
		{
			GL.EnableClientState(ArrayCap.ColorArray);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.TextureCoordArray);
			GL.EnableClientState(ArrayCap.NormalArray);
		}

		public void Render(PrimitiveType type)
		{
			GL.BindTexture(TextureTarget.Texture2D, _textureID);

			GL.BindBuffer(BufferTarget.ArrayBuffer, _vboID);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboID);

			GL.VertexPointer(3, VertexPointerType.Float, _stride, new IntPtr(OFFSET_POSITION));
			GL.ColorPointer(4, ColorPointerType.UnsignedByte, _stride, new IntPtr(OFFSET_COLOR));
			GL.TexCoordPointer(2, TexCoordPointerType.Float, _stride, new IntPtr(OFFSET_TEXTUREUV));
			GL.NormalPointer(NormalPointerType.Float, _stride, new IntPtr(OFFSET_NORMAL));

			GL.DrawElements(type, _numElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					GL.DeleteBuffer(_vboID);
					GL.DeleteBuffer(_eboID);
				}
				_disposed = true;
			}
		}

		private void LoadData()
		{
			int size;

			_vboID = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vboID);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_vertices.Length * _stride), _vertices, BufferUsageHint.StaticDraw);
			GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
			if (_vertices.Length * _stride != size)
			{
				throw new ApplicationException("Vertex data not uploaded correctly");
			}

			_eboID = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboID);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_elements.Length * sizeof(short)), _elements, BufferUsageHint.StaticDraw);
			GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
			if (_elements.Length * sizeof(short) != size)
			{
				throw new ApplicationException("Element data not uploaded correctly");
			}

			_numElements = _elements.Length;
		}

		#endregion
	}
}