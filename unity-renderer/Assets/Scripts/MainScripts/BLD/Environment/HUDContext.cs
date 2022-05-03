using BLD.Controllers;
using UnityEngine;

namespace BLD
{
    /// <summary>
    /// Context related to all the systems involved in the execution of beland scenes.
    /// </summary>
    [System.Obsolete("This is kept for retrocompatibilty and will be removed in the future. Use Environment.i.serviceLocator instead.")]
    public class HUDContext
    {
        private ServiceLocator serviceLocator;
        public IHUDFactory factory => serviceLocator.Get<IHUDFactory>();
        public IHUDController controller => serviceLocator.Get<IHUDController>();

        public HUDContext (ServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }
    }
}