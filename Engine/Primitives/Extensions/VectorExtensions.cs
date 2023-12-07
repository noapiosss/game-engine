using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RenderingGL.Engine.Primitives.Extensions
{
    public static class Vector3Extension
    {
        public static float Angle(this Vector3 vector1, Vector3 vector2)
        {
            return MathF.Acos(Vector3.Dot(vector1, vector2) / (vector1.Length * vector2.Length));
        }
    }
}
