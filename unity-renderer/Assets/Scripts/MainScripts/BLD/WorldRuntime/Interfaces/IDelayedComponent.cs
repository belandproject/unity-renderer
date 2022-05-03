using UnityEngine;

namespace BLD.Components
{
    public interface IDelayedComponent : IComponent, ICleanable
    {
        CustomYieldInstruction yieldInstruction { get; }
        Coroutine routine { get; }
        bool isRoutineRunning { get; }
    }
}