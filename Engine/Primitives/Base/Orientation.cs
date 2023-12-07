using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using RenderingGL.Engine.Primitives.Extensions;

namespace RenderingGL.Engine.Primitives.Base
{
    public class Orientation
    {
        public Vector3 Forward { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Right { get; set; }

        public Orientation(Vector3 forward, Vector3 up, Vector3 right)
        {
            Forward = forward;
            Up = up;
            Right = right;
        }

        public Orientation()
        {
            Forward = Vector3.UnitZ;
            Up = Vector3.UnitY;
            Right = Vector3.UnitX;
        }

        public void Rotate(Vector3 axis, float angle)
        {
            Quaternion quaternion = Quaternion.FromAxisAngle(axis, angle);
            Matrix3 rotation = Matrix3.CreateFromQuaternion(quaternion);
            Forward *= rotation;
            Up *= rotation;
            Right *= rotation;
        }

        public void RotateVertical(float angle)
        {
            Quaternion quaternion = Quaternion.FromAxisAngle(new Vector3(Right.X, Right.Y, Right.Z), angle);
            Matrix3 rotation = Matrix3.CreateFromQuaternion(quaternion);
            Forward *= rotation;
            Up *= rotation;
        }

        public void RotateHorizontal(float angle)
        {
            Quaternion quaternion = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), angle);
            Matrix3 rotation = Matrix3.CreateFromQuaternion(quaternion);
            Forward *= rotation;
            Up *= rotation;
            Right *= rotation;
        }
    }
}
