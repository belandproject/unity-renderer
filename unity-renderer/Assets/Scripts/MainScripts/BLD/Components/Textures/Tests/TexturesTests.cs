using BLD;
using BLD.Helpers;
using BLD.Models;
using NUnit.Framework;
using System.Collections;
using BLD.Controllers;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TexturesTests : IntegrationTestSuite_Legacy
    {
        private ParcelScene scene;

        protected override IEnumerator SetUp()
        {
            yield return base.SetUp();
            scene = TestUtils.CreateTestScene();
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        [Explicit("Broke with Legacy suite refactor, fix later")]
        [Category("Explicit")]
        public IEnumerator TextureCreateAndLoadTest()
        {
            BLDTexture bldTexture = TestUtils.CreateBLDTexture(scene,
                TestAssetsUtils.GetPath() + "/Images/avatar.png",
                BLDTexture.BabylonWrapMode.CLAMP,
                FilterMode.Bilinear);

            yield return bldTexture.routine;

            Assert.IsTrue(bldTexture.texture != null, "Texture didn't load correctly?");
            Assert.IsTrue(bldTexture.unityWrap == TextureWrapMode.Clamp, "Bad wrap mode!");
            Assert.IsTrue(bldTexture.unitySamplingMode == FilterMode.Bilinear, "Bad sampling mode!");

            bldTexture.Dispose();

            yield return null;
            Assert.IsTrue(bldTexture.texture == null, "Texture didn't dispose correctly?");
        }

        [UnityTest]
        public IEnumerator TextureAttachedGetsReplacedOnNewAttachment()
        {
            yield return TestUtils.TestAttachedSharedComponentOfSameTypeIsReplaced<BLDTexture.Model, BLDTexture>(
                scene, CLASS_ID.TEXTURE);
        }

        [Test]
        public void Texture_OnReadyBeforeLoading()
        {
            BLDTexture bldTexture = TestUtils.CreateBLDTexture(scene, TestAssetsUtils.GetPath() + "/Images/avatar.png");
            bool isOnReady = false;
            bldTexture.CallWhenReady((x) => { isOnReady = true; });

            Assert.IsTrue(isOnReady); //BLDTexture is ready on creation
        }

        [UnityTest]
        public IEnumerator Texture_OnReadyWaitLoading()
        {
            BLDTexture bldTexture = TestUtils.CreateBLDTexture(scene, TestAssetsUtils.GetPath() + "/Images/avatar.png");
            bool isOnReady = false;
            bldTexture.CallWhenReady((x) => { isOnReady = true; });
            yield return bldTexture.routine;

            Assert.IsTrue(isOnReady);
        }

        [UnityTest]
        public IEnumerator Texture_OnReadyAfterLoadingInstantlyCalled()
        {
            BLDTexture bldTexture = TestUtils.CreateBLDTexture(scene, TestAssetsUtils.GetPath() + "/Images/avatar.png");
            yield return bldTexture.routine;

            bool isOnReady = false;
            bldTexture.CallWhenReady((x) => { isOnReady = true; });
            Assert.IsTrue(isOnReady);
        }
    }
}