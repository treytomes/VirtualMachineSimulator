using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace VirtualMachineScreenSaver.Rendering
{
	public class OrthographicProjection : IProjection
	{
		#region Constants

		private const float DEFAULT_ZNEAR = 100;
		private const float DEFAULT_ZFAR = -100;

		#endregion

		#region Fields

		private Matrix4 _projection;
		private float _orthographicSize;

		#endregion

		#region Constructors

		public OrthographicProjection(Viewport viewport)
		{
			Resize(viewport);

			//Left = Viewport.Left;
			////Right = Viewport.Right;
			//Top = Viewport.Top;
			////Bottom = Viewport.Bottom;

			ZNear = DEFAULT_ZNEAR;
			ZFar = DEFAULT_ZFAR;

			OrthographicSize = 12;

			_projection = Matrix4.Identity;
		}

		#endregion

		#region Properties

		public Viewport Viewport { get; private set; }

		public float OrthographicSize
		{
			get
			{
				return _orthographicSize;
			}
			set
			{
				_orthographicSize = value;

				// I chose "1" arbitrarily.  It just needs to stay > 0.
				if (_orthographicSize < 1)
				{
					_orthographicSize = 1;
				}
			}
		}

		public float Left
		{
			get
			{
				return -OrthographicSize * Viewport.AspectRatio;
			}
		}

		public float Right
		{
			get
			{
				return OrthographicSize * Viewport.AspectRatio;
			}
		}

		public float Top
		{
			get
			{
				return -OrthographicSize;
			}
		}

		public float Bottom
		{
			get
			{
				return OrthographicSize;
			}
		}

		/// <summary>
		/// The distance to the near clipping plane.
		/// </summary>
		public float ZNear { get; set; }

		/// <summary>
		/// The distance to the far clipping plane.
		/// </summary>
		public float ZFar { get; set; }

		public Matrix4 ProjectionMatrix
		{
			get
			{
				return _projection;
			}
		}

		#endregion

		#region Methods

		public void Resize(Viewport viewport)
		{
			Viewport = viewport.Clone();

			//Left = Viewport.Left;
			//Right = Viewport.Right;
			//Top = Viewport.Top;
			//Bottom = Viewport.Bottom;
		}

		public void Apply()
		{
			Viewport.Apply();

			_projection = Matrix4.CreateOrthographicOffCenter(Left, Right, Bottom, Top, ZNear, ZFar);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref _projection);
		}

		public bool Contains(float x, float y, float z = 0)
		{
			return (Left <= x) && (x <= Right) && (Top <= y) && (y <= Bottom);
		}

		//public void MoveBy(Vector2 delta)
		//{
		//	MoveBy(delta.X, delta.Y);
		//}

		//public void MoveBy(Vector3 delta)
		//{
		//	MoveBy(delta.X, delta.Y, delta.Z);
		//}

		//public void MoveBy(float deltaX, float deltaY, float deltaZ = 0)
		//{
		//	Top += deltaY;
		//	//Bottom += deltaY;

		//	Left += deltaX;
		//	//Right += deltaX;

		//	ZNear += deltaZ;
		//	ZFar += deltaZ;
		//}

		//public void MoveTo(Vector2 position)
		//{
		//	MoveTo(position.X, position.Y);
		//}

		//public void MoveTo(Vector3 position)
		//{
		//	MoveTo(position.X, position.Y, position.Z);
		//}

		//public void MoveTo(float x, float y, float z = 0)
		//{
		//	Top = y;
		//	//Bottom = y;

		//	Left = x;
		//	//Right = x;

		//	ZNear = z;
		//	ZFar = z;
		//}

		#endregion
	}
}