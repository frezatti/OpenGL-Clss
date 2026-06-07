namespace Graphics_engine.Shader;

public static class GLSL
{
    public const string vertexShader = @" 
                #version 330 core

                layout (location = 0) in vec3 aPosition; 
                layout (location = 1) in vec3 aColor; 

                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;

                out vec3 vertexColor;

                void main()
                {
                    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
                    vertexColor = aColor;
                }";

    public static string fragmentShader = @"
                #version 330 core

                in vec3 vertexColor;
                out vec4 FragColor;

                uniform vec4 baseColor;
                uniform int colorMode;
                uniform bool selected;

                void main()
                {
                    vec3 finalColor = vertexColor;

                    if (colorMode == 1)
                    {
                        finalColor = baseColor.rgb;
                    }
                    else if (colorMode == 2)
                    {
                        finalColor = vertexColor * baseColor.rgb;
                    }

                    if (selected)
                    {
                        finalColor = min(finalColor + vec3(0.25), vec3(1.0));
                    }

                    FragColor = vec4(finalColor, baseColor.a);
                }
                ";
}
