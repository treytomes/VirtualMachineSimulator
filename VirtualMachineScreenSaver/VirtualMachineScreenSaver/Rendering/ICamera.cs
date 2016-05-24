using OpenTK;

namespace VirtualMachineScreenSaver.Rendering
{
	public interface ICamera
	{
		Matrix4 Transformation { get; }

		void PushTransform();
		void PopTransform();

		void LoadIdentity();
		void Rotate(float angle, float x, float y, float z);
		void Scale(float x, float y);
		void Scale(float x, float y, float z);
		void Translate(float x, float y, float z);
		void Translate(float x, float y);
		void Translate(Vector3 position);
		void Translate(Vector2 position);

		/// <summary>
		/// Apply the given transformation matrix to the current transformation.
		/// </summary>
		void Apply(Matrix4 matrix);

		Vector3 Transform(Vector3 vector);
		Vector2 Transform(Vector2 vector);
	}
}