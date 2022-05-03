using BLD.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIWEntityAction
{
    public string entityId;

    public object oldValue;
    public object newValue;

    public BIWEntityAction(string entityId) { this.entityId = entityId; }

    public BIWEntityAction(string entityId, object oldValue, object newValue)
    {
        this.entityId = entityId;
        this.oldValue = oldValue;
        this.newValue = newValue;
    }

    public BIWEntityAction(object oldValue, object newValue)
    {
        this.oldValue = oldValue;
        this.newValue = newValue;
    }

    public BIWEntityAction(IBLDEntity entity) { this.entityId = entity.entityId; }

    public BIWEntityAction(IBLDEntity entity, object oldValue, object newValue)
    {
        this.entityId = entity.entityId;
        this.oldValue = oldValue;
        this.newValue = newValue;
    }
}