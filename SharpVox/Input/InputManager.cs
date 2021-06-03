using System;
using System.Collections.Generic;
using System.Text;
using SFML.Window;
using SharpVox.Core;
using SharpVox.Environment;

namespace SharpVox.Input
{
    class InputManager
    {
        public static Dictionary<Keyboard.Key, bool> keys = new Dictionary<Keyboard.Key, bool>();

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
        /// Check if key was pressed this frame.
        /// </summary>
        public static bool GetKey(Keyboard.Key key)
        {
            if (!keys.ContainsKey(key))
                return false;

            return keys[key];
        }

        /// <summary>
        /// Set the visibility of the cursor.
        /// </summary>
        public static void SetCursorVisible(bool toggle)
        {
            Program.window.SetMouseCursorVisible(toggle);
        }
    }
}
