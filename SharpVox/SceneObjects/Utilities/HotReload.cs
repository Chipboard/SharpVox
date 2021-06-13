using System;
using System.Collections.Generic;
using System.Text;
using SFML.Window;
using SharpVox.Environment;
using SharpVox.Input;

namespace SharpVox.Utilities
{
    class HotReload : SceneObject
    {
        public override void Start()
        {
            
        }

        public override void Update()
        {
            if(InputManager.GetKey(Keyboard.Key.LControl) && InputManager.GetKeyDown(Keyboard.Key.R))
            {
                    Graphics.Renderer.renderPasses = null;
                    Core.Program.InitRenderer();
            }
        }
    }
}
