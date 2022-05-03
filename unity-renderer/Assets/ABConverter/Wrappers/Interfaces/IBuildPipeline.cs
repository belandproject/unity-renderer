using UnityEditor;
using UnityEngine;

namespace BLD
{
    public interface IBuildPipeline
    {
        AssetBundleManifest BuildAssetBundles(
            string outputPath,
            BuildAssetBundleOptions assetBundleOptions,
            BuildTarget targetPlatform);
    }
}