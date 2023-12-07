using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RenderingGL.Engine.Primitives.Base
{
    public class Pivot
    {
        public Vector3 Position { get; private set; }
        public Orientation Orientation { get; private set; }

        public Pivot(Vector3 position, Orientation orientation)
        {
            Position = position;
            Orientation = orientation;
        }

        public void Move(Vector3 v)
        {
            Position += v;
        }

        public void MoveForward(float distance)
        {
            Position += Orientation.Forward * distance;
        }

        public void MoveRight(float distance)
        {
            Position += Orientation.Right * distance;
        }

        public void MoveUp(float distance)
        {
            Position += Vector3.UnitY * distance;
        }

        public void Rotate(Vector3 axis, float angle)
        {
            Orientation.Rotate(axis, angle);
        }

        public void RotateVertical(float angle)
        {
            Orientation.RotateVertical(angle);
        }

        public void RotateHorizontal(float angle)
        {
            Orientation.RotateHorizontal(angle);
        }
    }
}
