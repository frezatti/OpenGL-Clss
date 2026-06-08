using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Graphics_engine.Shader;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using Graphics_engine.Scenes;
using NumericsMatrix4x4 = System.Numerics.Matrix4x4;

namespace Graphics_engine;

public class Window : GameWindow
{
    private IScene _current_scene;
    private readonly Dictionary<Mesh, GPUMesh> _gpu_mesh = new Dictionary<Mesh, GPUMesh>();
    private readonly Camera _camera = new();

    private int _shaderProgram;
    private int _modelLocation;
    private int _viewLocation;
    private int _projectionLocation;
    private int _baseColorLocation;
    private int _colorModeLocation;
    private MouseState _previousMouseState;
    private int _selectedLocation;
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
            GL.VertexArrayVertexBuffer(gpumesh.VAO, 0, gpumesh.VBO, IntPtr.Zero, 6 * sizeof(float));

            GL.EnableVertexArrayAttrib(gpumesh.VAO, 0);
            GL.VertexArrayAttribFormat(gpumesh.VAO, 0, 3, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(gpumesh.VAO, 0, 0);

            GL.EnableVertexArrayAttrib(gpumesh.VAO, 1);
            GL.VertexArrayAttribFormat(gpumesh.VAO, 1, 3, VertexAttribType.Float, false, 3 * sizeof(float));
            GL.VertexArrayAttribBinding(gpumesh.VAO, 1, 0);

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

        var context = new SceneContext()
        {
            DeltaTime = (float)args.Time,
            MouseState = this.MouseState,
            PreviousMouseState = _previousMouseState,
            KeyboardState = this.KeyboardState,
            PreviousKeyboardState = _previousKeyboardState,
            ClientWidth = ClientSize.X,
            ClientHeight = ClientSize.Y,
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

        var view = ToOpenTkMatrix(_camera.GetViewMatrix());
        var projection = ToOpenTkMatrix(_camera.GetProjectionMatrix(aspectRatio));

        GL.UniformMatrix4(_viewLocation, true, ref view);
        GL.UniformMatrix4(_projectionLocation, true, ref projection);

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


            var baseColor = item.Material.BaseColor;
            GL.Uniform1(_selectedLocation, item.Selected ? 1 : 0);

            GL.Uniform4(
                _baseColorLocation,
                baseColor.X,
                baseColor.Y,
                baseColor.Z,
                baseColor.W
            );

            GL.Uniform1(_colorModeLocation, (int)item.Material.ColorMode);
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

        GL.DeleteProgram(_shaderProgram);
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
