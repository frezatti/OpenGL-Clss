using OpenTK.Mathematics;

namespace Graphics_engine;

public class Transform
{
    public Vector3 Position { get; set; } = Vector3.Zero;
    public Vector3 Scale { get; set; } = Vector3.One;
    public float Rotation { get; set; } = 0.0f;
}
