using System;
using System.Collections.Generic;
using System.Text;
using SFML.System;
using SFML.Window;
using SharpVox.Core;

namespace SharpVox.Input
{
    class InputManager
    {
        public static Dictionary<Keyboard.Key, bool> keys = new Dictionary<Keyboard.Key, bool>();
        public static Dictionary<Mouse.Button, bool> mouseButtons = new Dictionary<Mouse.Button, bool>();
        public static int mousePositionX, mousePositionY, mouseMovementX, mouseMovementY;
        public static int mouseMovementCorrectionX, mouseMovementCorrectionY;

        /// <summary>
        /// Get mouse movement.
        /// </summary>
        public static void MouseMoved(object sender, MouseMoveEventArgs e)
        {
            mouseMovementX = e.X - mousePositionX + mouseMovementCorrectionX;
            mouseMovementY = e.Y - mousePositionY + mouseMovementCorrectionY;

            mousePositionX = e.X;
            mousePositionY = e.Y;
        }

        /// <summary>
        /// Add key to list of pressed keys.
        /// </summary>
        public static void KeyPressed(object sender, KeyEventArgs e)
        {
            if (!keys.ContainsKey(e.Code))
                keys.Add(e.Code, true);
            else
                keys[e.Code] = true;
        }

        /// <summary>
        /// Remove key from list of pressed keys.
        /// </summary>
        public static void KeyReleased(object sender, KeyEventArgs e)
        {
            keys[e.Code] = false;
        }

        /// <summary>
        /// Add mouse button press to list of mouse button presses.
        /// </summary>
        public static void MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (!mouseButtons.ContainsKey(e.Button))
                mouseButtons.Add(e.Button, true);
            else
                mouseButtons[e.Button] = true;
        }

        /// <summary>
        /// Remove mouse button press from list of mouse button presses.
        /// </summary>
        public static void MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            mouseButtons[e.Button] = false;
        }

        /// <summary>
        /// Check if key was pressed this frame.
        /// </summary>
        public static bool GetKey(Keyboard.Key key)
        {
            if (!keys.ContainsKey(key))
                return false;

            return keys[key];
        }

        /// <summary>
        /// Check if mouse button was pressed this frame.
        /// </summary>
        public static bool GetMouseButton(Mouse.Button button)
        {
            if (!mouseButtons.ContainsKey(button))
                return false;

            return mouseButtons[button];
        }

        /// <summary>
        /// Get the mouse movement this frame.
        /// </summary>
        public static Vector2i GetMouseMovement()
        {
            return new Vector2i(mouseMovementX, mouseMovementY);
        }

        /// <summary>
        /// Set the visibility of the cursor.
        /// </summary>
        public static void SetCursorVisible(bool toggle)
        {
            Program.window.SetMouseCursorVisible(toggle);
        }

        /// <summary>
        /// Set the position of the cursor.
        /// </summary>
        public static void SetCursorPosition(int x, int y)
        {
            Mouse.SetPosition(new Vector2i(x, y), Program.window);
            mouseMovementCorrectionX = mousePositionX - x;
            mouseMovementCorrectionY = mousePositionY - y;
        }

        /// <summary>
        /// Set the position of the cursor to the center of the screen.
        /// </summary>
        public static void CenterCursor()
        {
            Mouse.SetPosition(new Vector2i((int)Program.window.Size.X/2, (int)Program.window.Size.Y/2), Program.window);
            mouseMovementCorrectionX = (int)(mousePositionX - (Program.window.Size.X / 2));
            mouseMovementCorrectionY = (int)(mousePositionY - (Program.window.Size.Y / 2));
        }
    }
}
