namespace Graphics_engine;

public class Mesh
{
    public const int PositionFloatCount = 3;
    public const int ColorFloatCount = 3;
    public const int NormalFloatCount = 3;
    public const int TextureCoordinateFloatCount = 2;

    public const int PositionOffset = 0;
    public const int ColorOffset = PositionOffset + PositionFloatCount;
    public const int NormalOffset = ColorOffset + ColorFloatCount;
    public const int TextureCoordinateOffset = NormalOffset + NormalFloatCount;
    public const int FloatsPerVertex = PositionFloatCount + ColorFloatCount + NormalFloatCount + TextureCoordinateFloatCount;

    public float[] Vertice_Data { get; set; } = [];
    public int Vertex_Count { get; set; }
    public Bounds3D Bounds { get; set; }
}
