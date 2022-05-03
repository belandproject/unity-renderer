using BLD.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Note (Adrian): DLCBuilderEntity and BelandEntityToEdit should be merged somehow and this class should change or dissapear
public class EditableEntity : MonoBehaviour
{
    public IBLDEntity rootEntity { protected set; get; }

    public virtual void SetSelectLayer() { }

    public virtual void SetDefaultLayer() { }
}