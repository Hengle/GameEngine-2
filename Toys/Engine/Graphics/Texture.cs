﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Toys
{
	public enum TextureType
	{
		diffuse,
		toon,
		specular,
		shadow
	};

	public class Texture
	{
		int texture_id;
		TextureType type;
		string name;

		//default texture
		static Texture def;

		public Texture(string path, TextureType type,string name)
		{
			texture_id = GL.GenTexture();
			this.type = type;
            this.name = name;
			Bitmap tex1;

			//check texture
			try
			{
				///cause .NET cant tga natievly
				/// still need to load .spa textures
				///png , jpg, bmp is ok
				if (path.EndsWith("tga",StringComparison.OrdinalIgnoreCase))
					tex1 = Paloma.TargaImage.LoadTargaImage(path);
				else
					tex1 = new Bitmap(path);
                LoadTexture(tex1);
			}
			catch (Exception)
			{
				Console.Write("cant load texture  ");
				Console.WriteLine(path);
				//load default texture if fail
				//without reloading to memory
				Texture empty = LoadEmpty();
				texture_id = empty.texture_id;
				this.type = empty.type;
				this.name = empty.name;
			}


		}

		//for framebuffer
		Texture(int texture, string name)
		{
			texture_id = texture;
            this.name = name;
		}

		//for build in textures
		Texture(Bitmap tex, TextureType type, string name)
		{
			texture_id = GL.GenTexture();
            this.type = type;
            this.name = name;
			LoadTexture(tex);

		}


		void LoadTexture(Bitmap texture)
		{
			//inverting y axis			
			//texture.RotateFlip(RotateFlipType.Rotate180FlipX);

			GL.BindTexture(TextureTarget.Texture2D,texture_id);

			//setting wrapper
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Repeat);

			//setting interpolation
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            //load to static memory
            System.Drawing.Imaging.BitmapData data =
			texture.LockBits(new Rectangle(0, 0, texture.Width, texture.Height),
	  		System.Drawing.Imaging.ImageLockMode.ReadOnly, texture.PixelFormat);

			//recognithing pixel format type
			PixelFormat format;
			if (Image.IsAlphaPixelFormat(texture.PixelFormat) || texture.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppRgb)
				format = PixelFormat.Bgra;
			else
				format = PixelFormat.Bgr;

			//loading to video memory
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
			//              texture.Width, texture.Height, 0, format, PixelType.UnsignedByte, IntPtr.Zero);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
			              texture.Width, texture.Height, 0, format, PixelType.UnsignedByte, data.Scan0);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

			//clear resources
			texture.UnlockBits(data);
			texture.Dispose();
		}


		//for postprocessing framebuffer
		public static Texture LoadFrameBufer(int Width, int Height, string type)
		{
			int texture_id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D,texture_id);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Width, Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture_id, 0);
			return new Texture(texture_id,type);

		}


		//loading blank texture
		public static Texture LoadEmpty()
		{
			if (def == null)
			{
				var assembly = System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(Texture)).Assembly;
				Bitmap pic = new Bitmap(assembly.GetManifestResourceStream("Toys.Resourses.textures.empty.png"));
				def = new Texture(pic, TextureType.toon, "empty");
			}

			return def;
		}


		//Shadow texture
		public static Texture CreateShadowMap(int Width, int Height)
		{
			string type = "shadowmap";
			int texture_id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, texture_id);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, 
			              Width, Height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
			//setting wrapper
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, texture_id, 0);
			return new Texture(texture_id, type);
		}

		//examplar method for binding texture to shader
		public void BindTexture()
		{
			GL.BindTexture(TextureTarget.Texture2D,texture_id);
		}

		public TextureType GetTextureType
		{
			get { return type; }
		}
		public string GetName
		{
			get { return name; }
		}
	}
}
