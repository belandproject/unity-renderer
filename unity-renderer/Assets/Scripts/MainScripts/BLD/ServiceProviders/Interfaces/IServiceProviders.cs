using System;
using BLD.Helpers.NFT.Markets;

namespace BLD
{
    public interface IServiceProviders : IService
    {
        ITheGraph theGraph { get; }
        ICatalyst catalyst { get; }
        IAnalytics analytics { get; }
        INFTMarket openSea { get; }
    }
}