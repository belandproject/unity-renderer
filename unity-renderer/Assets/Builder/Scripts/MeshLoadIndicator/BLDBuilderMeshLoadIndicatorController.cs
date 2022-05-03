using UnityEngine;
using System.Collections.Generic;

namespace Builder.MeshLoadIndicator
{
    public class BLDBuilderMeshLoadIndicatorController : MonoBehaviour
    {
        public BLDBuilderMeshLoadIndicator indicator => baseIndicator;

        [SerializeField] private BLDBuilderMeshLoadIndicator baseIndicator = null;

        private Queue<BLDBuilderMeshLoadIndicator> indicatorsAvailable;
        private List<BLDBuilderMeshLoadIndicator> indicatorsInUse;

        private bool isGameObjectActive = false;
        private bool isPreviewMode = false;

        private void Awake() { Init(); }

        private void OnEnable()
        {
            if (!isGameObjectActive)
            {
                BLDBuilderEntity.OnEntityAddedWithTransform += OnEntityAdded;
                BLDBuilderEntity.OnEntityShapeUpdated += OnShapeUpdated;
                BLDBuilderBridge.OnResetBuilderScene += OnResetBuilderScene;
                BLDBuilderBridge.OnPreviewModeChanged += OnPreviewModeChanged;
            }
            isGameObjectActive = true;
        }

        private void OnDisable()
        {
            isGameObjectActive = false;
            BLDBuilderEntity.OnEntityAddedWithTransform -= OnEntityAdded;
            BLDBuilderEntity.OnEntityShapeUpdated -= OnShapeUpdated;
            BLDBuilderBridge.OnResetBuilderScene -= OnResetBuilderScene;
            BLDBuilderBridge.OnPreviewModeChanged -= OnPreviewModeChanged;
        }

        public void Init()
        {
            indicatorsAvailable = new Queue<BLDBuilderMeshLoadIndicator>();
            indicatorsInUse = new List<BLDBuilderMeshLoadIndicator>();
        }

        public void Dispose()
        {
            if (indicatorsAvailable == null || indicatorsInUse == null)
                return;

            foreach (BLDBuilderMeshLoadIndicator indicator in indicatorsAvailable)
            {
                Destroy(indicator.gameObject);
            }

            foreach (BLDBuilderMeshLoadIndicator indicator in indicatorsInUse)
            {
                Destroy(indicator.gameObject);
            }
        }

        private void OnEntityAdded(BLDBuilderEntity entity)
        {
            if (!entity.HasShape() && !isPreviewMode)
            {
                ShowIndicator(entity.transform.position, entity.rootEntity.entityId);
            }
        }

        private void OnShapeUpdated(BLDBuilderEntity entity)
        {
            if (!isPreviewMode)
            {
                HideIndicator(entity.rootEntity.entityId);
            }
        }

        private void OnResetBuilderScene() { HideAllIndicators(); }

        private void OnPreviewModeChanged(bool isPreview)
        {
            isPreviewMode = isPreview;
            if (isPreview)
            {
                HideAllIndicators();
            }
        }

        public BLDBuilderMeshLoadIndicator ShowIndicator(Vector3 position, string entityId)
        {
            BLDBuilderMeshLoadIndicator ret;

            if (indicatorsAvailable == null)
                return null;

            if (indicatorsAvailable.Count > 0)
            {
                ret = indicatorsAvailable.Dequeue();
                ret.transform.position = position;
            }
            else
            {
                ret = Object.Instantiate(baseIndicator, position, Quaternion.identity, transform);
            }

            ret.loadingEntityId = entityId;
            ret.gameObject.SetActive(true);
            indicatorsInUse.Add(ret);
            return ret;
        }

        public void HideIndicator(string entityId)
        {
            if (indicatorsInUse == null)
                return;

            for (int i = 0; i < indicatorsInUse.Count; i++)
            {
                if (indicatorsInUse[i].loadingEntityId == entityId)
                {
                    indicatorsInUse[i].gameObject.SetActive(false);
                    indicatorsAvailable.Enqueue(indicatorsInUse[i]);
                    indicatorsInUse.RemoveAt(i);
                    break;
                }
            }
        }

        public void HideAllIndicators()
        {
            if (indicatorsInUse == null)
                return;

            for (int i = 0; i < indicatorsInUse.Count; i++)
            {
                indicatorsInUse[i].gameObject.SetActive(false);
                indicatorsAvailable.Enqueue(indicatorsInUse[i]);
            }
            indicatorsInUse.Clear();
        }
    }
}