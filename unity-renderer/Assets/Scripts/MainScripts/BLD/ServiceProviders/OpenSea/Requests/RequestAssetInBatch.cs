namespace BLD.Helpers.NFT.Markets.OpenSea_Internal
{
    public class RequestAssetInBatch : RequestBase<AssetResponse>
    {
        public string contractAddress { get; }
        public string tokenId { get; }
        public override string requestId => GetId(contractAddress, tokenId);

        internal RequestAssetInBatch(string contractAddress, string tokenId)
        {
            this.contractAddress = contractAddress;
            this.tokenId = tokenId;
        }

        internal static string GetId(string contractAddress, string tokenId) { return $"{contractAddress}/{tokenId}"; }
    }
}