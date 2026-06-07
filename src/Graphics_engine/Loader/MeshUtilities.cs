namespace Graphics_engine;

public class MeshUtilities
{

    public static void FinalizeMesh(Mesh mesh)
    {
        mesh.Vertex_Count = mesh.Vertice_Data.Length / 6;
        mesh.Bounds = CalculateBounds(mesh.Vertice_Data);
    }

    public static Bounds3D CalculateBounds(float[] vertices)
    {
        if (vertices.Length < 6)
        {
            return new Bounds3D(0, 0, 0, 0, 0, 0);
        }

        float minX = vertices[0];
        float maxX = vertices[0];

        float minY = vertices[1];
        float maxY = vertices[1];

        float minZ = vertices[2];
        float maxZ = vertices[2];

        for (int i = 0; i < vertices.Length; i += 6)
        {
            float x = vertices[i];
            float y = vertices[i + 1];
            float z = vertices[i + 2];

            minX = MathF.Min(minX, x);
            maxX = MathF.Max(maxX, x);

            minY = MathF.Min(minY, y);
            maxY = MathF.Max(maxY, y);

            minZ = MathF.Min(minZ, z);
            maxZ = MathF.Max(maxZ, z);
        }

        return new Bounds3D(minX, maxX, minY, maxY, minZ, maxZ);
    }
}
