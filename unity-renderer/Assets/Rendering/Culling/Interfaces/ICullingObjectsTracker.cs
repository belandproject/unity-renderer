using System;
using System.Collections;
using UnityEngine;

namespace BLD.Rendering
{
    public interface ICullingObjectsTracker : IDisposable
    {
        void SetIgnoredLayersMask(int ignoredLayersMask);
        void MarkDirty();
        bool IsDirty();
        Renderer[] GetRenderers();
        SkinnedMeshRenderer[] GetSkinnedRenderers();
        Animation[] GetAnimations();
        IEnumerator PopulateRenderersList();
        void ForcePopulateRenderersList(bool includeInactives);
    }
}