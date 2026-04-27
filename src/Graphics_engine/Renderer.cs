using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Graphics_engine.Shader;
using System.Numerics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Platform.Windows;
using OpenTK.Mathematics;
using NumVector4 = System.Numerics.Vector4;

namespace Graphics_engine;

public class Window : GameWindow
{
    readonly private RenderItem[] _render_items;
    private Dictionary<Mesh, GPUMesh> _gpu_mesh = new Dictionary<Mesh, GPUMesh>();
    private Dictionary<Mesh, Bounds2D> _bounds;
    private int _shaderProgram;
    private int _transformLocation;
    private int _baseColorLocation;
    private int _colorModeLocation;
    private int _current_color = 0;
    NumVector4[] rainbowColors =
    {
    new NumVector4(1.00f, 0.00f, 0.00f, 1.00f), // Red
    new NumVector4(1.00f, 0.50f, 0.00f, 1.00f), // Orange
    new NumVector4(1.00f, 1.00f, 0.00f, 1.00f), // Yellow
    new NumVector4(0.00f, 1.00f, 0.00f, 1.00f), // Green
    new NumVector4(0.00f, 0.50f, 1.00f, 1.00f), // Light blue / cyan-blue
    new NumVector4(0.00f, 0.00f, 1.00f, 1.00f), // Blue
    new NumVector4(0.50f, 0.00f, 1.00f, 1.00f), // Purple / violet
};


    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        (_render_items, _bounds) = MeshLoader.LoadExercise2();
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.85f, 0.85f, 0.88f, 1.0f);
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

        _transformLocation = GL.GetUniformLocation(_shaderProgram, "transform");
        _baseColorLocation = GL.GetUniformLocation(_shaderProgram, "baseColor");
        _colorModeLocation = GL.GetUniformLocation(_shaderProgram, "colorMode");

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

        if (MouseState.IsButtonPressed(MouseButton.Left))
        {
            Console.WriteLine($"MouseState X: {MouseState.X}\n");
            Console.WriteLine($"MouseState Y: {MouseState.Y}\n");
            Console.WriteLine("--------------------------------------------");

            var x_normalized = (MouseState.X * 2f) / (float)ClientSize.X - 1f;
            var y_normalized = 1 - (MouseState.Y * 2f) / (float)ClientSize.Y;
            Console.WriteLine($"x_normalized: {x_normalized}\n");
            Console.WriteLine($"y_normalized: {y_normalized}\n");
            Console.WriteLine("--------------------------------------------");
            foreach (var item in _render_items)
            {
                var localX = (x_normalized - item.Transfom.Position.X) / item.Transfom.Scale.X;
                var localY = (y_normalized - item.Transfom.Position.Y) / item.Transfom.Scale.Y;
                _bounds.TryGetValue(item.Mesh, out var bound);

                Console.WriteLine($"max X: {bound.MaxX},");
                Console.WriteLine($"min X: {bound.MinX},");
                Console.WriteLine($"max Y: {bound.MaxY},");
                Console.WriteLine($"min Y: {bound.MinY},\n");
                Console.WriteLine("--------------------------------------------\n");

                Console.WriteLine($"localX: {localX}\n");
                Console.WriteLine($"localY: {localY}\n");

                Console.WriteLine("--------------------------------------------");

                if (localX <= bound.MaxX && localX >= bound.MinX && localY <= bound.MaxY && localY >= bound.MinY)
                {
                    var material = item.Material;
                    material.BaseColor = rainbowColors[_current_color];
                    _current_color = (_current_color + 1) % rainbowColors.Length;
                }
            }
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.UseProgram(_shaderProgram);


        foreach (var item in _render_items)
        {
            if (!_gpu_mesh.TryGetValue(item.Mesh, out GPUMesh gpumesh) || gpumesh is null)
                continue;

            GL.BindVertexArray(gpumesh.VAO);

            var translate = Matrix4.CreateTranslation(item.Transfom.Position);
            var rotation = Matrix4.CreateRotationZ(item.Transfom.Rotation);
            var scale = Matrix4.CreateScale(item.Transfom.Scale);

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

