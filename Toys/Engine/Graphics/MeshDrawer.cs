﻿namespace Toys
{
	public class MeshDrawer : Component
	{
		public Mesh mesh { get; private set; }
		public IMaterial[] mats { get; private set; }
		public Morph[] morph { get; private set; }


		public bool OutlineDrawing;
		public bool CastShadow;


		public MeshDrawer(Mesh mesh, IMaterial[] mats,Morph[] mor = null) : base (typeof(MeshDrawer))
		{
			this.mesh = mesh;
			this.mats = mats;
			morph = mor;
		}

		//for single material mesh
		public MeshDrawer(Mesh mesh, IMaterial mat) : this(mesh, new IMaterial[] { mat })
		{
			mat.count = mesh.indexes.Length;
		}


		/*
		 * main drawing
		*/
		public virtual void Draw()
		{
			mesh.BindVAO();
			foreach (var mat in mats)
			{
				var rdirs = mat.rndrDirrectives;
				if (!rdirs.render)
					continue;

				mat.ApplyMaterial();
				mesh.Draw(mat.offset, mat.count);
			}
			mesh.ReleaseVAO();
		}

		//drawing model outline
		public virtual void DrawOutline()
		{
			
			mesh.BindVAO();

			foreach (var mat in mats)
			{
				var	otl = ((Material)mat).outln;
				var rndr = mat.rndrDirrectives;
				//outline = mat.outline;
				if (!rndr.render || !rndr.hasEdges)
					continue;
				otl.ApplyOutline();
				mesh.Draw(mat.offset, mat.count);
			}
			mesh.ReleaseVAO();
		}

		//for shadow mapping
		public virtual void DrawSimple()
		{
			mesh.BindVAO();
			foreach (var mat in mats)
			{
				
				if (!mat.rndrDirrectives.render)
					continue;
				//mat.GetShader.ApplyShader();
				mesh.Draw(mat.offset, mat.count);
			}

			mesh.ReleaseVAO();

		}

		internal override void Unload()
		{
			mesh.Delete();
		}

	}
}
