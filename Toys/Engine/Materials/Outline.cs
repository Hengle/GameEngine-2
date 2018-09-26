using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Toys
{
	//default outline class
	public class Outline: IOutline
	{
		Shader outline;
		public Vector4 EdgeColour { get; set; }
		public float EdgeScaler { get; set; }
		public bool hasEdge { get; set; }

		public Outline()
		{
			outline = ShaderManager.GetInstance.GetShader("outline");
			hasEdge = true;
			EdgeScaler = 1f;
			EdgeColour = new Vector4(0);
		}

		public void ApplyOutline()
		{
			outline.SetUniform(EdgeColour, "EdgeColor");
			outline.SetUniform(EdgeScaler, "EdgeScaler");
		}
	}
}
