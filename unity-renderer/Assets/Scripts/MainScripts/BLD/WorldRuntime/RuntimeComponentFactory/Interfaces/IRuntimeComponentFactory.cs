using System;
using BLD.Components;

namespace BLD
{
    public interface IRuntimeComponentFactory : IService
    {
        IComponent CreateComponent(int classId);
    }
}