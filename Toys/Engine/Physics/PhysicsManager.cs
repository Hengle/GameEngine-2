using System;
using BulletSharp;
using BulletSharp.Math;
using System.Text;

namespace Toys
{
    public class PhysicsManager : ScriptingComponent
    {
        public delegate void BoneBodySyncer(OpenTK.Matrix4 world);
        RigidBodyBone[] rigitBodies;
        BoneController bones;
        Joint[] joints;
        BoneBodySyncer prePhysics;
        BoneBodySyncer postPhysics;
        Transform worldTrans;

        DiscreteDynamicsWorld World;

        RigidContainer[] Rigits;
        JointContainer[] Jcons;

        public PhysicsManager(RigidContainer[] rigits, JointContainer[] jcons, BoneController bons, Transform trans)
        {
            //setup world physics
            bones = bons;
            worldTrans = trans;
            //instalize delegates
            prePhysics = (m) => { };
            postPhysics = (m) => { };
            Rigits = rigits;
            Jcons = jcons;
            //CreateGeneric6DofSpringConstraint();
            Type = typeof(PhysicsManager);
        }

        void Awake()
        {
            World = CoreEngine.pEngine.World;
            InstalizeRigitBody(Rigits);
            InstalizeJoints(Jcons);
        }

        void InstalizeRigitBody(RigidContainer[] rigits)
        {
            rigitBodies = new RigidBodyBone[rigits.Length];
            for (int i = 0; i < rigits.Length; i++)
            {

				rigitBodies[i] = new RigidBodyBone(rigits[i]);

                //skipping bone binding for no index riggs (ushort indexes only)
                if (rigits[i].BoneIndex < bones.GetBones.Length && rigits[i].BoneIndex >=0)
                {
                    rigitBodies[i].BoneID = rigits[i].BoneIndex;
                    rigitBodies[i].BoneController = bones;
                    if (rigits[i].Phys == PhysType.FollowBone || rigits[i].Phys == PhysType.GravityBone)
                        prePhysics += rigitBodies[i].SyncBone2Body;

                    if (rigits[i].Phys == PhysType.GravityBone || rigits[i].Phys == PhysType.Gravity)
                    {
                        postPhysics += rigitBodies[i].SyncBody2Bone;
                       // bones.GetBone(rigits[i].BoneIndex).Phys = true;
                    }
                       
                }
                World.AddRigidBody(rigitBodies[i].Body, (int)Math.Pow(2, rigits[i].GroupId), rigits[i].NonCollisionGroup);

            }
        }

        void InstalizeJoints(JointContainer[] jcons)
        {
            joints = new Joint[jcons.Length];
            for (int i = 0; i < jcons.Length; i++)
            {
                joints[i] = new Joint(jcons[i], rigitBodies);
				if (joints[i].Constraint != null)
                World.AddConstraint(joints[i].Constraint, true);
            }
        }

        void Update()
        {
            prePhysics(worldTrans.GlobalTransform);
        }

        void PreRender()
        {
            var worldInverted = worldTrans.GlobalTransform;
            worldInverted.Invert();
            postPhysics(worldInverted);
        }

		//test
		public void ReinstalizeBodys()
		{
			foreach (var body in rigitBodies)
				body.Reinstalize(worldTrans.GlobalTransform);	
		}

        internal override void Unload()
        {

            foreach (var joint in joints)
            {
                World.RemoveConstraint(joint.Constraint);
                joint.Constraint.Dispose();
            }
            foreach (var rigid in rigitBodies)
            {
                World.RemoveCollisionObject(rigid.Body);
                rigid.Body.Dispose();
            }
            base.Unload();
        }
    }
}
