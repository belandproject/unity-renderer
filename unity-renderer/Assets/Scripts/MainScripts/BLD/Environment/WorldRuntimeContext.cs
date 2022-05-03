using BLD.Controllers;
using UnityEngine;

namespace BLD
{
    /// <summary>
    /// Context related to all the systems involved in the execution of Beland scenes.
    /// </summary>
    [System.Obsolete("This is kept for retrocompatibilty and will be removed in the future. Use Environment.i.serviceLocator instead.")]
    public class WorldRuntimeContext
    {
        private ServiceLocator serviceLocator;
        public IWorldState state => serviceLocator.Get<IWorldState>();
        public ISceneController sceneController => serviceLocator.Get<ISceneController>();
        public IPointerEventsController pointerEventsController => serviceLocator.Get<IPointerEventsController>();
        public ISceneBoundsChecker sceneBoundsChecker => serviceLocator.Get<ISceneBoundsChecker>();
        public IWorldBlockersController blockersController => serviceLocator.Get<IWorldBlockersController>();
        public IRuntimeComponentFactory componentFactory => serviceLocator.Get<IRuntimeComponentFactory>();

        public WorldRuntimeContext (ServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }
    }
}