using System;
using System.Collections.Generic;
using System.Text;

namespace SharpVox.Environment
{
    public abstract class SceneObject
    {
        private readonly SceneData parentScene;
        public readonly uint objectID;

        /// <summary>
        /// Start is called when the object is first created.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Update is called before every frame.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Create a new sceneObject.
        /// </summary>
        public SceneObject(SceneData scene = null)
        {
            if (scene != null)
            {
                objectID = scene.Insert(this);
                parentScene = scene;
            }
            else if (World.activeScene != null)
            {
                objectID = World.activeScene.Insert(this);
                parentScene = World.activeScene;
            }
            else
            {
                Console.WriteLine("Could not find scene for SceneObject!");
                return;
            }

            Start();
        }

        ~SceneObject()
        {
            parentScene.Remove(objectID);
        }
    }
}
