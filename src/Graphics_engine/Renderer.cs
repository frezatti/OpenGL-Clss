using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Graphics_engine.Shader;
using OpenTK.Mathematics;

namespace Graphics_engine;

public class Window : GameWindow
{
    readonly private RenderItem[] _render_items;
    private Dictionary<Mesh, GPUMesh> _gpu_mesh = new Dictionary<Mesh, GPUMesh>();
    readonly private RenderItem ball;
    private int _shaderProgram;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        var loaded_Render_Items = new List<RenderItem>();

        ball = new RenderItem();
        ball.Mesh = MeshFactory.CreateCircle(60, 1.0f, 1.0f, 1.0f);
        ball.Transfom.Scale = (0.2f, 0.2f, 1.0f);
        ball.Rendering_Type = PrimitiveType.TriangleFan;
        loaded_Render_Items.Add(ball);


        _render_items = loaded_Render_Items.ToArray();
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        //        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

        GL.ClearColor(0.12f, 0.12f, 0.14f, 0.3f);
        var mesh_hash = _render_items.Select(item => item.Mesh).ToHashSet();

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

        GL.DetachShader(_shaderProgram, vertexShader);
        GL.DetachShader(_shaderProgram, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
        {
            Close();
        }

        if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right))
        {
            var vector = ball.Transfom.Position;
            vector[0] += 0.001f;
            if (vector[0] > 1.0f) vector[0] = 1.0f;
            ball.Transfom.Position = vector;
        }
        if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left))
        {
            var vector = ball.Transfom.Position;
            vector[0] -= 0.001f;
            if (vector[0] < -1.0f) vector[0] = -1.0f;
            ball.Transfom.Position = vector;
        }
        if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up))
        {
            var vector = ball.Transfom.Position;
            vector[1] += 0.001f;
            if (vector[1] > 1.0f) vector[1] = 1.0f;
            ball.Transfom.Position = vector;
        }
        if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Down))
        {
            var vector = ball.Transfom.Position;
            vector[1] -= 0.001f;
            if (vector[1] < -1.0f) vector[1] = -1.0f;
            ball.Transfom.Position = vector;
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.UseProgram(_shaderProgram);


        int transformLocation = GL.GetUniformLocation(_shaderProgram, "transform");


        foreach (var item in _render_items)
        {
            if (!_gpu_mesh.TryGetValue(item.Mesh, out GPUMesh gpumesh) || gpumesh is null) continue;

            // Shape
            GL.BindVertexArray(gpumesh.VAO);

            // Transform postion into a tranlastion.Vec3 rotation to Matrix4 rotation, and same for the scale
            var translate = Matrix4.CreateTranslation(item.Transfom.Position);
            var rotation = Matrix4.CreateRotationZ(item.Transfom.Rotation);
            var scale = Matrix4.CreateScale(item.Transfom.Scale);

            var final_matrix = scale * rotation * translate;

            GL.UniformMatrix4(transformLocation, true, ref final_matrix);
            GL.DrawArrays(item.Rendering_Type, 0, item.Mesh.Vertex_Count);
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

