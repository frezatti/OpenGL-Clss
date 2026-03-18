using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Graphics_engine.Shader;

namespace Graphics_engine;

public class Window : GameWindow
{
    private readonly float[] _vertices =
    {
        //Triangle 01
       -0.5f, -0.5f, 0.0f, // Botton-left   
        0.5f, -0.5f, 0.0f, // Botton-right 
        0.0f,  0.5f, 0.0f,  // Top-center

        // Triangle 02
        0.5f,  0.5f, 0.0f, // Botton-left   
        0.5f,  0.5f, 0.0f, // Botton-right 
        0.0f, -0.5f, 0.0f  // Top-center
    };
    private int _vbo;
    private int _vao;
    private int _shaderProgram;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

        //Creates the VBO (Vertex Buffer Object)
        GL.CreateBuffers(1, out _vbo);
        GL.NamedBufferData(_vbo, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        // Creates the VAO (Vertex Array Buffer)
        GL.CreateVertexArrays(1, out _vao);
        GL.VertexArrayVertexBuffer(_vao, 0, _vbo, IntPtr.Zero, 3 * sizeof(float));

        // Shader Binding and Settings
        GL.EnableVertexArrayAttrib(_vao, 0);
        GL.VertexArrayAttribFormat(_vao, 0, 3, VertexAttribType.Float, false, 0);
        GL.VertexArrayAttribBinding(_vao, 0, 0);

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
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.UseProgram(_shaderProgram);

        GL.BindVertexArray(_vao);

        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        GL.DeleteBuffer(_vbo);

        GL.DeleteVertexArray(_vao);

        GL.DeleteProgram(_shaderProgram);
    }
}

