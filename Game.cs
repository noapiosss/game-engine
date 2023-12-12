using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RenderingGL.Engine;
using Rendering.Engine.Primitives;
using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderingGL.Engine.Primitives;

namespace RenderingGL
{
    public class Game : GameWindow
    {
        private Camera camera;
        private Primitive[] primitives;

        public Game(Camera camera, Primitive[] primitives, int width = 1280, int height = 768)
            : base(
                GameWindowSettings.Default,
                new NativeWindowSettings()
                {
                    Title = "RenderingGL",
                    Size = new Vector2i(width, height),
                    StartVisible = false,
                    StartFocused = true,
                    API = ContextAPI.OpenGL,
                    Profile = ContextProfile.Core,
                    APIVersion = new Version(3, 3)
                })
        {
            CenterWindow();
            UpdateFrequency = 1/60f;
            this.camera = camera;
            this.primitives = primitives;

            KeyDown += (e) =>
            {
                float step = 0.05f;
                float angle = 0.05f;

                switch(e.Key)
                {
                    case Keys.W:
                        camera.MoveForward(step);
                    break;
                    
                    case Keys.S:
                        camera.MoveForward(-step);
                    break;

                    case Keys.A:
                        camera.MoveRight(step);
                    break;

                    case Keys.D:
                        camera.MoveRight(-step);
                    break;

                    case Keys.C:
                        camera.MoveUp(-step);
                    break;

                    case Keys.Space:
                        camera.MoveUp(step);
                    break;

                    case Keys.Left:
                        camera.LookSide(angle);
                    break;

                    case Keys.Right:
                        camera.LookSide(-angle);
                    break;

                    case Keys.Down:
                        camera.LookUp(angle);
                    break;

                    case Keys.Up:
                        camera.LookUp(-angle);
                    break;
                }
            };
        }

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            camera.UpdateResolution(e.Width, e.Height);
            
            GL.Viewport(0, 0, e.Width, e.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 perspective = camera.GetProjectionMatrix();
            
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);

        }

        protected override void OnLoad()
        {      
            IsVisible = true;
            GL.ClearColor(Color4.Black);

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {

            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {         
            GL.LoadIdentity();   
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            GL.Begin(BeginMode.Triangles);

            foreach (Primitive primitive in primitives)
            {
                Polygon3[] polygons = primitive.GetPolygons();

                for (int i = 0; i < polygons.Length; ++i)
                {
                    GL.Color4(polygons[i].Color);
                    GL.Vertex3(polygons[i].Vertices[0]);
                    GL.Vertex3(polygons[i].Vertices[1]);
                    GL.Vertex3(polygons[i].Vertices[2]);
                }
            }
            
            GL.End();
            
            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}