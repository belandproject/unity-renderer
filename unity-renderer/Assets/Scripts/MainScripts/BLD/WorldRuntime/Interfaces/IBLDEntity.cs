using System;
using System.Collections.Generic;
using BLD.Components;
using BLD.Controllers;
using UnityEngine;

namespace BLD.Models
{
    public interface IBLDEntity : ICleanable, ICleanableEventDispatcher
    {
        GameObject gameObject { get; }
        string entityId { get; set; }
        MeshesInfo meshesInfo { get; set; }
        GameObject meshRootGameObject { get; }
        Renderer[] renderers { get; }
        void SetParent(IBLDEntity entity);
        void AddChild(IBLDEntity entity);
        void RemoveChild(IBLDEntity entity);
        void EnsureMeshGameObject(string gameObjectName = null);
        void ResetRelease();
        void AddSharedComponent(System.Type componentType, ISharedComponent component);
        void RemoveSharedComponent(System.Type targetType, bool triggerDetaching = true);

        /// <summary>
        /// This function is designed to get interfaces implemented by diverse components.
        ///
        /// If you want to get the component itself please use TryGetBaseComponent or TryGetSharedComponent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T TryGetComponent<T>() where T : class;

        bool TryGetBaseComponent(CLASS_ID_COMPONENT componentId, out IEntityComponent component);
        bool TryGetSharedComponent(CLASS_ID componentId, out ISharedComponent component);
        ISharedComponent GetSharedComponent(System.Type targetType);
        IParcelScene scene { get; set; }
        bool markedForCleanup { get; set; }
        bool isInsideBoundaries { get; set; }
        Dictionary<string, IBLDEntity> children { get; }
        IBLDEntity parent { get; }
        Dictionary<CLASS_ID_COMPONENT, IEntityComponent> components { get; }
        Dictionary<System.Type, ISharedComponent> sharedComponents { get; }
        Action<IBLDEntity> OnShapeUpdated { get; set; }
        Action<IBLDEntity> OnShapeLoaded { get; set; }
        Action<IBLDEntity> OnRemoved { get; set; }
        Action<IBLDEntity> OnMeshesInfoUpdated { get; set; }
        Action<IBLDEntity> OnMeshesInfoCleaned { get; set; }

        Action<object> OnNameChange { get; set; }
        Action<object> OnTransformChange { get; set; }
    }
}