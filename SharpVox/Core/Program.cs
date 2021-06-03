using System;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using SharpVox.Graphics;
using SharpVox.Environment;
using SharpVox.Input;

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

            InputManager.SetCursorVisible(false);

            while (window.IsOpen)
            {
                //Handle window events
                window.DispatchEvents();

                //Game update loop
                if (World.activeScene != null)
                    World.activeScene.Update();
                else
                    World.CreateScene();

                //Draw the scene
                if(window.HasFocus())
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
                window = new RenderWindow(new VideoMode(pixelsX, pixelsY), "SharpVox");
                window.KeyPressed += InputManager.KeyPressed;
                window.KeyReleased += InputManager.KeyReleased;
                window.Resized += OnWindowResized;
                window.Closed += OnWindowClosed;
            }

            InitRenderer();

            return window;
        }

        /// <summary>
        /// Initialize the renderer.
        /// </summary>
        static void InitRenderer()
        {
            if (Renderer.screenShape == null)
            {
                Renderer.screenShape = new RectangleShape(new Vector2f(window.Size.X, window.Size.Y))
                {
                    Texture = new Texture(new Image("Images/BrDevBackdrop.png"))
                };
            }

            if(Renderer.screenStates.Shader == null)
            Renderer.screenStates = new RenderStates(new Shader(null, null, "Shaders/Voxel.frag"));

            Renderer.screenStates.Shader.SetUniform("resolution", new Vec2(window.Size.X, window.Size.Y));
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
