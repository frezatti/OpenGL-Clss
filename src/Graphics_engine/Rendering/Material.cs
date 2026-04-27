using System.Numerics;

public enum ColorMode
{
    VertexColor = 0,
    SolidColor = 1,
    Tinted = 2
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
