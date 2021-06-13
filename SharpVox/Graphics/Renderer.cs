using System;
using System.Collections.Generic;
using System.Text;
using SFML.Window;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using OpenTK.Graphics.OpenGL;

namespace SharpVox.Graphics
{
    class Renderer
    {
        public static Dictionary<string, object> uniformRegister = new Dictionary<string, object>();
        public static RectangleShape screenShape;
        public static RenderPass[] renderPasses;
        public static int frame;

        /// <summary>
        /// The render loop, handles displaying of all graphics in the program.
        /// </summary>
        public static void Render(RenderWindow window)
        {
            window.Clear();
            frame++;

            for (int i = 0; i < renderPasses.Length; i++)
            {
                //Update all uniforms
                if (renderPasses[i].uniforms != null)
                {
                    for (int u = 0; u < renderPasses[i].uniforms.Length; u++)
                    {
                        switch (renderPasses[i].uniforms[u].Type)
                        {
                            case UniformType.Texture:
                                renderPasses[i].renderStates.Shader.SetUniform(renderPasses[i].uniforms[u].Name, (Texture)uniformRegister[renderPasses[i].uniforms[u].Name]);
                                break;

                            case UniformType.Float:
                                renderPasses[i].renderStates.Shader.SetUniform(renderPasses[i].uniforms[u].Name, (float)uniformRegister[renderPasses[i].uniforms[u].Name]);
                                break;

                            case UniformType.Vec3:
                                renderPasses[i].renderStates.Shader.SetUniform(renderPasses[i].uniforms[u].Name, (Vec3)uniformRegister[renderPasses[i].uniforms[u].Name]);
                                break;

                            case UniformType.Vec2:
                                renderPasses[i].renderStates.Shader.SetUniform(renderPasses[i].uniforms[u].Name, (Vec2)uniformRegister[renderPasses[i].uniforms[u].Name]);
                                break;

                            case UniformType.Int:
                                renderPasses[i].renderStates.Shader.SetUniform(renderPasses[i].uniforms[u].Name, (int)uniformRegister[renderPasses[i].uniforms[u].Name]);
                                break;
                        }
                    }
                }

                //Update render texture if applicable
                if (renderPasses[i].renderTexture != null)
                {
                    renderPasses[i].renderTexture.Draw(screenShape, renderPasses[i].renderStates);
                    renderPasses[i].renderTexture.Display();
                }

                //Draw the render pass
                if (renderPasses[i].display)
                    window.Draw(screenShape, renderPasses[i].renderStates);

                //Check if render pass requires another pass' texture
                if (renderPasses[i].renderTextureCopy != null)
                {
                    for (int c = 0; c < renderPasses[i].renderTextureCopy.Length; c++)
                    {
                        renderPasses[i].renderStates.Shader.SetUniform("renderTexture" + c, renderPasses[renderPasses[i].renderTextureCopy[c]].renderTexture.Texture);
                    }
                }
            }

            SetUniform("frame", frame);
            window.Display();
        }

        /// <summary>
        /// Add a render pass to the renderer.
        /// </summary>
        public static void AddPass(RenderPass pass)
        {
            RenderPass[] newPasses = new RenderPass[renderPasses.Length + 1];

            for (int i = 0; i < renderPasses.Length; i++)
            {
                newPasses[i] = renderPasses[i];
            }

            newPasses[^1] = pass;

            renderPasses = newPasses;
        }

        public static void RegisterUniform(string key, object o)
        {
            if (uniformRegister.ContainsKey(key))
                uniformRegister[key] = o;
            else
                uniformRegister.Add(key, o);
        }

        public static void SetUniform(string key, object o)
        {
            uniformRegister[key] = o;
        }

        public static void ResizeRenderStates(ref RenderStates states, uint width, uint height)
        {
            if(states.Texture != null)
                states.Texture.Dispose();

            states.Texture = new Texture(width, height);
            states.Shader.SetUniform("resolution", new Vec2(width, height));
        }

        public static void ResizeRenderPass(ref RenderPass renderPass, uint width, uint height)
        {
            if(renderPass.renderTexture != null)
            {
                renderPass.renderTexture.Dispose();
                renderPass.renderTexture = new RenderTexture(width, height);
            }

            ResizeRenderStates(ref renderPass.renderStates, width, height);
        }

        public static void DisposeRenderPasses()
        {
            for(int i = 0; i < renderPasses.Length; i++)
            {
                if (renderPasses[i].renderStates.Shader != null)
                    renderPasses[i].renderStates.Shader.Dispose();

                if (renderPasses[i].renderStates.Texture != null)
                    renderPasses[i].renderStates.Shader.Dispose();

                if (renderPasses[i].renderTexture != null)
                    renderPasses[i].renderTexture.Dispose();
            }

            renderPasses = null;
        }

        public static void SetUniformArray(object uniform, int length)
        {

        }
    }

    public class RenderPass
    {
        public RenderStates renderStates;
        public RenderTexture renderTexture;
        public int[] renderTextureCopy;
        public bool display;

        public UniformData[] uniforms;

        public RenderPass(RenderStates states, RenderTexture renderTex, UniformData[] uniformData = null, int[] renderTextureIndexes = null, bool displayOnScreen = false)
        {
            renderStates = states;
            renderTextureCopy = renderTextureIndexes;
            display = displayOnScreen;
            uniforms = uniformData;
            renderTexture = renderTex;
        }

        public RenderPass(RenderStates states, UniformData[] uniformData = null, int[] renderTextureIndexes = null, bool displayOnScreen = false)
        {
            renderStates = states;
            renderTextureCopy = renderTextureIndexes;
            display = displayOnScreen;
            uniforms = uniformData;
            renderTexture = null;
        }

        public RenderPass(RenderStates states, UniformData[] uniformData = null, bool displayOnScreen = false)
        {
            renderTextureCopy = null;
            renderStates = states;
            display = displayOnScreen;
            uniforms = uniformData;
            renderTexture = null;
        }

        public RenderPass(RenderStates states, bool displayOnScreen = false)
        {
            renderTextureCopy = null;
            renderStates = states;
            display = displayOnScreen;
            uniforms = null;
            renderTexture = null;
        }

        ~RenderPass()
        {
            renderTexture.Texture.Dispose();
            renderTexture.Dispose();
        }
    }

    /*public struct ShapeData
    {
        readonly Sphere[] spheres;

        public ShapeData(Sphere[] shapes)
        {
            spheres = shapes;
        }
    }

    public struct Sphere
    {
        Vec3 pos;
        Vec4 color;
        float radius;
        bool used;
    };*/

    public struct UniformData
    {
        public UniformType Type;
        public string Name;

        public UniformData(string name, UniformType type)
        {
            Name = name;
            Type = type;
        }
    }

    public enum UniformType
    {
        Texture,
        Float,
        Vec3,
        Vec2,
        Int
    }
}
