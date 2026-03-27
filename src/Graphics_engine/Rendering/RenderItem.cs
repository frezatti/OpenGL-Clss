using OpenTK.Graphics.OpenGL4;
namespace Graphics_engine;

public class RenderItem
{
    public Mesh Mesh { get; set; } = new Mesh();
    public Transform Transfom { get; set; } = new Transform();
    public PrimitiveType Rendering_Type { get; set; }
}
