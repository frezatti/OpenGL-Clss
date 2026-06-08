using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace Graphics_engine.Scenes;

public static class SceneObjectFactory
{
    private const string CubeTexturePath = "Assets/Textures/metal.png";
    private const string SphereTexturePath = "Assets/Textures/wood.png";

    public static SceneObject CreateCube(string name, Vector3 position, Vector3 scale, Vector3 rotation, Vector4 color)
    {
        return CreateBoxObject(name, position, scale, rotation, color, CubeTexturePath, new Vector2(3.0f, 3.0f));
    }

    public static SceneObject CreateBox(string name, Vector3 position, Vector3 scale, Vector3 rotation, Vector4 color)
    {
        return CreateBoxObject(name, position, scale, rotation, color, null, Vector2.One);
    }

    private static SceneObject CreateBoxObject(string name, Vector3 position, Vector3 scale, Vector3 rotation, Vector4 color, string? texturePath, Vector2 textureScale)
    {
        return new SceneObject
        {
            Name = name,
            Mesh = MeshFactory.CreateCube(1.0f, new Vector3(color.X, color.Y, color.Z)),
            PrimitiveType = PrimitiveType.Triangles,
            Transform = new Transform
            {
                Position = position,
                Scale = scale,
                Rotation = rotation
            },
            Material = new Material(color, ColorMode.Tinted, texturePath, textureScale),
            Selectable = true
        };
    }

    public static SceneObject CreateGrid(string name, Vector3 position, Vector3 scale, Vector3 rotation, Vector4 color)
    {
        return new SceneObject
        {
            Name = name,
            Mesh = MeshFactory.CreateGrid(),
            PrimitiveType = PrimitiveType.Lines,
            Transform = new Transform
            {
                Position = position,
                Scale = scale,
                Rotation = rotation
            },
            Material = new Material(color, ColorMode.Tinted),
            Selectable = false
        };
    }

    public static SceneObject CreatePyramid(string name, Vector3 position, Vector3 scale, Vector3 rotation, Vector4 color)
    {
        return new SceneObject
        {
            Name = name,
            Mesh = MeshFactory.CreatePyramid(1.0f, 1.0f, new Vector3(color.X, color.Y, color.Z)),
            PrimitiveType = PrimitiveType.Triangles,
            Transform = new Transform
            {
                Position = position,
                Scale = scale,
                Rotation = rotation
            },
            Material = new Material(color, ColorMode.Tinted),
            Selectable = true
        };
    }

    public static SceneObject CreateCylinder(string name, Vector3 position, Vector3 scale, Vector3 rotation, Vector4 color, int segments = 32)
    {
        return new SceneObject
        {
            Name = name,
            Mesh = MeshFactory.CreateCylinder(0.5f, 1.0f, segments, new Vector3(color.X, color.Y, color.Z)),
            PrimitiveType = PrimitiveType.Triangles,
            Transform = new Transform
            {
                Position = position,
                Scale = scale,
                Rotation = rotation
            },
            Material = new Material(color, ColorMode.Tinted),
            Selectable = true
        };
    }

    public static SceneObject CreateSphere(string name, Vector3 position, Vector3 scale, Vector3 rotation, Vector4 color, int sectors = 32, int stacks = 16)
    {
        return new SceneObject
        {
            Name = name,
            Mesh = MeshFactory.CreateUVSphere(0.5f, sectors, stacks, new Vector3(color.X, color.Y, color.Z)),
            PrimitiveType = PrimitiveType.Triangles,
            Transform = new Transform
            {
                Position = position,
                Scale = scale,
                Rotation = rotation
            },
            Material = new Material(color, ColorMode.Tinted, SphereTexturePath, new Vector2(1.0f, 1.0f)),
            Selectable = true
        };
    }
}
