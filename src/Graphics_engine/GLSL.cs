namespace Graphics_engine.Shader;

public static class GLSL
{
    public const string vertexShader = @" 
                #version 330 core

                layout (location = 0) in vec3 aPosition; 

                void main()
                {
                    gl_Position = vec4(aPosition, 1.0); 
                }";

    public const string fragmentShader = @"
                #version 330 core

                out vec4 FragColor;

                void main()
                {
                    FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f); 
                }
                        ";
}
