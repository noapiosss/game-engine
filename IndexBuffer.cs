using System;
using OpenTK.Graphics.OpenGL4;

namespace RenderingGL
{
    public sealed class IndexBuffer : IDisposable
    {
        public static readonly int MinIndexCount = 1;
        public static readonly int MaxIndexCount = 250_000;

        private bool disposed;

        public readonly int IndexBufferHandler;
        public readonly int IndexCount;
        public readonly bool IsStatic;

        public IndexBuffer(int indexCount, bool isStatic = true)
        {
            if (indexCount < MinIndexCount || indexCount > MaxIndexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(indexCount));
            }

            disposed = false;
            IndexCount = indexCount;
            IsStatic = isStatic;

            BufferUsageHint hint = isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.StreamDraw;

            IndexBufferHandler = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferHandler);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexCount * sizeof(int), IntPtr.Zero, hint);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        ~IndexBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(IndexBufferHandler);

            disposed = true;
            GC.SuppressFinalize(this);
        }

        public void SetData(int[] data, int count)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            if (count <= 0 || count > IndexCount || count > data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferHandler);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, count * sizeof(int), data);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}