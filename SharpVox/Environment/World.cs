using System;
using System.Collections.Generic;
using System.Text;

namespace SharpVox.Environment
{
    public static class World
    {
        public static List<SceneData> memoryScenes = new List<SceneData>();
        public static SceneData activeScene;

        /// <summary>
        /// Create a new scene.
        /// </summary>
        public static SceneData CreateScene()
        {
            Console.WriteLine("Creating new scene");
            SceneData newScene = new SceneData();

            if(activeScene == null)
                ActivateScene(ref newScene);

            new Graphics.Camera();
            new Utilities.HotReload();
            new Utilities.Screenshot();
            
            return newScene;
        }

        /// <summary>
        /// Activate an existing scene.
        /// </summary>
        public static void ActivateScene(ref SceneData scene)
        {
            Console.WriteLine("Activating scene");
            if(scene == activeScene)
            {
                Console.WriteLine("Scene was already active");
                return;
            }

            for (int i = 0; i < memoryScenes.Count; i++)
            {
                if (memoryScenes[i] == activeScene)
                {
                    memoryScenes.RemoveAt(i);
                    break;
                }
            }

            memoryScenes.Add(activeScene);

            activeScene = scene;
        }

        /// <summary>
        /// Remove a specified scene.
        /// </summary>
        public static void RemoveScene(SceneData scene)
        {
            Console.WriteLine("Removing scene");
            if (scene == activeScene)
            {
                activeScene = null;
            }

            for (int i = 0; i < memoryScenes.Count; i++)
            {
                if (memoryScenes[i] == scene)
                {
                    memoryScenes.RemoveAt(i);
                    break;
                }
            }

            if (memoryScenes.Count > 0)
            {
                SceneData activationScene = memoryScenes[0];
                ActivateScene(ref activationScene);
            }
        }
    }
}