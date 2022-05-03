using System.Collections;
using System.Net.Configuration;
using BLD.Controllers;
using BLD.Models;
using UnityEngine;

namespace BLD.Components
{
    public class BLDTransform : IEntityComponent
    {
        [System.Serializable]
        public class Model : BaseModel
        {
            public Vector3 position = Vector3.zero;
            public Quaternion rotation = Quaternion.identity;
            public Vector3 scale = Vector3.one;

            public override BaseModel GetDataFromJSON(string json)
            {
                MessageDecoder.DecodeTransform(json, ref BLDTransform.model);
                return BLDTransform.model;
            }
        }

        public static Model model = new Model();

        public void Cleanup() { }

        public string componentName { get; } = "Transform";
        public IParcelScene scene { get; private set; }
        public IBLDEntity entity { get; private set; }
        public Transform GetTransform() => null;

        public void Initialize(IParcelScene scene, IBLDEntity entity)
        {
            this.scene = scene;
            this.entity = entity;
        }

        public void UpdateFromJSON(string json)
        {
            model.GetDataFromJSON(json);
            UpdateFromModel(model);
        }

        public void UpdateFromModel(BaseModel model)
        {
            BLDTransform.model = model as Model;

            if (entity.OnTransformChange != null) // AvatarShape interpolation hack
            {
                entity.OnTransformChange.Invoke(BLDTransform.model);
            }
            else
            {
                entity.gameObject.transform.localPosition = BLDTransform.model.position;
                entity.gameObject.transform.localRotation = BLDTransform.model.rotation;
                entity.gameObject.transform.localScale = BLDTransform.model.scale;

                BLD.Environment.i.world.sceneBoundsChecker?.AddEntityToBeChecked(entity);
            }
        }

        public IEnumerator ApplyChanges(BaseModel model) { return null; }

        public void RaiseOnAppliedChanges() { }

        public bool IsValid() => true;
        public BaseModel GetModel() => BLDTransform.model;
        public int GetClassId() => (int) CLASS_ID_COMPONENT.TRANSFORM;
    }
}