using System;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SharpVox.Graphics;
using SharpVox.Environment;
using SharpVox.Input;
using OpenTK.Windowing.Desktop;

namespace SharpVox.Core
{
    class Program
    {
        public static RenderWindow window;
        public static Clock deltaClock = new Clock();
        public static float deltaTime;

        /// <summary>
        /// The main loop of this program.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine("Program started with args:");
                for(int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine(args[i]);
                }
            }

            InitWindow(VideoMode.DesktopMode.Width, VideoMode.DesktopMode.Height);

            while (window.IsOpen)
            {
                //Handle window events
                window.DispatchEvents();

                //Game update loop
                if (World.activeScene != null)
                    World.activeScene.Update();
                else
                    World.CreateScene();

                //Reset input
                InputManager.Reset();

                //Draw the scene
                //if(window.HasFocus())
                Renderer.Render(window);

                deltaTime = deltaClock.Restart().AsSeconds();
            }
        }

        /// <summary>
        /// Initialize the main window of the program.
        /// </summary>
        static RenderWindow InitWindow(uint pixelsX, uint pixelsY)
        {
            if (window == null)
            {
                /*GameWindow Window = new GameWindow(
                    new GameWindowSettings() { IsMultiThreaded = true, RenderFrequency = 120, UpdateFrequency = 120 },
                    new NativeWindowSettings() { API = OpenTK.Windowing.Common.ContextAPI.OpenGL, AutoLoadBindings = true });*/

                window = new RenderWindow(new VideoMode(pixelsX, pixelsY), "SharpVox", Styles.Default, new ContextSettings(0, 0, 4, 1, 1, ContextSettings.Attribute.Default, false));
                window.KeyPressed += InputManager.KeyPressed;
                window.KeyReleased += InputManager.KeyReleased;
                window.MouseButtonPressed += InputManager.MouseButtonPressed;
                window.MouseButtonReleased += InputManager.MouseButtonReleased;
                window.MouseMoved += InputManager.MouseMoved;
                window.MouseWheelScrolled += InputManager.MouseScrolled;
                window.Resized += OnWindowResized;
                window.Closed += OnWindowClosed;
                window.SetVerticalSyncEnabled(true);
            }

            InitRenderer();

            return window;
        }

        /// <summary>
        /// Initialize the renderer.
        /// </summary>
        public static void InitRenderer()
        {
            if (Renderer.screenShape == null)
            {
                Renderer.screenShape = new RectangleShape(new Vector2f(window.Size.X, window.Size.Y));
            }

            if (Renderer.renderPasses == null)
            {
                Renderer.renderPasses = new RenderPass[0];

                //Main renderer
                RenderPass rendererPass = new RenderPass(new RenderStates(new Shader(null, null, "Graphics/Shaders/Renderer.frag")),
                    new RenderTexture(window.Size.X, window.Size.Y),
                    new UniformData[] { new UniformData("camPos", UniformType.Vec3),
                        new UniformData("camForward", UniformType.Vec3),
                        new UniformData("camRight", UniformType.Vec3),
                        new UniformData("camUp", UniformType.Vec3),
                        new UniformData("frame", UniformType.Int)}, null, false);

                rendererPass.renderStates.Shader.SetUniform("noiseTexture", new Texture("Graphics/Images/Noise/Noise.png") { Repeated = true, Smooth = false });
                rendererPass.renderStates.Shader.SetUniform("skyTexture", new Texture("Graphics/Images/Sky/immenstadter_horn_8k.hdr") { Repeated = true, Smooth = true });
                Renderer.AddPass(rendererPass);

                //Anti Aliasing
                RenderPass aliasingPass = new RenderPass(new RenderStates(new Shader(null, null, "Graphics/Shaders/AntiAliasing.frag")),
                    new RenderTexture(window.Size.X, window.Size.Y), new UniformData[] { new UniformData("frame", UniformType.Int) }, new int[] { 0, 1 }, false);
                Renderer.AddPass(aliasingPass);

                //Noise
                RenderPass noisePass = new RenderPass(new RenderStates(new Shader(null, null, "Graphics/Shaders/Noise.frag")),
                    new RenderTexture(window.Size.X, window.Size.Y),
                    new UniformData[] { new UniformData("camForward", UniformType.Vec3),
                    new UniformData("camPos", UniformType.Vec3)}, new int[] { 1 }, true);
                noisePass.renderStates.Shader.SetUniform("noiseTexture", new Texture("Graphics/Images/Noise/Noise.png") { Repeated = true, Smooth = true });
                Renderer.AddPass(noisePass);

                //Shapes
                //apeData shapeData = new ShapeData(new Sphere[10000]);
                //nderer.renderPasses[0].renderStates.Shader.
                
            }

            Renderer.ResizeRenderPass(ref Renderer.renderPasses[0], window.Size.X, window.Size.Y);
            Renderer.ResizeRenderPass(ref Renderer.renderPasses[1], window.Size.X, window.Size.Y);
            Renderer.ResizeRenderPass(ref Renderer.renderPasses[2], window.Size.X, window.Size.Y);
            Renderer.RegisterUniform("frame", 0);
        }

        /// <summary>
        /// Called when the window resizes.
        /// </summary>
        static void OnWindowResized(object sender, EventArgs e)
        {
            InitWindow(window.Size.X, window.Size.Y);
        }

        /// <summary>
        /// Called when the window closes.
        /// </summary>
        static void OnWindowClosed(object sender, EventArgs e)
        {
            window.Close();
        }
    }
}
