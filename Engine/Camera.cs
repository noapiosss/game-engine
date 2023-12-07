using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using RenderingGL.Engine.Primitives.Base;
using RenderingGL.Engine.Primitives.Extensions;

namespace RenderingGL.Engine
{
    public class Camera
    {
        public Pivot Pivot { get; set; }
        public float FovX { get; set; }
        public float FovY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float AspectRatio { get; set; }
        public float NearPlane { get; set; }
        public float FarPlane { get; set; }

        public Camera(Pivot pivot, int width, int height, float nearPlane, float farPlane, float fovX)
        {
            Pivot = pivot;
            Width = width;
            Height = height;
            AspectRatio = (float) height / width; 
            FovX = fovX;
            FovY = 2 * MathF.Atan(MathF.Tan(fovX / 2) / AspectRatio);
            NearPlane = nearPlane;
            FarPlane = farPlane;
        }

        public void UpdateResolution(int height, int width)
        {          
            Width = width;
            Height = height;       
            AspectRatio = (float) height / width;            
            FovY = 2 * MathF.Atan(MathF.Tan(FovX / 2) / AspectRatio);
        }

        public Vector3 GetNormalLeft()
        {
            Quaternion rotation = Quaternion.FromAxisAngle(Pivot.Orientation.Up, -FovX / 2);
            return Vector3.Transform(Pivot.Orientation.Right, rotation);
        }

        public Vector3 GetNormalRight()
        {
            Quaternion rotation = Quaternion.FromAxisAngle(Pivot.Orientation.Up, FovX / 2);
            return -Vector3.Transform(Pivot.Orientation.Right, rotation);
        }

        public Vector3 GetNormalBottom()
        {
            Quaternion rotation = Quaternion.FromAxisAngle(Pivot.Orientation.Right, FovY / 2);
            return Vector3.Transform(Pivot.Orientation.Up, rotation);
        }

        public Vector3 GetNormalTop()
        {
            Quaternion rotation = Quaternion.FromAxisAngle(Pivot.Orientation.Right, -FovY / 2);
            return -Vector3.Transform(Pivot.Orientation.Up, rotation);
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Pivot.Position, Pivot.Position + Pivot.Orientation.Forward, Pivot.Orientation.Up);
        }

        public static Matrix4 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            return Matrix4.LookAt(cameraPosition, cameraTarget, cameraUpVector);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(FovY, AspectRatio, NearPlane, FarPlane);
        }

        public void LookUp(float angle)
        {
            Vector3 forward = new(Pivot.Orientation.Forward.X, Pivot.Orientation.Forward.Y, Pivot.Orientation.Forward.Z);
            float nextAngle = forward.Angle(Vector3.UnitY) + angle;
            if (nextAngle is < 0 or > MathF.PI)
            {
                return;
            }

            Pivot.Orientation.RotateVertical(angle);
        }

        public void LookSide(float angle)
        {
            Pivot.RotateHorizontal(angle);
        }

        public void MoveForward(float distance)
        {
            Pivot.MoveForward(distance);
        }

        public void MoveUp(float distance)
        {
            Pivot.MoveUp(distance);
        }

        public void MoveRight(float distance)
        {
            Pivot.MoveRight(distance);
        }
    }
}
