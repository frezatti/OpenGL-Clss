using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace Graphics_engine.Scenes;


public static class SceneObjectFactory
{
    public static SceneObject CreateCube(
        string name,
        Vector3 position,
        Vector3 scale,
        Vector3 rotation,
        Vector4 color)
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
            Material = new Material(color, ColorMode.Tinted)
        };
    }

    public static SceneObject CreateGrid(
        string name,
        Vector3 position,
        Vector3 scale,
        Vector3 rotation,
        Vector4 color)
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
            Material = new Material(color, ColorMode.Tinted)
        };
    }
}
