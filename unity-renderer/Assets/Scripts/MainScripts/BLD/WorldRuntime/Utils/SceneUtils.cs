using System.Collections.Generic;
using BLD.Components;
using BLD.Models;

namespace BLD.Controllers
{
    public static class SceneUtils
    {
        public static IBLDEntity DuplicateEntity(ParcelScene scene, IBLDEntity entity)
        {
            if (!scene.entities.ContainsKey(entity.entityId))
                return null;

            IBLDEntity newEntity = scene.CreateEntity(System.Guid.NewGuid().ToString());

            if (entity.children.Count > 0)
            {
                using (var iterator = entity.children.GetEnumerator())
                {
                    while (iterator.MoveNext())
                    {
                        IBLDEntity childDuplicate = DuplicateEntity(scene, iterator.Current.Value);
                        childDuplicate.SetParent(newEntity);
                    }
                }
            }

            if (entity.parent != null)
                scene.SetEntityParent(newEntity.entityId, entity.parent.entityId);

            BLDTransform.model.position = WorldStateUtils.ConvertUnityToScenePosition(entity.gameObject.transform.position);
            BLDTransform.model.rotation = entity.gameObject.transform.rotation;
            BLDTransform.model.scale = entity.gameObject.transform.lossyScale;

            foreach (KeyValuePair<CLASS_ID_COMPONENT, IEntityComponent> component in entity.components)
            {
                scene.EntityComponentCreateOrUpdateWithModel(newEntity.entityId, component.Key, component.Value.GetModel());
            }

            foreach (KeyValuePair<System.Type, ISharedComponent> component in entity.sharedComponents)
            {
                ISharedComponent sharedComponent = scene.SharedComponentCreate(System.Guid.NewGuid().ToString(), component.Value.GetClassId());
                sharedComponent.UpdateFromModel(component.Value.GetModel());
                scene.SharedComponentAttach(newEntity.entityId, sharedComponent.id);
            }

            return newEntity;
        }
    }
}