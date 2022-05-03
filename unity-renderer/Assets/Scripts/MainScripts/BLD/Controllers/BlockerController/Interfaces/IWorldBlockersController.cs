using System;
using BLD.Rendering;

namespace BLD.Controllers
{
    public interface IWorldBlockersController : IService
    {
        void SetupWorldBlockers();
        void SetEnabled(bool targetValue);
    }
}