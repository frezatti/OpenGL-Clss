namespace Graphics_engine;

public class MeshUtilities
{
    public static void FinalizeMesh(Mesh mesh)
    {
        if (mesh.Vertice_Data.Length % Mesh.FloatsPerVertex != 0)
        {
            throw new InvalidOperationException(
                $"Mesh vertex data must use {Mesh.FloatsPerVertex} floats per vertex: x, y, z, r, g, b, nx, ny, nz."
            );
        }

        mesh.Vertex_Count = mesh.Vertice_Data.Length / Mesh.FloatsPerVertex;
        mesh.Bounds = CalculateBounds(mesh.Vertice_Data);
    }

    public static Bounds3D CalculateBounds(float[] vertices)
    {
        if (vertices.Length % Mesh.FloatsPerVertex != 0)
        {
            throw new ArgumentException(
                $"Vertex data length must be divisible by {Mesh.FloatsPerVertex}.",
                nameof(vertices)
            );
        }

        if (vertices.Length < Mesh.FloatsPerVertex)
        {
            return new Bounds3D(0, 0, 0, 0, 0, 0);
        }

        float minX = vertices[Mesh.PositionOffset];
        float maxX = vertices[Mesh.PositionOffset];

        float minY = vertices[Mesh.PositionOffset + 1];
        float maxY = vertices[Mesh.PositionOffset + 1];

        float minZ = vertices[Mesh.PositionOffset + 2];
        float maxZ = vertices[Mesh.PositionOffset + 2];

        for (int i = 0; i < vertices.Length; i += Mesh.FloatsPerVertex)
        {
            float x = vertices[i + Mesh.PositionOffset];
            float y = vertices[i + Mesh.PositionOffset + 1];
            float z = vertices[i + Mesh.PositionOffset + 2];

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
