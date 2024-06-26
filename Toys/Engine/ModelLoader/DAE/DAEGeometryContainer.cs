﻿using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Toys
{
	public class DAEGeometryContainer
	{
		public string ID;
		public string Name;
		public Vector3[] Positions;
		public Vector3[] Normals;
		public Vector2[] UVs;
		public Vector3[] Colors;
		public Vector4[] BoneWeigths;
		public IVector4[] BoneIndeces;
        public List<DAEMaterialBinding> MaterialIndexTable;
		public int[] Indeces;
		//public string MaterialName;
        internal string ControllerId;

		public DAEGeometryContainer()
		{
            /*
			Positions = new Vector3[vertCount];
			Normals = new Vector3[vertCount];
			UVs = new Vector2[vertCount];
			BoneWeigths = new Vector4[vertCount];
			BoneIndeces = new IVector4[vertCount];
            */
            MaterialIndexTable = new List<DAEMaterialBinding>();

        }
	}
}
