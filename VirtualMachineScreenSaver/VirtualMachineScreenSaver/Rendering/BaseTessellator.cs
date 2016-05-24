using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using VirtualMachineScreenSaver.Utility;

namespace VirtualMachineScreenSaver.Rendering
{
	/// <summary>
	/// Common functions used by all tessellators, e.g. the matrix stack calculations.
	/// </summary>
	public abstract class BaseTessellator : ViewCamera, ITessellator
	{
		#region Events

		public event EventHandler<PropertyChangingEventArgs<Texture2D>> TextureChanging;
		public event EventHandler<PropertyChangingEventArgs<Color>> ColorChanging;

		#endregion

		#region Constructors

		public BaseTessellator()
			: base()
		{
			BindTexture(null);
			BindColor(Color.White);
		}

		#endregion

		#region Properties

		public PrimitiveType PrimitiveType { get; private set; }

		public Color CurrentColor { get; private set; }

		protected Texture2D CurrentTexture { get; private set; }

		protected int CurrentTextureID
		{
			get
			{
				if (CurrentTexture == null)
				{
					return 0;
				}
				else
				{
					return (CurrentTexture as IGraphicsResource).Id;
				}
			}
		}

		protected int PointsPerPrimitive
		{
			get
			{
				switch (PrimitiveType)
				{
					case PrimitiveType.Points:
						return 1;
					case PrimitiveType.Lines:
						return 2;
					case PrimitiveType.Triangles:
						return 3;
					case PrimitiveType.Quads:
						return 4;
					default:
						throw new InvalidOperationException();
				}
			}
		}

		#endregion

		#region Methods

		public virtual void Begin(PrimitiveType primitiveType)
		{
			PrimitiveType = primitiveType;
		}

		public abstract void End();

		// TODO: Implement Rx-style events.
		public void BindColor(Color color)
		{
			if (CurrentColor != color)
			{
				if (ColorChanging != null)
				{
					ColorChanging(this, new PropertyChangingEventArgs<Color>("CurrentColor", CurrentColor, color));
				}
				CurrentColor = color;
			}
		}

		/// <summary>
		/// Bind an integer color in the format of 0xAARRGGBB.
		/// </summary>
		public void BindColor(int color)
		{
			var alpha = (color >> 24) & 255;
			var red = (color >> 16) & 255;
			var green = (color >> 8) & 255;
			var blue = color & 255;
			BindColor(Color.FromArgb(alpha, red, green, blue));
		}

		public void BindColor(float red, float green, float blue, float alpha = 1.0f)
		{
			BindColor(Color.FromArgb((int)(alpha * 255), (int)(red * 255), (int)(green * 255), (int)(blue * 255)));
		}

		public void BindColor(byte red, byte green, byte blue, byte alpha = 255)
		{
			BindColor(Color.FromArgb(alpha, red, green, blue));
		}

		public void BindTexture(Texture2D texture)
		{
			if (CurrentTexture != texture)
			{
				if (TextureChanging != null)
				{
					TextureChanging(this, new PropertyChangingEventArgs<Texture2D>("CurrentTexture", CurrentTexture, texture));
				}
				CurrentTexture = texture;
			}
		}

		public void AddPoint(float x, float y, float z, float u, float v)
		{
			var position = Transform(new Vector3(x, y, z));
			AddTransformedPoint(position, u, v);
		}

		public void AddPoint(float x, float y, float z)
		{
			AddPoint(x, y, z, 0, 0);
		}

		public void AddPoint(float x, float y, float u, float v)
		{
			AddPoint(x, y, 0, u, v);
		}

		public void AddPoint(float x, float y)
		{
			AddPoint(x, y, 0, 0, 0);
		}

		protected abstract void AddTransformedPoint(Vector3 transformedVector, float u, float v);

		#endregion
	}
}