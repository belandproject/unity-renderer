using BLD.Controllers;
using BLD.Helpers;
using BLD.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace BLD.Components
{
    public class BasicMaterial : BaseDisposable
    {
        [System.Serializable]
        public class Model : BaseModel
        {
            public string texture;

            // value that defines if a pixel is visible or invisible (no transparency gradients)
            [Range(0f, 1f)]
            public float alphaTest = 0.5f;

            public bool castShadows = true;

            public override BaseModel GetDataFromJSON(string json) { return Utils.SafeFromJson<Model>(json); }
        }

        public Material material;

        private BLDTexture bldTexture = null;

        private static readonly int _BaseMap = Shader.PropertyToID("_BaseMap");
        private static readonly int _AlphaClip = Shader.PropertyToID("_AlphaClip");
        private static readonly int _Cutoff = Shader.PropertyToID("_Cutoff");
        private static readonly int _ZWrite = Shader.PropertyToID("_ZWrite");

        public BasicMaterial()
        {
            material = new Material(Utils.EnsureResourcesMaterial("Materials/BasicShapeMaterial"));

            OnAttach += OnMaterialAttached;
            OnDetach += OnMaterialDetached;
            model = new Model();
        }

        new public Model GetModel() { return (Model) model; }

        public override int GetClassId() { return (int) CLASS_ID.BASIC_MATERIAL; }

        public override void AttachTo(IBLDEntity entity, System.Type overridenAttachedType = null)
        {
            if (attachedEntities.Contains(entity))
                return;

            entity.RemoveSharedComponent(typeof(PBRMaterial));
            base.AttachTo(entity, overridenAttachedType);
        }

        public override void DetachFrom(IBLDEntity entity, System.Type overridenAttachedType = null)
        {
            base.DetachFrom(entity, overridenAttachedType);
        }

        public override IEnumerator ApplyChanges(BaseModel newModel)
        {
            if (material == null)
            {
                yield break; // We escape ApplyChanges called in the parent's constructor
            }

#if UNITY_EDITOR
            material.name = "BasicMaterial_" + id;
#endif

            Model model = (Model) newModel;

            if (!string.IsNullOrEmpty(model.texture))
            {
                if (bldTexture == null || bldTexture.id != model.texture)
                {
                    yield return BLDTexture.FetchTextureComponent(scene, model.texture,
                        (downloadedTexture) =>
                        {
                            if ( bldTexture != null )
                            {
                                bldTexture.DetachFrom(this);
                            }

                            material.SetTexture(_BaseMap, downloadedTexture.texture);
                            bldTexture = downloadedTexture;
                            bldTexture.AttachTo(this);
                        }
                    );
                }
            }
            else
            {
                material.mainTexture = null;

                if ( bldTexture != null )
                {
                    bldTexture.DetachFrom(this);
                    bldTexture = null;
                }
            }

            material.EnableKeyword("_ALPHATEST_ON");
            material.SetInt(_ZWrite, 1);
            material.SetFloat(_AlphaClip, 1);
            material.SetFloat(_Cutoff, model.alphaTest);
            material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.AlphaTest;

            foreach (IBLDEntity entity in attachedEntities)
            {
                InitMaterial(entity);
            }
        }

        void OnMaterialAttached(IBLDEntity entity)
        {
            entity.OnShapeUpdated -= OnShapeUpdated;
            entity.OnShapeUpdated += OnShapeUpdated;

            if (entity.meshRootGameObject != null)
            {
                var meshRenderer = entity.meshRootGameObject.GetComponent<MeshRenderer>();

                if (meshRenderer != null)
                    InitMaterial(entity);
            }
        }

        void InitMaterial(IBLDEntity entity)
        {
            var meshGameObject = entity.meshRootGameObject;

            if (meshGameObject == null)
                return;

            var meshRenderer = meshGameObject.GetComponent<MeshRenderer>();

            if (meshRenderer == null)
                return;

            Model model = (Model) this.model;

            meshRenderer.shadowCastingMode = model.castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;

            if (meshRenderer.sharedMaterial == material)
                return;

            MaterialTransitionController
                matTransition = meshGameObject.GetComponent<MaterialTransitionController>();

            if (matTransition != null && matTransition.canSwitchMaterial)
            {
                matTransition.finalMaterials = new Material[] { material };
                matTransition.PopulateTargetRendererWithMaterial(matTransition.finalMaterials);
            }

            SRPBatchingHelper.OptimizeMaterial(material);

            Material oldMaterial = meshRenderer.sharedMaterial;
            meshRenderer.sharedMaterial = material;

            DataStore.i.sceneWorldObjects.RemoveMaterial(scene.sceneData.id, entity.entityId, oldMaterial);
            DataStore.i.sceneWorldObjects.AddMaterial(scene.sceneData.id, entity.entityId, material);
        }

        private void OnShapeUpdated(IBLDEntity entity)
        {
            if (entity != null)
                InitMaterial(entity);
        }

        void OnMaterialDetached(IBLDEntity entity)
        {
            if (entity.meshRootGameObject == null)
                return;

            entity.OnShapeUpdated -= OnShapeUpdated;

            var meshRenderer = entity.meshRootGameObject.GetComponent<MeshRenderer>();

            if (meshRenderer && meshRenderer.sharedMaterial == material)
                meshRenderer.sharedMaterial = null;

            DataStore.i.sceneWorldObjects.RemoveMaterial(scene.sceneData.id, entity.entityId, material);
        }

        public override void Dispose()
        {
            bldTexture?.DetachFrom(this);

            while ( attachedEntities.Count > 0 )
            {
                DetachFrom(attachedEntities.First());
            }

            Object.Destroy(material);
            base.Dispose();
        }
    }
}