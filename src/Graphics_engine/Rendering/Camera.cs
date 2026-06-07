using System.Numerics;

namespace Graphics_engine;

public sealed class Camera
{
    public Vector3 Position { get; set; } = new(0.0f, 0.0f, 3.0f);
    public Vector3 Target { get; set; } = Vector3.Zero;
    public Vector3 Up { get; set; } = Vector3.UnitY;

    public float FieldOfViewRadians { get; set; } = MathF.PI / 3.0f;
    public float NearPlane { get; set; } = 0.1f;
    public float FarPlane { get; set; } = 100.0f;

    public Matrix4x4 GetViewMatrix()
    {
        return Matrix4x4.CreateLookAt(Position, Target, Up);
    }

    public Matrix4x4 GetProjectionMatrix(float aspectRatio)
    {
        return Matrix4x4.CreatePerspectiveFieldOfView(
            FieldOfViewRadians,
            aspectRatio,
            NearPlane,
            FarPlane
        );
    }
}
