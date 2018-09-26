using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace Toys
{
	//singleton class
	public class ShaderManager
	{
		Dictionary<string, Shader> shaders;
		static ShaderManager shdrMgmr;
		string defPath = "Toys.Resourses.shaders.";
		
		private ShaderManager()
		{
			var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ShaderManager)).Assembly;
			string frag = ReadFromStream(assembly.GetManifestResourceStream(defPath + "def.fs"));
			string vert = ReadFromStream(assembly.GetManifestResourceStream(defPath + "def.vs"));

			shaders = new Dictionary<string, Shader>();
			Shader def = new ShaderMain(vert, frag);
			shaders.Add("def", def);
		}

		//get singleton instance
		public static ShaderManager GetInstance
		{
			get
			{
				if (shdrMgmr == null)
					shdrMgmr = new ShaderManager();

				return shdrMgmr;
			}
		}

		public void LoadShader(string name)
		{
			
			if (shaders.ContainsKey(name))
				return;
			var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ShaderManager)).Assembly;
			string file_path = defPath + name;
			string frag = ReadFromStream(assembly.GetManifestResourceStream(file_path + ".fs"));
			string vert = ReadFromStream(assembly.GetManifestResourceStream(file_path + ".vs"));
			LoadShader(name, vert, frag);
		}

		public void LoadShader(string name, string vert, string frag)
		{
			if (shaders.ContainsKey(name))
				return;
			Shader shdr = new ShaderMain(vert, frag);
			shdr.ApplyShader();
			shdr.SetUniform(0, "material.texture_difuse");
			shdr.SetUniform(1, "material.texture_toon");

			shdr.SetUniform(10, "shadowMap");

			shaders.Add(name, shdr);
		}

		public Shader LoadShader(string name, string compute)
		{
			if (shaders.ContainsKey(name))
				return shaders[name];
			var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ShaderManager)).Assembly;
			var names = assembly.GetManifestResourceNames();
			string cmp = ReadFromStream(assembly.GetManifestResourceStream(defPath + compute));
			Shader shdr = new ShaderCompute(cmp);

			shaders.Add(name, shdr);
			return shdr;
		}

		public Shader GetShader(string name)
		{
			if (shaders.ContainsKey(name))
				return shaders[name];
			Console.WriteLine(shaders.Count);

			Console.WriteLine("shader {0} not found", name);
			return shaders["def"];
		}



		public void SetBinding(int index, string name)
		{
			foreach (var shader in shaders)
			{
				shader.Value.SetUBO(index, name);
			}
		}

		string ReadFromStream(Stream stream)
		{
			string str = "";
			using (var reader = new StreamReader(stream)) {
				str = reader.ReadToEnd ();
			}
			return str;
		}

	}
}
