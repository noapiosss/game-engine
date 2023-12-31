using System.Collections.Generic;
using OpenTK.Mathematics;
using Rendering.Engine.Primitives;
using RenderingGL.Engine.Primitives.Base;

namespace RenderingGL.Engine.Primitives
{
    public class Square : Primitive
    {
        private float length;
        public Square(Pivot pivot, Color4 color)
            : base(pivot, color, Vector3.Zero, 0, 1, true)
        {
            length = 20;
        }

        public override Polygon3[] GetPolygons()
        {
            Vector3[] v = new Vector3[]
            {
                ToGlobal(new(-length/2, 0, -length/2)), //0
                ToGlobal(new(-length/2, 0, length/2)), //1
                ToGlobal(new(length/2, 0, length/2)), //2
                ToGlobal(new(length/2, 0, -length/2)) //3
            };


            return new Polygon3[]
            {
                new(v[0], v[1], v[2], Color),
                new(v[0], v[2], v[3], Color)
            };
        }
    }
}
