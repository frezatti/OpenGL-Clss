using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Graphics_engine.Shader;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using Graphics_engine.Scenes;
using NumericsMatrix4x4 = System.Numerics.Matrix4x4;
using NumericsVector3 = System.Numerics.Vector3;

namespace Graphics_engine;

public class Window : GameWindow
{
    private IScene _current_scene;
    private readonly Dictionary<Mesh, GPUMesh> _gpu_mesh = new Dictionary<Mesh, GPUMesh>();
    private readonly Dictionary<string, int> _texturesByPath = new Dictionary<string, int>();
    private readonly Camera _camera = new();

    private int _shaderProgram;
    private int _modelLocation;
    private int _viewLocation;
    private int _projectionLocation;
    private int _baseColorLocation;
    private int _colorModeLocation;
    private MouseState _previousMouseState;
    private int _selectedLocation;
    private int _lightDirectionLocation;
    private int _lightColorLocation;
    private int _ambientStrengthLocation;
    private int _cameraPositionLocation;

    private int _baseColorMapLocation;
    private int _metallicMapLocation;
    private int _roughnessMapLocation;
    private int _ambientOcclusionMapLocation;

    private int _useBaseColorMapLocation;
    private int _useMetallicMapLocation;
    private int _useRoughnessMapLocation;
    private int _useAmbientOcclusionMapLocation;

    private int _textureScaleLocation;
    private int _materialMetallicLocation;
    private int _materialRoughnessLocation;
    private int _materialAmbientOcclusionLocation;

    private KeyboardState _previousKeyboardState;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        GL.ClearColor(0.35f, 0.35f, 0.38f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, GLSL.vertexShader);
        GL.CompileShader(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, GLSL.fragmentShader);
        GL.CompileShader(fragmentShader);

        _shaderProgram = GL.CreateProgram();
        GL.AttachShader(_shaderProgram, vertexShader);
        GL.AttachShader(_shaderProgram, fragmentShader);
        GL.LinkProgram(_shaderProgram);

        _modelLocation = GL.GetUniformLocation(_shaderProgram, "model");
        _viewLocation = GL.GetUniformLocation(_shaderProgram, "view");
        _projectionLocation = GL.GetUniformLocation(_shaderProgram, "projection");
        _baseColorLocation = GL.GetUniformLocation(_shaderProgram, "baseColor");
        _colorModeLocation = GL.GetUniformLocation(_shaderProgram, "colorMode");
        _selectedLocation = GL.GetUniformLocation(_shaderProgram, "selected");
        _lightDirectionLocation = GL.GetUniformLocation(_shaderProgram, "lightDirection");
        _lightColorLocation = GL.GetUniformLocation(_shaderProgram, "lightColor");
        _ambientStrengthLocation = GL.GetUniformLocation(_shaderProgram, "ambientStrength");
        _cameraPositionLocation = GL.GetUniformLocation(_shaderProgram, "cameraPosition");

        _baseColorMapLocation = GL.GetUniformLocation(_shaderProgram, "baseColorMap");
        _metallicMapLocation = GL.GetUniformLocation(_shaderProgram, "metallicMap");
        _roughnessMapLocation = GL.GetUniformLocation(_shaderProgram, "roughnessMap");
        _ambientOcclusionMapLocation = GL.GetUniformLocation(_shaderProgram, "ambientOcclusionMap");

        _useBaseColorMapLocation = GL.GetUniformLocation(_shaderProgram, "useBaseColorMap");
        _useMetallicMapLocation = GL.GetUniformLocation(_shaderProgram, "useMetallicMap");
        _useRoughnessMapLocation = GL.GetUniformLocation(_shaderProgram, "useRoughnessMap");
        _useAmbientOcclusionMapLocation = GL.GetUniformLocation(_shaderProgram, "useAmbientOcclusionMap");

        _textureScaleLocation = GL.GetUniformLocation(_shaderProgram, "textureScale");
        _materialMetallicLocation = GL.GetUniformLocation(_shaderProgram, "materialMetallic");
        _materialRoughnessLocation = GL.GetUniformLocation(_shaderProgram, "materialRoughness");
        _materialAmbientOcclusionLocation = GL.GetUniformLocation(_shaderProgram, "materialAmbientOcclusion");

        GL.DetachShader(_shaderProgram, vertexShader);
        GL.DetachShader(_shaderProgram, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        _current_scene = new ModelingScene();
        UploadObjects(_current_scene.Objects);
    }

    private void SyncSceneObjects(IReadOnlyList<SceneObject> objects)
    {
        UploadObjects(objects);
    }

    private void UploadObjects(IReadOnlyList<SceneObject> objects)
    {
        var uniqueMeshes = objects.Select(item => item.Mesh).ToHashSet();

        foreach (var mesh in uniqueMeshes)
        {
            if (_gpu_mesh.ContainsKey(mesh))
            {
                continue;
            }

            var gpumesh = new GPUMesh();

            GL.CreateBuffers(1, out int vbo);
            gpumesh.VBO = vbo;
            GL.NamedBufferData(gpumesh.VBO, mesh.Vertice_Data.Length * sizeof(float), mesh.Vertice_Data, BufferUsageHint.StaticDraw);

            GL.CreateVertexArrays(1, out int vao);
            gpumesh.VAO = vao;

            int stride = Mesh.FloatsPerVertex * sizeof(float);
            GL.VertexArrayVertexBuffer(gpumesh.VAO, 0, gpumesh.VBO, IntPtr.Zero, stride);

            GL.EnableVertexArrayAttrib(gpumesh.VAO, 0);
            GL.VertexArrayAttribFormat(gpumesh.VAO, 0, Mesh.PositionFloatCount, VertexAttribType.Float, false, Mesh.PositionOffset * sizeof(float));
            GL.VertexArrayAttribBinding(gpumesh.VAO, 0, 0);

            GL.EnableVertexArrayAttrib(gpumesh.VAO, 1);
            GL.VertexArrayAttribFormat(gpumesh.VAO, 1, Mesh.ColorFloatCount, VertexAttribType.Float, false, Mesh.ColorOffset * sizeof(float));
            GL.VertexArrayAttribBinding(gpumesh.VAO, 1, 0);

            GL.EnableVertexArrayAttrib(gpumesh.VAO, 2);
            GL.VertexArrayAttribFormat(gpumesh.VAO, 2, Mesh.NormalFloatCount, VertexAttribType.Float, false, Mesh.NormalOffset * sizeof(float));
            GL.VertexArrayAttribBinding(gpumesh.VAO, 2, 0);

            GL.EnableVertexArrayAttrib(gpumesh.VAO, 3);
            GL.VertexArrayAttribFormat(gpumesh.VAO, 3, Mesh.TextureCoordinateFloatCount, VertexAttribType.Float, false, Mesh.TextureCoordinateOffset * sizeof(float));
            GL.VertexArrayAttribBinding(gpumesh.VAO, 3, 0);

            _gpu_mesh.Add(mesh, gpumesh);
        }
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
        {
            Close();
        }

        float aspectRatio = ClientSize.Y == 0 ? 1.0f : ClientSize.X / (float)ClientSize.Y;

        var context = new SceneContext()
        {
            DeltaTime = (float)args.Time,
            MouseState = this.MouseState,
            PreviousMouseState = _previousMouseState,
            KeyboardState = this.KeyboardState,
            PreviousKeyboardState = _previousKeyboardState,
            ClientWidth = ClientSize.X,
            ClientHeight = ClientSize.Y,
            ViewMatrix = _camera.GetViewMatrix(),
            ProjectionMatrix = _camera.GetProjectionMatrix(aspectRatio),
        };

        _current_scene.Update(context);
        _previousMouseState = this.MouseState;
        _previousKeyboardState = this.KeyboardState;
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.UseProgram(_shaderProgram);

        float aspectRatio = ClientSize.Y == 0 ? 1.0f : ClientSize.X / (float)ClientSize.Y;

        var numericsView = _camera.GetViewMatrix();
        var numericsProjection = _camera.GetProjectionMatrix(aspectRatio);
        var view = ToOpenTkMatrix(numericsView);
        var projection = ToOpenTkMatrix(numericsProjection);

        GL.UniformMatrix4(_viewLocation, true, ref view);
        GL.UniformMatrix4(_projectionLocation, true, ref projection);

        var cameraPosition = GetCameraWorldPosition(numericsView);
        GL.Uniform3(_cameraPositionLocation, cameraPosition.X, cameraPosition.Y, cameraPosition.Z);

        GL.Uniform3(_lightDirectionLocation, -0.4f, -1.0f, -0.6f);
        GL.Uniform3(_lightColorLocation, 1.0f, 0.96f, 0.88f);
        GL.Uniform1(_ambientStrengthLocation, 0.25f);

        GL.Uniform1(_baseColorMapLocation, 0);
        GL.Uniform1(_metallicMapLocation, 1);
        GL.Uniform1(_roughnessMapLocation, 2);
        GL.Uniform1(_ambientOcclusionMapLocation, 3);

        SyncSceneObjects(_current_scene.Objects);

        foreach (var item in _current_scene.Objects)
        {
            if (!item.Visible)
            {
                continue;
            }

            if (!_gpu_mesh.TryGetValue(item.Mesh, out var gpumesh) || gpumesh is null)
            {
                continue;
            }

            GL.BindVertexArray(gpumesh.VAO);

            var model = ToOpenTkMatrix(item.Transform.ToModelMatrix());
            GL.UniformMatrix4(_modelLocation, true, ref model);

            var material = item.Material;
            var baseColor = material.BaseColor;

            GL.Uniform1(_selectedLocation, item.Selected ? 1 : 0);

            bool baseColorBound = TryBindTexture(material.BaseColorTexturePath, TextureUnit.Texture0);
            bool metallicBound = TryBindTexture(material.MetallicTexturePath, TextureUnit.Texture1);
            bool roughnessBound = TryBindTexture(material.RoughnessTexturePath, TextureUnit.Texture2);
            bool aoBound = TryBindTexture(material.AmbientOcclusionTexturePath, TextureUnit.Texture3);

            GL.Uniform1(_useBaseColorMapLocation, baseColorBound ? 1 : 0);
            GL.Uniform1(_useMetallicMapLocation, metallicBound ? 1 : 0);
            GL.Uniform1(_useRoughnessMapLocation, roughnessBound ? 1 : 0);
            GL.Uniform1(_useAmbientOcclusionMapLocation, aoBound ? 1 : 0);

            GL.Uniform2(_textureScaleLocation, material.TextureScale.X, material.TextureScale.Y);
            GL.Uniform1(_materialMetallicLocation, material.Metallic);
            GL.Uniform1(_materialRoughnessLocation, material.Roughness);
            GL.Uniform1(_materialAmbientOcclusionLocation, material.AmbientOcclusion);

            GL.Uniform4(
                _baseColorLocation,
                baseColor.X,
                baseColor.Y,
                baseColor.Z,
                baseColor.W
            );

            GL.Uniform1(_colorModeLocation, (int)material.ColorMode);
            GL.DrawArrays(item.PrimitiveType, 0, item.Mesh.Vertex_Count);
        }

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        foreach (var item in _gpu_mesh)
        {
            GL.DeleteBuffer(item.Value.VBO);
            GL.DeleteVertexArray(item.Value.VAO);
        }

        foreach (var texture in _texturesByPath.Values)
        {
            GL.DeleteTexture(texture);
        }

        GL.DeleteProgram(_shaderProgram);
    }

    private bool TryBindTexture(string? texturePath, TextureUnit textureUnit)
    {
        GL.ActiveTexture(textureUnit);

        if (string.IsNullOrWhiteSpace(texturePath))
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            return false;
        }

        if (!_texturesByPath.TryGetValue(texturePath, out int texture))
        {
            if (!TextureLoader.TryLoadFromFile(texturePath, out texture))
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                return false;
            }

            _texturesByPath.Add(texturePath, texture);
        }

        GL.BindTexture(TextureTarget.Texture2D, texture);
        return true;
    }

    private static NumericsVector3 GetCameraWorldPosition(NumericsMatrix4x4 view)
    {
        if (!NumericsMatrix4x4.Invert(view, out var inverseView))
        {
            return NumericsVector3.Zero;
        }

        return new NumericsVector3(inverseView.M41, inverseView.M42, inverseView.M43);
    }

    private static Matrix4 ToOpenTkMatrix(NumericsMatrix4x4 m)
    {
        return new Matrix4(
            m.M11, m.M12, m.M13, m.M14,
            m.M21, m.M22, m.M23, m.M24,
            m.M31, m.M32, m.M33, m.M34,
            m.M41, m.M42, m.M43, m.M44
        );
    }
}