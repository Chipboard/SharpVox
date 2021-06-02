using System;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using SharpVox.Graphics;
using SharpVox.Environment;

namespace SharpVox.Core
{
    class Program
    {
        public static RenderWindow window;

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

                //Draw the scene
                if(window.HasFocus())
                Renderer.Render(window);
            }
        }

        static RenderWindow InitWindow(uint pixelsX, uint pixelsY)
        {
            if (window != null && window.IsOpen)
                window.Close();

            window = new RenderWindow(new VideoMode(pixelsX, pixelsY), "SharpVox", Styles.None);
            window.KeyPressed += InputManager.KeyPressed;
            window.Resized += OnWindowResized;
            window.Closed += OnWindowClosed;

            InitRenderer();

            return window;
        }

        static void InitRenderer()
        {
            Renderer.screenShape = new RectangleShape(new Vector2f(window.Size.X, window.Size.Y))
            {
                Texture = new Texture(new Image("Images/BrDevBackdrop.png"))
            };

            Renderer.screenStates = new RenderStates(new Shader(null, null, "Shaders/Voxel.frag"));
            Renderer.screenStates.Shader.SetUniform("resolution", new Vec2(window.Size.X, window.Size.Y));
        }

        static void OnWindowResized(object sender, EventArgs e)
        {
            InitWindow(window.Size.X, window.Size.Y);
        }

        static void OnWindowClosed(object sender, EventArgs e)
        {
            window.Close();
        }
    }
}
