using System;
using BLD.Models;

namespace BLDPlugins.DebugPlugins.Commons
{
    public interface ISceneListener : IDisposable
    {
        void OnEntityAdded(IBLDEntity entity);
        void OnEntityRemoved(IBLDEntity entity);
    }
}