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
        private float theta = 0;

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
                    Flags = ContextFlags.Default,
                    Profile = ContextProfile.Compatability,
                    APIVersion = new Version(3, 3)
                })
        {
            CenterWindow();UpdateFrequency = 1/60f;
            this.camera = camera;
            this.primitives = primitives;

            KeyDown += (e) =>
            {
                float step = 0.1f;
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
        }

        protected override void OnLoad()
        {      
            IsVisible = true;
            GL.ClearColor(Color4.Black);       

            GL.Enable(EnableCap.DepthTest);

            base.OnLoad();            
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            Title = $"Camera position = {camera.Pivot.Position}, Camera forward = {camera.Pivot.Orientation.Forward}";
            GL.ClearColor(Color4.Black);

            Matrix4 perspective = camera.GetProjectionMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);

            Matrix4 lookAt = camera.GetViewMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookAt);

            theta += 0.001f;

            
            primitives[0].Pivot.RotateHorizontal(0.005f);
            primitives[0].Pivot.RotateVertical(0.001f);

            // orbit movement
            Vector3 cubeToSphere = primitives[0].Pivot.Position - primitives[1].Pivot.Position;
            float orbit = cubeToSphere.Length;
            primitives[0].Velocity = Vector3.Normalize(new(-cubeToSphere.Z, cubeToSphere.Y, cubeToSphere.X)) * 0.01f;
            primitives[0].Move();

            // compensate movement away from center
            cubeToSphere = primitives[0].Pivot.Position - primitives[1].Pivot.Position;
            float extraOrbit = cubeToSphere.Length - orbit;
            primitives[0].Velocity = -cubeToSphere.Normalized() * extraOrbit;
            primitives[0].Move();
            

            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Polygon3[] spherePolygons = primitives[1].GetPolygons();

            for (int i = 0; i < spherePolygons.Length; ++i)
            {
                GL.Begin(PrimitiveType.Triangles);
                GL.Color4(spherePolygons[i].Color);
                GL.Vertex3(spherePolygons[i].Vertices[0]);
                GL.Vertex3(spherePolygons[i].Vertices[1]);
                GL.Vertex3(spherePolygons[i].Vertices[2]);
                GL.End();
            }           

            
            Vector3 lightDirection = Vector3.Normalize(primitives[0].Pivot.Position - primitives[1].Pivot.Position);

            Polygon3[] cubePolygons = primitives[0].GetPolygons();

            for (int i = 0; i < cubePolygons.Length; ++i)
            {
                float a = MathF.Acos(Vector3.Dot(cubePolygons[i].Normal, lightDirection) / (cubePolygons[i].Normal.Length * lightDirection.Length)) / 3.14f;
                
                GL.Begin(PrimitiveType.Triangles);
                GL.Color4(new Color4(cubePolygons[i].Color.R * a, cubePolygons[i].Color.G * a, cubePolygons[i].Color.B * a, 1));
                GL.Vertex3(cubePolygons[i].Vertices[0]);
                GL.Vertex3(cubePolygons[i].Vertices[1]);
                GL.Vertex3(cubePolygons[i].Vertices[2]);
                GL.End();
            }            
            
            Context.SwapBuffers();

            base.OnRenderFrame(args);
        }

        
    }
}