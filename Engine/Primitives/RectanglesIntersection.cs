using System;
using OpenTK.Mathematics;
using Rendering.Engine.Primitives;
using RenderingGL.Engine.Primitives.Base;

namespace RenderingGL.Engine.Primitives
{
    public static class RectanglesIntersection
    {
        public static bool BoundingBoxIntersect(Rectangle3D rect1, Rectangle3D rect2)
        {
            float rect1HalfWidth = rect1.Width / 2f;
            float rect1HalfHeight = rect1.Height / 2f;
            float rect1HalfLength = rect1.Length / 2f;

            float rect2HalfWidth = rect2.Width / 2f;
            float rect2HalfHeight = rect2.Height / 2f;
            float rect2HalfLength = rect2.Length / 2f;

            // Calculate centers of the rectangles
            Vector3 rect1Center = rect1.Pivot.Position;
            Vector3 rect2Center = rect2.Pivot.Position;

            // AABB collision check between rect1 and rect2
            return rect1Center.X - rect1HalfLength < rect2Center.X + rect2HalfLength &&
                   rect1Center.X + rect1HalfLength > rect2Center.X - rect2HalfLength &&
                   rect1Center.Y - rect1HalfHeight < rect2Center.Y + rect2HalfHeight &&
                   rect1Center.Y + rect1HalfHeight > rect2Center.Y - rect2HalfHeight &&
                   rect1Center.Z - rect1HalfWidth < rect2Center.Z + rect2HalfWidth &&
                   rect1Center.Z + rect1HalfWidth > rect2Center.Z - rect2HalfWidth;
        }

        public static bool IntersectRectangles(Rectangle3D rect1, Rectangle3D rect2, out Vector3 normal, out float depth)
        {
            normal = Vector3.Zero;
            depth = float.MaxValue;

            Vector3[] axises = rect1.GetEdges();

            foreach(Vector3 axis in axises)
            {
                ProjectRectangleOnAxis(rect1, axis, out float minA, out float maxA);
                ProjectRectangleOnAxis(rect2, axis, out float minB, out float maxB);

                if (minA >= maxB || minB >= maxA)
                {
                    return false;
                }

                float axisDepth = MathF.Min(maxB - minA, maxA - minB);
                
                if (axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }

            axises = rect2.GetEdges();

            foreach(Vector3 axis in axises)
            {
                ProjectRectangleOnAxis(rect1, axis, out float minA, out float maxA);
                ProjectRectangleOnAxis(rect2, axis, out float minB, out float maxB);

                if (minA >= maxB || minB >= maxA)
                {
                    return false;
                }

                float axisDepth = MathF.Min(maxB - minA, maxA - minB);
                
                if (axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }
            
            Vector3 direction = rect2.Pivot.Position - rect1.Pivot.Position;

            if (Vector3.Dot(direction, normal) < 0)
            {
                normal = -normal;
            }



            return true;
        }

        private static void ProjectRectangleOnAxis(Rectangle3D rect, Vector3 axis, out float min, out float max)
        {
            // Get the center of the rectangle
            Vector3 center = rect.Pivot.Position;

            // Calculate half-width, half-height, and half-length
            float halfWidth = rect.Width / 2f;
            float halfHeight = rect.Height / 2f;
            float halfLength = rect.Length / 2f;

            // Calculate the projection of the rectangle's center on the axis
            float centerProjection = Vector3.Dot(center, axis);

            // Calculate the projection extents based on the rectangle's size and axis direction
            float projectionExtent = Math.Abs(halfLength * Vector3.Dot(axis, rect.Pivot.Orientation.Right)) +
                                    Math.Abs(halfHeight * Vector3.Dot(axis, rect.Pivot.Orientation.Up)) +
                                    Math.Abs(halfWidth * Vector3.Dot(axis, rect.Pivot.Orientation.Forward));

            // Calculate the min and max values of the projection
            min = centerProjection - projectionExtent;
            max = centerProjection + projectionExtent;
        }
    }
}