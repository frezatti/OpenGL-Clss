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
    public string? TexturePath;
    public Vector2 TextureScale;

    public bool UseTexture => !string.IsNullOrWhiteSpace(TexturePath);

    public Material(Vector4 baseColor, ColorMode colorMode, string? texturePath = null, Vector2? textureScale = null)
    {
        BaseColor = baseColor;
        ColorMode = colorMode;
        TexturePath = texturePath;
        TextureScale = textureScale ?? Vector2.One;
    }

    public static Material Default()
    {
        return new Material(
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            ColorMode.VertexColor
        );
    }
}
