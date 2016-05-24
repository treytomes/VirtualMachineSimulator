using OpenTK;
using System;
using System.Linq;

namespace VirtualMachineScreenSaver.Rendering
{
	public class TileSet
	{
		public class Tile
		{
			#region Constructors

			public Tile(string name, float uLeft, float uRight, float vTop, float vBottom)
			{
				Name = name;
				ULeft = uLeft;
				URight = uRight;
				VTop = vTop;
				VBottom = vBottom;
			}

			#endregion

			#region Properties

			public string Name { get; internal set; }

			public float ULeft { get; private set; }
			public float URight { get; private set; }
			public float VTop { get; private set; }
			public float VBottom { get; private set; }

			#endregion
		}

		#region Fields

		private Texture2D _texture;
		private int _rows;
		private int _columns;
		private Tile[] _tiles;

		#endregion

		#region Constructors

		public TileSet(Texture2D texture, int rows, int columns)
		{
			Initialize(texture, rows, columns);
		}

		protected TileSet()
		{
		}

		#endregion

		#region Properties

		public int Count
		{
			get
			{
				return _rows * _columns;
			}
		}

		public int Width { get; private set; }

		public int Height { get; private set; }

		/// <summary>
		/// Should the tiles be drawn with a width and height of 1?  Or of their pixel height?
		/// </summary>
		/// <remarks>
		/// Defaults to True.
		/// </remarks>
		public bool IsNormalized { get; set; }

		#endregion

		#region Methods

		public void Render(ITessellator tessellator, int tileIndex, bool mirrorX = false, bool mirrorY = false)
		{
			var tile = _tiles[tileIndex];

			var minU = mirrorX ? tile.URight : tile.ULeft;
			var maxU = mirrorX ? tile.ULeft : tile.URight;
			var minV = mirrorY ? tile.VBottom : tile.VTop;
			var maxV = mirrorY ? tile.VTop : tile.VBottom;

			var width = IsNormalized ? 1 : Width;
			var height = IsNormalized ? 1 : Height;

			tessellator.BindTexture(_texture);
			tessellator.AddPoint(0, 0, minU, minV);
			tessellator.AddPoint(0, height, minU, maxV);
			tessellator.AddPoint(width, height, maxU, maxV);
			tessellator.AddPoint(width, 0, maxU, minV);
		}

		public void RenderText(ITessellator tessellator, string format, params object[] args)
		{
			var unitX = tessellator.Transform(Vector2.UnitX * (IsNormalized ? 1 : Width)) - tessellator.Transform(Vector2.Zero);

			format = string.Format(format, args);
			for (var index = 0; index < format.Length; index++)
			{
				Render(tessellator, format[index]);
				tessellator.Translate(unitX);
			}

			tessellator.Translate(-unitX * format.Length);
		}

		public int GetTileIndexFromName(string name)
		{
			return Array.IndexOf(_tiles, _tiles.Single(x => x.Name == name));
		}

		public string GetNameFromTileIndex(int tileIndex)
		{
			return _tiles[tileIndex].Name;
		}

		protected void Initialize(Texture2D texture, int rows, int columns)
		{
			if (rows <= 0)
			{
				throw new ArgumentException("Value must be greater than 0.", "rows");
			}
			if (columns <= 0)
			{
				throw new ArgumentException("Value must be greater than 0.", "columns");
			}

			_texture = texture;
			_rows = rows;
			_columns = columns;

			Width = _texture.Width / columns;
			Height = _texture.Height / rows;

			_tiles = new Tile[Count];
			for (var n = 0; n < Count; n++)
			{
				float x = (n % _columns) * Width;
				float y = (n / _columns) * Height;

				float texLeft = x / _texture.Width;
				float texTop = y / _texture.Height;
				float texRight = (x + Width) / _texture.Width;
				float texBottom = (y + Height) / _texture.Height;

				_tiles[n] = new Tile(n.ToString(), texLeft, texRight, texTop, texBottom);
			}

			IsNormalized = true;
		}

		protected void SetTileName(int tileIndex, string name)
		{
			_tiles[tileIndex].Name = name;
		}

		#endregion
	}
}