﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;

namespace Toys
{
    class DynamicFormScript : ScriptingComponent
    {
        int renderBufferId = 0;
        int offsetX, offsetY;
        int width, height;
        Camera camera;
        Bitmap imageBitmap;
        DynamicForm form;
        RenderTexture renderTex;
        RenderBuffer renderBuffer;
        bool isLeftButtonMouseDown = false;
        bool keyPressed = false;
        BackgroundBase backgroundBackup;

        //post processing
        PostProcessing ppSh;
        Shader shaderPP;
        ShaderUniformVector3 aberation;
        Vector3 pixelAberation = new Vector3(5,3, -7);
        int n = 0;

        void Awake()
        {
            camera = CoreEngine.gEngine.MainCamera;
            width = camera.Width;
            height = camera.Height;

            renderBuffer = new RenderBuffer(camera);
            renderBufferId = renderBuffer.RenderBufferMS;
            renderTex = renderBuffer.RenderTexture;
            imageBitmap = new Bitmap(width, height);
            form = new DynamicForm();

            form.LeftMouseDown += LeftMouseDown;
            form.LeftMouseUp += LeftMouseUp;

            //postprocessing testing
            ppSh = new PostProcessing(camera);
            ShaderManager shdrm = ShaderManager.GetInstance;
            shdrm.LoadShader("FormPP");
            shaderPP = shdrm.GetShader("FormPP");
            shaderPP.ApplyShader();
            shaderPP.SetUniform(0,"texture_diffuse");
            shaderPP.SetUniform(new Vector3(0.01f,0.005f,-0.01f), "colorOffset");
            aberation = (ShaderUniformVector3)shaderPP.GetUniforms[0];
            pixelAberation /= width;
        }


        void Update()
        {
            
            if (width != camera.Width || height != camera.Height)
            {
                width = camera.Width;
                height = camera.Height;
                renderTex.ResizeTexture(width, height);
                form.Width = camera.Width;
                form.Height = camera.Height;
            }
            
            KeyboardState keyState = Keyboard.GetState();
            if (keyState[Key.B] && !keyPressed)
            {
                if (camera.RenderBuffer != 0)
                {
                    camera.Background = backgroundBackup;
                    camera.RenderBuffer = 0;
                    form.Hide();
                }
                else
                {
                    backgroundBackup = camera.Background;
                    camera.Background = null;
                    camera.RenderBuffer = renderBufferId;
                    form.Show();
                }

                keyPressed = true;
            }

            if (!keyState[Key.B] && keyPressed)
                keyPressed = false;
        }

        void PostRender()
        {
            if (camera.RenderBuffer != 0)
            {
                renderBuffer.DownSample();
               
                shaderPP.ApplyShader();
                GL.ActiveTexture(TextureUnit.Texture0);
                renderTex.BindTexture();
                //
                float attitude = (float)Math.Sin((double)n / 45 * Math.PI);
                aberation.SetValue(pixelAberation * attitude);
                //
                ppSh.RenderScreen();
                ppSh.OutputTexture.GetImage(imageBitmap);
                //renderTex.GetImage(imageBitmap);
                //imageBitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                form.UpdateFormDisplay(imageBitmap);

                n++;
            }
            

            if (isLeftButtonMouseDown)
            {
                form.Left = Cursor.Position.X + offsetX;
                form.Top = Cursor.Position.Y + offsetY;
            }
            
            KeyboardState keyState = Keyboard.GetState();
            if (keyState[Key.S])
            {
                imageBitmap.Save("test.png");
            }
            
        }

        void OnDestroy()
        {
            imageBitmap.Dispose();
            form.Dispose();
        }

        private void LeftMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            offsetX = form.Left - Cursor.Position.X;
            offsetY = form.Top - Cursor.Position.Y;
            isLeftButtonMouseDown = true;
        }

        private void LeftMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isLeftButtonMouseDown = false;
        }
    }
}
