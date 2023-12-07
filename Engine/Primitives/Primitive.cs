using OpenTK.Mathematics;
using Rendering.Engine.Primitives;
using RenderingGL.Engine.Primitives.Base;

namespace RenderingGL.Engine.Primitives
{
    public abstract class Primitive
    {
        public Pivot Pivot { get; set; }
        public Color4 Color { get; set; }
        public Vector3 Speed { get; set; }

        public abstract Polygon3[] GetPolygons();
        public void Move()
        {
            Pivot.Move(Speed);
        }

        public abstract void OnCollision();
        public abstract bool IsCollision(Primitive primitive);

        protected Vector3 ToGlobal(Vector3 point)
        {
            Matrix4 t = Matrix4.Identity;

            t.M11 = Pivot.Orientation.Right.X;
            t.M12 = Pivot.Orientation.Right.Y;
            t.M13 = Pivot.Orientation.Right.Z;

            t.M21 = Pivot.Orientation.Up.X;
            t.M22 = Pivot.Orientation.Up.Y;
            t.M23 = Pivot.Orientation.Up.Z;

            t.M31 = Pivot.Orientation.Forward.X;
            t.M32 = Pivot.Orientation.Forward.Y;
            t.M33 = Pivot.Orientation.Forward.Z;

            Vector4 result = new Vector4(point, 1f) * t;            

            return result.Xyz + Pivot.Position;
        }
    }
}