using UnityEngine.Networking;

namespace BLD
{
    /// <summary>
    /// Our custom implementation of the UnityWebRequest.
    /// </summary>
    public class WebRequest : IWebRequest
    {
        public UnityWebRequest CreateWebRequest(string url) { return UnityWebRequest.Get(url); }
    }
}