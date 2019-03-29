﻿using System;
using BulletSharp;
using BulletSharp.Math;

namespace Toys
{
	public class Joint
	{
		public JointContainer jcon { get; private set; }
		public TypedConstraint joint { get; private set; }

		int NoBody = 255;

		public Joint(JointContainer jc, RigidBodyBone[] rbodies)
		{
			jcon = jc;
			Instalize(rbodies);
		}

		void Instalize(RigidBodyBone[] rbodies)
		{
			//Matrix JointPos = Matrix.RotationYawPitchRoll(jcon.Rotation.Y, jcon.Rotation.X, jcon.Rotation.Z);
			//JointPos *= Matrix.Translation(GetVec3(jcon.Position));

			//look for no second body
			RigidBody Body1 = null;
			RigidBody Body2 = null;

			try
			{
				Body1 = rbodies[jcon.RigitBody1].Body;

				try
				{
					Body2 = rbodies[jcon.RigitBody2].Body;
				}
				catch (IndexOutOfRangeException) { }
			}
			catch (IndexOutOfRangeException)
			{
				try
				{
					Body1 = rbodies[jcon.RigitBody2].Body;
				}
				catch (IndexOutOfRangeException) { return; }
			}


			var JointSpace = Matrix.RotationYawPitchRoll(jcon.Rotation.Y, jcon.Rotation.X, jcon.Rotation.Z) * Matrix.Translation(GetVec3(jcon.Position));
			var temp1 = Body1.WorldTransform;
			temp1.Invert();
			var Conn1 = JointSpace * temp1;

			Matrix Conn2 = Matrix.Identity;
			if (Body2 != null)
			{
				//calculating joi9nt arms space
				var temp2 = Body2.WorldTransform;
				temp2.Invert();
				Conn2 = JointSpace* temp2;

				//for the older version of BulletSharp
				//temp1 = RigitBodyBone.GetMat(RigitBodyBone.GetMat(temp1).Inverted());
				//temp2 = RigitBodyBone.GetMat(RigitBodyBone.GetMat(temp2).Inverted());

			}
            switch (jcon.jType)
			{
				case JointType.ConeTwist:
					ConeTwistConstraint jointCone = null;
					if (Body2 != null)
						jointCone = new ConeTwistConstraint(Body1, Body2, Conn1, Conn2);
					else
						jointCone = new ConeTwistConstraint(Body1, Conn1);
					
					break;
				case JointType.SpringSixDOF: //the only one used
					Generic6DofSpring2Constraint jointSpring6 = null;
					if (Body2 != null)
						jointSpring6 = new Generic6DofSpring2Constraint(Body1, Body2, Conn1, Conn2);
					else 
						jointSpring6 = new Generic6DofSpring2Constraint(Body1, Conn1);
					
                    jointSpring6.AngularLowerLimit = GetVec3(jcon.RotMin);
					jointSpring6.AngularUpperLimit = GetVec3(jcon.RotMax);
                    
                    jointSpring6.LinearLowerLimit = GetVec3(jcon.PosMin);
					jointSpring6.LinearUpperLimit = GetVec3(jcon.PosMax);

                    joint = jointSpring6;
                   
                    break;
			}



        }


		private Vector3 GetVec3(OpenTK.Vector3 vec3)
		{
			return new Vector3(vec3.X, vec3.Y, vec3.Z);
		}
	}
}
