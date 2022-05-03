using System.Collections;
using BLD;
using NSubstitute;
using NSubstitute.ClearExtensions;
using UnityEngine.TestTools;

namespace Tests
{
    public class IntegrationTestSuite
    {
        protected virtual void InitializeServices(ServiceLocator serviceLocator)
        {
        }

        [UnitySetUp]
        protected virtual IEnumerator SetUp()
        {
            CommonScriptableObjects.rendererState.Set(true);
            BLD.Configuration.EnvironmentSettings.RUNNING_TESTS = true;
            BLD.Configuration.ParcelSettings.VISUAL_LOADING_ENABLED = false;
            AssetPromiseKeeper_GLTF.i.throttlingCounter.enabled = false;
            PoolManager.enablePrewarm = false;

            ServiceLocator serviceLocator = BLD.ServiceLocatorTestFactory.CreateMocked();
            InitializeServices(serviceLocator);
            Environment.Setup(serviceLocator);
            yield break;
        }

        [UnityTearDown]
        protected virtual IEnumerator TearDown()
        {
            PoolManager.i?.Dispose();
            AssetPromiseKeeper_GLTF.i?.Cleanup();
            AssetPromiseKeeper_AB_GameObject.i?.Cleanup();
            AssetPromiseKeeper_AB.i?.Cleanup();
            AssetPromiseKeeper_Texture.i?.Cleanup();
            AssetPromiseKeeper_AudioClip.i?.Cleanup();
            AssetPromiseKeeper_Gif.i?.Cleanup();

            DataStore.Clear();

            yield return null;
            Environment.Dispose();
        }
    }
}