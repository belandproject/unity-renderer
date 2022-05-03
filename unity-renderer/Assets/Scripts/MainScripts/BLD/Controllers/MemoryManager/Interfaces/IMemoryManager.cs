using System;
using System.Collections;

namespace BLD
{
    public interface IMemoryManager : IService
    {
        public event System.Action OnCriticalMemory;
    }
}