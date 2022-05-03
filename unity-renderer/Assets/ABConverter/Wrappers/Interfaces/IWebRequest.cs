using UnityEngine.Networking;

namespace BLD
{
    public interface IWebRequest
    {
        DownloadHandler Get(string url);
        void GetAsync(string url, System.Action<DownloadHandler> OnCompleted, System.Action<string> OnFail);
    }
}