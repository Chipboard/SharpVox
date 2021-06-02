using System;
using System.Collections.Generic;
using System.Text;

namespace SharpVox.Environment
{
    public abstract class SceneObject
    {
        private readonly SceneData parentScene;
        public int objectID;

        /// <summary>
        /// Update is called before every frame.
        /// </summary>
        public abstract void Update();

        public SceneObject(SceneData scene = null)
        {
            if (scene != null)
            {
                scene.Insert(this);
                parentScene = scene;
            }
            else if (World.activeScene != null)
            {
                World.activeScene.Insert(this);
                parentScene = World.activeScene;
            }
            else
                Console.WriteLine("Could not find scene for SceneObject!");
        }

        ~SceneObject()
        {
            parentScene.Remove(objectID);
        }
    }
}
