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
    public class Cube : Primitive
    {
        private float sideLength;
        public Cube(Vector3 center, float sideLength, Color4 color)
        {
            Pivot = new(center, new());
            this.sideLength = sideLength;
            Color = color;
            Speed = Vector3.Zero;
        }

        public Cube(Vector3 center, float sideLength, Color4 color, Vector3 speed)
        {
            Pivot = new(center, new());
            this.sideLength = sideLength;
            Color = color;
            Speed = speed;
        }

        public override void OnCollision()
        {
            Speed = -Speed * 4 / 5;
        }

        public override bool IsCollision(Primitive primitive)
        {
            if (primitive is not Square)
            {
                return false;
            }

            float distance = MathF.Abs(primitive.Pivot.Position.Y - this.Pivot.Position.Y);
            bool isOver = 
                primitive.Pivot.Position.X - 10 < Pivot.Position.X + sideLength/2 && Pivot.Position.X < primitive.Pivot.Position.X + 10 - sideLength/2 &&
                primitive.Pivot.Position.Z - 10 < Pivot.Position.Z + sideLength/2 && Pivot.Position.Z < primitive.Pivot.Position.X + 10 - sideLength/2;
            
            if (!isOver)
            {
                return false;
            }

            if (distance > sideLength/2)
            {
                return false;
            }
            else if (distance == sideLength/2)
            {
                Speed = -Speed * 4 / 5;
            }
            else
            {
                Pivot.Move(Vector3.UnitY*(sideLength/2 - distance));
                float t = (sideLength/2 - distance) / (2 * Speed.Length);
                Speed = new Vector3(Speed.X, -Speed.Y * 4 / 5, Speed.Z);
                Pivot.Move(t * Speed);
            }
                
            return true;
        }

        public override Polygon3[] GetPolygons()
        {
            Vector3[] v = new Vector3[]
            {
                ToGlobal(new(- sideLength/2, + sideLength/2, - sideLength/2)), //0
                ToGlobal(new(- sideLength/2, + sideLength/2, + sideLength/2)), //1
                ToGlobal(new(+ sideLength/2, + sideLength/2, + sideLength/2)), //2
                ToGlobal(new(+ sideLength/2, + sideLength/2, - sideLength/2)), //3
                 
                ToGlobal(new(- sideLength/2, - sideLength/2, - sideLength/2)), //4
                ToGlobal(new(- sideLength/2, - sideLength/2, + sideLength/2)), //5
                ToGlobal(new(+ sideLength/2, - sideLength/2, + sideLength/2)), //6
                ToGlobal(new(+ sideLength/2, - sideLength/2, - sideLength/2))  //7
            };

            

            return new Polygon3[]
            {
                new(v[0], v[1], v[2], Color),
                new(v[0], v[2], v[3], Color),
                
                new(v[5], v[1], v[0], Color),
                new(v[5], v[0], v[4], Color),

                new(v[6], v[2], v[1], Color),
                new(v[6], v[1], v[5], Color),

                new(v[7], v[3], v[2], Color),
                new(v[7], v[2], v[6], Color),

                new(v[4], v[0], v[3], Color),
                new(v[4], v[3], v[7], Color),  
                
                new(v[4], v[7], v[6], Color),
                new(v[4], v[6], v[5], Color)
            };
        }
    }
}
