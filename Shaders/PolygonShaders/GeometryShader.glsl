#version 400 core

uniform mat4 ViewMatrix;
uniform mat4 ProjectMatrix;
uniform vec2 ViewportSize;
uniform float zNear;
uniform vec3 cameraDirection;
uniform vec3 cameraPosition;
uniform vec3 leftNormal;
uniform vec3 rightNormal;
uniform vec3 bottomNormal;
uniform vec3 topNormal;

uniform vec3 lightPosition;

layout(triangles) in;
layout(triangle_strip, max_vertices = 21) out;

in VS
{
    vec4 geomColor;
    vec3 geomNormal;
    vec3 geomBarycentric;
} source[];


out FS
{
    vec4 fragColor;
    vec3 fragNormal;
    vec3 fragBarycentric;
} dest;

struct polygon
{
    vec4 v0;
    vec4 v1;
    vec4 v2;
    vec4 color;
};

struct plane
{
    vec3 normal;
    vec3 point;
};

float getBrightness(vec3 point, vec3 pointNormal)
{
    vec3 normalizedNormal = normalize(pointNormal);                
    vec3 normalizedLightDir = normalize(lightPosition - point);

    return 1 - acos(dot(normalizedNormal, normalizedLightDir) / (normalizedNormal.length * normalizedLightDir.length)) / 3.14f;
}

float myDistance(vec3 planeNormal, vec3 planePoint, vec3 point)
{
    return dot(point - planePoint, planeNormal);
}

vec4 planeLineIntersection(vec3 planeNormal, vec3 planePoint, vec3 lineStart, vec3 lineEnd)
{
    vec3 direction = lineEnd - lineStart;
    vec3 planeToPoint = lineEnd - planePoint;

    float d = dot(planeNormal, direction);
    float t = -dot(planeNormal, planeToPoint) / d;

    return vec4(lineEnd + t * direction, 1.0);
}

void emitPolygon(vec4 v0, vec4 v1, vec4 v2, vec4 color)
{
    dest.fragColor = vec4(color.xyz * getBrightness(v0.xyz, source[0].geomNormal), 1f);
        
    dest.fragBarycentric = vec3(1,0,0);
    vec4 s = ProjectMatrix * ViewMatrix * v0;
    //s.y *= 2.779;
    gl_Position = s;
    EmitVertex();

    dest.fragColor = vec4(color.xyz * getBrightness(v1.xyz, source[0].geomNormal), 1f);

    dest.fragBarycentric = vec3(0,1,0);
    s = ProjectMatrix * ViewMatrix * v1;
    //s.y *= 2.779;
    gl_Position = s;
    EmitVertex();

    dest.fragColor = vec4(color.xyz * getBrightness(v2.xyz, source[0].geomNormal), 1f);

    dest.fragBarycentric = vec3(0,0,1);
    s = ProjectMatrix * ViewMatrix * v2;
    //s.y *= 2.779;
    gl_Position = s;
    EmitVertex();

    EndPrimitive();
}

int planeClip(polygon p, plane pl, inout polygon polygon1, inout polygon polygon2)
{
    float d1 = myDistance(pl.normal, pl.point, p.v0.xyz);
    float d2 = myDistance(pl.normal, pl.point, p.v1.xyz);
    float d3 = myDistance(pl.normal, pl.point, p.v2.xyz);

    if (d1 < 0 && d2 < 0 && d3 < 0)
    {
        return 0;
    }
    else if (d1 < 0 && d2 < 0)
    {
        vec4 v00 = planeLineIntersection(pl.normal, pl.point, p.v0.xyz, p.v2.xyz);
        vec4 v11 = planeLineIntersection(pl.normal, pl.point, p.v1.xyz, p.v2.xyz);

        polygon1 = polygon(v00, v11, p.v2, p.color);
        polygon2 = polygon(v00, v11, p.v2, p.color);
        return 1;
    }
    else if (d1 < 0 && d3 < 0)
    {
        vec4 v00 = planeLineIntersection(pl.normal, pl.point, p.v0.xyz, p.v1.xyz);
        vec4 v22 = planeLineIntersection(pl.normal, pl.point, p.v1.xyz, p.v2.xyz);
        
        polygon1 = polygon(v00, p.v1, v22, p.color);
        polygon2 = polygon(v00, p.v1, v22, p.color);
        return 1;
    }
    else if (d2 < 0 && d3 < 0)
    {
        vec4 v11 = planeLineIntersection(pl.normal, pl.point, p.v0.xyz, p.v1.xyz);
        vec4 v22 = planeLineIntersection(pl.normal, pl.point, p.v0.xyz, p.v2.xyz);
        
        polygon1 = polygon(p.v0, v11, v22, p.color);
        polygon2 = polygon(p.v0, v11, v22, p.color);
        return 1;
    }
    else if (d1 < 0)
    {
        vec4 v11 = planeLineIntersection(pl.normal, pl.point, p.v0.xyz, p.v1.xyz);
        vec4 v22 = planeLineIntersection(pl.normal, pl.point, p.v0.xyz, p.v2.xyz);
        
        polygon1 = polygon(v11, p.v1, p.v2, p.color);
        polygon2 = polygon(v22, v11, p.v2, p.color);
        return 2;
    }
    else if (d2 < 0)
    {
        vec4 v00 = planeLineIntersection(pl.normal, pl.point, p.v0.xyz, p.v1.xyz);
        vec4 v22 = planeLineIntersection(pl.normal, pl.point, p.v1.xyz, p.v2.xyz);
        
        polygon1 = polygon(v00, v22, p.v0, p.color);
        polygon2 = polygon(v22, p.v2, p.v0, p.color);
        return 2;
    }
    else if (d3 < 0)
    {
        vec4 v00 = planeLineIntersection(pl.normal, pl.point, p.v0.xyz, p.v2.xyz);
        vec4 v11 = planeLineIntersection(pl.normal, pl.point, p.v1.xyz, p.v2.xyz);
        
        polygon1 = polygon(v00, p.v0, p.v1, p.color);
        polygon2 = polygon(v11, v00, p.v1, p.color);
        return 2;
    }
    else
    {
        polygon1 = p;
        polygon2 = p;
        return 1;
    }
}

void main()
{
    dest.fragNormal = source[0].geomNormal;

    polygon polygon1 = polygon(gl_in[0].gl_Position, gl_in[1].gl_Position, gl_in[2].gl_Position, source[0].geomColor);
    polygon polygon2;

    float near = zNear;

    plane planes[5];

    planes[0] = plane(cameraDirection, cameraPosition+cameraDirection*near); //near
    planes[1] = plane(leftNormal, cameraPosition); //left
    planes[2] = plane(rightNormal, cameraPosition); //right
    planes[3] = plane(bottomNormal, cameraPosition); //bottom
    planes[4] = plane(topNormal, cameraPosition); //top
         
    int polygonsCount = planeClip(polygon1, planes[0], polygon1, polygon2);
    
    polygon polygons[9];
    polygon polygonsBuffer[9];

    if (polygonsCount == 1)
    {
        polygons[0] = polygon1;
    }
    if (polygonsCount == 2)
    {
        polygons[0] = polygon1;
        polygons[1] = polygon2;
    }

    for (int i = 1; i < 5; ++i)
    {
        int newPolygonsCount = 0;
        for (int j = 0; j < polygonsCount; ++j)
        {
            int newCulledPolygonsCount = planeClip(polygons[j], planes[i], polygon1, polygon2);
            
            if (newCulledPolygonsCount == 1)
            {
                polygonsBuffer[newPolygonsCount++] = polygon1;
            }
            if (newCulledPolygonsCount == 2)
            {
                polygonsBuffer[newPolygonsCount++] = polygon1;
                polygonsBuffer[newPolygonsCount++] = polygon2;
            }
        }

        for (int j = 0; j < newPolygonsCount; ++j)
        {
            polygons[j] = polygonsBuffer[j];
        }

        polygonsCount = newPolygonsCount;
    }

    for (int i = 0; i < polygonsCount; ++i)
    {        
        emitPolygon(polygons[i].v0, polygons[i].v1, polygons[i].v2, polygons[i].color);
    }
}