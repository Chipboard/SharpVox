using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;

namespace SharpVox.Graphics
{
    class Renderer
    {
        public static RectangleShape screenShape;
        public static RenderStates screenStates;

        public static void Render(RenderWindow window)
        {
            window.Clear();

            window.Draw(screenShape, screenStates);

            window.Display();
        }
    }
}
