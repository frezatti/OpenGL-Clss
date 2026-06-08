using System.Numerics;

public enum ColorMode
{
    VertexColor = 0,      // use the colors from the mesh
    SolidColor = 1,       // ignore vertex colors and use one material color
    Tinted = 2            // multiply vertex colors by the material color
}

public struct Material
{
    public Vector4 BaseColor;
    public ColorMode ColorMode;
    public bool UseTexture;

    public Material(Vector4 baseColor, ColorMode colorMode, bool useTexture = false)
    {
        BaseColor = baseColor;
        ColorMode = colorMode;
        UseTexture = useTexture;
    }

    public static Material Default()
    {
        return new Material(
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            ColorMode.VertexColor,
            false
        );
    }
}
