using UnityEngine;

/// <summary>
/// An instantaneous action which dispatches an event as soon as the input is read
/// </summary>
[CreateAssetMenu(fileName = "InputAction_Trigger", menuName = "InputActions/Trigger")]
public class InputAction_Trigger : ScriptableObject
{
    public delegate void Triggered(BLDAction_Trigger action);
    public event Triggered OnTriggered;

    [SerializeField] internal BLDAction_Trigger bldAction;
    public BLDAction_Trigger GetBLDAction() => bldAction;

    [SerializeField] internal BooleanVariable blockTrigger;
    public BooleanVariable isTriggerBlocked { get => blockTrigger; set => blockTrigger = value; }

    private int triggeredInFrame = -1;

    public bool WasTriggeredThisFrame() { return triggeredInFrame == Time.frameCount; }

    public void RaiseOnTriggered()
    {
        triggeredInFrame = Time.frameCount;
        OnTriggered?.Invoke(bldAction);
    }

    #region Editor

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(InputAction_Trigger), true)]
    internal class InputAction_TriggerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (Application.isPlaying && GUILayout.Button("Raise OnChange"))
            {
                ((InputAction_Trigger)target).RaiseOnTriggered();
            }
        }
    }
#endif

    #endregion

}