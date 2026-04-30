using System.Numerics;
using Graphics_engine;
using OpenTK.Graphics.OpenGL4;

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

            AddPoint(points, x, y, z, new Vector3(r, g, b));

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

            AddPoint(points, x, y, z, new Vector3(r, g, b));

            angle_in_radians += delta;
        }
        mesh.Vertice_Data = points.ToArray();
        mesh.Vertex_Count = mesh.Vertice_Data.Length / 6;
        return mesh;
    }

    public static Mesh CreateRectangleBase(float width, float length)
    {
        var mesh = new Mesh();
        var points = new List<float>();

        //left bottom
        AddPoint(points, -width / 2, 0, 0, new Vector3(1.0f, 1.0f, 1.0f));
        //right bottom 
        AddPoint(points, width / 2, 0, 0, new Vector3(1.0f, 1.0f, 1.0f));
        //right top 
        AddPoint(points, width / 2, length, 0, new Vector3(1.0f, 1.0f, 1.0f));


        AddPoint(points, -width / 2, 0, 0, new Vector3(1.0f, 1.0f, 1.0f));
        AddPoint(points, width / 2, length, 0, new Vector3(1.0f, 1.0f, 1.0f));
        AddPoint(points, -width / 2, length, 0, new Vector3(1.0f, 1.0f, 1.0f));

        mesh.Vertice_Data = points.ToArray();
        mesh.Vertex_Count = mesh.Vertice_Data.Length / 6;

        return mesh;
    }

    public static Mesh CreateRectangleCenter(float width, float heigth)
    {
        var mesh = new Mesh();
        var points = new List<float>();

        return mesh;
    }

    public static Mesh CreateTickMarks(float innerRadius, float outerRadius, float startAngle, float endAngle, int tickCount, Vector3? color)
    {
        var mesh = new Mesh();
        var points = new List<float>();

        var angle_in_radians_start = startAngle * MathF.PI / 180.0f;
        var angle_in_radians_end = endAngle * MathF.PI / 180.0f;
        var delta = (angle_in_radians_end - angle_in_radians_start) / (tickCount - 1);
        var angle_in_radians = angle_in_radians_start;


        var actualColor = color ?? new Vector3(1f, 1f, 1f);

        for (var i = 0; i < tickCount; i++)
        {
            angle_in_radians = angle_in_radians_start + i * delta;

            var x_inner = innerRadius * MathF.Cos(angle_in_radians);
            var y_inner = innerRadius * MathF.Sin(angle_in_radians);

            var x_outer = outerRadius * MathF.Cos(angle_in_radians);
            var y_outer = outerRadius * MathF.Sin(angle_in_radians);

            var z = 0.0f;

            AddPoint(points, x_inner, y_inner, z, actualColor);
            AddPoint(points, x_outer, y_outer, z, actualColor);

        }
        mesh.Vertice_Data = points.ToArray();
        mesh.Vertex_Count = mesh.Vertice_Data.Length / 6;
        return mesh;
    }

    public static Mesh CreateNeedle(float width, float length, Vector3? color)
    {
        var mesh = new Mesh();
        var points = new List<float>();

        var actualColor = color ?? new Vector3(1.0f, 0.0f, 0.0f);

        AddPoint(points, -width / 2.0f, 0.0f, 0.0f, actualColor);
        AddPoint(points, width / 2.0f, 0.0f, 0.0f, actualColor);
        AddPoint(points, 0.0f, length, 0.0f, actualColor);

        mesh.Vertice_Data = points.ToArray();
        mesh.Vertex_Count = mesh.Vertice_Data.Length / 6;

        return mesh;
    }

    public static Mesh CreateArc(float radius, float startAngle, float endAngle, int segments, Vector3? color)
    {
        var mesh = new Mesh();
        var points = new List<float>();

        var actualColor = color ?? new Vector3(1.0f, 1.0f, 1.0f);

        var start = startAngle * MathF.PI / 180.0f;
        var end = endAngle * MathF.PI / 180.0f;
        var delta = (end - start) / segments;

        for (var i = 0; i <= segments; i++)
        {
            var angle = start + i * delta;

            var x = radius * MathF.Cos(angle);
            var y = radius * MathF.Sin(angle);

            AddPoint(points, x, y, 0.0f, actualColor);
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

    public static Mesh CreateSevenSegmentNumber(string text, float digitWidth = 0.18f, float digitHeight = 0.32f, float thickness = 0.035f, float spacing = 0.04f, System.Numerics.Vector3? color = null)
    {
        var mesh = new Mesh();
        var points = new List<float>();

        var actualColor = color ?? new System.Numerics.Vector3(1.0f, 1.0f, 1.0f);

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

        mesh.Vertice_Data = points.ToArray();
        mesh.Vertex_Count = mesh.Vertice_Data.Length / 6;

        return mesh;
    }

    private static void AddDigit(
        List<float> points,
        int digit,
        float x,
        float y,
        float width,
        float height,
        float thickness,
        System.Numerics.Vector3 color)
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

        if (segments[0])
            AddRectangle(points, x + thickness, y + height - thickness, x + width - thickness, y + height, color);

        if (segments[1])
            AddRectangle(points, x + width - thickness, y + halfHeight, x + width, y + height - thickness, color);

        if (segments[2])
            AddRectangle(points, x + width - thickness, y + thickness, x + width, y + halfHeight, color);

        if (segments[3])
            AddRectangle(points, x + thickness, y, x + width - thickness, y + thickness, color);

        if (segments[4])
            AddRectangle(points, x, y + thickness, x + thickness, y + halfHeight, color);

        if (segments[5])
            AddRectangle(points, x, y + halfHeight, x + thickness, y + height - thickness, color);

        if (segments[6])
            AddRectangle(
                points,
                x + thickness,
                y + halfHeight - thickness / 2.0f,
                x + width - thickness,
                y + halfHeight + thickness / 2.0f,
                color
            );
    }

    private static void AddRectangle(
        List<float> points,
        float minX,
        float minY,
        float maxX,
        float maxY,
        System.Numerics.Vector3 color)
    {
        // Triangle 1
        AddPoint(points, minX, minY, 0.0f, color);
        AddPoint(points, maxX, minY, 0.0f, color);
        AddPoint(points, maxX, maxY, 0.0f, color);

        // Triangle 2
        AddPoint(points, minX, minY, 0.0f, color);
        AddPoint(points, maxX, maxY, 0.0f, color);
        AddPoint(points, minX, maxY, 0.0f, color);
    }

    public static Mesh CreateMeshFromPoints(IReadOnlyList<Vector3> positions, Vector3? color = null)
    {
        var mesh = new Mesh();
        var points = new List<float>();

        var actualColor = color ?? new Vector3(1.0f, 1.0f, 1.0f);

        foreach (var position in positions)
        {
            AddPoint(points, position.X, position.Y, position.Z, actualColor);
        }

        mesh.Vertice_Data = points.ToArray();
        mesh.Vertex_Count = mesh.Vertice_Data.Length / 6;

        return mesh;
    }

}


