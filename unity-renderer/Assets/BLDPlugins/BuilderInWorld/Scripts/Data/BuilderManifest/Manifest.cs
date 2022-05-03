using System;

namespace BLD.Builder.Manifest
{
    [Serializable]
    public class Manifest
    {
        public int version;
        public ProjectData project;
        public WebBuilderScene scene;
    }
}