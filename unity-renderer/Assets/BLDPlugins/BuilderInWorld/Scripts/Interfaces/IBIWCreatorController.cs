using System;
using BLD.Models;
using UnityEngine;

public interface IBIWCreatorController : IBIWController
{
    event Action OnCatalogItemPlaced;
    event Action OnInputDone;
    void CreateCatalogItem(CatalogItem catalogItem, bool autoSelect = true, bool isFloor = false);
    BIWEntity CreateCatalogItem(CatalogItem catalogItem, Vector3 startPosition, bool autoSelect = true, bool isFloor = false, Action<IBLDEntity> onFloorLoadedAction = null);
    void CreateErrorOnEntity(BIWEntity entity);
    void RemoveLoadingObjectInmediate(string entityId);
    bool IsAnyErrorOnEntities();
    void CreateLoadingObject(BIWEntity entity);
    void CleanUp();
}