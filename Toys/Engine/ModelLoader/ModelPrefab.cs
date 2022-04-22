﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Toys
{
    public class ModelPrefab : Resource
    {
        List<Component> components;
        public ModelPrefab(List<Component> list)
        {
            components = list;
        }


        public SceneNode CreateNode()
        {
            var result = new SceneNode();

            foreach (var cpmnt in components)
            {
                result.AddComponent(cpmnt.Clone());
            }
            return result;
        }
        internal override void Unload()
        {
            foreach(var comp in components)
                comp.Unload();
        }
    }
}
