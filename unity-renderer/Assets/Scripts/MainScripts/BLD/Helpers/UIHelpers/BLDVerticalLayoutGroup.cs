using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Unity's default VerticalLayoutGroup aligns the child horizontally. We override this behaviour to avoid that 
/// </summary>
public class BLDVerticalLayoutGroup : VerticalLayoutGroup
{
    public override void SetLayoutHorizontal() { }
}