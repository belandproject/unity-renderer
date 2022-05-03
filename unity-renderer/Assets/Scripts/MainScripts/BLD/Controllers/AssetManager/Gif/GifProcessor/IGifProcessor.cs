using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace BLD
{
    public interface IGifProcessor
    {
        void DisposeGif();
        UniTask Load(Action<GifFrameData[]> loadSuccsess, Action<Exception> fail, CancellationToken token);
    }
}