namespace Graphics_engine.Shader;

public static class GLSL
{
    public const string vertexShader = @" 
                #version 330 core

                layout (location = 0) in vec3 aPosition; 
                layout (location = 1) in vec3 aColor; 

                uniform mat4 transform;

                out vec3 vertexColor;

                void main()
                {
                    gl_Position = vec4(aPosition, 1.0) * transform;
                    vertexColor = aColor;
                }";

    public const string fragmentShader = @"
                #version 330 core
                
                in vec3 vertexColor;

                out vec4 FragColor;

                void main()
                {
                    FragColor = vec4(vertexColor,1.0f); 

                }
                        ";
}
