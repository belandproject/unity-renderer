using System;

namespace BLD
{
    public interface IService : IDisposable
    {
        void Initialize();
    }
}