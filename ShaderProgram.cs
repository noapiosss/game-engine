using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace RenderingGL
{
    public readonly struct ShaderUniform
    {
        public readonly string Name;
        public readonly int Location;
        public readonly ActiveUniformType Type;

        public ShaderUniform(string name, int location, ActiveUniformType type)
        {
            Name = name;
            Location = location;
            Type = type;
        }
    }

    public readonly struct ShaderAttribute
    {
        public readonly string Name;
        public readonly int Location;
        public readonly ActiveAttribType Type;

        public ShaderAttribute(string name, int location, ActiveAttribType type)
        {
            Name = name;
            Location = location;
            Type = type;
        }
    }

    public sealed class ShaderProgram : IDisposable
    {
        private bool disposed;
        public readonly int ShaderProgramHandler;
        public readonly int VertexShaderHandler;
        public readonly int GeometryShaderHandler;
        public readonly int FragmentShaderHandler;

        private readonly ShaderUniform[] uniforms;
        private readonly ShaderAttribute[] attributes;

        public ShaderProgram(string vertexShaderCode, string fragmentShaderCode, string geometryShaderCode)
        {
            disposed = false;

            if (!ComplieShader(vertexShaderCode, ShaderType.VertexShader, out int VertexShaderHandler, out string errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            if (!ComplieShader(geometryShaderCode, ShaderType.GeometryShader, out int GeometryShaderHandler, out errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            if (!ComplieShader(fragmentShaderCode, ShaderType.FragmentShader, out int FragmentShaderHandler, out errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            ShaderProgramHandler = CreateLinkProgram(VertexShaderHandler, FragmentShaderHandler, GeometryShaderHandler);
            uniforms = CreateUniformList(ShaderProgramHandler);
            attributes = CreateAttributeList(ShaderProgramHandler);
        }

        ~ShaderProgram()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }   

            GL.DeleteShader(VertexShaderHandler);
            GL.DeleteShader(FragmentShaderHandler);
            GL.DeleteShader(GeometryShaderHandler);  

            GL.UseProgram(0);
            GL.DeleteProgram(ShaderProgramHandler);

            disposed = true;
            GC.SuppressFinalize(this);
        }

        public ShaderUniform[] GetUniformList()
        {
            ShaderUniform[] result = new ShaderUniform[uniforms.Length];
            Array.Copy(uniforms, result, uniforms.Length);
            return result;
        }

        public ShaderAttribute[] GetAttributeList()
        {
            ShaderAttribute[] result = new ShaderAttribute[attributes.Length];
            Array.Copy(attributes, result, attributes.Length);
            return result;
        }

        public void SetUniform(string name, float v1)
        {
            if (!GetShaderUniform(name, out ShaderUniform uniform))
            {
                throw new ArgumentException($"Name {name} was not found.");
            }

            if (uniform.Type != ActiveUniformType.Float)
            {
                throw new ArgumentException("Uniform type is not Float.");
            }            

            GL.UseProgram(ShaderProgramHandler);
            GL.Uniform1(uniform.Location, v1);
            GL.UseProgram(0);
        }

        public void SetUniform(string name, float v1, float v2)
        {
            if (!GetShaderUniform(name, out ShaderUniform uniform))
            {
                throw new ArgumentException($"Name {name} was not found.");
            }

            if (uniform.Type != ActiveUniformType.FloatVec2)
            {
                throw new ArgumentException("Uniform type is not FloatVec2.");
            }            

            GL.UseProgram(ShaderProgramHandler);
            GL.Uniform2(uniform.Location, v1, v2);
            GL.UseProgram(0);
        }

        public void SetUniform(string name, float v1, float v2, float v3)
        {
            if (!GetShaderUniform(name, out ShaderUniform uniform))
            {
                throw new ArgumentException($"Name {name} was not found.");
            }

            if (uniform.Type != ActiveUniformType.FloatVec3)
            {
                throw new ArgumentException("Uniform type is not FloatVec3.");
            }            

            GL.UseProgram(ShaderProgramHandler);
            GL.Uniform3(uniform.Location, v1, v2, v3);
            GL.UseProgram(0);
        }

        public void SetUniform(string name, float v1, float v2, float v3, float v4)
        {
            if (!GetShaderUniform(name, out ShaderUniform uniform))
            {
                throw new ArgumentException($"Name {name} was not found.");
            }

            if (uniform.Type != ActiveUniformType.FloatVec4)
            {
                throw new ArgumentException("Uniform type is not FloatVec4.");
            }            

            GL.UseProgram(ShaderProgramHandler);
            GL.Uniform4(uniform.Location, v1, v2, v3, v4);
            GL.UseProgram(0);
        }

        public void SetUniform(string name, Matrix4 matrix)
        {
            if (!GetShaderUniform(name, out ShaderUniform uniform))
            {
                throw new ArgumentException($"Name {name} was not found.");
            }

            if (uniform.Type != ActiveUniformType.FloatMat4)
            {
                throw new ArgumentException("Uniform type is not FloatMat4.");
            }            

            GL.UseProgram(ShaderProgramHandler);
            GL.UniformMatrix4(uniform.Location, false, ref matrix);
            GL.UseProgram(0);
        }

        private bool GetShaderUniform(string name, out ShaderUniform uniform)
        {
            for (int i = 0; i < uniforms.Length; ++i)
            {
                if (uniforms[i].Name == name)
                {
                    uniform = uniforms[i];
                    return true;
                }
            }

            uniform = new();
            return false;
        }

        public static bool ComplieShader(string shaderCode, ShaderType shaderType, out int shaderHandler, out string errorMessage)
        {
            shaderHandler = GL.CreateShader(shaderType);            
            GL.ShaderSource(shaderHandler, shaderCode);
            GL.CompileShader(shaderHandler);

            errorMessage = GL.GetShaderInfoLog(shaderHandler);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.WriteLine(errorMessage);
                return false;
            }

            return true;
        }

        public static int CreateLinkProgram(int vertexShaderHandler, int fragmentShaderHandler, int geometryShaderHandler)
        {
            int shaderProgramHandler = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandler, vertexShaderHandler);
            GL.AttachShader(shaderProgramHandler, geometryShaderHandler);
            GL.AttachShader(shaderProgramHandler, fragmentShaderHandler);

            GL.LinkProgram(shaderProgramHandler);

            GL.DetachShader(shaderProgramHandler, vertexShaderHandler);
            GL.DetachShader(shaderProgramHandler, geometryShaderHandler);
            GL.DetachShader(shaderProgramHandler, fragmentShaderHandler);

            return shaderProgramHandler;
        }

        public static ShaderUniform[] CreateUniformList(int shaderProgramHandler)
        {
            GL.GetProgram(shaderProgramHandler, GetProgramParameterName.ActiveUniforms, out int uniformCount);
            
            ShaderUniform[] uniforms = new ShaderUniform[uniformCount];

            for (int i = 0; i < uniformCount; ++i)
            {
                GL.GetActiveUniform(shaderProgramHandler, i, 256, out _, out _, out ActiveUniformType type, out string name);
                int location = GL.GetUniformLocation(shaderProgramHandler, name);

                uniforms[i] = new ShaderUniform(name, location, type);
            }

            return uniforms;
        }

        public static ShaderAttribute[] CreateAttributeList(int shaderProgramHandler)
        {
            GL.GetProgram(shaderProgramHandler, GetProgramParameterName.ActiveAttributes, out int attributesCount);
            
            ShaderAttribute[] attributes = new ShaderAttribute[attributesCount];

            for (int i = 0; i < attributesCount; ++i)
            {
                GL.GetActiveAttrib(shaderProgramHandler, i, 256, out _, out _, out ActiveAttribType type, out string name);
                int location = GL.GetAttribLocation(shaderProgramHandler, name);

                attributes[i] = new ShaderAttribute(name, location, type);
            }

            return attributes;
        }
    }
}