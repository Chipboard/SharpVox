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
                Texture screenTexture = new Texture(Program.window.Size.X, Program.window.Size.Y);
                screenTexture.Update(Program.window);
                Image screenshot = screenTexture.CopyToImage();

                if (!Directory.Exists("Screenshots/"))
                    Directory.CreateDirectory("Screenshots/");

                int index = 0;
                while(File.Exists("Screenshots/Screenshot_" + index + ".jpg"))
                {
                    index++;
                }

                screenshot.SaveToFile("Screenshots/Screenshot_" + index + ".jpg");

                screenTexture.Dispose();
                screenshot.Dispose();
            }
        }
    }
}
