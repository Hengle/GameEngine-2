﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Toys
{
    class RenderTexture : Texture
    {
        public RenderTexture(int width, int height)
        {
            GenerateTextureID();
            Width = width;
            Height = height;
            textureType = TextureTarget.Texture2D;

            BindTexture();
            GL.TexImage2D(textureType, 0, PixelInternalFormat.Rgba,
                          width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            wrapModeU = TextureWrapMode.ClampToEdge;
            wrapModeV = TextureWrapMode.ClampToEdge;
            FillterMode = TextureFillterMode.Bilinear;
        }


        internal void AttachToCurrentBuffer()
        {
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, textureType, textureID, 0);
        }


        public void GetImage(Bitmap image)
        {
            BindTexture();
            var imageBits = image.LockBits(new Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GetImage(imageBits.Scan0);
            image.UnlockBits(imageBits);
        }

        public void GetImage(IntPtr imagePointer)
        {
            GL.GetTexImage(textureType, 0, PixelFormat.Bgra, PixelType.UnsignedByte, imagePointer);
        }

        internal void ResizeTexture(int width, int heigth)
        {
            Width = width;
            Height = heigth;
            BindTexture();
            GL.TexImage2D(textureType, 0, PixelInternalFormat.Rgba,
                          width, heigth, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
        }
    }
}
