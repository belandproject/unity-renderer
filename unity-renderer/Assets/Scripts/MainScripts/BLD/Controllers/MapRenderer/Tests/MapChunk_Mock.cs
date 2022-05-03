using UnityEngine.Networking;

namespace BLD
{
    public class MapChunk_Mock : MapChunk
    {
        public override WebRequestAsyncOperation LoadChunkImage()
        {
            isLoadingOrLoaded = true;

            return new WebRequestAsyncOperation(null);
        }
    }
}