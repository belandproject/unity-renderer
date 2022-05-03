using System.Collections;
using System.Collections.Generic;
using BLD.Builder;
using UnityEngine;

public interface IBIWGizmosAxis
{
    void SetColorHighlight();
    void SetColorDefault();
    IBIWGizmos GetGizmo();

    Transform axisTransform { get; }
}