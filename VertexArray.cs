using System;
using OpenTK.Graphics.OpenGL4;

namespace RenderingGL
{
    public sealed class VertexArray : IDisposable
    {
        private bool disposed;
        public readonly int VertexArrayHandler;
        public readonly VertexBuffer VertexBuffer;
        
        public VertexArray(VertexBuffer vertexBuffer)
        {
            if (vertexBuffer is null)
            {
                throw new NullReferenceException(nameof(vertexBuffer));
            }

            disposed = false;
            VertexBuffer = vertexBuffer;

            int vertexSizeInBytes = VertexBuffer.VertexInfo.SizeInBytes;
            VertexAttribute[] attributes = VertexBuffer.VertexInfo.VertexAttributes;

            VertexArrayHandler = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayHandler);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer.VertexBufferHandler);

            foreach (VertexAttribute attribute in attributes)
            {
                GL.VertexAttribPointer(attribute.Index, attribute.ComponentCount, VertexAttribPointerType.Float, false, vertexSizeInBytes, attribute.Offset);
                GL.EnableVertexAttribArray(attribute.Index);
            }            

            GL.BindVertexArray(0);
        }

        ~VertexArray()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(VertexArrayHandler);

            disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}