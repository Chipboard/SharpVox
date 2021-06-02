using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SharpVox.Environment
{
    public class SceneData
    {
        public SceneObject[] sceneObjects;
        public uint identificationCount;


        /// <summary>
        /// Calls the update function on all objects in the scene.
        /// </summary>
        public void Update()
        {
            if (sceneObjects == null)
                return;

            for (int i = 0; i < sceneObjects.Length; i++)
            {
                sceneObjects[i].Update();
            }
        }


        /// <summary>
        /// Remove null references from the scene. You shouldn't need to call this.
        /// </summary>
        public void Rebuild()
        {
            //Get the amount of still active sceneObjects
            int sceneCount = sceneObjects.Length;
            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if (sceneObjects[i] == null)
                    sceneCount--;
            }

            //Allocate to new array
            int currentIndex = 0;
            SceneObject[] newSceneObjects = new SceneObject[sceneCount];
            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if (sceneObjects[i] != null)
                {
                    newSceneObjects[currentIndex++] = sceneObjects[i];
                }
            }
        }

        /// <summary>
        /// Instert a sceneObject into the scene.
        /// </summary>
        public uint Insert(SceneObject sceneObject)
        {
            int newLength = sceneObjects.Length + 1;
            SceneObject[] newSceneObjects = new SceneObject[newLength];

            for (int i = 0; i < sceneObjects.Length; i++)
            {
                newSceneObjects[i] = sceneObjects[i];
            }

            newSceneObjects[newLength] = sceneObject;

            sceneObjects = newSceneObjects;

            return identificationCount++;
        }

        /// <summary>
        /// Remove a sceneObject from the scene, based on its ID.
        /// </summary>
        public bool Remove(int id)
        {
            int removalPosition = -1;
            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if (sceneObjects[i].objectID == id)
                    removalPosition = i;
            }

            if (removalPosition != -1)
            {
                sceneObjects[removalPosition] = null;
                Rebuild();
                return true;
            }
            else
            {
                return false;
            }
        }

        public SceneData()
        {
            if (World.activeScene == null)
            {
                SceneData self = this;
                World.ActivateScene(ref self);
            }
        }

        ~SceneData()
        {
            if (this == World.activeScene && World.memoryScenes.Count > 0)
            {
                World.activeScene = World.memoryScenes[0];
                World.memoryScenes.RemoveAt(0);
            }
        }
    }
}
