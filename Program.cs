using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using Rendering.Engine.Primitives;
using RenderingGL;
using RenderingGL.Engine;
using RenderingGL.Engine.Primitives;
using RenderingGL.Engine.Primitives.Base;

internal class Program
{
    private static void Main(string[] args)
    {
        int width = 1280;
        int height = 768;

        Pivot mainCameraPivot = new(new(0, 3, -5), new(Vector3.UnitZ, Vector3.UnitY, Vector3.UnitX));
        Camera camera = new(mainCameraPivot, width, height, 0.1f, 1000, MathF.PI / 2);

        Primitive[] primitives = new Primitive[]
        {
            new Cube(new Vector3(0, 0.1f, 2), 1, new Color4(0.5f, 0.4f, 0.2f, 1f), new Vector3(-0.001f, 0, 0)),
            new Cube(new Vector3(-2, 0.2f, 2), 1, new Color4(0.2f, 0.4f, 0.4f, 1f), new Vector3(-0.001f, 0, 0)),
            new Cube(new Vector3(2, 0.3f, 2), 1, new Color4(0.4f, 0.3f, 0.5f, 1f), new Vector3(-0.001f, 0, 0)),
            new Cube(new Vector3(0, 0.4f, 4), 1, new Color4(0.4f, 0.2f, 0.1f, 1f), new Vector3(-0.001f, 0, 0)),
            new Cube(new Vector3(-2, 0.5f, 4), 1, new Color4(0.4f, 0.4f, 0.5f, 1f), new Vector3(-0.001f, 0, 0)),
            new Cube(new Vector3(2, 0.6f, 4), 1, new Color4(0.5f, 0.4f, 0.3f, 1f), new Vector3(-0.001f, 0, 0)),
            new Cube(new Vector3(0, 0.7f, 6), 1, new Color4(0.2f, 0.6f, 0.3f, 1f), new Vector3(-0.001f, 0, 0)),
            new Cube(new Vector3(-2, 0.8f, 6), 1, new Color4(0.6f, 0.2f, 0.6f, 1f), new Vector3(-0.001f, 0, 0)),
            new Cube(new Vector3(2, 0.9f, 6), 1, new Color4(0.4f, 0.1f, 0.8f, 1f), new Vector3(-0.001f, 0, 0)),
            new Square(new(new(0,-2,0), new()), new Color4(0.6f, 0.6f, 0.6f, 1f))
        };

        using (Game game = new(camera, primitives, width, height))
        {
            game.Run();
        } 
    }
}