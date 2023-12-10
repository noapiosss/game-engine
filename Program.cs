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
        orientation.RotateVertical(MathF.PI / 4);
        Pivot mainCameraPivot = new(new(0, 4, -6), orientation);
        Camera camera = new(mainCameraPivot, width, height, 0.1f, 1000, MathF.PI / 2);

        Primitive[] primitives = new Primitive[]
        {
            // new Cube(new Vector3(0, 0.1f, 2), 1, new Color4(0.5f, 0.4f, 0.2f, 1f), new Vector3(-0.0001f, 0, 0)),
            // new Cube(new Vector3(-2, 0.2f, 2), 1, new Color4(0.2f, 0.4f, 0.4f, 1f), new Vector3(0.0001f, 0, 0)),
            // new Cube(new Vector3(2, 0.3f, 2), 1, new Color4(0.4f, 0.3f, 0.5f, 1f), new Vector3(-0.0001f, 0, 0.0001f)),

            // new Cube(new Vector3(0, 0.4f, 4), 1, new Color4(0.4f, 0.2f, 0.1f, 1f), new Vector3(-0.0001f, 0, -0.0001f)),
            // new Cube(new Vector3(-2, 0.5f, 4), 1, new Color4(0.4f, 0.4f, 0.5f, 1f), new Vector3(-0.0001f, 0, 0)),
            // new Cube(new Vector3(2, 0.6f, 4), 1, new Color4(0.5f, 0.4f, 0.3f, 1f), new Vector3(-0.0001f, 0, 0.0001f)),

            // new Cube(new Vector3(0, 0.7f, 6), 1, new Color4(0.2f, 0.6f, 0.3f, 1f), new Vector3(-0.0001f, 0, -0.0001f)),
            // new Cube(new Vector3(-2, 0.8f, 6), 1, new Color4(0.6f, 0.2f, 0.6f, 1f), new Vector3(-0.0001f, 0, 0)),
            // new Cube(new Vector3(2, 0.9f, 6), 1, new Color4(0.4f, 0.1f, 0.8f, 1f), new Vector3(-0.0001f, 0, 0)),
            new Cube(new Vector3(-4, -4 + 3f/2, 2), 3, new Color4(0.5f, 0.4f, 0.2f, 1f), new Vector3(0.0001f, 0, 0), 1),
            new Cube(new Vector3(0, -4, 2), 1, new Color4(0.2f, 0.4f, 0.4f, 1f), new Vector3(-0.002f, 0, 0), 1),
            new Cube(new Vector3(4, -4 + 3f/2, 2), 3, new Color4(0.4f, 0.3f, 0.5f, 1f), new Vector3(-0.0001f, 0, 0), 1),
            
            new Square(new(new(0,-4,0), new()), new Color4(0.6f, 0.6f, 0.6f, 1f))
        };

        using (Game game = new(camera, primitives, width, height))
        {
            game.Run();
        } 
    }
}