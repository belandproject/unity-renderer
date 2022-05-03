using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BLD.Controllers;
using BLD.Helpers;
using BLD.Models;

namespace BLD.Components
{
    public class BLDVideoClip : BaseDisposable
    {
        [System.Serializable]
        public class Model : BaseModel
        {
            public string url;

            public override BaseModel GetDataFromJSON(string json) { return Utils.SafeFromJson<Model>(json); }
        }

        public bool isExternalURL { get; private set; }
        public bool isStream { get; private set; }

        public BLDVideoClip() { model = new Model(); }

        public override int GetClassId() { return (int) CLASS_ID.VIDEO_CLIP; }

        public override IEnumerator ApplyChanges(BaseModel newModel)
        {
            Model model = (Model) newModel;
            isExternalURL = model.url.StartsWith("http://") || model.url.StartsWith("https://");
            isStream = !new[] { ".mp4", ".ogg", ".mov", ".webm" }.Any(x => model.url.EndsWith(x));
            yield break;
        }

        public string GetUrl()
        {
            Model model = (Model) this.model;

            string contentsUrl = model.url;

            if (!isExternalURL)
            {
                scene.contentProvider.TryGetContentsUrl(model.url, out contentsUrl);
            }

            return contentsUrl;
        }
    }
}