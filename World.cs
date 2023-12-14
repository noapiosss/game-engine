using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using Rendering.Engine.Primitives;
using RenderingGL.Engine.Primitives;

namespace RenderingGL
{
    public class World
    {
        private List<Primitive> primitives;
        private Vector3 gravity = new Vector3(0, -0.0000032f, 0);

        public World(ICollection<Primitive> primitives, Vector3 gravity)
        {
            this.primitives = primitives.ToList();
            if (gravity != Vector3.Zero)
            {
                this.gravity = gravity;
            }
        }

        public void AddPrimitive(Primitive primitive)
        {
            primitives.Add(primitive);
        }

        public bool RemovePrimitive(Primitive primitive)
        {
            return primitives.Remove(primitive);
        }

        public Primitive GetPrimitive(int index)
        {
            return primitives[index];
        }

        public List<Primitive> GetPrimitives()
        {
            return primitives;
        }

        public void Tick()
        {
            foreach (Primitive primitive in primitives)
            {
                primitive.Velocity += gravity;
                primitive.Move();
            }

            for (int i = 0; i < primitives.Count - 1; ++i)
            {
                for (int j = i + 1; j < primitives.Count; ++j)
                {
                    if (primitives[i].IsStatic && primitives[j].IsStatic)
                    {
                        continue;
                    }

                    if (Collide(primitives[i], primitives[j], out Vector3 normal, out float depth))
                    {
                        if (primitives[i].IsStatic)
                        {
                            primitives[j].Pivot.Move(normal * depth);
                        }
                        else if (primitives[j].IsStatic)
                        {
                            primitives[i].Pivot.Move(-normal * depth);
                            primitives[i].Velocity = Reflect(primitives[i].Velocity * 4 / 5, normal);
                        }
                        else
                        {
                            primitives[i].Pivot.Move(-normal * depth / 2);
                            primitives[j].Pivot.Move(normal * depth / 2);
                        }

                        ResolveCollision(primitives[i], primitives[j], normal, depth);
                    }
                }
            }
        }

        private static Vector3 Reflect(Vector3 v, Vector3 normal)
        {
            float dot = Vector3.Dot(v, normal);
            return v - 2 * dot * normal;
        }

        public void ResolveCollision(Primitive bodyA, Primitive bodyB, Vector3 normal, float depth)
        {
            Vector3 relativeVelocity = bodyB.Velocity - bodyA.Velocity;

            if (Vector3.Dot(relativeVelocity, normal) > 0)
            {
                return;
            }

            float e = MathF.Min(bodyA.Restitution, bodyB.Restitution);

            float j = -(1f + e) * Vector3.Dot(relativeVelocity, normal);
            j /= 1 / bodyA.Mass + 1 / bodyB.Mass;

            Vector3 impulse = j * normal;

            bodyA.Velocity -= impulse / bodyA.Mass;
            bodyB.Velocity += impulse / bodyB.Mass;
        }

        private bool Collide(Primitive bodyA, Primitive bodyB, out Vector3 normal, out float depth)
        {
            normal = Vector3.Zero;
            depth = 0;

            if (bodyA is Rectangle3D && bodyB is Rectangle3D)
            {
                if (RectanglesIntersection.IntersectRectangles((Rectangle3D)bodyA, (Rectangle3D)bodyB, out normal, out depth))
                {
                    return true;
                }
            }

            return false;
        }
    }
}