using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace VirtualMachineScreenSaver.Rendering
{
	public class VertexBufferTessellator : BaseTessellator
	{
		#region Fields

		private Dictionary<int, List<VertexBufferElement>> _requests;

		#endregion

		#region Constructors

		public VertexBufferTessellator()
			: base()
		{
			GeneratedVBOs = new List<VertexBufferObject>();
			_requests = new Dictionary<int, List<VertexBufferElement>>();
		}

		#endregion

		#region Properties

		public List<VertexBufferObject> GeneratedVBOs { get; private set; }

		#endregion

		#region Methods

		public override void Begin(PrimitiveType primitiveType)
		{
			base.Begin(primitiveType);

			if ((primitiveType != PrimitiveType.Triangles) && (primitiveType != PrimitiveType.Quads))
			{
				throw new ArgumentException("Invalid value.", "primitiveType");
			}

			GeneratedVBOs.Clear();
			_requests.Clear();
		}

		public override void End()
		{
			foreach (var request in _requests)
			{
				var vertArray = request.Value.ToArray();
				var elemArray = new short[vertArray.Length];
				var elemIndex = 0;
				for (var n = 0; n < vertArray.Length; n += PointsPerPrimitive)
				{
					for (var p = 0; p < PointsPerPrimitive; p++)
					{
						elemArray[elemIndex++] = (short)(n + p);
					}
				}

				CalculateNormals(vertArray);

				var vbo = new VertexBufferObject(request.Key, vertArray, elemArray);
				vbo.Render(PrimitiveType);
				vbo.Dispose();
			}
		}

		protected override void AddTransformedPoint(Vector3 transformedPoint, float u, float v)
		{
			if (!_requests.ContainsKey(CurrentTextureID))
			{
				_requests.Add(CurrentTextureID, new List<VertexBufferElement>());
			}
			_requests[CurrentTextureID].Add(new VertexBufferElement(transformedPoint, ColorHelper.ToArgb(CurrentColor), u, v, Vector3.Zero));
		}

		/// <remarks>
		/// Based on the algorithm described here: http://www.opengl.org/wiki/Calculating_a_Surface_Normal
		/// </remarks>
		private void CalculateNormals(VertexBufferElement[] vertices)
		{
			for (var n = 0; n < vertices.Length; n += PointsPerPrimitive)
			{
				var i1 = n + 0;
				var i2 = n + 1;
				var i3 = n + 2;
				//var i4 = n + 3;

				var p1 = vertices[i1].Position;
				var p2 = vertices[i2].Position;
				var p3 = vertices[i3].Position;

				var U = p2 - p1;
				var V = p3 - p1;

				var normal = Vector3.Cross(U, V);
				for (var p = 0; p < PointsPerPrimitive; p++)
				{
					vertices[n + p].Normal = normal;
				}
			}
		}

		#endregion
	}
}