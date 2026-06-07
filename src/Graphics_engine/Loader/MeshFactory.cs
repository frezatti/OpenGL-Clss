using System.Numerics;
using Graphics_engine;

public class MeshFactory
{
    public static void AddPoint(List<float> points, float x, float y, float z, Vector3 color)
    {
        points.Add(x);
        points.Add(y);
        points.Add(z);
        points.Add(color.X);
        points.Add(color.Y);
        points.Add(color.Z);
    }

    private static Mesh BuildMesh(List<float> points)
    {
        var mesh = new Mesh
        {
            Vertice_Data = points.ToArray()
        };

        MeshUtilities.FinalizeMesh(mesh);
        return mesh;
    }

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
        var points = new List<float>() { 0.0f, 0.0f, 0.0f, r, g, b };
        var color = new Vector3(r, g, b);
        var delta = (2 * MathF.PI) / segments;
        var angleInRadians = 0.0f;

        for (var i = 0; i < segments + 1; i++)
        {
            var x = radius * MathF.Cos(angleInRadians);
            var y = radius * MathF.Sin(angleInRadians);

            AddPoint(points, x, y, 0.0f, color);
            angleInRadians += delta;
        }

        return BuildMesh(points);
    }

    public static Mesh CreateRegularPolygonOutLine(int segments, float r, float g, float b, float radius = 1.0f)
    {
        var points = new List<float>();
        var color = new Vector3(r, g, b);
        var delta = (2 * MathF.PI) / segments;
        var angleInRadians = 0.0f;

        for (var i = 0; i < segments; i++)
        {
            var x = radius * MathF.Cos(angleInRadians);
            var y = radius * MathF.Sin(angleInRadians);

            AddPoint(points, x, y, 0.0f, color);
            angleInRadians += delta;
        }

        return BuildMesh(points);
    }

    public static Mesh CreateRectangleBase(float width, float length)
    {
        var points = new List<float>();
        var color = new Vector3(1.0f, 1.0f, 1.0f);

        AddPoint(points, -width / 2, 0, 0, color);
        AddPoint(points, width / 2, 0, 0, color);
        AddPoint(points, width / 2, length, 0, color);

        AddPoint(points, -width / 2, 0, 0, color);
        AddPoint(points, width / 2, length, 0, color);
        AddPoint(points, -width / 2, length, 0, color);

        return BuildMesh(points);
    }

    public static Mesh CreateRectangleCenter(float width, float height)
    {
        var points = new List<float>();
        AddRectangle(
            points,
            -width / 2.0f,
            -height / 2.0f,
            width / 2.0f,
            height / 2.0f,
            new Vector3(1.0f, 1.0f, 1.0f)
        );

        return BuildMesh(points);
    }

    public static Mesh CreateTickMarks(float innerRadius, float outerRadius, float startAngle, float endAngle, int tickCount, Vector3? color)
    {
        var points = new List<float>();
        var start = startAngle * MathF.PI / 180.0f;
        var end = endAngle * MathF.PI / 180.0f;
        var delta = (end - start) / (tickCount - 1);
        var actualColor = color ?? new Vector3(1f, 1f, 1f);

        for (var i = 0; i < tickCount; i++)
        {
            var angle = start + i * delta;
            AddPoint(points, innerRadius * MathF.Cos(angle), innerRadius * MathF.Sin(angle), 0.0f, actualColor);
            AddPoint(points, outerRadius * MathF.Cos(angle), outerRadius * MathF.Sin(angle), 0.0f, actualColor);
        }

        return BuildMesh(points);
    }

    public static Mesh CreateNeedle(float width, float length, Vector3? color)
    {
        var points = new List<float>();
        var actualColor = color ?? new Vector3(1.0f, 0.0f, 0.0f);

        AddPoint(points, -width / 2.0f, 0.0f, 0.0f, actualColor);
        AddPoint(points, width / 2.0f, 0.0f, 0.0f, actualColor);
        AddPoint(points, 0.0f, length, 0.0f, actualColor);

        return BuildMesh(points);
    }

    public static Mesh CreateArc(float radius, float startAngle, float endAngle, int segments, Vector3? color)
    {
        var points = new List<float>();
        var actualColor = color ?? new Vector3(1.0f, 1.0f, 1.0f);
        var start = startAngle * MathF.PI / 180.0f;
        var end = endAngle * MathF.PI / 180.0f;
        var delta = (end - start) / segments;

        for (var i = 0; i <= segments; i++)
        {
            var angle = start + i * delta;
            AddPoint(points, radius * MathF.Cos(angle), radius * MathF.Sin(angle), 0.0f, actualColor);
        }

        return BuildMesh(points);
    }

    public static Mesh CreateLine()
    {
        return BuildMesh(new List<float>());
    }

    public static Mesh CreateGrid()
    {
        var grid = new List<float>();

        for (float x = -1.0f; x <= 1.0001f; x += 0.1f)
        {
            bool isAxis = Math.Abs(x) < 0.0001f;
            float r = isAxis ? 0.7f : 0.10f;
            float g = isAxis ? 0.7f : 0.10f;
            float b = isAxis ? 0.7f : 0.10f;
            var color = new Vector3(r, g, b);

            AddPoint(grid, x, -1.0f, 0.0f, color);
            AddPoint(grid, x, 1.0f, 0.0f, color);
        }

        for (float y = -1.0f; y <= 1.0001f; y += 0.1f)
        {
            bool isAxis = Math.Abs(y) < 0.0001f;
            float r = isAxis ? 0.7f : 0.10f;
            float g = isAxis ? 0.7f : 0.10f;
            float b = isAxis ? 0.7f : 0.10f;
            var color = new Vector3(r, g, b);

            AddPoint(grid, -1.0f, y, 0.0f, color);
            AddPoint(grid, 1.0f, y, 0.0f, color);
        }

        return BuildMesh(grid);
    }

    public static Mesh CreateCube(float size = 1.0f, Vector3? color = null)
    {
        var points = new List<float>();
        var c = color ?? new Vector3(0.8f, 0.8f, 0.8f);
        float h = size / 2.0f;

        void AddTriangle(Vector3 a, Vector3 b, Vector3 d)
        {
            AddPoint(points, a.X, a.Y, a.Z, c);
            AddPoint(points, b.X, b.Y, b.Z, c);
            AddPoint(points, d.X, d.Y, d.Z, c);
        }

        var p000 = new Vector3(-h, -h, -h);
        var p001 = new Vector3(-h, -h, h);
        var p010 = new Vector3(-h, h, -h);
        var p011 = new Vector3(-h, h, h);
        var p100 = new Vector3(h, -h, -h);
        var p101 = new Vector3(h, -h, h);
        var p110 = new Vector3(h, h, -h);
        var p111 = new Vector3(h, h, h);

        // Front (+Z)
        AddTriangle(p001, p101, p111);
        AddTriangle(p001, p111, p011);

        // Back (-Z)
        AddTriangle(p100, p000, p010);
        AddTriangle(p100, p010, p110);

        // Left (-X)
        AddTriangle(p000, p001, p011);
        AddTriangle(p000, p011, p010);

        // Right (+X)
        AddTriangle(p101, p100, p110);
        AddTriangle(p101, p110, p111);

        // Top (+Y)
        AddTriangle(p010, p011, p111);
        AddTriangle(p010, p111, p110);

        // Bottom (-Y)
        AddTriangle(p000, p100, p101);
        AddTriangle(p000, p101, p001);

        return BuildMesh(points);
    }

    public static Mesh CreateSevenSegmentNumber(string text, float digitWidth = 0.18f, float digitHeight = 0.32f, float thickness = 0.035f, float spacing = 0.04f, Vector3? color = null)
    {
        var points = new List<float>();
        var actualColor = color ?? new Vector3(1.0f, 1.0f, 1.0f);

        float totalWidth = text.Length * digitWidth + (text.Length - 1) * spacing;
        float startX = -totalWidth / 2.0f;
        float startY = -digitHeight / 2.0f;

        for (var i = 0; i < text.Length; i++)
        {
            char character = text[i];

            if (character < '0' || character > '9')
            {
                continue;
            }

            int digit = character - '0';
            float x = startX + i * (digitWidth + spacing);
            float y = startY;

            AddDigit(points, digit, x, y, digitWidth, digitHeight, thickness, actualColor);
        }

        return BuildMesh(points);
    }

    private static void AddDigit(List<float> points, int digit, float x, float y, float width, float height, float thickness, Vector3 color)
    {
        bool[] segments = digit switch
        {
            0 => new[] { true, true, true, true, true, true, false },
            1 => new[] { false, true, true, false, false, false, false },
            2 => new[] { true, true, false, true, true, false, true },
            3 => new[] { true, true, true, true, false, false, true },
            4 => new[] { false, true, true, false, false, true, true },
            5 => new[] { true, false, true, true, false, true, true },
            6 => new[] { true, false, true, true, true, true, true },
            7 => new[] { true, true, true, false, false, false, false },
            8 => new[] { true, true, true, true, true, true, true },
            9 => new[] { true, true, true, true, false, true, true },
            _ => new[] { false, false, false, false, false, false, false }
        };

        float halfHeight = height / 2.0f;

        if (segments[0]) AddRectangle(points, x + thickness, y + height - thickness, x + width - thickness, y + height, color);
        if (segments[1]) AddRectangle(points, x + width - thickness, y + halfHeight, x + width, y + height - thickness, color);
        if (segments[2]) AddRectangle(points, x + width - thickness, y + thickness, x + width, y + halfHeight, color);
        if (segments[3]) AddRectangle(points, x + thickness, y, x + width - thickness, y + thickness, color);
        if (segments[4]) AddRectangle(points, x, y + thickness, x + thickness, y + halfHeight, color);
        if (segments[5]) AddRectangle(points, x, y + halfHeight, x + thickness, y + height - thickness, color);
        if (segments[6]) AddRectangle(points, x + thickness, y + halfHeight - thickness / 2.0f, x + width - thickness, y + halfHeight + thickness / 2.0f, color);
    }

    private static void AddRectangle(List<float> points, float minX, float minY, float maxX, float maxY, Vector3 color)
    {
        AddPoint(points, minX, minY, 0.0f, color);
        AddPoint(points, maxX, minY, 0.0f, color);
        AddPoint(points, maxX, maxY, 0.0f, color);

        AddPoint(points, minX, minY, 0.0f, color);
        AddPoint(points, maxX, maxY, 0.0f, color);
        AddPoint(points, minX, maxY, 0.0f, color);
    }

    public static Mesh CreateMeshFromPoints(IReadOnlyList<Vector3> positions, Vector3? color = null)
    {
        var points = new List<float>();
        var actualColor = color ?? new Vector3(1.0f, 1.0f, 1.0f);

        foreach (var position in positions)
        {
            AddPoint(points, position.X, position.Y, position.Z, actualColor);
        }

        return BuildMesh(points);
    }
}
