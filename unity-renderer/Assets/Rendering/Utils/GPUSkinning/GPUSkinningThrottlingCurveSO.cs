using UnityEngine;

[CreateAssetMenu(fileName = "GPUSkinningThrottlingCurve", menuName = "BLD/GPUSkinning throttling curve")]
public class GPUSkinningThrottlingCurveSO : ScriptableObject
{
    public AnimationCurve curve;
}