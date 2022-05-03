using System;
using System.Collections.Generic;
using BLD.Controllers;
using BLD.Models;

namespace BLD.Components
{
    public interface ISharedComponent : IComponent, IDisposable
    {
        string id { get; }
        void AttachTo(IBLDEntity entity, Type overridenAttachedType = null);
        void DetachFrom(IBLDEntity entity, Type overridenAttachedType = null);
        void DetachFromEveryEntity();
        void Initialize(IParcelScene scene, string id);
        HashSet<IBLDEntity> GetAttachedEntities();
        void CallWhenReady(Action<ISharedComponent> callback);
    }
}