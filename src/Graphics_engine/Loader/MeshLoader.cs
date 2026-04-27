using System.Globalization;
using Graphics_engine;
using OpenTK.Graphics.OpenGL4;
using System.Numerics;

public class MeshLoader
{
    public static bool TryLoadFromFile(string path, out Mesh mesh, out LoaderError loader_error)
    {
        mesh = new Mesh();
        var loadedNumbers = new List<float>();

        var file_exists = File.Exists(path);

        if (!file_exists)
        {
            loader_error = new LoaderError(LoaderErrorCode.InvalidFloat, $"File path: {path} does not exist\n");
            return false;
        }

        try
        {

            var data_from_file = File.ReadLines(path);
            var line_number = 0;

            foreach (var line in data_from_file)
            {
                line_number++;
                if (string.IsNullOrWhiteSpace(line)) continue;
                var points = line.Split(",");
                if (points.Length != 6)
                {
                    loader_error = new LoaderError(LoaderErrorCode.InvalidFieldCount, $"Wrong number of parameters. Expected 6 parameters per line recevied {points.Length} parameters on Line {line_number}\n");
                    return false;
                }

                if (float.TryParse(points[0], CultureInfo.InvariantCulture, out float x) &&
                    float.TryParse(points[1], CultureInfo.InvariantCulture, out float y) &&
                    float.TryParse(points[2], CultureInfo.InvariantCulture, out float z) &&
                    float.TryParse(points[3], CultureInfo.InvariantCulture, out float r) &&
                    float.TryParse(points[4], CultureInfo.InvariantCulture, out float g) &&
                    float.TryParse(points[5], CultureInfo.InvariantCulture, out float b))
                {
                    loadedNumbers.Add(x);
                    loadedNumbers.Add(y);
                    loadedNumbers.Add(z);
                    loadedNumbers.Add(r);
                    loadedNumbers.Add(g);
                    loadedNumbers.Add(b);
                }
                else
                {

                    loader_error = new LoaderError(LoaderErrorCode.InvalidFloat, $"One of the numbers was not formated correctly on line {line_number}\n");
                    return false;
                }
            }
            mesh.Vertice_Data = loadedNumbers.ToArray();
            mesh.Vertex_Count = mesh.Vertice_Data.Length / 6;


        }
        catch (Exception ex)
        {
            loader_error = new LoaderError(LoaderErrorCode.FileReadingError, $"there was an error while reading the file :{path} \n error: \n {ex.Data}");
            return false;
        }

        loader_error = new LoaderError(LoaderErrorCode.None, "");
        return true;

    }


    public static (RenderItem[], Dictionary<Mesh, Bounds2D>) LoadExample()
    {
        var loadedRenderItems = new List<RenderItem>();
        var bounds = new Dictionary<Mesh, Bounds2D>();

        void AddFromMesh(
            Mesh mesh,
            PrimitiveType primitiveType,
            Vector3 position,
            Vector3 scale,
            float rotation = 0.0f,
            Vector4? baseColor = null,
            ColorMode colorMode = ColorMode.SolidColor)
        {
            var item = new RenderItem();

            item.Mesh = mesh;
            item.Rendering_Type = primitiveType;
            item.Transfom.Position = position;
            item.Transfom.Scale = scale;
            item.Transfom.Rotation = rotation;

            var material = item.Material;
            material.ColorMode = colorMode;
            material.BaseColor = baseColor ?? new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            item.Material = material;

            loadedRenderItems.Add(item);
        }

        var white = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        var red = new Vector4(1.0f, 0.1f, 0.1f, 1.0f);
        var yellow = new Vector4(1.0f, 0.85f, 0.1f, 1.0f);

        var white3 = new Vector3(1.0f, 1.0f, 1.0f);
        var red3 = new Vector3(1.0f, 0.1f, 0.1f);
        var yellow3 = new Vector3(1.0f, 0.85f, 0.1f);

        // Optional grid
        AddFromMesh(
            MeshFactory.CreateGrid(),
            PrimitiveType.Lines,
            Vector3.Zero,
            Vector3.One,
            0.0f,
            white,
            ColorMode.VertexColor
        );

        // Main speedometer arc
        AddFromMesh(
            MeshFactory.CreateArc(0.75f, 200.0f, -20.0f, 64, white3),
            PrimitiveType.LineStrip,
            new Vector3(0.0f, -0.25f, 0.0f),
            Vector3.One,
            0.0f,
            white
        );

        // Tick marks
        AddFromMesh(
            MeshFactory.CreateTickMarks(0.62f, 0.75f, 200.0f, -20.0f, 13, white3),
            PrimitiveType.Lines,
            new Vector3(0.0f, -0.25f, 0.0f),
            Vector3.One,
            0.0f,
            white
        );

        // Red warning ticks on the right side
        AddFromMesh(
            MeshFactory.CreateTickMarks(0.58f, 0.75f, 20.0f, -20.0f, 4, red3),
            PrimitiveType.Lines,
            new Vector3(0.0f, -0.25f, 0.0f),
            Vector3.One,
            0.0f,
            red
        );

        // Center circle
        AddFromMesh(
            MeshFactory.CreateCircle(32, 1.0f, 1.0f, 1.0f, 0.055f),
            PrimitiveType.TriangleFan,
            new Vector3(0.0f, -0.25f, 0.0f),
            Vector3.One,
            0.0f,
            white
        );

        // Needle
        // Important: keep this as item index 5 if your update logic depends on index.
        AddFromMesh(
            MeshFactory.CreateNeedle(0.055f, 0.55f, yellow3),
            PrimitiveType.Triangles,
            new Vector3(0.0f, -0.25f, 0.0f),
            Vector3.One,
            110.0f * MathF.PI / 180.0f,
            yellow
        );

        // Left number
        AddFromMesh(
            MeshFactory.CreateSevenSegmentNumber("0", 0.12f, 0.22f, 0.025f, 0.03f, white3),
            PrimitiveType.Triangles,
            new Vector3(-0.63f, -0.52f, 0.0f),
            Vector3.One,
            0.0f,
            white
        );

        // Middle number
        AddFromMesh(
            MeshFactory.CreateSevenSegmentNumber("60", 0.10f, 0.20f, 0.022f, 0.025f, white3),
            PrimitiveType.Triangles,
            new Vector3(0.0f, 0.35f, 0.0f),
            Vector3.One,
            0.0f,
            white
        );

        // Right number
        AddFromMesh(
            MeshFactory.CreateSevenSegmentNumber("120", 0.09f, 0.18f, 0.020f, 0.022f, red3),
            PrimitiveType.Triangles,
            new Vector3(0.62f, -0.52f, 0.0f),
            Vector3.One,
            0.0f,
            red
        );

        return (loadedRenderItems.ToArray(), bounds);
    }

}
