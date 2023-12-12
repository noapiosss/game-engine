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

        Orientation orientation = new();
        Pivot mainCameraPivot = new(new(0, 0, 0), orientation);
        Camera camera = new(mainCameraPivot, width, height, 0.1f, 100, MathF.PI / 2);

        Primitive[] primitives = new Primitive[]
        {
            new Cube(new Vector3(0, 0, 5), 1, new Color4(1f, 0f, 0f, 1f), new Vector3(0, 0, 0), 1),
            new Spere(new Vector3(0, 0, 3), 1, 20, 20, new Color4(1f, 0.6f, 0f, 1f), Vector3.Zero, 1)
        };

        using (Game game = new(camera, primitives, width, height))
        {
            game.Run();
        } 
    }
}