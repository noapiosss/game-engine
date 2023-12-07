using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OpenTK.Mathematics;
using static System.Net.Mime.MediaTypeNames;

namespace Rendering.Engine.Primitives
{
    public class Polygon3
    {
        public Vector3 Normal { get; set; }
        public Color4 Color { get; set; }
        public Vector3[] Vertices { get; set; }

        public Polygon3(Vector3 v1, Vector3 v2, Vector3 v3, Color4 color)
        {
            Vertices = new Vector3[] { v1, v2, v3 };
            Normal = Vector3.Normalize(Vector3.Cross(v2 - v1, v3 - v1));
            Color = color;
        }
    }
}
