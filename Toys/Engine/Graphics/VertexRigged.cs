﻿using System;
using OpenTK;

namespace Toys
{
	/// <summary>
	/// Vertex rigged.
	/// 
	/// Vector4 position normal uvtex
	/// for std430
	/// structire alligmetn
	/// </summary>
	public struct VertexRigged
	{

		public Vector4 position;
		public Vector4 normal;
		public Vector4 uvtex;
		//public Vector3 position;
		//public Vector3 normal;
		//public Vector2 uvtex;
		public IVector4 boneIndexes;
		public Vector4 weigth;


		public VertexRigged(Vector3 pos, Vector3 norm, Vector2 tex, IVector4 indexes, Vector4 weigth)
		{
			position = new Vector4(pos, 1.0f);
			normal = new Vector4(norm, 1.0f);
			uvtex = new Vector4(tex);
			//position = pos;
			//normal = norm;
			//uvtex = tex;
			boneIndexes = indexes;
			this.weigth = weigth;
		}
	
	}

	public struct IVector4
	{
		public int bone1;
		public int bone2;
		public int bone3;
		public int bone4;

		public IVector4(int[] bones)
		{
			bone1 = bones[0];
			bone2 = bones[1];
			bone3 = bones[2];
			bone4 = bones[3];
		}

		public override string ToString()
		{
			return string.Format("({0},{1},{2},{3})",bone1,bone2,bone3,bone4);
		}

		public int this[int i]
		{
			get
			{
					
				switch (i)
				{
					case 0:
						return bone1;
					case 1:
						return bone2;
					case 2:
						return bone3;
					case 3:
						return bone4;
				}

				return 0;
			}
		}
	}

}
