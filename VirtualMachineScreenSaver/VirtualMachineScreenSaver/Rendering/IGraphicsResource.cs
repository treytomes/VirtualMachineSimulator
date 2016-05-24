using OpenTK.Graphics;
using System;

namespace VirtualMachineScreenSaver.Rendering
{
	/// <summary>
	/// Defines a common interface to all OpenGL resources.
	/// </summary>
	public interface IGraphicsResource : IDisposable
	{
		/// <summary>
		/// Gets the GraphicsContext that owns this resource.
		/// </summary>
		IGraphicsContext Context { get; }

		/// <summary>
		/// Gets the Id of this IGraphicsResource.
		/// </summary>
		int Id { get; }
	}
}