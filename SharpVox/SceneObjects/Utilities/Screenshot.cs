using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpVox.Environment;
using SharpVox.Graphics;
using SharpVox.Input;
using SharpVox.Core;
using SFML.Graphics;
using SFML.Window;

namespace SharpVox.Utilities
{
    class Screenshot : SceneObject
    {
        public override void Start()
        {
            
        }

        public override void Update()
        {
            if(InputManager.GetKey(Keyboard.Key.LControl) && InputManager.GetKeyDown(Keyboard.Key.P))
            {
                if (InputManager.GetKey(Keyboard.Key.P))
                {
                    //High detail screenshot or regular screenshot?
                    if (InputManager.GetKey(Keyboard.Key.LShift))
                    {
                        //High detail
                        Renderer.renderPasses[0].renderStates.Shader.SetUniform("epsilon", 0.0001f);
                        Renderer.renderPasses[0].renderStates.Shader.SetUniform("maxBounces", 1000);
                        Renderer.renderPasses[0].renderStates.Shader.SetUniform("maxIterations", 2000);
                        Renderer.renderPasses[1].renderStates.Shader.SetUniform("blendFactor", 0.9f);

                        for(int i = 0; i < 100; i++)
                        {
                            Renderer.Render(Program.window);
                        }

                        DoScreenshot();

                        Renderer.renderPasses = null;
                        Program.InitRenderer();
                    } else
                    {
                        //Regular
                        DoScreenshot();
                    }
                }
            }
        }

        public void DoScreenshot()
        {
            Texture screenTexture = new Texture(Program.window.Size.X, Program.window.Size.Y);
            screenTexture.Update(Program.window);
            Image screenshot = screenTexture.CopyToImage();

            if (!Directory.Exists("Screenshots/"))
                Directory.CreateDirectory("Screenshots/");

            int index = 0;
            while (File.Exists("Screenshots/Screenshot_" + index + ".jpg"))
            {
                index++;
            }

            screenshot.SaveToFile("Screenshots/Screenshot_" + index + ".jpg");
            Console.WriteLine("Wrote screenshot: Screenshot_" + index + ".jpg");

            screenTexture.Dispose();
            screenshot.Dispose();
        }
    }
}
