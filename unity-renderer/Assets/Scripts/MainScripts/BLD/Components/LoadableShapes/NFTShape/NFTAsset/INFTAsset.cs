using System;
using UnityEngine;

namespace NFTShape_Internal
{
    public interface INFTAsset : IDisposable
    {
        bool isHQ { get; }
        BLD.ITexture previewAsset { get; }
        BLD.ITexture hqAsset { get; }
        void FetchAndSetHQAsset(string url, Action onSuccess, Action<Exception> onFail);
        void RestorePreviewAsset();
        event Action<Texture2D> OnTextureUpdate;
    }
}