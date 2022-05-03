using System;
using BLD.Controllers;
using BLD.Models;

namespace BLDPlugins.DebugPlugins.Commons
{
    public class WatchSceneHandler : IDisposable
    {
        private readonly IParcelScene scene;
        private readonly ISceneListener sceneListener;

        public WatchSceneHandler(IParcelScene scene, ISceneListener sceneListener)
        {
            this.scene = scene;
            this.sceneListener = sceneListener;

            scene.OnEntityAdded += SceneOnOnEntityAdded;
            scene.OnEntityRemoved += SceneOnOnEntityRemoved;

            if (scene.entities?.Values != null)
            {
                foreach (IBLDEntity entity in scene.entities.Values)
                {
                    sceneListener.OnEntityAdded(entity);
                }
            }
        }

        public void Dispose()
        {
            scene.OnEntityAdded -= SceneOnOnEntityAdded;
            scene.OnEntityRemoved -= SceneOnOnEntityRemoved;

            sceneListener.Dispose();
        }

        private void SceneOnOnEntityAdded(IBLDEntity entity)
        {
            sceneListener.OnEntityAdded(entity);
        }

        private void SceneOnOnEntityRemoved(IBLDEntity entity)
        {
            sceneListener.OnEntityRemoved(entity);
        }
    }
}