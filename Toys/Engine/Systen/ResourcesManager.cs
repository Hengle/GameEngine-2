﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

namespace Toys
{
	public class ResourcesManager
	{
		static Dictionary<string, WeakReference> resources = new Dictionary<string, WeakReference>();
		CoreEngine ce;

		public ResourcesManager(CoreEngine core)
		{
			ce = core;
		}

        public static T LoadAsset<T>(string path) where T : Resource
        {

            Type tp = typeof(T);
            if (resources.ContainsKey(path))
            {
                if (resources[path].IsAlive)
                {
                    T result = resources[path].Target as T;
                    return result;
                }
                else
                    resources.Remove(path);
            }

            var stream = File.OpenRead(path);
            Resource asset = null;
            if (tp == typeof(Texture2D))
            {

                asset = new Texture2D(stream, path);
            }
            else if (tp == typeof(ModelPrefab))
            {
                IModelLoader model = ModelLoader.Load(stream, path);
                asset = model?.GetModel;
            }
            else if (tp == typeof(AudioClip))
            {
                asset = new AudioClip(stream, path);
            }
            else if (tp == typeof(Animation))
            {
                asset = AnimationLoader.Load(stream, path);
            }
            else {
                Console.WriteLine("Type not supported");
            }

            stream.Dispose();
            if (asset)
			{
				asset.Id = path;
				asset.Type = tp;
                resources.Add(path, new WeakReference(asset));
            }
            return asset as T;
		}

        internal static void AddAsset(Resource asset, string name)
        {
            if (!resources.ContainsKey(name))
                resources.Add(name, new WeakReference(asset));
        }

        public static T[] GetResourses<T>() where T : Resource
		{
			List<T> result = new List<T>();
			foreach (var val in resources.Values)
				if (val is T)
					result.Add(val.Target as T);

			return result.ToArray();
		}

        public static T GetResourse<T>(string name) where T : Resource
        {
            if (resources.ContainsKey(name))
                return resources[name] as T;
            return null;
        }

        public static T[] GetComponents<T>() where T : Component
		{
			List<T> result = new List<T>();
			foreach (var val in resources.Values)
			{
				if (val.Target is T && ((Component)val.Target).Node.Active)
					result.Add((T)val.Target);
			}

			return result.ToArray();
		}

        public static string ReadFromInternalResource(string path)
        {
            var stream = ReadFromInternalResourceStream(path);

            string str = "";
            using (var reader = new StreamReader(stream))
            {
                str = reader.ReadToEnd();
            }
            return str;
        }

        public static Stream ReadFromInternalResourceStream(string path)
        {
            string defPath = "Toys.Resourses.";
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ShaderManager)).Assembly;
            var stream = assembly.GetManifestResourceStream(defPath + path);
            return stream;
        }

        public static Stream GetResourceStream(string path)
        {
            return File.OpenRead(path); ;
        }

        public static bool DeleteResource(Resource resource)
        {
            string name = resources.FirstOrDefault(x => (Resource)x.Value.Target == resource).Key;
            if (name != default(string))
                return DeleteResource(name);
            else
            {
                CoreEngine.ActiveCore.AddTask = () => resource.Unload();
                return true;
            }
        }

        /// <summary>
        /// Path to assets directoy \..abs..\Assets\
        /// </summary>
        /// <returns></returns>
        public static string GetAbsoluteAssetDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "Assets" + Path.DirectorySeparatorChar;
        }

        public static bool DeleteResource(string res)
        {
            WeakReference resource;
            if (resources.TryGetValue(res, out resource))
            {
                CoreEngine.ActiveCore.AddTask = () => ((Resource)resource.Target).Unload();
                resources.Remove(res);
                return true;
            }

            return false;
        }
    }
}
