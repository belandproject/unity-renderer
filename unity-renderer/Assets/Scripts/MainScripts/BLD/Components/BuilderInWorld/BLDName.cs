using BLD;
using BLD.Components;
using BLD.Controllers;
using BLD.Models;
using System.Collections;
using System.Collections.Generic;
using BLD.Helpers;
using UnityEngine;

/// <summary>
/// This component is a descriptive name of the Entity. In the BuilderInWorld you can give an entity a descriptive name through the entity list.
/// Builder in World send a message to kernel to change the value of this component in order to assign a descriptive name
/// </summary>
public class BLDName : BaseDisposable
{
    [System.Serializable]
    public class Model : BaseModel
    {
        public string value;
        //TODO: This value is used for builder to manage the smart items, when the builder is no longer active we should remove it
        public string builderValue;

        public override BaseModel GetDataFromJSON(string json) { return Utils.SafeFromJson<Model>(json); }
    }

    public BLDName() { model = new Model(); }

    private string oldName;

    public override int GetClassId() { return (int) CLASS_ID.NAME; }

    public override IEnumerator ApplyChanges(BaseModel newModel)
    {
        Model modelToApply = (Model) newModel;

        model = modelToApply;

        foreach (IBLDEntity entity in attachedEntities)
        {
            entity.OnNameChange?.Invoke(modelToApply);
        }

#if UNITY_EDITOR
        foreach (IBLDEntity belandEntity in this.attachedEntities)
        {
            if (!string.IsNullOrEmpty(oldName))
                belandEntity.gameObject.name.Replace(oldName, "");

            belandEntity.gameObject.name += $"-{modelToApply.value}";
        }
#endif
        oldName = modelToApply.value;
        return null;
    }

    public void SetNewName(string value)
    {
        Model newModel = new Model();
        newModel.value = value;
        UpdateFromModel(newModel);
    }
}