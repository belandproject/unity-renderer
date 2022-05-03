using BLD.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using BLD.Controllers;
using UnityEngine;

namespace BLD.Components
{
    public abstract class BaseDisposable : IDelayedComponent, ISharedComponent
    {
        public virtual string componentName => GetType().Name;
        public string id { get; private set; }
        public IParcelScene scene { get; private set; }

        public abstract int GetClassId();

        public virtual void Initialize(IParcelScene scene, string id)
        {
            this.scene = scene;
            this.id = id;
        }

        ComponentUpdateHandler updateHandler;
        public CustomYieldInstruction yieldInstruction => updateHandler.yieldInstruction;
        public Coroutine routine => updateHandler.routine;
        public bool isRoutineRunning => updateHandler.isRoutineRunning;

        public event System.Action<IBLDEntity> OnAttach;
        public event System.Action<IBLDEntity> OnDetach;
        public event System.Action<BaseDisposable> OnDispose;
        public event Action<BaseDisposable> OnAppliedChanges;

        public HashSet<IBLDEntity> attachedEntities = new HashSet<IBLDEntity>();

        protected BaseModel model;

        public HashSet<IBLDEntity> GetAttachedEntities() { return attachedEntities; }

        public virtual void UpdateFromJSON(string json) { UpdateFromModel(model.GetDataFromJSON(json)); }

        public virtual void UpdateFromModel(BaseModel newModel)
        {
            model = newModel;
            updateHandler.ApplyChangesIfModified(model);
        }

        public BaseDisposable() { updateHandler = CreateUpdateHandler(); }

        public virtual void RaiseOnAppliedChanges() { OnAppliedChanges?.Invoke(this); }

        public virtual void AttachTo(IBLDEntity entity, System.Type overridenAttachedType = null)
        {
            if (attachedEntities.Contains(entity))
            {
                return;
            }

            System.Type thisType = overridenAttachedType != null ? overridenAttachedType : GetType();
            entity.AddSharedComponent(thisType, this);

            attachedEntities.Add(entity);

            entity.OnRemoved += OnEntityRemoved;

            OnAttach?.Invoke(entity);
        }

        private void OnEntityRemoved(IBLDEntity entity) { DetachFrom(entity); }

        public virtual void DetachFrom(IBLDEntity entity, System.Type overridenAttachedType = null)
        {
            if (!attachedEntities.Contains(entity))
                return;

            entity.OnRemoved -= OnEntityRemoved;

            System.Type thisType = overridenAttachedType != null ? overridenAttachedType : GetType();
            entity.RemoveSharedComponent(thisType, false);

            attachedEntities.Remove(entity);

            OnDetach?.Invoke(entity);
        }

        public void DetachFromEveryEntity()
        {
            IBLDEntity[] attachedEntitiesArray = new IBLDEntity[attachedEntities.Count];

            attachedEntities.CopyTo(attachedEntitiesArray);

            for (int i = 0; i < attachedEntitiesArray.Length; i++)
            {
                DetachFrom(attachedEntitiesArray[i]);
            }
        }

        public virtual void Dispose()
        {
            OnDispose?.Invoke(this);
            DetachFromEveryEntity();
        }

        public virtual BaseModel GetModel() => model;

        public abstract IEnumerator ApplyChanges(BaseModel model);

        public virtual ComponentUpdateHandler CreateUpdateHandler() { return new ComponentUpdateHandler(this); }

        public bool IsValid() { return true; }

        public void Cleanup()
        {
            if (isRoutineRunning)
            {
                CoroutineStarter.Stop(routine);
            }
        }

        public virtual void CallWhenReady(Action<ISharedComponent> callback)
        {
            //By default there's no initialization process and we call back as soon as we get the suscription
            callback.Invoke(this);
        }
    }
}