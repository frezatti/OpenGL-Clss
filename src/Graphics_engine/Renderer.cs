using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Graphics_engine.Shader;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using Graphics_engine.Scenes;

namespace Graphics_engine;

public class Window : GameWindow
{
    private IScene _current_scene;
    private Dictionary<Mesh, GPUMesh> _gpu_mesh = new Dictionary<Mesh, GPUMesh>();
    private int _shaderProgram;
    private int _transformLocation;
    private int _baseColorLocation;
    private int _colorModeLocation;



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

        // Vertex Shader and Frament Shader Compilation
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, GLSL.vertexShader);
        GL.CompileShader(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, GLSL.fragmentShader);
        GL.CompileShader(fragmentShader);

        // Creating a Program with the Shaders
        _shaderProgram = GL.CreateProgram();

        //Attaching the Shader to the Program;
        GL.AttachShader(_shaderProgram, vertexShader);
        GL.AttachShader(_shaderProgram, fragmentShader);

        GL.LinkProgram(_shaderProgram);

        _transformLocation = GL.GetUniformLocation(_shaderProgram, "transform");
        _baseColorLocation = GL.GetUniformLocation(_shaderProgram, "baseColor");
        _colorModeLocation = GL.GetUniformLocation(_shaderProgram, "colorMode");

        GL.DetachShader(_shaderProgram, vertexShader);
        GL.DetachShader(_shaderProgram, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        _current_scene = new SpeedScene();

        UploadRenderItems(_current_scene.Objects);

    }

    private void UploadRenderItems(IReadOnlyList<SceneObject> renderItems)
    {
        var mesh_hash = renderItems.Select(item => item.Mesh).ToHashSet();

        foreach (var item in mesh_hash)
        {
            var gpumesh = new GPUMesh();
            GL.CreateBuffers(1, out int vbo);
            gpumesh.VBO = vbo;
            GL.NamedBufferData(gpumesh.VBO, item.Vertice_Data.Length * sizeof(float), item.Vertice_Data, BufferUsageHint.StaticDraw);

            // Creates the VAO (Vertex Array Buffer)
            GL.CreateVertexArrays(1, out int vao);
            gpumesh.VAO = vao;
            GL.VertexArrayVertexBuffer(gpumesh.VAO, 0, gpumesh.VBO, IntPtr.Zero, 6 * sizeof(float));

            // Shader Binding and Settings. (Positions)
            GL.EnableVertexArrayAttrib(gpumesh.VAO, 0);
            GL.VertexArrayAttribFormat(gpumesh.VAO, 0, 3, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(gpumesh.VAO, 0, 0);

            // Shader Binding and Setting. (Color)
            GL.EnableVertexArrayAttrib(gpumesh.VAO, 1);
            GL.VertexArrayAttribFormat(gpumesh.VAO, 1, 3, VertexAttribType.Float, false, 3 * sizeof(float));
            GL.VertexArrayAttribBinding(gpumesh.VAO, 1, 0);
            _gpu_mesh.Add(item, gpumesh);
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
            KeyboardState = this.KeyboardState,
            ClientWidth = ClientSize.X,
            ClientHeight = ClientSize.Y,
        };
        _current_scene.Update(context);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.UseProgram(_shaderProgram);

        foreach (var item in _current_scene.Objects)
        {
            if (!item.Visible) continue;

            if (!_gpu_mesh.TryGetValue(item.Mesh, out var gpumesh) || gpumesh is null) continue;

            GL.BindVertexArray(gpumesh.VAO);

            var scale = Matrix4.CreateScale(
                item.Transform.Scale.X,
                item.Transform.Scale.Y,
                item.Transform.Scale.Z
            );

            var rotation = Matrix4.CreateRotationZ(item.Transform.Rotation);

            var translate = Matrix4.CreateTranslation(
                item.Transform.Position.X,
                item.Transform.Position.Y,
                item.Transform.Position.Z
            );

            var final_matrix = scale * rotation * translate;

            GL.UniformMatrix4(_transformLocation, true, ref final_matrix);

            GL.Uniform4(
                _baseColorLocation,
                item.Material.BaseColor.X,
                item.Material.BaseColor.Y,
                item.Material.BaseColor.Z,
                item.Material.BaseColor.W
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
}

