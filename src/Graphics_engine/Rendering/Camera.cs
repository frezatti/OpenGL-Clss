using OpenTK.Mathematics;

namespace Graphics_engine;

public sealed class Camera
{
    public Vector3 Position { get; set; } = new Vector3(0.0f, 0.0f, 3.0f);
    public Vector3 Target { get; set; } = Vector3.Zero;
    public Vector3 Up { get; set; } = Vector3.UnitY;

    public float FieldOfViewRadians { get; set; } = MathHelper.DegreesToRadians(60.0f);
    public float NearPlane { get; set; } = 0.1f;
    public float FarPlane { get; set; } = 100.0f;

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Target, Up);
    }

    public Matrix4 GetProjectionMatrix(float aspectRatio)
    {
        return Matrix4.CreatePerspectiveFieldOfView(
            FieldOfViewRadians,
            aspectRatio,
            NearPlane,
            FarPlane
        );
    }
}
