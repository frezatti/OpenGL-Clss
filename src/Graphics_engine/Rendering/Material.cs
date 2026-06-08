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

    public string? BaseColorTexturePath;
    public string? MetallicTexturePath;
    public string? RoughnessTexturePath;
    public string? AmbientOcclusionTexturePath;

    public Vector2 TextureScale;
    public float Metallic;
    public float Roughness;
    public float AmbientOcclusion;

    public bool UseBaseColorTexture => !string.IsNullOrWhiteSpace(BaseColorTexturePath);
    public bool UseMetallicTexture => !string.IsNullOrWhiteSpace(MetallicTexturePath);
    public bool UseRoughnessTexture => !string.IsNullOrWhiteSpace(RoughnessTexturePath);
    public bool UseAmbientOcclusionTexture => !string.IsNullOrWhiteSpace(AmbientOcclusionTexturePath);

    // Compatibility alias for older renderer/factory code.
    public string? TexturePath
    {
        get => BaseColorTexturePath;
        set => BaseColorTexturePath = value;
    }

    public bool UseTexture => UseBaseColorTexture;

    public Material(Vector4 baseColor, ColorMode colorMode, string? texturePath = null, Vector2? textureScale = null)
    {
        BaseColor = baseColor;
        ColorMode = colorMode;
        BaseColorTexturePath = texturePath;
        MetallicTexturePath = null;
        RoughnessTexturePath = null;
        AmbientOcclusionTexturePath = null;
        TextureScale = textureScale ?? Vector2.One;
        Metallic = 0.0f;
        Roughness = 0.5f;
        AmbientOcclusion = 1.0f;
    }

    public static Material CreatePbr(
        Vector4 baseColor,
        ColorMode colorMode,
        string? baseColorTexturePath,
        string? metallicTexturePath,
        string? roughnessTexturePath,
        string? ambientOcclusionTexturePath,
        Vector2? textureScale = null,
        float metallic = 1.0f,
        float roughness = 0.35f,
        float ambientOcclusion = 1.0f)
    {
        return new Material(baseColor, colorMode)
        {
            BaseColorTexturePath = baseColorTexturePath,
            MetallicTexturePath = metallicTexturePath,
            RoughnessTexturePath = roughnessTexturePath,
            AmbientOcclusionTexturePath = ambientOcclusionTexturePath,
            TextureScale = textureScale ?? Vector2.One,
            Metallic = metallic,
            Roughness = roughness,
            AmbientOcclusion = ambientOcclusion
        };
    }

    public static Material Default()
    {
        return new Material(
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            ColorMode.VertexColor
        );
    }
}
