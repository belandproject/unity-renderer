using BLD.SettingsPanelHUD.Sections;
using ReorderableList;
using UnityEngine;

namespace BLD.SettingsPanelHUD
{
    [System.Serializable]
    public class SettingsSectionList : ReorderableArray<SettingsSectionModel> { }

    /// <summary>
    /// Model that represents the main settings panel. It contains a list of SECTIONS.
    /// </summary>
    [CreateAssetMenu(menuName = "Settings/Configuration/MainPanel", fileName = "MainPanelConfiguration")]
    public class SettingsPanelModel : ScriptableObject
    {
        [Reorderable]
        public SettingsSectionList sections;
    }
}