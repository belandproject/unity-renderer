using System;
using System.Collections;
using BLD.Controllers;
using BLD.Helpers;
using BLD.Models;
using UnityEngine;

namespace BLD.Components
{
    public class AvatarAttachComponent : IEntityComponent
    {
        [Serializable]
        public class Model : BaseModel
        {
            public string avatarId = null;
            public int anchorPointId = 0;

            public override BaseModel GetDataFromJSON(string json)
            {
                return Utils.SafeFromJson<Model>(json);
            }
        }

        IParcelScene IComponent.scene => handler.scene;
        IBLDEntity IEntityComponent.entity => handler.entity;

        string IComponent.componentName => "AvatarAttach";

        private readonly AvatarAttachHandler handler = new AvatarAttachHandler();

        void IEntityComponent.Initialize(IParcelScene scene, IBLDEntity entity)
        {
            handler.Initialize(scene, entity, Environment.i.platform.updateEventHandler);
        }

        bool IComponent.IsValid() => true;

        BaseModel IComponent.GetModel() => handler.model;

        int IComponent.GetClassId() => (int)CLASS_ID_COMPONENT.AVATAR_ATTACH;

        void IComponent.UpdateFromJSON(string json)
        {
            handler.OnModelUpdated(json);
        }

        void IComponent.UpdateFromModel(BaseModel newModel)
        {
            handler.OnModelUpdated(newModel as Model);
        }

        IEnumerator IComponent.ApplyChanges(BaseModel newModel)
        {
            yield break;
        }

        void IComponent.RaiseOnAppliedChanges() { }

        Transform IMonoBehaviour.GetTransform() => handler.entity?.gameObject.transform;

        void ICleanable.Cleanup() => handler.Dispose();
    }
}