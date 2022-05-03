using System;
using BLD.Models;

namespace BLDPlugins.DebugPlugins.Commons
{
    public interface IShapeListener : IDisposable
    {
        void OnShapeUpdated(IBLDEntity entity);
        void OnShapeCleaned(IBLDEntity entity);
    }
}