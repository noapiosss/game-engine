using System;
using OpenTK.Mathematics;
using Rendering.Engine.Primitives;

namespace RenderingGL.Engine.Primitives
{
    public static class CubeCubeIntersection
    {
        public static bool BoundingBoxIntersect(Cube cube1, Cube cube2)
        {
            // Calculate the extents of each cube
            Vector3 min1 = cube1.Pivot.Position - new Vector3(cube1.SideLength / 2);
            Vector3 max1 = cube1.Pivot.Position + new Vector3(cube1.SideLength / 2);
            Vector3 min2 = cube2.Pivot.Position - new Vector3(cube2.SideLength / 2);
            Vector3 max2 = cube2.Pivot.Position + new Vector3(cube2.SideLength / 2);

            // Check for overlap along each axis
            if (max1.X < min2.X || min1.X > max2.X) return false; // No overlap on X-axis
            if (max1.Y < min2.Y || min1.Y > max2.Y) return false; // No overlap on Y-axis
            if (max1.Z < min2.Z || min1.Z > max2.Z) return false; // No overlap on Z-axis

            // Overlapping along all axes implies intersection
            return true;
        }

        public static bool PreciseCollisionDetection(Cube cube1, Cube cube2)
        {
            // Check for separation along each axis
            for (int i = 0; i < 3; i++) // Three axes (X, Y, Z)
            {
                Vector3 axis = Vector3.UnitX; // Unit vector along X-axis
                if (i == 1) axis = Vector3.UnitY; // Y-axis
                else if (i == 2) axis = Vector3.UnitZ; // Z-axis

                if (!IsAxisOverlapping(cube1, cube2, axis))
                    return false; // Separating axis found
            }

            return true; // No separating axis found, cubes intersect
        }

        private static bool IsAxisOverlapping(Cube cube1, Cube cube2, Vector3 axis)
        {
            // Project vertices of both cubes onto the axis
            float projection1 = ProjectCubeOntoAxis(cube1, axis);
            float projection2 = ProjectCubeOntoAxis(cube2, axis);

            // Check for overlap
            float distance = Math.Abs(projection1 - projection2);
            float combinedExtent = (cube1.SideLength + cube2.SideLength) / 2;

            return distance <= combinedExtent;
        }

        private static float ProjectCubeOntoAxis(Cube cube, Vector3 axis)
        {
            Matrix3 rotation = new Matrix3(cube.Pivot.Orientation.Right, cube.Pivot.Orientation.Up, cube.Pivot.Orientation.Forward);
            Quaternion orientation = Quaternion.FromMatrix(rotation);

            // Project cube's vertices onto the axis and return min/max projection
            Vector3 halfExtents = new Vector3(cube.SideLength / 2);
            Vector3[] vertices = new Vector3[]
            {
                cube.Pivot.Position + Vector3.Transform(-halfExtents, orientation),
                cube.Pivot.Position + Vector3.Transform(new Vector3(halfExtents.X, -halfExtents.Y, -halfExtents.Z), orientation),
                cube.Pivot.Position + Vector3.Transform(new Vector3(-halfExtents.X, halfExtents.Y, -halfExtents.Z), orientation),
                cube.Pivot.Position + Vector3.Transform(new Vector3(-halfExtents.X, -halfExtents.Y, halfExtents.Z), orientation),
                cube.Pivot.Position + Vector3.Transform(new Vector3(halfExtents.X, halfExtents.Y, -halfExtents.Z), orientation),
                cube.Pivot.Position + Vector3.Transform(new Vector3(-halfExtents.X, halfExtents.Y, halfExtents.Z), orientation),
                cube.Pivot.Position + Vector3.Transform(new Vector3(halfExtents.X, -halfExtents.Y, halfExtents.Z), orientation),
                cube.Pivot.Position + Vector3.Transform(halfExtents, orientation)
            };

            float min = float.MaxValue;
            float max = float.MinValue;

            foreach (Vector3 vertex in vertices)
            {
                float projection = Vector3.Dot(vertex, axis);
                if (projection < min) min = projection;
                if (projection > max) max = projection;
            }

            return min;
        }

        public static Vector3 CalculateCollisionNormal(Cube cube1, Cube cube2)
        {
            // Calculate the vector between the centers of the cubes
            Vector3 centerOffset = cube2.Pivot.Position - cube1.Pivot.Position;

            // Calculate the minimum penetration depth among axes
            float minPenetrationDepth = float.MaxValue;
            Vector3 collisionNormal = Vector3.Zero;

            Vector3[] axes = new Vector3[]
            {
                Vector3.UnitX, -Vector3.UnitX,
                Vector3.UnitY, -Vector3.UnitY,
                Vector3.UnitZ, -Vector3.UnitZ
            };

            foreach (Vector3 axis in axes)
            {
                // Project the cubes onto the axis
                float projection1 = ProjectCubeOntoAxis(cube1, axis);
                float projection2 = ProjectCubeOntoAxis(cube2, axis);

                // Calculate penetration depth along this axis
                float penetrationDepth = Math.Abs(projection1 - projection2);

                if (penetrationDepth < minPenetrationDepth)
                {
                    minPenetrationDepth = penetrationDepth;
                    collisionNormal = axis;
                }
            }

            return collisionNormal;
        }

        public static void ResolvePenetration(Cube cube1, Cube cube2, Vector3 collisionNormal)
        {
            // Calculate the vector between the centers of the cubes
            Vector3 centerOffset = cube2.Pivot.Position - cube1.Pivot.Position;

            // Calculate the penetration depth
            float penetrationDepth = CalculatePenetrationDepth(cube1, cube2, collisionNormal, centerOffset);

            // Move the cubes back along the collision normal
            MoveCubesBack(cube1, cube2, collisionNormal, penetrationDepth);
        }

        private static float CalculatePenetrationDepth(Cube cube1, Cube cube2, Vector3 collisionNormal, Vector3 centerOffset)
        {
            // Calculate the relative velocity if needed (for dynamic simulations)
            Vector3 relativeVelocity = cube2.Velocity - cube1.Velocity;

            // Calculate the relative velocity along the collision normal
            float relativeSpeedAlongNormal = Vector3.Dot(relativeVelocity, collisionNormal);
            float coefficientOfRestitution = 0.5f;

            // Calculate penetration depth based on relative speed (for dynamic simulations)
            float penetrationDepth = -(1 + coefficientOfRestitution) * relativeSpeedAlongNormal;

            // You might need additional calculations based on your specific simulation

            return penetrationDepth;
        }

        private static void MoveCubesBack(Cube cube1, Cube cube2, Vector3 collisionNormal, float penetrationDepth)
        {
            // Move the cubes back along the collision normal
            Vector3 moveVector = collisionNormal * penetrationDepth;

            // Move cube 1 back by half the distance and cube 2 forward by half the distance
            cube1.Pivot.Move(- moveVector * cube2.Mass / (cube1.Mass + cube2.Mass));
            cube2.Pivot.Move(moveVector * cube1.Mass / (cube1.Mass + cube2.Mass));
        }

        public static void AdjustSpeeds(Cube cube1, Cube cube2, Vector3 collisionNormal)
        {
            // Optional: Adjust speeds/velocities based on collision
            Vector3 scalar = new
            (
                MathF.Sqrt(cube1.Velocity.X * cube1.Velocity.X + cube2.Velocity.X * cube2.Velocity.X),
                MathF.Sqrt(cube1.Velocity.Y * cube1.Velocity.Y + cube2.Velocity.Y * cube2.Velocity.Y),
                MathF.Sqrt(cube1.Velocity.Z * cube1.Velocity.Z + cube2.Velocity.Z * cube2.Velocity.Z)
            );

            Vector3 v2 = cube1.Velocity + cube2.Velocity + scalar;
            Vector3 v1 = cube1.Velocity + cube2.Velocity - v2;

            cube1.Velocity = v2;
            cube2.Velocity = v1;
        }

        public static void AdjustSpeedsWithMass(Cube cube1, Cube cube2, Vector3 collisionNormal)
        {
            // Optional: Adjust speeds/velocities based on collision
            Vector3 scalar = new
            (
                MathF.Sqrt(cube1.Velocity.X * cube1.Velocity.X + 2 * cube1.Velocity.X * cube2.Velocity.X + cube2.Velocity.X * cube2.Velocity.X),
                MathF.Sqrt(cube1.Velocity.Y * cube1.Velocity.Y + 2 * cube1.Velocity.Y * cube2.Velocity.Y + cube2.Velocity.Y * cube2.Velocity.Y),
                MathF.Sqrt(cube1.Velocity.Z * cube1.Velocity.Z + 2 * cube1.Velocity.Z * cube2.Velocity.Z + cube2.Velocity.Z * cube2.Velocity.Z)
            );

            Vector3 v2 = (cube1.Velocity * cube1.Mass + cube2.Velocity * cube2.Mass + scalar * cube1.Mass) / (cube1.Mass + cube2.Mass);
            Vector3 v1 = cube1.Velocity + (cube2.Velocity - v2) * cube2.Mass / cube1.Mass;

            cube1.Velocity = v1;
            cube2.Velocity = v2;
            
            cube1.Move();
            cube2.Move();
        }
    }
}