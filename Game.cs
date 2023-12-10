using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using OpenTK.Mathematics;
using RenderingGL.Engine;
using Rendering.Engine.Primitives;
using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.IO;
using RenderingGL.Engine.Primitives;

namespace RenderingGL
{
    public class Game : GameWindow
    {
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private VertexArray vertexArray;
        private ShaderProgram shaderProgram;

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
            base.OnResize(e);
        }

        protected override void OnLoad()
        {      
            IsVisible = true;

            GL.ClearColor(Color4.Black);

            int polygonCount = 0;
            foreach (Primitive primitive in primitives)
            {
                polygonCount += primitive.GetPolygons().Length;
            }

            VertexPositionColor[] vertices = new VertexPositionColor[polygonCount * 3];
            int[] indices = new int[polygonCount * 3];

            int alreadyProcessed = 0;
            foreach (Primitive primitive in primitives)
            {
                Polygon3[] polygons = primitive.GetPolygons();

                for (int i = 0; i < polygons.Length; ++i)
                {
                    vertices[(alreadyProcessed + i) * 3] = new (polygons[i].Vertices[0], polygons[i].Color, polygons[i].Normal, new(1,0,0));
                    vertices[(alreadyProcessed + i) * 3 + 1] = new (polygons[i].Vertices[1], polygons[i].Color, polygons[i].Normal, new(0,1,0));
                    vertices[(alreadyProcessed + i) * 3 + 2] = new (polygons[i].Vertices[2], polygons[i].Color, polygons[i].Normal, new(0,0,1));

                    indices[(alreadyProcessed + i) * 3] = (alreadyProcessed + i) * 3;
                    indices[(alreadyProcessed + i) * 3 + 1] = (alreadyProcessed + i) * 3 + 1;
                    indices[(alreadyProcessed + i) * 3 + 2] = (alreadyProcessed + i) * 3 + 2;
                }

                alreadyProcessed += polygons.Length;
            }

            vertexBuffer = new(VertexPositionColor.VertexInfo, vertices.Length, false);
            vertexBuffer.SetData(vertices, vertices.Length);

            indexBuffer = new(indices.Length, true);
            indexBuffer.SetData(indices, indices.Length);

            vertexArray = new(vertexBuffer);

            string vertexShaderCode = File.ReadAllText("C:\\Users\\Lenovo\\Desktop\\RenderingGL\\Shaders\\PolygonShaders\\VertexShader.glsl");
            string fragmentShaderCode = File.ReadAllText("C:\\Users\\Lenovo\\Desktop\\RenderingGL\\Shaders\\PolygonShaders\\FragmentShader.glsl");
            string geometryShaderCode = File.ReadAllText("C:\\Users\\Lenovo\\Desktop\\RenderingGL\\Shaders\\PolygonShaders\\GeometryShader.glsl");
            
            shaderProgram = new(vertexShaderCode, fragmentShaderCode, geometryShaderCode);

            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);

            
            //shaderProgram.SetUniform("ViewportSize", (float)viewport[2], (float)viewport[3]);            
            //shaderProgram.SetUniform("ModelMatrix", Matrix4.CreateScale((float)camera.Height / camera.Width, 1, 1));
            shaderProgram.SetUniform("cameraDirection", camera.Pivot.Orientation.Forward.X, camera.Pivot.Orientation.Forward.Y, camera.Pivot.Orientation.Forward.Z);
            shaderProgram.SetUniform("cameraPosition", camera.Pivot.Position.X, camera.Pivot.Position.Y, camera.Pivot.Position.Z);
            shaderProgram.SetUniform("zNear", camera.NearPlane);

            Vector3 leftNormal = camera.GetNormalLeft();
            shaderProgram.SetUniform("leftNormal", leftNormal.X, leftNormal.Y, leftNormal.Z);
            Vector3 rightNormal = camera.GetNormalRight();
            shaderProgram.SetUniform("rightNormal", rightNormal.X, rightNormal.Y, rightNormal.Z);
            Vector3 bottomNormal = camera.GetNormalBottom();
            shaderProgram.SetUniform("bottomNormal", bottomNormal.X, bottomNormal.Y, bottomNormal.Z);
            Vector3 topNormal = camera.GetNormalTop();
            shaderProgram.SetUniform("topNormal", topNormal.X, topNormal.Y, topNormal.Z);

            shaderProgram.SetUniform("ViewMatrix", camera.GetViewMatrix());
            shaderProgram.SetUniform("ProjectMatrix", camera.GetProjectionMatrix());
            shaderProgram.SetUniform("lightPosition", 0, -2, 0);
            //shaderProgram.SetUniform("ScaleMatrix", Matrix4.CreateScale(new Vector3(viewport[2] / 8, viewport[3] / 8, 1)));


            base.OnLoad();
        }

        protected override void OnUnload()
        {
            vertexArray?.Dispose();
            indexBuffer?.Dispose();
            vertexBuffer?.Dispose();
            shaderProgram?.Dispose();

            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            foreach (Primitive primitive1 in primitives)
            {
                primitive1.Velocity -= Vector3.UnitY * 0.0000032f;
                primitive1.Move();

                foreach (Primitive primitive2 in primitives)
                {
                    if (primitive1.Equals(primitive2))
                    {
                        continue;
                    }

                    primitive1.SolveCollision(primitive2);
                }
            }

            int polygonCount = 0;
            foreach (Primitive primitive in primitives)
            {
                polygonCount += primitive.GetPolygons().Length;
            }

            VertexPositionColor[] vertices = new VertexPositionColor[polygonCount * 3];

            int alreadyProcessed = 0;
            foreach (Primitive primitive in primitives)
            {
                Polygon3[] polygons = primitive.GetPolygons();

                for (int i = 0; i < polygons.Length; ++i)
                {
                    vertices[(alreadyProcessed + i) * 3] = new (polygons[i].Vertices[0], polygons[i].Color, polygons[i].Normal, new(1,0,0));
                    vertices[(alreadyProcessed + i) * 3 + 1] = new (polygons[i].Vertices[1], polygons[i].Color, polygons[i].Normal, new(0,1,0));
                    vertices[(alreadyProcessed + i) * 3 + 2] = new (polygons[i].Vertices[2], polygons[i].Color, polygons[i].Normal, new(0,0,1));
                }

                alreadyProcessed += polygons.Length;
            }

            vertexBuffer.SetData(vertices, vertices.Length);

            shaderProgram.SetUniform("cameraDirection", camera.Pivot.Orientation.Forward.X, camera.Pivot.Orientation.Forward.Y, camera.Pivot.Orientation.Forward.Z);
            shaderProgram.SetUniform("cameraPosition", camera.Pivot.Position.X, camera.Pivot.Position.Y, camera.Pivot.Position.Z);

            Vector3 leftNormal = camera.GetNormalLeft();
            shaderProgram.SetUniform("leftNormal", leftNormal.X, leftNormal.Y, leftNormal.Z);
            Vector3 rightNormal = camera.GetNormalRight();
            shaderProgram.SetUniform("rightNormal", rightNormal.X, rightNormal.Y, rightNormal.Z);
            Vector3 bottomNormal = camera.GetNormalBottom();
            shaderProgram.SetUniform("bottomNormal", bottomNormal.X, bottomNormal.Y, bottomNormal.Z);
            Vector3 topNormal = camera.GetNormalTop();
            shaderProgram.SetUniform("topNormal", topNormal.X, topNormal.Y, topNormal.Z);

            shaderProgram.SetUniform("ViewMatrix", camera.GetViewMatrix());            
            shaderProgram.SetUniform("ProjectMatrix", camera.GetProjectionMatrix());
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {            
            GL.Clear(ClearBufferMask.ColorBufferBit);            
            
            // clear the render buffer....
            //GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);          

            // Render main 3d scene
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthClamp);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.DepthMask(true);

            // GL.Enable(EnableCap.StencilTest);
            // GL.Enable(EnableCap.Blend); // Enable blending for transparency
            // GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha); // Set the blend function


            GL.UseProgram(shaderProgram.ShaderProgramHandler);
            GL.BindVertexArray(vertexArray.VertexArrayHandler);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer.IndexBufferHandler);
            GL.DrawElements(PrimitiveType.Triangles, indexBuffer.IndexCount, DrawElementsType.UnsignedInt, 0);
            
            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}