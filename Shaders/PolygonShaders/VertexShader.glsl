#version 400 core            

in vec3 inPosition;
in vec4 inColor;
in vec3 inNormal;
in vec3 inBarycentric;

out VS
{
    vec4 geomColor;
    vec3 geomNormal;
    vec3 geomBarycentric;
} dest;

void main()
{

    gl_Position = vec4(inPosition, 1f);

    dest.geomColor = inColor;
    dest.geomNormal = inNormal;
    dest.geomBarycentric = inBarycentric;
}