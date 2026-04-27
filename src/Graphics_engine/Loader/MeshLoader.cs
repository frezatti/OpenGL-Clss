using System.Globalization;
using Graphics_engine;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

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

    public static RenderItem[] LoadExercise1()
    {
        var loadedRenderItems = new List<RenderItem>();

        void AddFromFile(string path, PrimitiveType primitiveType, Vector3 position, Vector3 scale)
        {
            if (!MeshLoader.TryLoadFromFile(path, out Mesh mesh, out LoaderError error))
            {
                throw new Exception($"Failed to load mesh: {path}\n{error.Message}");
            }

            var item = new RenderItem();
            item.Mesh = mesh;
            item.Rendering_Type = primitiveType;
            item.Transfom.Position = position;
            item.Transfom.Scale = scale;
            item.Transfom.Rotation = 0.0f;

            loadedRenderItems.Add(item);
        }

        var leftPosition = new Vector3(-0.55f, 0.00f, 0.0f);
        var rightPosition = new Vector3(0.55f, 0.00f, 0.0f);
        var birdScale = new Vector3(0.42f, 0.42f, 1.0f);

        // Filled bird (left)
        AddFromFile("Assets/bird_leg1_fill.txt", PrimitiveType.Triangles, leftPosition, birdScale);
        AddFromFile("Assets/bird_leg2_fill.txt", PrimitiveType.Triangles, leftPosition, birdScale);
        AddFromFile("Assets/bird_tail_fill.txt", PrimitiveType.Triangles, leftPosition, birdScale);
        AddFromFile("Assets/bird_body_fill.txt", PrimitiveType.TriangleFan, leftPosition, birdScale);
        AddFromFile("Assets/bird_head_fill.txt", PrimitiveType.TriangleFan, leftPosition, birdScale);
        AddFromFile("Assets/bird_wing_fill.txt", PrimitiveType.Triangles, leftPosition, birdScale);
        AddFromFile("Assets/bird_beak_fill.txt", PrimitiveType.Triangles, leftPosition, birdScale);
        AddFromFile("Assets/bird_eye_fill.txt", PrimitiveType.TriangleFan, leftPosition, birdScale);

        // Outline bird (right)
        AddFromFile("Assets/bird_leg1_outline.txt", PrimitiveType.LineStrip, rightPosition, birdScale);
        AddFromFile("Assets/bird_leg2_outline.txt", PrimitiveType.LineStrip, rightPosition, birdScale);
        AddFromFile("Assets/bird_tail_outline.txt", PrimitiveType.LineLoop, rightPosition, birdScale);
        AddFromFile("Assets/bird_body_outline.txt", PrimitiveType.LineLoop, rightPosition, birdScale);
        AddFromFile("Assets/bird_head_outline.txt", PrimitiveType.LineLoop, rightPosition, birdScale);
        AddFromFile("Assets/bird_wing_outline.txt", PrimitiveType.LineLoop, rightPosition, birdScale);
        AddFromFile("Assets/bird_beak_outline.txt", PrimitiveType.LineLoop, rightPosition, birdScale);
        AddFromFile("Assets/bird_eye_outline.txt", PrimitiveType.LineLoop, rightPosition, birdScale);

        return loadedRenderItems.ToArray();
    }

    public static (RenderItem[], Dictionary<Mesh, Bounds2D>) LoadExercise2()
    {
        var loadedRenderItems = new List<RenderItem>();
        var bounds = new Dictionary<Mesh, Bounds2D>();

        void AddFromFile(string path, PrimitiveType primitiveType, Vector3 position, Vector3 scale)
        {
            if (!MeshLoader.TryLoadFromFile(path, out Mesh mesh, out LoaderError error))
            {
                throw new Exception($"Failed to load mesh: {path}\n{error.Message}");
            }

            var item = new RenderItem();
            item.Mesh = mesh;
            item.Rendering_Type = primitiveType;
            item.Transfom.Position = position;
            item.Transfom.Scale = scale;
            item.Transfom.Rotation = 0.0f;
            var material = item.Material;
            material.ColorMode = ColorMode.SolidColor;
            item.Material = material;


            var xmax = mesh.Vertice_Data[0];
            var xmin = xmax;
            var ymax = mesh.Vertice_Data[1];
            var ymin = ymax;

            for (var i = 0; i < mesh.Vertice_Data.Length; i += 6)
            {

                if (mesh.Vertice_Data[i] > xmax)
                {
                    xmax = mesh.Vertice_Data[i];
                }
                if (mesh.Vertice_Data[i] < xmin)
                {
                    xmin = mesh.Vertice_Data[i];
                }

                if (mesh.Vertice_Data[i + 1] > ymax)
                {
                    ymax = mesh.Vertice_Data[i + 1];
                }
                if (mesh.Vertice_Data[i + 1] < ymin)
                {
                    ymin = mesh.Vertice_Data[i + 1];
                }
            }
            var bound = new Bounds2D() { MaxX = xmax, MinX = xmin, MaxY = ymax, MinY = ymin };
            bounds.Add(mesh, bound);
            loadedRenderItems.Add(item);
        }

        var item = new RenderItem();
        item.Mesh = MeshFactory.CreateGrid();
        item.Rendering_Type = PrimitiveType.Lines;
        loadedRenderItems.Add(item);

        var scale = new Vector3(0.42f, 0.42f, 1.0f);

        // Filled bird (left)
        AddFromFile("Assets/letter_1.txt", PrimitiveType.Triangles, (-0.3f, 0.0f, 0.0f), scale);
        AddFromFile("Assets/letter_2.txt", PrimitiveType.Triangles, (0.0f, 0.0f, 0.0f), scale);
        AddFromFile("Assets/letter_1.txt", PrimitiveType.Triangles, (0.3f, 0.0f, 0.0f), scale);


        return (loadedRenderItems.ToArray(), bounds);
    }

}
