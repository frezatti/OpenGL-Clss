using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace Graphics_engine.Scenes;

public static class SceneObjectFactory
{
    private const string MetalMaterialDirectory = "Assets/Textures/Poliigon_MetalSteelBrushed_7174";
    private const string SphereTexturePath = "Assets/Textures/wood.png";

    public static SceneObject CreateCube(string name, Vector3 position, Vector3 scale, Vector3 rotation, Vector4 color)
    {
        return CreateBoxObject(name, position, scale, rotation, color, CreateMetalMaterial(color));
    }

    public static SceneObject CreateBox(string name, Vector3 position, Vector3 scale, Vector3 rotation, Vector4 color)
    {
        return CreateBoxObject(name, position, scale, rotation, color, new Material(color, ColorMode.Tinted));
    }

    private static SceneObject CreateBoxObject(string name, Vector3 position, Vector3 scale, Vector3 rotation, Vector4 color, Material material)
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
            Material = material,
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

    private static Material CreateMetalMaterial(Vector4 color)
    {
        return Material.CreatePbr(
            color,
            ColorMode.Tinted,
            CombineMaterialPath("Poliigon_MetalSteelBrushed_7174_BaseColor.jpg"),
            CombineMaterialPath("Poliigon_MetalSteelBrushed_7174_Metallic.jpg"),
            CombineMaterialPath("Poliigon_MetalSteelBrushed_7174_Roughness.jpg"),
            CombineMaterialPath("Poliigon_MetalSteelBrushed_7174_AmbientOcclusion.jpg"),
            new Vector2(3.0f, 3.0f),
            metallic: 1.0f,
            roughness: 0.65f,
            ambientOcclusion: 1.0f
        );
    }

    private static string CombineMaterialPath(string fileName)
    {
        return $"{MetalMaterialDirectory}/{fileName}";
    }
}