using System;
using BLD.Models;
using BLDPlugins.DebugPlugins.Commons;
using UnityEngine;
using Object = UnityEngine.Object;

internal class EntityWireframe : IShapeListener
{
    private const float WIREFRAME_SIZE_MULTIPLIER = 1.01f;

    private readonly GameObject wireframeOriginal;

    private GameObject entityWireframe;

    public EntityWireframe(GameObject wireframeOriginal)
    {
        this.wireframeOriginal = wireframeOriginal;
    }

    void IDisposable.Dispose()
    {
        CleanWireframe();
    }

    void IShapeListener.OnShapeUpdated(IBLDEntity entity)
    {
        entityWireframe ??= Object.Instantiate(wireframeOriginal);

        Transform wireframeT = entityWireframe.transform;

        wireframeT.position = entity.meshesInfo.mergedBounds.center;
        wireframeT.localScale = entity.meshesInfo.mergedBounds.size * WIREFRAME_SIZE_MULTIPLIER;

        wireframeT.SetParent(entity.gameObject.transform);
        entityWireframe.SetActive(true);
    }

    void IShapeListener.OnShapeCleaned(IBLDEntity entity)
    {
        CleanWireframe();
    }

    private void CleanWireframe()
    {
        if (entityWireframe == null)
            return;

        entityWireframe.SetActive(false);
        Object.Destroy(entityWireframe);
        entityWireframe = null;
    }
}