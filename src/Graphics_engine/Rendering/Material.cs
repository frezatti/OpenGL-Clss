using System.Numerics;

public enum ColorMode
{
    VertexColor = 1,      // use the colors from the mesh
    SolidColor = 2,       // ignore vertex colors and use one material color
    Tinted = 3     // multiply vertex colors by the material color
}

public struct Material
{
    public Vector4 BaseColor;
    public ColorMode ColorMode;

    public Material(Vector4 baseColor, ColorMode colorMode)
    {
        BaseColor = baseColor;
        ColorMode = colorMode;
    }

    public static Material Default()
    {
        return new Material(
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            ColorMode.VertexColor
        );
    }
}
