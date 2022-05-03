using System;
using System.Collections;
using System.Collections.Generic;
using BLD.Controllers;
using BLD.Models;

namespace BLD
{
    public interface IParcelScenesCleaner : IService
    {
        void MarkForCleanup(IBLDEntity entity);
        void MarkRootEntityForCleanup(IParcelScene scene, IBLDEntity entity);
        void MarkDisposableComponentForCleanup(IParcelScene scene, string componentId);
        void CleanMarkedEntities();
        public IEnumerator CleanMarkedEntitiesAsync(bool immediate = false);
    }
}