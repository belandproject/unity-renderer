using UnityEngine.Networking;

namespace BLD
{
    /// <summary>
    /// Our custom implementation of the UnityWebRequestTexture.
    /// </summary>
    public class WebRequestTexture : IWebRequestTexture
    {
        public bool isReadable { get; set; }

        public UnityWebRequest CreateWebRequest(string url)
        {
            return UnityWebRequestTexture.GetTexture(url, !isReadable);
        }
    }
}