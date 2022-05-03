using System;

namespace BLD.Helpers.NFT.Markets.OpenSea_Internal
{
    internal class SchedulableRequestHandler
    {
        public event Action<IRequestHandler> OnReadyToSchedule;
        public bool isReadyToSchedule { private set; get; }

        public void SetReadyToBeScheduled(IRequestHandler handler)
        {
            isReadyToSchedule = true;
            OnReadyToSchedule?.Invoke(handler);
        }
    }
}