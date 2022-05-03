using System;
using UnityEngine;

namespace BLD
{
    public interface IPointerEventsController : IService
    {
        void Update();
        Ray GetRayFromCamera();
    }
}