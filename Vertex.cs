using System;
using OpenTK.Mathematics;

namespace RenderingGL
{
    public readonly struct VertexAttribute
    {
        public readonly string Name;
        public readonly int Index;
        public readonly int ComponentCount;
        public readonly int Offset;

        public VertexAttribute(string name, int index, int componentCount, int offset)
        {
            Name = name;
            Index = index;
            ComponentCount = componentCount;
            Offset = offset;
        }
    }

    public sealed class VertexInfo
    {
        public readonly Type Type;
        public readonly int SizeInBytes;
        public readonly VertexAttribute[] VertexAttributes;

        public VertexInfo(Type type, params VertexAttribute[] attribures)
        {
            Type = type;
            SizeInBytes = 0;

            VertexAttributes = attribures;

            for (int i = 0; i < VertexAttributes.Length; ++i)
            {
                VertexAttribute attribute = VertexAttributes[i];
                SizeInBytes += attribute.ComponentCount * sizeof(float);
            }
        }
    }

    public readonly struct VertexPositionColor
    {
        public readonly Vector3 Position;
        public readonly Color4 Color;
        public readonly Vector3 Normal;
        public readonly Vector3 Barycentric;

        public static readonly VertexInfo VertexInfo = new
        (
            typeof(VertexPositionColor),
            new VertexAttribute("Position", 0, 3, 0),
            new VertexAttribute("Color", 1, 4, 3 * sizeof(float)),
            new VertexAttribute("Normal", 2, 3, 7 * sizeof(float)),
            new VertexAttribute("Barycentric", 3, 3, 10 *sizeof(float))
        );

        public VertexPositionColor(Vector3 position, Color4 color, Vector3 normal, Vector3 barycentric)
        {
            Position = position;
            Color = color;
            Normal = normal;
            Barycentric = barycentric;
        }
    }

    public readonly struct VertexPositionTexture
    {
        public readonly Vector3 Position;
        public readonly Vector2 TexCoord;        
        public readonly Vector3 Normal;

        public static readonly VertexInfo VertexInfo = new
        (
            typeof(VertexPositionTexture),
            new VertexAttribute("Position", 0, 3, 0),
            new VertexAttribute("TexCoord", 1, 2, 3 * sizeof(float)),
            new VertexAttribute("Normal", 2, 3, 7 * sizeof(float))
        );

        public VertexPositionTexture(Vector3 position, Vector2 texCoord)
        {
            Position = position;
            TexCoord = texCoord;
        }
    }
}