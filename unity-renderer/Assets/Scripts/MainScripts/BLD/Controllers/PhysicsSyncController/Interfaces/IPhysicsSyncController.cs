using System;

namespace BLD
{
    public interface IPhysicsSyncController : IService
    {
        bool isDirty { get; }
        void MarkDirty();
        void Sync();
    }
}