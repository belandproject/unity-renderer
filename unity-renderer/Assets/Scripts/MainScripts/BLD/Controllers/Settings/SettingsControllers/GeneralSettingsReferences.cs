using Cinemachine;
using UnityEngine;

namespace BLD.SettingsCommon.SettingsControllers
{
    /// <summary>
    /// This MonoBehaviour will only contain the external references needed for the general settings.
    /// </summary>
    public class GeneralSettingsReferences : MonoBehaviour
    {
        public static GeneralSettingsReferences i { get; private set; }

        private void Awake() { i = this; }
    }
}