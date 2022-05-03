using BLD.Models;
using System;

namespace BLD.Components
{
    public abstract class LoadWrapper
    {
        public bool useVisualFeedback = true;
        public bool initialVisibility = true;
        public bool alreadyLoaded = false;

        public IBLDEntity entity;

        public abstract void Load(string url, Action<LoadWrapper> OnSuccess, Action<LoadWrapper, Exception> OnFail);
        public abstract void Unload();
    }
}