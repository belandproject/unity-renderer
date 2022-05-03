using System.Collections;
using System.Collections.Generic;
using BLD.Controllers;
using BLD.Helpers;
using BLD.Models;
using TMPro;
using UnityEngine;

namespace BLD.Components
{
    public class BLDFont : BaseDisposable
    {
        const string RESOURCE_FONT_FOLDER = "Fonts & Materials";

        private const string DEFAULT_SANS_SERIF_HEAVY = "Inter-Heavy SDF";
        private const string DEFAULT_SANS_SERIF_BOLD = "Inter-Bold SDF";
        private const string DEFAULT_SANS_SERIF_SEMIBOLD = "Inter-SemiBold SDF";
        private const string DEFAULT_SANS_SERIF = "Inter-Regular SDF";

        private readonly Dictionary<string, string> fontsMapping = new Dictionary<string, string>()
        {
            { "builtin:SF-UI-Text-Regular SDF", DEFAULT_SANS_SERIF },
            { "builtin:SF-UI-Text-Heavy SDF", DEFAULT_SANS_SERIF_HEAVY },
            { "builtin:SF-UI-Text-Semibold SDF", DEFAULT_SANS_SERIF_SEMIBOLD },
            { "builtin:LiberationSans SDF", "LiberationSans SDF" },
            { "SansSerif", DEFAULT_SANS_SERIF },
            { "SansSerif_Heavy", DEFAULT_SANS_SERIF_HEAVY },
            { "SansSerif_Bold", DEFAULT_SANS_SERIF_BOLD },
            { "SansSerif_SemiBold", DEFAULT_SANS_SERIF_SEMIBOLD },
        };

        [System.Serializable]
        public class Model : BaseModel
        {
            public string src;

            public override BaseModel GetDataFromJSON(string json) { return Utils.SafeFromJson<Model>(json); }
        }

        public bool loaded { private set; get; } = false;
        public bool error { private set; get; } = false;

        public TMP_FontAsset fontAsset { private set; get; }

        public BLDFont() { model = new Model(); }

        public override int GetClassId() { return (int) CLASS_ID.FONT; }

        public static bool IsFontLoaded(IParcelScene scene, string componentId)
        {
            if ( string.IsNullOrEmpty(componentId))
                return true;

            if (!scene.disposableComponents.ContainsKey(componentId))
            {
                Debug.Log($"couldn't fetch font, the BLDFont component with id {componentId} doesn't exist");
                return false;
            }

            BLDFont fontComponent = scene.disposableComponents[componentId] as BLDFont;

            if (fontComponent == null)
            {
                Debug.Log($"couldn't fetch font, the shared component with id {componentId} is NOT a BLDFont");
                return false;
            }

            return true;
        }

        public static IEnumerator WaitUntilFontIsReady(IParcelScene scene, string componentId)
        {
            if ( string.IsNullOrEmpty(componentId))
                yield break;

            BLDFont fontComponent = scene.disposableComponents[componentId] as BLDFont;

            while (!fontComponent.loaded && !fontComponent.error)
            {
                yield return null;
            }
        }

        public static void SetFontFromComponent(IParcelScene scene, string componentId, TMP_Text text)
        {
            if ( string.IsNullOrEmpty(componentId))
                return;

            BLDFont fontComponent = scene.disposableComponents[componentId] as BLDFont;

            if (!fontComponent.error)
            {
                text.font = fontComponent.fontAsset;
            }
        }

        public override IEnumerator ApplyChanges(BaseModel newModel)
        {
            Model model = (Model) newModel;

            if (string.IsNullOrEmpty(model.src))
            {
                error = true;
                yield break;
            }

            if (fontsMapping.TryGetValue(model.src, out string fontResourceName))
            {
                ResourceRequest request = Resources.LoadAsync($"{RESOURCE_FONT_FOLDER}/{fontResourceName}", 
                    typeof(TMP_FontAsset));
                
                yield return request;

                if (request.asset != null)
                {
                    fontAsset = request.asset as TMP_FontAsset;
                }
                else
                {
                    Debug.Log($"couldn't fetch font from resources {fontResourceName}");
                }

                loaded = true;
                error = fontAsset == null;
            }
            else
            {
                // NOTE: only support fonts in resources
                error = true;
            }
        }
    }
}