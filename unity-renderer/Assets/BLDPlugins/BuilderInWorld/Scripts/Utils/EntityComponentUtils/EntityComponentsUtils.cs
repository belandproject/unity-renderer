using System;
using System.Collections;
using System.Collections.Generic;
using BLD.Components;
using BLD.Controllers;
using BLD.Models;
using UnityEditor;
using UnityEngine;

namespace BLD.Builder
{
    public static class EntityComponentsUtils
    {
        public static void AddTransformComponent(IParcelScene scene, IBLDEntity entity, BLDTransform.Model model)
        {
            scene.EntityComponentCreateOrUpdateWithModel(entity.entityId, CLASS_ID_COMPONENT.TRANSFORM, model);
        }

        public static NFTShape AddNFTShapeComponent(IParcelScene scene, IBLDEntity entity, NFTShape.Model model, string id = "")
        {
            id = EnsureId(id);
            
            NFTShape nftShape = (NFTShape) scene.SharedComponentCreate(id, Convert.ToInt32(CLASS_ID.NFT_SHAPE));
            nftShape.model = model;
            scene.SharedComponentAttach(entity.entityId, nftShape.id);
            return nftShape;
        }
        
        public static GLTFShape AddGLTFComponent(IParcelScene scene, IBLDEntity entity, GLTFShape.Model model, string id = "")
        {
            id = EnsureId(id);
            
            GLTFShape gltfComponent = (GLTFShape) scene.SharedComponentCreate(id, Convert.ToInt32(CLASS_ID.GLTF_SHAPE));
            gltfComponent.model = model;
            scene.SharedComponentAttach(entity.entityId, gltfComponent.id);
            return gltfComponent;
        }

        public static BLDName AddNameComponent(IParcelScene scene, IBLDEntity entity, BLDName.Model model, string id = "")
        {
            id = EnsureId(id);
            
            BLDName name = (BLDName) scene.SharedComponentCreate(id, Convert.ToInt32(CLASS_ID.NAME));
            name.UpdateFromModel(model);
            scene.SharedComponentAttach(entity.entityId, name.id);
            return name;
        }
        
        public static BLDLockedOnEdit AddLockedOnEditComponent(IParcelScene scene, IBLDEntity entity, BLDLockedOnEdit.Model model, string id = "")
        {
            id = EnsureId(id);
            
            BLDLockedOnEdit lockedOnEditComponent = (BLDLockedOnEdit) scene.SharedComponentCreate(id, Convert.ToInt32(CLASS_ID.LOCKED_ON_EDIT));
            lockedOnEditComponent.UpdateFromModel(model);
            scene.SharedComponentAttach(entity.entityId, lockedOnEditComponent.id);
            return lockedOnEditComponent;
        }

        private static string EnsureId(string currentId)
        {
            if (string.IsNullOrEmpty(currentId))
                return Guid.NewGuid().ToString();
            return currentId;
        }
    }
}
