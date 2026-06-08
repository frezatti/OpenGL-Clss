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
                out vec3 worldPosition;
                out vec2 textureCoordinate;

                void main()
                {
                    vec4 world = vec4(aPosition, 1.0) * model;

                    gl_Position = world * view * projection;
                    vertexColor = aColor;
                    worldNormal = normalize((vec4(aNormal, 0.0) * model).xyz);
                    worldPosition = world.xyz;
                    textureCoordinate = aTexCoord;
                }";

    public static string fragmentShader = @"
                #version 330 core

                in vec3 vertexColor;
                in vec3 worldNormal;
                in vec3 worldPosition;
                in vec2 textureCoordinate;
                out vec4 FragColor;

                uniform vec4 baseColor;
                uniform int colorMode;
                uniform bool selected;

                uniform vec3 lightDirection;
                uniform vec3 lightColor;
                uniform float ambientStrength;
                uniform vec3 cameraPosition;

                uniform sampler2D baseColorMap;
                uniform sampler2D metallicMap;
                uniform sampler2D roughnessMap;
                uniform sampler2D ambientOcclusionMap;

                uniform bool useBaseColorMap;
                uniform bool useMetallicMap;
                uniform bool useRoughnessMap;
                uniform bool useAmbientOcclusionMap;

                uniform vec2 textureScale;
                uniform float materialMetallic;
                uniform float materialRoughness;
                uniform float materialAmbientOcclusion;

                void main()
                {
                    vec2 uv = textureCoordinate * textureScale;

                    vec3 albedo = vertexColor;
                    float finalAlpha = baseColor.a;

                    if (colorMode == 1)
                    {
                        albedo = baseColor.rgb;
                    }
                    else if (colorMode == 2)
                    {
                        albedo = vertexColor * baseColor.rgb;
                    }

                    if (useBaseColorMap)
                    {
                        vec4 baseTexture = texture(baseColorMap, uv);
                        albedo *= baseTexture.rgb;
                        finalAlpha *= baseTexture.a;
                    }

                    float metallic = materialMetallic;
                    if (useMetallicMap)
                    {
                        metallic *= texture(metallicMap, uv).r;
                    }

                    float roughness = materialRoughness;
                    if (useRoughnessMap)
                    {
                        roughness *= texture(roughnessMap, uv).r;
                    }
                    roughness = clamp(roughness, 0.04, 1.0);

                    float ao = materialAmbientOcclusion;
                    if (useAmbientOcclusionMap)
                    {
                        ao *= texture(ambientOcclusionMap, uv).r;
                    }
                    ao = clamp(ao, 0.0, 1.0);

                    vec3 normal = normalize(worldNormal);
                    vec3 lightDir = normalize(-lightDirection);
                    vec3 viewDir = normalize(cameraPosition - worldPosition);
                    vec3 halfDir = normalize(lightDir + viewDir);

                    float diffuseStrength = max(dot(normal, lightDir), 0.0);

                    float shininess = mix(8.0, 128.0, 1.0 - roughness);
                    float specularStrength = pow(max(dot(normal, halfDir), 0.0), shininess);
                    vec3 specularColor = mix(vec3(0.04), albedo, metallic);
                    vec3 specular = specularColor * specularStrength * (1.0 - roughness) * lightColor;

                    vec3 diffuse = albedo * diffuseStrength * lightColor * (1.0 - metallic * 0.75);
                    vec3 ambient = albedo * ambientStrength * ao;

                    vec3 finalColor = ambient + diffuse + specular;

                    if (selected)
                    {
                        finalColor = min(finalColor + vec3(0.25), vec3(1.0));
                    }

                    FragColor = vec4(finalColor, finalAlpha);
                }
                ";
}
