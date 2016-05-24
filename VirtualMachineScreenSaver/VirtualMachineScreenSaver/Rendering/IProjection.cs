using OpenTK;

namespace VirtualMachineScreenSaver.Rendering
{
	public interface IProjection
	{
		Viewport Viewport { get; }
		Matrix4 ProjectionMatrix { get; }

		void Resize(Viewport viewport);
		void Apply();

		bool Contains(float x, float y, float z = 0);
	}
}