using BLD.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace BLD.Components
{
    public class UIImageReferencesContainer : UIReferencesContainer
    {
        [Header("UI Image Fields")]
        public HorizontalLayoutGroup paddingLayoutGroup;

        public RawImage image;
        public RectTransform imageRectTransform;
    }
}