using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Graphics_engine;

public static class TextureLoader
{
    public static bool TryLoadFromFile(string path, out int texture)
    {
        texture = 0;

        if (!TryResolvePath(path, out string resolvedPath))
        {
            return false;
        }

        StbImage.stbi_set_flip_vertically_on_load(1);

        using var stream = File.OpenRead(resolvedPath);
        var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        texture = CreateTexture(image.Width, image.Height, image.Data);
        return true;
    }

    private static bool TryResolvePath(string path, out string resolvedPath)
    {
        string[] candidates =
        {
            path,
            Path.Combine(AppContext.BaseDirectory, path),
            Path.Combine(Directory.GetCurrentDirectory(), path),
            Path.Combine(Directory.GetCurrentDirectory(), "src", "Graphics_engine", path)
        };

        foreach (string candidate in candidates)
        {
            string fullPath = Path.GetFullPath(candidate);

            if (File.Exists(fullPath))
            {
                resolvedPath = fullPath;
                return true;
            }
        }

        resolvedPath = string.Empty;
        return false;
    }

    private static int CreateTexture(int width, int height, byte[] pixels)
    {
        GL.GenTextures(1, out int texture);
        GL.BindTexture(TextureTarget.Texture2D, texture);

        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        return texture;
    }
}