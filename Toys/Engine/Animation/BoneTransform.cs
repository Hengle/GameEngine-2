﻿using OpenTK;
using System;

namespace Toys
{
    public class BoneTransform
    {
        public readonly Bone Bone;
        public Matrix4 TransformMatrix;
        public Matrix4 LocalSpace;

        //initial bone space coordinetes for reference
        public Matrix4 LocalSpaceDefault;
        //inverted initial bone space coordinetes for calculating tranform
        public Matrix4 LocalSpaceInverted;

        public Matrix4 LocalMatrix;
        Matrix4 BoneMatrix;
        public Vector3 InitialOffset;

        Vector3 Translation;
        Quaternion Rotation;
        Vector3 Scale;
        Quaternion AddRotation;
        Vector3 AddTranslation;

        Vector3 LocalTranslation;
        //Quaternion LocalRotation;
        Vector3 LocalScale;

        //IK Variables
        public Quaternion IKRotation;
        public Quaternion LocalRotationForIKLink;
        Vector3 LocalTranslationForIKLink;

        public bool IsIK;
        public bool IsIKLink;
        public bool IsAddLocal;
        public bool IsRotateAdd;
        public bool IsTranslateAdd;
        public bool PhysicsIKSkip;
        public bool LocalRotationFlag;

        public BoneTransform Parent;
        public BoneTransform AddParent;
        public float AddRatio;
        public IKResolver IK;
        
        bool changed;

        public BoneTransform(Bone b)
        {
            Bone = b;
            LocalMatrix = Matrix4.Identity;
            LocalSpaceDefault = Matrix4.Identity;
            LocalSpaceInverted = Matrix4.Identity;
            IsIK = Bone.IK;
            IKRotation = Quaternion.Identity;
            ResetTransform(false);
        }

        public void UpdateLocalMatrix(bool ik = true)
        {
            Quaternion rot = Rotation;
            Vector3 trans = Translation;

            //additional parent bone
            if (IsRotateAdd && AddParent != null)
	        {
		        var addRotation = Quaternion.Identity;

		        if (!IsAddLocal)
		        {
			        addRotation = ((!AddParent.IsRotateAdd) ? AddParent.Rotation : AddRotation);
		        }
		        else
		        {
			        addRotation = AddParent.LocalMatrix.ExtractRotation();
		        }
		        if (AddParent.IsIKLink && !IsAddLocal)
		        {
			        addRotation *= AddParent.IKRotation;
		        }
		        if (AddRatio != 1f)
		        {
			        addRotation = Quaternion.Slerp(Quaternion.Identity, addRotation, AddRatio);
		        }
		        rot = (AddRotation = addRotation * rot);
	        }
	        if (IsTranslateAdd && AddParent != null)
	        {
		        Vector3 left = Vector3.Zero;
		        if (!IsAddLocal)
		        {
			        left = ((!AddParent.IsTranslateAdd) ? (AddParent.Translation) :AddTranslation);
		        }
		        else
		        {
			        left.X = AddParent.LocalMatrix.M41 - AddParent.Bone.Position.X;
			        left.Y = AddParent.LocalMatrix.M42 - AddParent.Bone.Position.Y;
			        left.Z = AddParent.LocalMatrix.M43 - AddParent.Bone.Position.Z;
		        }
		        if (AddRatio != 1f)
		        {
			        left *= AddRatio;
		        }
		        trans = (AddTranslation = left + trans);
	        }


            if (IsIKLink)
            {
                LocalRotationForIKLink = rot;
                LocalTranslationForIKLink = trans;
                rot *= IKRotation;
            }

            
            LocalMatrix = Matrix4.CreateFromQuaternion(rot);
            
            if (Scale.X != 1f || Scale.Y != 1f || Scale.Z != 1f)
                LocalMatrix *= Matrix4.CreateScale(Scale);
            LocalMatrix *= Matrix4.CreateTranslation(trans);
            BoneMatrix = LocalMatrix;
            LocalMatrix *= Matrix4.CreateTranslation(InitialOffset);

            if (Parent != null)
            {
                LocalScale = Vector3.Multiply(Parent.LocalScale, Scale);
                LocalMatrix *= Parent.LocalMatrix;
            }
            else
            {
                LocalScale = Scale;
            }
            
            if (ik && IsIK && IK != null && (!PhysicsIKSkip || !IK.IsPhysicsAllLink))
            {
               IK.Transform();
            }
            
        }

        public void UpdateLocalMatrixIKLink()
        {
            Quaternion rot = LocalRotationForIKLink * IKRotation;
            LocalMatrix = Matrix4.CreateFromQuaternion(rot);
            //if (Bone.Index == 163) { Console.WriteLine(LocalMatrix); }
            if (Scale.X != 1f || Scale.Y != 1f || Scale.Z != 1f)
            {
                LocalMatrix.M11 = LocalMatrix.M11 * Scale.X;
                LocalMatrix.M12 = LocalMatrix.M12 * Scale.X;
                LocalMatrix.M13 = LocalMatrix.M13 * Scale.X;
                LocalMatrix.M21 = LocalMatrix.M21 * Scale.Y;
                LocalMatrix.M22 = LocalMatrix.M22 * Scale.Y;
                LocalMatrix.M23 = LocalMatrix.M23 * Scale.Y;
                LocalMatrix.M31 = LocalMatrix.M31 * Scale.Z;
                LocalMatrix.M32 = LocalMatrix.M32 * Scale.Z;
                LocalMatrix.M33 = LocalMatrix.M33 * Scale.Z;
            }
            LocalMatrix.M41 = LocalMatrix.M41 + LocalTranslationForIKLink.X;
            LocalMatrix.M42 = LocalMatrix.M42 + LocalTranslationForIKLink.Y;
            LocalMatrix.M43 = LocalMatrix.M43 + LocalTranslationForIKLink.Z;
            BoneMatrix = LocalMatrix;
            LocalMatrix.M41 = LocalMatrix.M41 + InitialOffset.X;
            LocalMatrix.M42 = LocalMatrix.M42 + InitialOffset.Y;
            LocalMatrix.M43 = LocalMatrix.M43 + InitialOffset.Z;
            if (Parent != null)
            {
                LocalMatrix *= Parent.LocalMatrix;
            }
            //if (Bone.Index == 161)
            //    Console.WriteLine(LocalMatrix);
        }

        public void ResetTransform(bool link)
        {
            BoneMatrix = Matrix4.Identity;
            LocalMatrix = Matrix4.Identity;
            TransformMatrix = Matrix4.Identity;

            LocalScale = new Vector3(1f);
            //LocalRotation = Quaternion.Identity;

            Scale = Vector3.One;
            Rotation = Quaternion.Identity;
            Translation = Vector3.Zero;

            //IK
            LocalRotationForIKLink = Quaternion.Identity;
            LocalTranslationForIKLink = Vector3.Zero;
            if (link)
                IKRotation = Quaternion.Identity;

            AddTranslation = Vector3.Zero;
            AddRotation = Quaternion.Identity;
        }


        public void SetTransform(Vector3 scale, Quaternion rot, Vector3 trans)
        {
            Scale = scale;
            Rotation = rot;
            Translation = trans;
        }

        public void SetTransform(Quaternion rot, Vector3 trans)
        {
            Rotation = rot;
            Translation = trans;
        }

        public void UpdateTransformMatrix()
        {
            //TransformMatrix = LocalMatrix * LocalSpaceInverted;
            //TransformMatrix.Transpose();
            BoneMatrix = LocalSpaceInverted * BoneMatrix * LocalSpaceDefault;
            TransformMatrix = (Parent == null) ? BoneMatrix : BoneMatrix * Parent.TransformMatrix;
        }

        /*
        public void UpdateLocalRotation()
        {
            if (!LocalRotationFlag)
            {
                LocalMatrix = Matrix4.CreateFromQuaternion(LocalRotation);
                LocalRotation.Normalize();
                LocalRotationFlag = true;
            }
        }
        */

        public Vector3 GetTransformedBonePosition()
        {
            return new Vector3(LocalMatrix.M41, LocalMatrix.M42, LocalMatrix.M43);
        }
    }
}
