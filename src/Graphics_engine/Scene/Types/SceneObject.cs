using OpenTK.Graphics.OpenGL4;

namespace Graphics_engine;

public readonly record struct ObjectId(int Value);

[Flags]
public enum ObjectDirtyFlags
{
    None = 0,
    Transform = 1 << 0,
    Material = 1 << 1,
    Mesh = 1 << 2,
    Visibility = 1 << 3,
    Created = 1 << 4,
    Deleted = 1 << 5,
    Selection = 1 << 6,
}

public sealed class SceneObject
{
    public ObjectId Id { get; set; }

    public string Name { get; set; } = "";

    public Mesh Mesh { get; set; } = new Mesh();

    public Transform Transform { get; set; } = new Transform();

    public Material Material { get; set; } = Material.Default();

    public PrimitiveType PrimitiveType { get; set; }

    public ObjectDirtyFlags DirtyFlags { get; set; } = ObjectDirtyFlags.Created;

    public bool Visible { get; set; } = true;

    public bool Selected { get; set; } = false;
}
