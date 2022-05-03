using System;
using UnityEngine;

/// <summary>
/// An on/off action that triggers Start when the input is read and Finished when the input has gone
/// </summary>
[CreateAssetMenu(fileName = "InputAction_Hold", menuName = "InputActions/Hold")]
public class InputAction_Hold : ScriptableObject
{
    public delegate void Started(BLDAction_Hold action);
    public delegate void Finished(BLDAction_Hold action);
    public event Started OnStarted;
    public event Finished OnFinished;

    [SerializeField] internal BLDAction_Hold bldAction;
    public BLDAction_Hold GetBLDAction() => bldAction;

    public bool isOn { get; private set; }

    public void RaiseOnStarted()
    {
        isOn = true;
        OnStarted?.Invoke(bldAction);
    }

    public void RaiseOnFinished()
    {
        isOn = false;
        OnFinished?.Invoke(bldAction);
    }

    #region Editor

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(InputAction_Hold), true)]
    internal class InputAction_HoldEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (Application.isPlaying && GUILayout.Button("Raise OnStarted"))
            {
                ((InputAction_Hold)target).RaiseOnStarted();
            }
            if (Application.isPlaying && GUILayout.Button("Raise OnFinished"))
            {
                ((InputAction_Hold)target).RaiseOnFinished();
            }
        }
    }
#endif

    #endregion

}