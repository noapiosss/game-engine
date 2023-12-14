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
    public class Rectangle3D : Primitive
    {
        public float Height;
        public float Width;
        public float Length;

        public Vector3[] Edges;

        public Rectangle3D(Vector3 center, float height, float width, float length, Color4 color, Vector3 velocity, float restitution, float density, bool isStatic)
            : base(new(center, new()), color, velocity, density, restitution, isStatic)
        {
            Height = height;
            Width = width;
            Length = length;
            Volume = height * width * length;
            Mass = density * Volume;
            
            LocalVertices = new Vector3[]
            {
                new(- length/2, + height/2, - width/2), //0
                new(- length/2, + height/2, + width/2), //1
                new(+ length/2, + height/2, + width/2), //2
                new(+ length/2, + height/2, - width/2), //3
                
                new(- length/2, - height/2, - width/2), //4
                new(- length/2, - height/2, + width/2), //5
                new(+ length/2, - height/2, + width/2), //6
                new(+ length/2, - height/2, - width/2)  //7
            };

            GlobalVertices = new Vector3[LocalVertices.Length];
            Edges = new Vector3[12];
            Polygons = new Polygon3[12];
        }

        public Vector3[] GetGlobalVertices()
        {
            for (int i = 0; i < GlobalVertices.Length; ++i)
            {
                GlobalVertices[i] = ToGlobal(LocalVertices[i]);
            }

            return GlobalVertices;
        }

        public Vector3[] GetEdges()
        {
            for (int i = 0; i < GlobalVertices.Length; ++i)
            {
                GlobalVertices[i] = ToGlobal(LocalVertices[i]);
            }

            Edges[0] = Vector3.Normalize(GlobalVertices[0] - GlobalVertices[1]);
            Edges[1] = Vector3.Normalize(GlobalVertices[1] - GlobalVertices[2]);
            Edges[2] = Vector3.Normalize(GlobalVertices[2] - GlobalVertices[3]);
            Edges[3] = Vector3.Normalize(GlobalVertices[3] - GlobalVertices[0]);
            Edges[4] = Vector3.Normalize(GlobalVertices[4] - GlobalVertices[5]);
            Edges[5] = Vector3.Normalize(GlobalVertices[5] - GlobalVertices[6]);
            Edges[6] = Vector3.Normalize(GlobalVertices[6] - GlobalVertices[7]);
            Edges[7] = Vector3.Normalize(GlobalVertices[7] - GlobalVertices[4]);
            Edges[8] = Vector3.Normalize(GlobalVertices[0] - GlobalVertices[4]);
            Edges[9] = Vector3.Normalize(GlobalVertices[1] - GlobalVertices[5]);
            Edges[10] = Vector3.Normalize(GlobalVertices[2] - GlobalVertices[6]);
            Edges[11] = Vector3.Normalize(GlobalVertices[3] - GlobalVertices[7]);
        
            return Edges;
        }

        public override Polygon3[] GetPolygons()
        {
            for (int i = 0; i < GlobalVertices.Length; ++i)
            {
                GlobalVertices[i] = ToGlobal(LocalVertices[i]);
            }

            Polygons[0] = new(GlobalVertices[0], GlobalVertices[1], GlobalVertices[2], Color);
            Polygons[1] = new(GlobalVertices[0], GlobalVertices[2], GlobalVertices[3], Color);
            Polygons[2] = new(GlobalVertices[5], GlobalVertices[1], GlobalVertices[0], Color);
            Polygons[3] = new(GlobalVertices[5], GlobalVertices[0], GlobalVertices[4], Color);
            Polygons[4] = new(GlobalVertices[6], GlobalVertices[2], GlobalVertices[1], Color);
            Polygons[5] = new(GlobalVertices[6], GlobalVertices[1], GlobalVertices[5], Color);
            Polygons[6] = new(GlobalVertices[7], GlobalVertices[3], GlobalVertices[2], Color);
            Polygons[7] = new(GlobalVertices[7], GlobalVertices[2], GlobalVertices[6], Color);
            Polygons[8] = new(GlobalVertices[4], GlobalVertices[0], GlobalVertices[3], Color);
            Polygons[9] = new(GlobalVertices[4], GlobalVertices[3], GlobalVertices[7], Color);
            Polygons[10] = new(GlobalVertices[4], GlobalVertices[7], GlobalVertices[6], Color);
            Polygons[11] = new(GlobalVertices[4], GlobalVertices[6], GlobalVertices[5], Color);
        
            return Polygons;
        }
    }
}
