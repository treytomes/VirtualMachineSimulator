using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace VirtualMachineScreenSaver.Rendering
{
	public class Camera
	{
		public static Camera<OrthographicProjection> CreateOrthographicCamera(int width, int height)
		{
			return CreateOrthographicCamera(new Viewport(0, 0, width, height));
		}

		public static Camera<OrthographicProjection> CreateOrthographicCamera(Viewport viewport)
		{
			return new Camera<OrthographicProjection>(new OrthographicProjection(viewport));
		}
	}

	public class Camera<TProjection>
		where TProjection : IProjection
	{
		#region Constructors

		public Camera(TProjection projection)
		{
			Projection = projection;

			Eye = new Vector3(0, 0, 0);
			Forward = Vector3.UnitZ;
			Up = Vector3.UnitY;
		}

		#endregion

		#region Properties

		public Viewport Viewport
		{
			get
			{
				return Projection.Viewport;
			}
		}

		public Vector3 Eye { get; set; }

		public Vector3 Forward { get; set; }

		public Vector3 Up { get; set; }

		public Vector3 Target
		{
			get
			{
				return Vector3.Subtract(Eye, Forward);
			}
		}

		public TProjection Projection { get; private set; }

		public Matrix4 ModelViewMatrix
		{
			get
			{
				return Matrix4.LookAt(Eye, Target, Up);
			}
		}

		#endregion

		#region Methods

		public static Vector2 UnProject(Matrix4 projection, Matrix4 view, Size viewport, Vector2 position)
		{
			Vector4 vec;

			vec.X = 2.0f * position.X / (float)viewport.Width - 1;
			vec.Y = -(2.0f * position.Y / (float)viewport.Height - 1);
			vec.Z = 0;
			vec.W = 1.0f;

			var viewInv = Matrix4.Invert(view);
			var projInv = Matrix4.Invert(projection);

			vec = Vector4.Transform(vec, projInv);
			vec = Vector4.Transform(vec, viewInv);

			//Vector4.Transform(ref vec, ref projInv, out vec);
			//Vector4.Transform(ref vec, ref viewInv, out vec);

			// I'm not sure why this is here.  It might be important.
			if ((vec.W > float.Epsilon) || (vec.W < float.Epsilon))
			{
				vec.X /= vec.W;
				vec.Y /= vec.W;
				//vec.Z /= vec.W; // the z-coordinate doesn't make sense in this context
			}

			return new Vector2(vec.X, vec.Y);
		}

		public Vector2 UnProject(Vector2 position)
		{
			return UnProject(Projection.ProjectionMatrix, ModelViewMatrix, Viewport.Size, position);
		}

		public Vector2 UnProject(float x, float y)
		{
			return UnProject(new Vector2(x, y));
		}

		public void MoveTo(Vector3 position)
		{
			Eye = position;
		}

		public void MoveTo(Vector2 position)
		{
			Eye = new Vector3(position.X, position.Y, Eye.Z);
		}

		public void MoveBy(Vector3 amount)
		{
			Eye = Vector3.Add(Eye, amount);
		}

		public void MoveBy(Vector2 amount)
		{
			MoveBy(new Vector3(amount));
		}

		public void Resize(Viewport viewport)
		{
			Projection.Resize(viewport);
		}

		public void Apply()
		{
			Projection.Apply();

			var lookAt = ModelViewMatrix;
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref lookAt);
		}

		#endregion
	}
}
