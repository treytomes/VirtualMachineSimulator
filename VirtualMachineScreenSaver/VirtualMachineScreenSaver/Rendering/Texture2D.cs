using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace VirtualMachineScreenSaver.Rendering
{
	public class Texture2D : IGraphicsResource, IEquatable<IGraphicsResource>
	{
		#region Constants

		private const TextureMagFilter DEFAULT_FILTER_MAG = TextureMagFilter.Linear;
		private const TextureMinFilter DEFAULT_FILTER_MIN = TextureMinFilter.Linear;

		#endregion

		#region Fields

		private IGraphicsContext _context;
		private int _id;
		private int _width;
		private int _height;
		private bool _disposed;

		private TextureMagFilter _magFilter;
		private TextureMinFilter _minFilter;

		#endregion

		#region Constructors

		public Texture2D(int width, int height)
		{
			if (width <= 0)
			{
				throw new ArgumentException("Width must be greater than 0.", "width");
			}
			if (height <= 0)
			{
				throw new ArgumentException("Height must be greater than 0.", "height");
			}

			_magFilter = DEFAULT_FILTER_MAG;
			_minFilter = DEFAULT_FILTER_MIN;

			Width = width;
			Height = height;
		}

		public Texture2D(Bitmap bitmap)
			: this(bitmap.Width, bitmap.Height)
		{
			WriteRegion(bitmap);
		}

		#endregion

		#region Properties

		/// <summary>Gets the width of the texture.</summary>
		public int Width
		{
			get
			{
				return _width;
			}
			private set
			{
				_width = value;
			}
		}

		/// <summary>Gets the height of the texture.</summary>
		public int Height
		{
			get
			{
				return _height;
			}
			private set
			{
				_height = value;
			}
		}

		public TextureMagFilter MagnificationFilter
		{
			get
			{
				return _magFilter;
			}
			set
			{
				_magFilter = value;
			}
		}

		public TextureMinFilter MinificationFilter
		{
			get
			{
				return _minFilter;
			}
			set
			{
				_minFilter = value;
			}
		}

		IGraphicsContext IGraphicsResource.Context
		{
			get
			{
				return _context;
			}
		}

		int IGraphicsResource.Id
		{
			get
			{
				if (_id == 0)
				{
					GraphicsContext.Assert();
					_context = GraphicsContext.CurrentContext;

					_id = CreateTexture(Width, Height);
				}

				return _id;
			}
		}

		#endregion

		#region Methods

		public void Bind()
		{
			GL.BindTexture(TextureTarget.Texture2D, (this as IGraphicsResource).Id);
		}

		public void WriteRegion(Bitmap bitmap, Rectangle? source = null, Rectangle? target = null, int mipLevel = 0)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			if (mipLevel < 0)
			{
				throw new ArgumentException("Value must be >= 0.", "mipLevel");
			}

			if (source.HasValue)
			{
				var unit = GraphicsUnit.Pixel;
				if (!bitmap.GetBounds(ref unit).Contains(source.Value))
				{
					throw new ArgumentException("The source Rectangle cannot be larger than the bitmap.", "source");
				}
			}

			if (!source.HasValue)
			{
				source = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
			}
			if (!target.HasValue)
			{
				target = new Rectangle(0, 0, Width, Height);
			}

			Bind();

			BitmapData data = null;

			GL.PushClientAttrib(ClientAttribMask.ClientPixelStoreBit);
			try
			{
				data = bitmap.LockBits(source.Value, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
				GL.PixelStore(PixelStoreParameter.UnpackRowLength, bitmap.Width);
				GL.TexSubImage2D(TextureTarget.Texture2D, mipLevel,
					target.Value.Left, target.Value.Top,
					target.Value.Width, target.Value.Height,
					OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
					PixelType.UnsignedByte, data.Scan0);
			}
			finally
			{
				GL.PopClientAttrib();
				if (data != null)
				{
					bitmap.UnlockBits(data);
				}
			}
		}

		public TextureRegion2D ReadRegion(Rectangle rect, int mipLevel)
		{
			if (mipLevel < 0)
			{
				throw new ArgumentOutOfRangeException("miplevel");
			}

			TextureRegion2D<int> region = new TextureRegion2D<int>(rect);

			GL.GetTexImage(TextureTarget.Texture2D, mipLevel, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, region.Data);

			return region;
		}

		private int CreateTexture(int width, int height)
		{
			var id = GL.GenTexture();
			if (id == 0)
			{
				throw new Exception(string.Format("Texture creation failed, (Error: {0})", GL.GetError()));
			}
			SetDefaultTextureParameters(id);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

			return id;
		}

		private void SetDefaultTextureParameters(int id)
		{
			var version = GL.GetInteger(GetPName.MajorVersion) + GL.GetInteger(GetPName.MinorVersion) / 10.0;

			// Ensure the texture is allocated.
			GL.BindTexture(TextureTarget.Texture2D, id);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)MinificationFilter);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)MagnificationFilter);
			if (version >= 1.2)
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
			}
			else
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Clamp);
			}
		}

		public bool Equals(IGraphicsResource other)
		{
			return (this as IGraphicsResource).Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			return (obj is IGraphicsResource) && Equals(obj as IGraphicsResource);
		}

		public override int GetHashCode()
		{
			return (this as IGraphicsResource).Id;
		}

		public override string ToString()
		{
			return string.Format("Texture2D #{0} ({1}x{2}, {3})", (this as IGraphicsResource).Id.ToString(), Width, Height, PixelInternalFormat.Rgba);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					GL.DeleteTexture(_id);
				}
				else
				{
					Debug.Print("[Warning] {0} leaked.", this);
				}
				_disposed = true;
			}
		}

		~Texture2D()
		{
			Dispose(false);
		}

		#endregion
	}
}