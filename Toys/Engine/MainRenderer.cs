﻿using System;
using System.Linq;

namespace Toys
{
	public class MainRenderer
	{
		public Camera mainCamera;
		public Scene mainScene;

		UniformBufferSkeleton skeleton;
		UniformBufferLight ubl;
		UniformBufferSpace ubs;
		ModelRenderer renderer;

		internal MainRenderer(Camera camera, Scene scene)
		{
			mainCamera = camera;
			mainScene = scene;
			UniformBufferManager ubm = UniformBufferManager.GetInstance;
			skeleton = (UniformBufferSkeleton)ubm.GetBuffer("skeleton");

			ubs = (UniformBufferSpace)ubm.GetBuffer("space");
			ubl = (UniformBufferLight)ubm.GetBuffer("light");
			ubl.SetNearPlane(0.1f);
			ubl.SetFarPlane(10.0f);
			renderer = new ModelRenderer();
			renderer.projection = camera.projection;
		}

		public void Render() 
		{
            renderer.viev = mainCamera.GetLook;
			ubl.SetViewPos(mainCamera.GetPos);
			ubs.SetPvSpace(mainCamera.GetLook * mainCamera.projection);

			// get render nodes
			/*
			var nodes = from node in mainScene.GetNodes()
						where node.GetComponent(typeof(MeshDrawer)) != null
						select node.GetComponent(typeof(MeshDrawer));
*/

			foreach (var node in mainScene.GetNodes())
			{
				if (!node.Active)
					continue;

				MeshDrawer md = (MeshDrawer) node.GetComponent(typeof(MeshDrawer));

				if (md == null)
					continue;

				renderer.Render(md);
				/*
				if (node.anim != null)
					skeleton.SetBones(node.anim.GetSkeleton);
				if (node.model != null)
					renderer.Render(node);
					*/
			}
		}

		public void Render(MeshDrawer[] meshes)
		{
			renderer.viev = mainCamera.GetLook;
			ubl.SetViewPos(mainCamera.GetPos);
			ubs.SetPvSpace(mainCamera.GetLook * mainCamera.projection);

			foreach (var mesh in meshes)
			{
				renderer.Render(mesh);
			}
		}

		public void Resize()
		{
			renderer.projection = mainCamera.projection;
		}
	}
}
