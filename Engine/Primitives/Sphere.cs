using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using OpenTK.Mathematics;
using RenderingGL.Engine.Primitives;

namespace Rendering.Engine.Primitives
{
    public class Spere : Primitive
    {
        public float Radius;
        private int[] indices;
        private int stacks;
        private int slices;

        public Spere(Vector3 center, float radius, int stacks, int slices, Color4 color, Vector3 velocity, float density)
        {
            Pivot = new(center, new());
            Radius = radius;
            this.stacks = stacks;
            this.slices = slices;

            Color = color;
            Velocity = velocity;
            
            IsStatic = false;

            
            LocalVertices = new Vector3[(stacks + 1) * (slices + 1)];

            for (int i = 0; i <= stacks; i++)
            {
                float theta = (float)i * MathF.PI / stacks;
                float sinTheta = MathF.Sin(theta);
                float cosTheta = MathF.Cos(theta);

                for (int j = 0; j <= slices; j++)
                {
                    float phi = (float)j * 2.0f * MathF.PI / slices;
                    float sinPhi = MathF.Sin(phi);
                    float cosPhi = MathF.Cos(phi);

                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;

                    LocalVertices[i * (slices + 1) + j] = new Vector3(x, y, z) * radius;
                }
            }

            indices = new int[stacks * slices * 6];
            int index = 0;
            for (int i = 0; i < stacks; i++)
            {
                for (int j = 0; j < slices; j++)
                {
                    int vertexIndex = i * (slices + 1) + j;

                    indices[index++] = vertexIndex;
                    indices[index++] = vertexIndex + slices + 1;
                    indices[index++] = vertexIndex + 1;

                    indices[index++] = vertexIndex + 1;
                    indices[index++] = vertexIndex + slices + 1;
                    indices[index++] = vertexIndex + slices + 2;
                }
            }

            GlobalVertices = new Vector3[(stacks + 1) * (slices + 1)];
            Polygons = new Polygon3[indices.Length/3];
        }

        public override void OnCollision()
        {

        }

        public override void SolveCollision(Primitive primitive)
        {
            
        }

        public override Polygon3[] GetPolygons()
        {
            for (int i = 0; i < GlobalVertices.Length; ++i)
            {
                GlobalVertices[i] = ToGlobal(LocalVertices[i]);
            }

            for (int i = 0; i < Polygons.Length; ++i)
            {
                Polygons[i] = new(GlobalVertices[indices[i * 3 + 2]], GlobalVertices[indices[i * 3 + 1]], GlobalVertices[indices[i * 3]], Color);
            }
        
            return Polygons;
        }
    }
}
