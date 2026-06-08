namespace Graphics_engine.Shader;

public static class GLSL
{
    public const string vertexShader = @" 
                #version 330 core

                layout (location = 0) in vec3 aPosition; 
                layout (location = 1) in vec3 aColor; 
                layout (location = 2) in vec3 aNormal;
                layout (location = 3) in vec2 aTexCoord;

                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;

                out vec3 vertexColor;
                out vec3 worldNormal;
                out vec2 textureCoordinate;

                void main()
                {
                    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
                    vertexColor = aColor;
                    worldNormal = normalize((vec4(aNormal, 0.0) * model).xyz);
                    textureCoordinate = aTexCoord;
                }";

    public static string fragmentShader = @"
                #version 330 core

                in vec3 vertexColor;
                in vec3 worldNormal;
                in vec2 textureCoordinate;
                out vec4 FragColor;

                uniform vec4 baseColor;
                uniform int colorMode;
                uniform bool selected;
                uniform vec3 lightDirection;
                uniform vec3 lightColor;
                uniform float ambientStrength;
                uniform bool useTexture;
                uniform sampler2D diffuseMap;

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

                    if (useTexture)
                    {
                        finalColor *= texture(diffuseMap, textureCoordinate).rgb;
                    }

                    vec3 normal = normalize(worldNormal);
                    vec3 lightDir = normalize(-lightDirection);
                    float diffuseStrength = max(dot(normal, lightDir), 0.0);
                    vec3 lighting = (ambientStrength + diffuseStrength) * lightColor;

                    finalColor *= lighting;

                    if (selected)
                    {
                        finalColor = min(finalColor + vec3(0.25), vec3(1.0));
                    }

                    FragColor = vec4(finalColor, baseColor.a);
                }
                ";
}
