using System.Numerics;
using Graphics_engine;

public class MeshFactory
{
    private static readonly Vector3 DefaultNormal = Vector3.UnitZ;

    public static void AddPoint(List<float> points, float x, float y, float z, Vector3 color)
    {
        var normal = DefaultNormal;
        var uv = new Vector2(x + 0.5f, y + 0.5f);
        AddVertex(points, new Vector3(x, y, z), color, normal, uv);
    }

    private static void AddVertex(List<float> points, Vector3 position, Vector3 color, Vector3 normal, Vector2 uv)
    {
        if (normal.LengthSquared() <= 0.000001f)
        {
            normal = DefaultNormal;
        }
        else
        {
            normal = Vector3.Normalize(normal);
        }

        points.Add(position.X);
        points.Add(position.Y);
        points.Add(position.Z);
        points.Add(color.X);
        points.Add(color.Y);
        points.Add(color.Z);
        points.Add(normal.X);
        points.Add(normal.Y);
        points.Add(normal.Z);
        points.Add(uv.X);
        points.Add(uv.Y);
    }

    private static Vector3 CalculateNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        var normal = Vector3.Cross(b - a, c - a);
        return normal.LengthSquared() <= 0.000001f ? DefaultNormal : Vector3.Normalize(normal);
    }

    private static Vector2 ProjectUv(Vector3 position, Vector3 normal)
    {
        float ax = MathF.Abs(normal.X);
        float ay = MathF.Abs(normal.Y);
        float az = MathF.Abs(normal.Z);

        if (ay >= ax && ay >= az)
        {
            return new Vector2(position.X + 0.5f, position.Z + 0.5f);
        }

        if (ax >= ay && ax >= az)
        {
            return new Vector2(position.Z + 0.5f, position.Y + 0.5f);
        }

        return new Vector2(position.X + 0.5f, position.Y + 0.5f);
    }

    private static void AddTriangle(List<float> points, Vector3 a, Vector3 b, Vector3 c, Vector3 color)
    {
        var normal = CalculateNormal(a, b, c);
        AddVertex(points, a, color, normal, ProjectUv(a, normal));
        AddVertex(points, b, color, normal, ProjectUv(b, normal));
        AddVertex(points, c, color, normal, ProjectUv(c, normal));
    }

    private static void AddTriangleUv(List<float> points, Vector3 a, Vector3 b, Vector3 c, Vector2 uvA, Vector2 uvB, Vector2 uvC, Vector3 color)
    {
        var normal = CalculateNormal(a, b, c);
        AddVertex(points, a, color, normal, uvA);
        AddVertex(points, b, color, normal, uvB);
        AddVertex(points, c, color, normal, uvC);
    }

    private static void AddQuad(List<float> points, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 color)
    {
        AddTriangleUv(points, a, b, c, new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), color);
        AddTriangleUv(points, a, c, d, new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1), color);
    }

    private static void ValidateMinimum(string parameterName, int value, int minimum)
    {
        if (value < minimum)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} must be at least {minimum}.");
        }
    }

    private static void ValidatePositive(string parameterName, float value)
    {
        if (value <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} must be greater than zero.");
        }
    }

    private static Mesh BuildMesh(List<float> points)
    {
        var mesh = new Mesh { Vertice_Data = points.ToArray() };
        MeshUtilities.FinalizeMesh(mesh);
        return mesh;
    }

    public static Mesh CreateTriangle(float r, float g, float b) => CreateRegularPolygon(3, r, g, b);
    public static Mesh CreateSquare(float r, float g, float b) => CreateRegularPolygon(4, r, g, b);
    public static Mesh CreatePentagon(float r, float g, float b) => CreateRegularPolygon(5, r, g, b);
    public static Mesh CreateCircle(int segments, float r, float g, float b, float radius = 1.0f) => CreateRegularPolygon(segments, r, g, b, radius);
    public static Mesh CreateTriangleOutLine(float r, float g, float b) => CreateRegularPolygonOutLine(3, r, g, b);
    public static Mesh CreateSquareOutLine(float r, float g, float b) => CreateRegularPolygonOutLine(4, r, g, b);
    public static Mesh CreatePentagonOutLIne(float r, float g, float b) => CreateRegularPolygonOutLine(5, r, g, b);
    public static Mesh CreateCircleOutLine(int segments, float r, float g, float b, float radius = 1.0f) => CreateRegularPolygonOutLine(segments, r, g, b, radius);

    public static Mesh CreateRegularPolygon(int segments, float r, float g, float b, float radius = 1.0f)
    {
        ValidateMinimum(nameof(segments), segments, 3);
        ValidatePositive(nameof(radius), radius);
        var points = new List<float>();
        var color = new Vector3(r, g, b);
        var center = Vector3.Zero;
        float delta = 2.0f * MathF.PI / segments;

        for (int i = 0; i < segments; i++)
        {
            float a0 = i * delta;
            float a1 = (i + 1) * delta;
            var p0 = new Vector3(radius * MathF.Cos(a0), radius * MathF.Sin(a0), 0.0f);
            var p1 = new Vector3(radius * MathF.Cos(a1), radius * MathF.Sin(a1), 0.0f);
            AddTriangle(points, center, p0, p1, color);
        }

        return BuildMesh(points);
    }

    public static Mesh CreateRegularPolygonOutLine(int segments, float r, float g, float b, float radius = 1.0f)
    {
        ValidateMinimum(nameof(segments), segments, 3);
        ValidatePositive(nameof(radius), radius);
        var points = new List<float>();
        var color = new Vector3(r, g, b);
        float delta = 2.0f * MathF.PI / segments;

        for (int i = 0; i < segments; i++)
        {
            float a = i * delta;
            AddPoint(points, radius * MathF.Cos(a), radius * MathF.Sin(a), 0.0f, color);
        }

        return BuildMesh(points);
    }

    public static Mesh CreateRectangleBase(float width, float length)
    {
        ValidatePositive(nameof(width), width);
        ValidatePositive(nameof(length), length);
        var points = new List<float>();
        var color = Vector3.One;
        AddQuad(points, new Vector3(-width / 2, 0, 0), new Vector3(width / 2, 0, 0), new Vector3(width / 2, length, 0), new Vector3(-width / 2, length, 0), color);
        return BuildMesh(points);
    }

    public static Mesh CreateRectangleCenter(float width, float height)
    {
        ValidatePositive(nameof(width), width);
        ValidatePositive(nameof(height), height);
        var points = new List<float>();
        AddRectangle(points, -width / 2.0f, -height / 2.0f, width / 2.0f, height / 2.0f, Vector3.One);
        return BuildMesh(points);
    }

    public static Mesh CreateTickMarks(float innerRadius, float outerRadius, float startAngle, float endAngle, int tickCount, Vector3? color)
    {
        ValidatePositive(nameof(innerRadius), innerRadius);
        ValidatePositive(nameof(outerRadius), outerRadius);
        ValidateMinimum(nameof(tickCount), tickCount, 2);
        if (outerRadius <= innerRadius) throw new ArgumentOutOfRangeException(nameof(outerRadius), outerRadius, "outerRadius must be greater than innerRadius.");

        var points = new List<float>();
        var actualColor = color ?? Vector3.One;
        float start = startAngle * MathF.PI / 180.0f;
        float end = endAngle * MathF.PI / 180.0f;
        float delta = (end - start) / (tickCount - 1);

        for (int i = 0; i < tickCount; i++)
        {
            float angle = start + i * delta;
            AddPoint(points, innerRadius * MathF.Cos(angle), innerRadius * MathF.Sin(angle), 0.0f, actualColor);
            AddPoint(points, outerRadius * MathF.Cos(angle), outerRadius * MathF.Sin(angle), 0.0f, actualColor);
        }

        return BuildMesh(points);
    }

    public static Mesh CreateNeedle(float width, float length, Vector3? color)
    {
        ValidatePositive(nameof(width), width);
        ValidatePositive(nameof(length), length);
        var points = new List<float>();
        var actualColor = color ?? new Vector3(1, 0, 0);
        AddTriangle(points, new Vector3(-width / 2, 0, 0), new Vector3(width / 2, 0, 0), new Vector3(0, length, 0), actualColor);
        return BuildMesh(points);
    }

    public static Mesh CreateArc(float radius, float startAngle, float endAngle, int segments, Vector3? color)
    {
        ValidatePositive(nameof(radius), radius);
        ValidateMinimum(nameof(segments), segments, 1);
        var points = new List<float>();
        var actualColor = color ?? Vector3.One;
        float start = startAngle * MathF.PI / 180.0f;
        float end = endAngle * MathF.PI / 180.0f;
        float delta = (end - start) / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = start + i * delta;
            AddPoint(points, radius * MathF.Cos(angle), radius * MathF.Sin(angle), 0.0f, actualColor);
        }

        return BuildMesh(points);
    }

    public static Mesh CreateLine() => BuildMesh(new List<float>());

    public static Mesh CreateGrid()
    {
        var grid = new List<float>();
        for (float x = -1.0f; x <= 1.0001f; x += 0.1f)
        {
            bool isAxis = MathF.Abs(x) < 0.0001f;
            float c = isAxis ? 0.7f : 0.10f;
            AddPoint(grid, x, -1.0f, 0.0f, new Vector3(c, c, c));
            AddPoint(grid, x, 1.0f, 0.0f, new Vector3(c, c, c));
        }
        for (float y = -1.0f; y <= 1.0001f; y += 0.1f)
        {
            bool isAxis = MathF.Abs(y) < 0.0001f;
            float c = isAxis ? 0.7f : 0.10f;
            AddPoint(grid, -1.0f, y, 0.0f, new Vector3(c, c, c));
            AddPoint(grid, 1.0f, y, 0.0f, new Vector3(c, c, c));
        }
        return BuildMesh(grid);
    }

    public static Mesh CreatePlane(float width = 1.0f, float depth = 1.0f, Vector3? color = null)
    {
        ValidatePositive(nameof(width), width);
        ValidatePositive(nameof(depth), depth);
        var points = new List<float>();
        var c = color ?? new Vector3(0.8f, 0.8f, 0.8f);
        float hx = width / 2.0f;
        float hz = depth / 2.0f;
        AddQuad(points, new Vector3(-hx, 0, -hz), new Vector3(-hx, 0, hz), new Vector3(hx, 0, hz), new Vector3(hx, 0, -hz), c);
        return BuildMesh(points);
    }

    public static Mesh CreateCube(float size = 1.0f, Vector3? color = null) => CreateBox(size, size, size, color);

    public static Mesh CreateBox(float width = 1.0f, float height = 1.0f, float depth = 1.0f, Vector3? color = null)
    {
        ValidatePositive(nameof(width), width);
        ValidatePositive(nameof(height), height);
        ValidatePositive(nameof(depth), depth);
        var points = new List<float>();
        var c = color ?? new Vector3(0.8f, 0.8f, 0.8f);
        float hx = width / 2.0f;
        float hy = height / 2.0f;
        float hz = depth / 2.0f;

        var p000 = new Vector3(-hx, -hy, -hz);
        var p001 = new Vector3(-hx, -hy, hz);
        var p010 = new Vector3(-hx, hy, -hz);
        var p011 = new Vector3(-hx, hy, hz);
        var p100 = new Vector3(hx, -hy, -hz);
        var p101 = new Vector3(hx, -hy, hz);
        var p110 = new Vector3(hx, hy, -hz);
        var p111 = new Vector3(hx, hy, hz);

        AddQuad(points, p001, p101, p111, p011, c);
        AddQuad(points, p100, p000, p010, p110, c);
        AddQuad(points, p000, p001, p011, p010, c);
        AddQuad(points, p101, p100, p110, p111, c);
        AddQuad(points, p010, p011, p111, p110, c);
        AddQuad(points, p000, p100, p101, p001, c);
        return BuildMesh(points);
    }

    public static Mesh CreatePyramid(float baseSize = 1.0f, float height = 1.0f, Vector3? color = null)
    {
        ValidatePositive(nameof(baseSize), baseSize);
        ValidatePositive(nameof(height), height);
        var points = new List<float>();
        var c = color ?? new Vector3(0.8f, 0.8f, 0.8f);
        float h = baseSize / 2.0f;
        float bottomY = -height / 2.0f;
        float topY = height / 2.0f;
        var p00 = new Vector3(-h, bottomY, -h);
        var p01 = new Vector3(-h, bottomY, h);
        var p10 = new Vector3(h, bottomY, -h);
        var p11 = new Vector3(h, bottomY, h);
        var apex = new Vector3(0, topY, 0);

        AddQuad(points, p00, p10, p11, p01, c);
        AddTriangle(points, p01, p11, apex, c);
        AddTriangle(points, p10, p00, apex, c);
        AddTriangle(points, p00, p01, apex, c);
        AddTriangle(points, p11, p10, apex, c);
        return BuildMesh(points);
    }

    public static Mesh CreateCylinder(float radius = 0.5f, float height = 1.0f, int segments = 32, Vector3? color = null)
    {
        ValidatePositive(nameof(radius), radius);
        ValidatePositive(nameof(height), height);
        ValidateMinimum(nameof(segments), segments, 3);
        var points = new List<float>();
        var c = color ?? new Vector3(0.8f, 0.8f, 0.8f);
        float halfHeight = height / 2.0f;
        float delta = 2.0f * MathF.PI / segments;
        var topCenter = new Vector3(0, halfHeight, 0);
        var bottomCenter = new Vector3(0, -halfHeight, 0);

        for (int i = 0; i < segments; i++)
        {
            float a0 = i * delta;
            float a1 = (i + 1) * delta;
            float u0 = i / (float)segments;
            float u1 = (i + 1) / (float)segments;
            var bottom0 = new Vector3(radius * MathF.Cos(a0), -halfHeight, radius * MathF.Sin(a0));
            var bottom1 = new Vector3(radius * MathF.Cos(a1), -halfHeight, radius * MathF.Sin(a1));
            var top0 = new Vector3(bottom0.X, halfHeight, bottom0.Z);
            var top1 = new Vector3(bottom1.X, halfHeight, bottom1.Z);

            AddTriangleUv(points, bottom0, top0, top1, new Vector2(u0, 0), new Vector2(u0, 1), new Vector2(u1, 1), c);
            AddTriangleUv(points, bottom0, top1, bottom1, new Vector2(u0, 0), new Vector2(u1, 1), new Vector2(u1, 0), c);
            AddTriangle(points, topCenter, top1, top0, c);
            AddTriangle(points, bottomCenter, bottom0, bottom1, c);
        }
        return BuildMesh(points);
    }

    public static Mesh CreateCone(float radius = 0.5f, float height = 1.0f, int segments = 32, Vector3? color = null)
    {
        ValidatePositive(nameof(radius), radius);
        ValidatePositive(nameof(height), height);
        ValidateMinimum(nameof(segments), segments, 3);
        var points = new List<float>();
        var c = color ?? new Vector3(0.8f, 0.8f, 0.8f);
        float halfHeight = height / 2.0f;
        var apex = new Vector3(0, halfHeight, 0);
        var bottomCenter = new Vector3(0, -halfHeight, 0);
        float delta = 2.0f * MathF.PI / segments;

        for (int i = 0; i < segments; i++)
        {
            float a0 = i * delta;
            float a1 = (i + 1) * delta;
            var bottom0 = new Vector3(radius * MathF.Cos(a0), -halfHeight, radius * MathF.Sin(a0));
            var bottom1 = new Vector3(radius * MathF.Cos(a1), -halfHeight, radius * MathF.Sin(a1));
            AddTriangle(points, bottom0, apex, bottom1, c);
            AddTriangle(points, bottomCenter, bottom0, bottom1, c);
        }
        return BuildMesh(points);
    }

    public static Mesh CreateUVSphere(float radius = 0.5f, int sectors = 32, int stacks = 16, Vector3? color = null)
    {
        ValidatePositive(nameof(radius), radius);
        ValidateMinimum(nameof(sectors), sectors, 3);
        ValidateMinimum(nameof(stacks), stacks, 2);
        var points = new List<float>();
        var c = color ?? new Vector3(0.8f, 0.8f, 0.8f);
        float stackStep = MathF.PI / stacks;
        float sectorStep = 2.0f * MathF.PI / sectors;

        Vector3 GetPoint(int stack, int sector)
        {
            float phi = -MathF.PI / 2.0f + stack * stackStep;
            float theta = sector * sectorStep;
            float ringRadius = radius * MathF.Cos(phi);
            return new Vector3(ringRadius * MathF.Cos(theta), radius * MathF.Sin(phi), ringRadius * MathF.Sin(theta));
        }

        Vector2 GetUv(int stack, int sector) => new Vector2(sector / (float)sectors, stack / (float)stacks);

        for (int stack = 0; stack < stacks; stack++)
        {
            for (int sector = 0; sector < sectors; sector++)
            {
                var p00 = GetPoint(stack, sector);
                var p01 = GetPoint(stack, sector + 1);
                var p10 = GetPoint(stack + 1, sector);
                var p11 = GetPoint(stack + 1, sector + 1);

                if (stack != 0) AddTriangleUv(points, p00, p10, p11, GetUv(stack, sector), GetUv(stack + 1, sector), GetUv(stack + 1, sector + 1), c);
                if (stack != stacks - 1) AddTriangleUv(points, p00, p11, p01, GetUv(stack, sector), GetUv(stack + 1, sector + 1), GetUv(stack, sector + 1), c);
            }
        }
        return BuildMesh(points);
    }

    public static Mesh CreateSevenSegmentNumber(string text, float digitWidth = 0.18f, float digitHeight = 0.32f, float thickness = 0.035f, float spacing = 0.04f, Vector3? color = null)
    {
        ValidatePositive(nameof(digitWidth), digitWidth);
        ValidatePositive(nameof(digitHeight), digitHeight);
        ValidatePositive(nameof(thickness), thickness);
        if (spacing < 0.0f) throw new ArgumentOutOfRangeException(nameof(spacing), spacing, "spacing cannot be negative.");

        var points = new List<float>();
        var actualColor = color ?? Vector3.One;
        float totalWidth = text.Length * digitWidth + (text.Length - 1) * spacing;
        float startX = -totalWidth / 2.0f;
        float startY = -digitHeight / 2.0f;

        for (int i = 0; i < text.Length; i++)
        {
            char character = text[i];
            if (character < '0' || character > '9') continue;
            int digit = character - '0';
            float x = startX + i * (digitWidth + spacing);
            AddDigit(points, digit, x, startY, digitWidth, digitHeight, thickness, actualColor);
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
        AddQuad(points, new Vector3(minX, minY, 0), new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0), new Vector3(minX, maxY, 0), color);
    }

    public static Mesh CreateMeshFromPoints(IReadOnlyList<Vector3> positions, Vector3? color = null)
    {
        var points = new List<float>();
        var actualColor = color ?? Vector3.One;
        foreach (var position in positions) AddPoint(points, position.X, position.Y, position.Z, actualColor);
        return BuildMesh(points);
    }
}
