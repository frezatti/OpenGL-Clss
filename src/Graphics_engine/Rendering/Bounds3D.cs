namespace Graphics_engine;

public readonly struct Bounds3D
{
    public readonly float MinX;
    public readonly float MaxX;
    public readonly float MinY;
    public readonly float MaxY;
    public readonly float MinZ;
    public readonly float MaxZ;

    public Bounds3D(
        float minX,
        float maxX,
        float minY,
        float maxY,
        float minZ,
        float maxZ)
    {
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
        MinZ = minZ;
        MaxZ = maxZ;
    }

    public bool Contains(float x, float y, float z)
    {
        return x >= MinX && x <= MaxX &&
               y >= MinY && y <= MaxY &&
               z >= MinZ && z <= MaxZ;
    }
}
