using UnityEditor;
using UnityEngine;

namespace BLD
{
    public static partial class UnityEditorWrappers
    {
        public class BuildPipeline : IBuildPipeline
        {
            public AssetBundleManifest BuildAssetBundles(string outputPath, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform) { return UnityEditor.BuildPipeline.BuildAssetBundles(outputPath, assetBundleOptions, targetPlatform); }
        }
    }
}