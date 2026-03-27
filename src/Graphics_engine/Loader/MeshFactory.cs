using Graphics_engine;
using OpenTK.Graphics.OpenGL4;

public class MeshFactory
{

    public static Mesh CreateTriangle(float r, float g, float b)
    {
        return CreateRegularPolygon(3, r, g, b);
    }

    public static Mesh CreateSquare(float r, float g, float b)
    {
        return CreateRegularPolygon(4, r, g, b);
    }

    public static Mesh CreatePentagon(float r, float g, float b)
    {
        return CreateRegularPolygon(5, r, g, b);
    }

    public static Mesh CreateCircle(int segments, float r, float g, float b, float radius = 1.0f)
    {
        return CreateRegularPolygon(segments, r, g, b, radius);
    }

    public static Mesh CreateTriangleOutLine(float r, float g, float b)
    {
        return CreateRegularPolygonOutLine(3, r, g, b);
    }

    public static Mesh CreateSquareOutLine(float r, float g, float b)
    {
        return CreateRegularPolygonOutLine(4, r, g, b);
    }

    public static Mesh CreatePentagonOutLIne(float r, float g, float b)
    {
        return CreateRegularPolygonOutLine(5, r, g, b);
    }

    public static Mesh CreateCircleOutLine(int segments, float r, float g, float b, float radius = 1.0f)
    {
        return CreateRegularPolygonOutLine(segments, r, g, b, radius);
    }


    public static Mesh CreateRegularPolygon(int segments, float r, float g, float b, float radius = 1.0f)
    {
        var mesh = new Mesh();

        var points = new List<float>() { 0.0f, 0.0f, 0.0f, r, g, b };

        var delta = (2 * MathF.PI) / segments;
        var angle_in_radians = 0.0f;

        for (var i = 0; i < segments + 1; i++)
        {
            var x = radius * MathF.Cos(angle_in_radians);
            var y = radius * MathF.Sin(angle_in_radians);
            var z = 0.0f;

            // 1st half of the 1st quadrant
            points.Add(x);
            points.Add(y);
            points.Add(z);
            points.Add(r);
            points.Add(g);
            points.Add(b);

            angle_in_radians += delta;
        }
        mesh.Vertice_Data = points.ToArray();
        mesh.Vertex_Count = mesh.Vertice_Data.Length / 6;
        return mesh;
    }

    public static Mesh CreateRegularPolygonOutLine(int segments, float r, float g, float b, float radius = 1.0f)
    {
        var mesh = new Mesh();

        var points = new List<float>();

        var delta = (2 * MathF.PI) / segments;
        var angle_in_radians = 0.0f;

        for (var i = 0; i < segments; i++)
        {
            var x = radius * MathF.Cos(angle_in_radians);
            var y = radius * MathF.Sin(angle_in_radians);
            var z = 0.0f;

            // 1st half of the 1st quadrant
            points.Add(x);
            points.Add(y);
            points.Add(z);
            points.Add(r);
            points.Add(g);
            points.Add(b);

            angle_in_radians += delta;
        }
        mesh.Vertice_Data = points.ToArray();
        mesh.Vertex_Count = mesh.Vertice_Data.Length / 6;
        return mesh;
    }


    public static Mesh CreateLine()
    {
        var mesh = new Mesh();

        return mesh;
    }

    public static Mesh CreateGrid()
    {
        var mesh = new Mesh();
        var grid = new List<float>();

        // vertical lines
        for (float x = -1.0f; x <= 1.0001f; x += 0.1f)
        {
            bool isAxis = Math.Abs(x) < 0.0001f;

            float r = isAxis ? 0.7f : 0.10f;
            float g = isAxis ? 0.7f : 0.10f;
            float b = isAxis ? 0.7f : 0.10f;

            grid.Add(x); grid.Add(-1.0f); grid.Add(0.0f);
            grid.Add(r); grid.Add(g); grid.Add(b);

            grid.Add(x); grid.Add(1.0f); grid.Add(0.0f);
            grid.Add(r); grid.Add(g); grid.Add(b);
        }

        // horizontal lines
        for (float y = -1.0f; y <= 1.0001f; y += 0.1f)
        {
            bool isAxis = Math.Abs(y) < 0.0001f;

            float r = isAxis ? 0.7f : 0.10f;
            float g = isAxis ? 0.7f : 0.10f;
            float b = isAxis ? 0.7f : 0.10f;

            grid.Add(-1.0f); grid.Add(y); grid.Add(0.0f);
            grid.Add(r); grid.Add(g); grid.Add(b);

            grid.Add(1.0f); grid.Add(y); grid.Add(0.0f);
            grid.Add(r); grid.Add(g); grid.Add(b);
        }
        mesh.Vertice_Data = grid.ToArray();
        mesh.Vertex_Count = mesh.Vertice_Data.Length / 6;

        return mesh;
    }
}
