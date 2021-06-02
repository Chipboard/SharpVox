using System;
using System.Collections.Generic;
using System.Text;

namespace SharpVox.Environment
{
    public static class World
    {
        public static List<SceneData> memoryScenes = new List<SceneData>();
        public static SceneData activeScene;

        public static SceneData CreateScene()
        {
            Console.WriteLine("Creating new scene");
            SceneData newScene = new SceneData();

            if(activeScene == null)
                ActivateScene(ref newScene);
            
            return newScene;
        }

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

//JFS 800-324-8680
