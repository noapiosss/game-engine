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
        public float SideLength;

        public Cube(Vector3 center, float sideLength, Color4 color, Vector3 velocity)
        {
            Pivot = new(center, new());
            SideLength = sideLength;
            Color = color;
            Velocity = velocity;
            
            IsStatic = false;

            
            LocalVertices = new Vector3[]
            {
                new(- sideLength/2, + sideLength/2, - sideLength/2), //0
                new(- sideLength/2, + sideLength/2, + sideLength/2), //1
                new(+ sideLength/2, + sideLength/2, + sideLength/2), //2
                new(+ sideLength/2, + sideLength/2, - sideLength/2), //3
                
                new(- sideLength/2, - sideLength/2, - sideLength/2), //4
                new(- sideLength/2, - sideLength/2, + sideLength/2), //5
                new(+ sideLength/2, - sideLength/2, + sideLength/2), //6
                new(+ sideLength/2, - sideLength/2, - sideLength/2)  //7
            };

            GlobalVertices = new Vector3[LocalVertices.Length];
            Polygons = new Polygon3[12];
        }

        public Cube(Vector3 center, float sideLength, Color4 color, Vector3 velocity, float density)
        {
            Pivot = new(center, new());
            SideLength = sideLength;
            Color = color;
            Velocity = velocity;
            Density = density;
            Volume = sideLength * sideLength * sideLength;
            Mass = Density * Volume;
            
            IsStatic = false;

            
            LocalVertices = new Vector3[]
            {
                new(- sideLength/2, + sideLength/2, - sideLength/2), //0
                new(- sideLength/2, + sideLength/2, + sideLength/2), //1
                new(+ sideLength/2, + sideLength/2, + sideLength/2), //2
                new(+ sideLength/2, + sideLength/2, - sideLength/2), //3
                
                new(- sideLength/2, - sideLength/2, - sideLength/2), //4
                new(- sideLength/2, - sideLength/2, + sideLength/2), //5
                new(+ sideLength/2, - sideLength/2, + sideLength/2), //6
                new(+ sideLength/2, - sideLength/2, - sideLength/2)  //7
            };

            GlobalVertices = new Vector3[LocalVertices.Length];
            Polygons = new Polygon3[12];
        }

        public override void OnCollision()
        {
            Velocity = -Velocity * 4 / 5;
        }

        public override void SolveCollision(Primitive primitive)
        {
            if (primitive.IsStatic)
            {

                float distanceS = MathF.Abs(primitive.Pivot.Position.Y - this.Pivot.Position.Y);
                bool isOver = 
                    primitive.Pivot.Position.X - 10 - SideLength/2 < Pivot.Position.X && Pivot.Position.X < primitive.Pivot.Position.X + 10 + SideLength/2 &&
                    primitive.Pivot.Position.Z - 10 - SideLength/2 < Pivot.Position.Z && Pivot.Position.Z < primitive.Pivot.Position.X + 10 + SideLength/2;
                
                if (!isOver)
                {
                    return;
                }

                if (distanceS > SideLength/2)
                {
                    return;
                }
                else if (distanceS == SideLength/2)
                {
                    Velocity = -Velocity * 4 / 5;
                }
                else
                {
                    Pivot.Move(Vector3.UnitY*(SideLength/2 - distanceS));
                    float t = (SideLength/2 - distanceS) / (2 * Velocity.Length);
                    Velocity = new Vector3(Velocity.X, -Velocity.Y * 4 / 5, Velocity.Z);
                    Pivot.Move(t * Velocity);
                }
                    
                return;
            }

            if (CubeCubeIntersection.BoundingBoxIntersect(this, (Cube)primitive))
            {
                if (CubeCubeIntersection.PreciseCollisionDetection(this, (Cube)primitive))
                {
                    var collisionNormal = CubeCubeIntersection.CalculateCollisionNormal(this, (Cube)primitive);
                    CubeCubeIntersection.ResolvePenetration(this, (Cube)primitive, collisionNormal);
                    CubeCubeIntersection.AdjustSpeedsWithMass(this, (Cube)primitive, collisionNormal); // Optional
                }
            }
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
