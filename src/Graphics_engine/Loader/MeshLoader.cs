using System.Globalization;
using Graphics_engine;

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
}
