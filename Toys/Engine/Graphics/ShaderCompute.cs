﻿using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Text;

namespace Toys
{
	public class ShaderCompute: Shader
	{
		public ShaderCompute(string shader)
		{
			int computeShader = CompileShader(shader, ShaderType.ComputeShader);

			shaderProgram = GL.CreateProgram();
			GL.AttachShader(shaderProgram, computeShader);
			GL.LinkProgram(shaderProgram);

			GL.ValidateProgram(shaderProgram);
			//int[] info = new int[1];
			//GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, info);
			//Console.WriteLine("link status: {0}",info[0]);

			GL.DeleteShader(computeShader);

		}


		public void SetSSBO(int index, string name)
		{
			int indx = GL.GetProgramResourceIndex(shaderProgram, ProgramInterface.ShaderStorageBlock,name);

			if (indx == -1)
			{
				Console.WriteLine("uniform buffer '{0}' not found", name);
				return;
			}
			GL.ShaderStorageBlockBinding(shaderProgram, indx, index);
		}

		public void SetSSBO(int index)
		{
			GL.ShaderStorageBlockBinding(shaderProgram, index, index);
		}

		public void Check()
		{
			int bindPoint = GL.GetProgramResourceIndex(shaderProgram, ProgramInterface.ShaderStorageBlock, "Output");
			Console.WriteLine(bindPoint);
			//GL.GetProgramResource(shaderProgram,ProgramInterface.ShaderStorageBlock,0,1
		}
	}
}
