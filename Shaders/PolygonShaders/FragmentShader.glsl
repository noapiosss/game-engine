#version 400 core

in FS
{
    vec4 fragColor;
    vec3 fragNormal;
    vec3 fragBarycentric;
} source;

out vec4 color;

void main()
{
    //if (any(lessThan(source.fragBarycentric, vec3(0.005))))
    //{
    //    color = vec4(1.0, 1.0, 1.0, 1.0);
    //    return;
    //}
    
    //vec3 normalizedNormal = normalize(source.fragNormal);                
    //vec3 normalizedLightDir = normalize(lightDirection);

    //float diffuseFactor = acos(dot(normalizedNormal, normalizedLightDir) / (normalizedNormal.length * normalizedLightDir.length)) / 3.14f;
    //vec3 diffuseColor = source.fragColor.xyz * diffuseFactor;

    color = source.fragColor;
}