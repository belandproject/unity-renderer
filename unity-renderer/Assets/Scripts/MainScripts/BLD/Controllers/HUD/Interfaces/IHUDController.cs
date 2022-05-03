using System;

namespace BLD
{
    public interface IHUDController : IService
    {
        void ConfigureHUDElement(HUDElementID hudElementId, HUDConfiguration configuration, string extraPayload = null);
        IHUD GetHUDElement(HUDElementID id);
        void Cleanup();
    }
}