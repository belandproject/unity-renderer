using System;
using BLD;
using BLD.Helpers;
using BLD.Components;
using BLD.Models;
using NUnit.Framework;
using System.Collections;
using BLD.Components.Video.Plugin;
using UnityEngine;
using UnityEngine.TestTools;
using BLD.Controllers;
using BLD.Interface;
using BLD.SettingsCommon;
using NSubstitute;
using UnityEngine.Assertions;
using Assert = UnityEngine.Assertions.Assert;
using AudioSettings = BLD.SettingsCommon.AudioSettings;

namespace Tests
{
    public class VideoTextureShould : IntegrationTestSuite_Legacy
    {
        private Func<IVideoPluginWrapper> originalVideoPluginBuilder;

        private ISceneController sceneController => BLD.Environment.i.world.sceneController;
        private ParcelScene scene;

        protected override IEnumerator SetUp()
        {
            yield return base.SetUp();

            scene = TestUtils.CreateTestScene();
            IVideoPluginWrapper pluginWrapper = new VideoPluginWrapper_Mock();
            originalVideoPluginBuilder = BLDVideoTexture.videoPluginWrapperBuilder;
            BLDVideoTexture.videoPluginWrapperBuilder = () => pluginWrapper;
        }

        protected override IEnumerator TearDown()
        {
            BLDVideoTexture.videoPluginWrapperBuilder = originalVideoPluginBuilder;
            sceneController.enabled = true;
            return base.TearDown();
        }

        [UnityTest]
        public IEnumerator BeCreatedCorrectly()
        {
            BLDVideoTexture videoTexture = CreateBLDVideoTexture(scene, "it-wont-load-during-test");
            yield return videoTexture.routine;
            Assert.IsTrue(videoTexture.attachedMaterials.Count == 0, "BLDVideoTexture started with attachedMaterials != 0");
        }

        [UnityTest]
        public IEnumerator SendMessageWhenVideoPlays()
        {
            var id = CreateBLDVideoClip(scene, "http://it-wont-load-during-test").id;
            BLDVideoTexture.Model model = new BLDVideoTexture.Model()
            {
                videoClipId = id,
                playing = true,
                seek = 10
            };
            var component = CreateBLDVideoTextureWithCustomTextureModel(scene, model);
            yield return component.routine;

            var expectedEvent = new WebInterface.SendVideoProgressEvent()
            {
                sceneId = scene.sceneData.id,
                componentId = component.id,
                videoLength = 0,
                videoTextureId = id,
                currentOffset = 0,
                status = (int)VideoState.LOADING
            };

            var json = JsonUtility.ToJson(expectedEvent);
            var wasEventSent = false;
            yield return TestUtils.WaitForMessageFromEngine("VideoProgressEvent", json,
                () => { },
                () => wasEventSent = true);

            Assert.IsTrue(wasEventSent, $"Event of type {expectedEvent.GetType()} was not sent or its incorrect.");
        }

        [UnityTest]
        public IEnumerator SendMessageWhenVideoStops()
        {
            var id = CreateBLDVideoClip(scene, "http://it-wont-load-during-test").id;
            BLDVideoTexture.Model model = new BLDVideoTexture.Model()
            {
                videoClipId = id,
                playing = false
            };
            var component = CreateBLDVideoTextureWithCustomTextureModel(scene, model);

            var expectedEvent = new WebInterface.SendVideoProgressEvent()
            {
                sceneId = scene.sceneData.id,
                componentId = component.id,
                videoLength = 0,
                videoTextureId = id,
                currentOffset = 0,
                status = (int)VideoState.LOADING
            };

            var json = JsonUtility.ToJson(expectedEvent);
            var wasEventSent = false;
            yield return TestUtils.WaitForMessageFromEngine("VideoProgressEvent", json,
                () => { },
                () => wasEventSent = true);
            yield return component.routine;

            Assert.IsTrue(wasEventSent, $"Event of type {expectedEvent.GetType()} was not sent or its incorrect.");
        }

        [UnityTest]
        public IEnumerator SendMessageWhenVideoIsUpdatedAfterTime()
        {
            var id = CreateBLDVideoClip(scene, "http://it-wont-load-during-test").id;
            BLDVideoTexture.Model model = new BLDVideoTexture.Model()
            {
                videoClipId = id,
                playing = true
            };
            var component = CreateBLDVideoTextureWithCustomTextureModel(scene, model);
            yield return component.routine;

            var expectedEvent = new WebInterface.SendVideoProgressEvent()
            {
                sceneId = scene.sceneData.id,
                componentId = component.id,
                videoLength = 0,
                videoTextureId = id,
                currentOffset = 0,
                status = (int)VideoState.LOADING
            };

            var json = JsonUtility.ToJson(expectedEvent);
            var wasEventSent = false;
            yield return TestUtils.WaitForMessageFromEngine("VideoProgressEvent", json,
                () => { },
                () => wasEventSent = true);

            Assert.IsTrue(wasEventSent, $"Event of type {expectedEvent.GetType()} was not sent or its incorrect.");
        }

        [UnityTest]
        public IEnumerator VideoTextureReplaceOtherTextureCorrectly()
        {
            BLDVideoTexture videoTexture = CreateBLDVideoTexture(scene, "it-wont-load-during-test");
            yield return videoTexture.routine;
            Assert.IsTrue(videoTexture.attachedMaterials.Count == 0, "BLDVideoTexture started with attachedMaterials != 0");

            BLDTexture bldTexture = TestUtils.CreateBLDTexture(scene, TestAssetsUtils.GetPath() + "/Images/atlas.png");

            yield return bldTexture.routine;

            BasicMaterial mat = TestUtils.SharedComponentCreate<BasicMaterial, BasicMaterial.Model>
            (scene, CLASS_ID.BASIC_MATERIAL,
                new BasicMaterial.Model
                {
                    texture = bldTexture.id
                });

            yield return mat.routine;

            yield return TestUtils.SharedComponentUpdate(mat, new BasicMaterial.Model() { texture = videoTexture.id });

            Assert.IsTrue(videoTexture.attachedMaterials.Count == 1, $"did BLDVideoTexture attach to material? {videoTexture.attachedMaterials.Count} expected 1");
        }

        [UnityTest]
        public IEnumerator AttachAndDetachCorrectly()
        {
            BLDVideoTexture videoTexture = CreateBLDVideoTexture(scene, "it-wont-load-during-test");
            yield return videoTexture.routine;
            Assert.IsTrue(videoTexture.attachedMaterials.Count == 0, "BLDVideoTexture started with attachedMaterials != 0");

            BasicMaterial mat2 = TestUtils.SharedComponentCreate<BasicMaterial, BasicMaterial.Model>
            (
                scene,
                CLASS_ID.BASIC_MATERIAL,
                new BasicMaterial.Model
                {
                    texture = videoTexture.id
                }
            );

            yield return mat2.routine;

            Assert.IsTrue(videoTexture.attachedMaterials.Count == 1, $"did BLDVideoTexture attach to material? {videoTexture.attachedMaterials.Count} expected 1");

            // TEST: BLDVideoTexture detach on material disposed
            mat2.Dispose();
            Assert.IsTrue(videoTexture.attachedMaterials.Count == 0, $"did BLDVideoTexture detach from material? {videoTexture.attachedMaterials.Count} expected 0");

            videoTexture.Dispose();

            yield return null;
            Assert.IsTrue(videoTexture.texture == null, "BLDVideoTexture didn't dispose correctly?");
        }

        [UnityTest]
        public IEnumerator SetVisibleStateCorrectlyWhenAddedToAMaterialNotAttachedToShape()
        {
            BLDVideoTexture videoTexture = CreateBLDVideoTexture(scene, "it-wont-load-during-test");
            yield return videoTexture.routine;

            var ent1 = TestUtils.CreateSceneEntity(scene);
            BasicMaterial ent1Mat = TestUtils.SharedComponentCreate<BasicMaterial, BasicMaterial.Model>(scene, CLASS_ID.BASIC_MATERIAL, new BasicMaterial.Model() { texture = videoTexture.id });
            TestUtils.SharedComponentAttach(ent1Mat, ent1);
            yield return ent1Mat.routine;

            Assert.IsTrue(!videoTexture.isVisible, "BLDVideoTexture should not be visible without a shape");
        }

        [UnityTest]
        public IEnumerator SetVisibleStateCorrectly()
        {
            BLDVideoTexture videoTexture = CreateBLDVideoTexture(scene, "it-wont-load-during-test");
            yield return videoTexture.routine;

            var ent1 = TestUtils.CreateSceneEntity(scene);
            BasicMaterial ent1Mat = TestUtils.SharedComponentCreate<BasicMaterial, BasicMaterial.Model>(scene, CLASS_ID.BASIC_MATERIAL, new BasicMaterial.Model() { texture = videoTexture.id });
            TestUtils.SharedComponentAttach(ent1Mat, ent1);
            yield return ent1Mat.routine;

            BoxShape ent1Shape = TestUtils.SharedComponentCreate<BoxShape, BoxShape.Model>(scene, CLASS_ID.BOX_SHAPE, new BoxShape.Model());
            yield return ent1Shape.routine;

            TestUtils.SharedComponentAttach(ent1Shape, ent1);
            yield return new WaitForAllMessagesProcessed();
            Assert.IsTrue(videoTexture.isVisible, "BLDVideoTexture should be visible");

            yield return TestUtils.SharedComponentUpdate<BoxShape, BoxShape.Model>(ent1Shape, new BoxShape.Model() { visible = false });
            yield return new WaitForAllMessagesProcessed();

            Assert.IsTrue(!videoTexture.isVisible, "BLDVideoTexture should not be visible ");
        }

        [UnityTest]
        public IEnumerator SetVisibleStateCorrectlyWhenAddedToAlreadyAttachedMaterial()
        {
            BLDVideoTexture videoTexture = CreateBLDVideoTexture(scene, "it-wont-load-during-test");
            yield return videoTexture.routine;

            var ent1 = TestUtils.CreateSceneEntity(scene);
            BasicMaterial ent1Mat = TestUtils.SharedComponentCreate<BasicMaterial, BasicMaterial.Model>(scene, CLASS_ID.BASIC_MATERIAL, new BasicMaterial.Model());
            TestUtils.SharedComponentAttach(ent1Mat, ent1);
            yield return ent1Mat.routine;

            BoxShape ent1Shape = TestUtils.SharedComponentCreate<BoxShape, BoxShape.Model>(scene, CLASS_ID.BOX_SHAPE, new BoxShape.Model());
            yield return ent1Shape.routine;

            TestUtils.SharedComponentAttach(ent1Shape, ent1);

            yield return TestUtils.SharedComponentUpdate<BasicMaterial, BasicMaterial.Model>(ent1Mat, new BasicMaterial.Model() { texture = videoTexture.id });
            yield return new WaitForAllMessagesProcessed();
            Assert.IsTrue(videoTexture.isVisible, "BLDVideoTexture should be visible");

            yield return TestUtils.SharedComponentUpdate<BoxShape, BoxShape.Model>(ent1Shape, new BoxShape.Model() { visible = false });
            yield return new WaitForAllMessagesProcessed();

            Assert.IsTrue(!videoTexture.isVisible, "BLDVideoTexture should not be visible ");
        }

        [UnityTest]
        public IEnumerator SetVisibleStateCorrectlyWhenEntityIsRemoved()
        {
            BLDVideoTexture videoTexture = CreateBLDVideoTexture(scene, "it-wont-load-during-test");
            yield return videoTexture.routine;

            var ent1 = TestUtils.CreateSceneEntity(scene);
            BasicMaterial ent1Mat = TestUtils.SharedComponentCreate<BasicMaterial, BasicMaterial.Model>(scene, CLASS_ID.BASIC_MATERIAL, new BasicMaterial.Model() { texture = videoTexture.id });
            TestUtils.SharedComponentAttach(ent1Mat, ent1);
            yield return ent1Mat.routine;

            BoxShape ent1Shape = TestUtils.SharedComponentCreate<BoxShape, BoxShape.Model>(scene, CLASS_ID.BOX_SHAPE, new BoxShape.Model());
            yield return ent1Shape.routine;

            TestUtils.SharedComponentAttach(ent1Shape, ent1);
            yield return new WaitForAllMessagesProcessed();
            Assert.IsTrue(videoTexture.isVisible, "BLDVideoTexture should be visible");

            scene.RemoveEntity(ent1.entityId, true);
            yield return new WaitForAllMessagesProcessed();

            Assert.IsTrue(!videoTexture.isVisible, "BLDVideoTexture should not be visible ");
        }

        [UnityTest]
        public IEnumerator MuteWhenCreatedAndNoUserIsInTheScene()
        {
            // We disable SceneController monobehaviour to avoid its current scene id update
            sceneController.enabled = false;

            // Set current scene as a different one
            CommonScriptableObjects.sceneID.Set("non-existent-scene");

            BLDVideoTexture videoTexture = CreateBLDVideoTexture(scene, "it-wont-load-during-test");
            yield return videoTexture.routine;

            var ent1 = TestUtils.CreateSceneEntity(scene);
            BasicMaterial ent1Mat = TestUtils.SharedComponentCreate<BasicMaterial, BasicMaterial.Model>(scene, CLASS_ID.BASIC_MATERIAL, new BasicMaterial.Model() { texture = videoTexture.id });
            TestUtils.SharedComponentAttach(ent1Mat, ent1);
            yield return ent1Mat.routine;

            BoxShape ent1Shape = TestUtils.SharedComponentCreate<BoxShape, BoxShape.Model>(scene, CLASS_ID.BOX_SHAPE, new BoxShape.Model());
            yield return ent1Shape.routine;

            TestUtils.SharedComponentAttach(ent1Shape, ent1);
            yield return new WaitForAllMessagesProcessed();

            // Check the volume
            Assert.AreEqual(0f, videoTexture.texturePlayer.volume);
        }

        [UnityTest]
        public IEnumerator UpdateTexturePlayerVolumeWhenAudioSettingsChange()
        {
            var id = CreateBLDVideoClip(scene, "http://it-wont-load-during-test").id;

            BLDVideoTexture.Model model = new BLDVideoTexture.Model()
            {
                videoClipId = id,
                playing = true,
                volume = 1
            };
            var component = CreateBLDVideoTextureWithCustomTextureModel(scene, model);

            yield return component.routine;

            Assert.AreApproximatelyEqual(1f, component.texturePlayer.volume, 0.01f);

            AudioSettings settings = Settings.i.audioSettings.Data;
            settings.sceneSFXVolume = 0.5f;
            Settings.i.audioSettings.Apply(settings);

            var expectedVolume = Utils.ToVolumeCurve(0.5f);
            Assert.AreApproximatelyEqual(expectedVolume, component.texturePlayer.volume, 0.01f);

            settings.sceneSFXVolume = 1f;
            Settings.i.audioSettings.Apply(settings);

            Assert.AreApproximatelyEqual(1, component.texturePlayer.volume, 0.01f);

            BLDVideoTexture.videoPluginWrapperBuilder = originalVideoPluginBuilder;
        }


        [UnityTest]
        public IEnumerator UnmuteWhenVideoIsCreatedWithUserInScene()
        {
            // We disable SceneController monobehaviour to avoid its current scene id update
            sceneController.enabled = false;

            // Set current scene with this scene's id
            CommonScriptableObjects.sceneID.Set(scene.sceneData.id);

            BLDVideoTexture videoTexture = CreateBLDVideoTexture(scene, "it-wont-load-during-test");
            yield return videoTexture.routine;

            var ent1 = TestUtils.CreateSceneEntity(scene);
            BasicMaterial ent1Mat = TestUtils.SharedComponentCreate<BasicMaterial, BasicMaterial.Model>(scene, CLASS_ID.BASIC_MATERIAL, new BasicMaterial.Model() { texture = videoTexture.id });
            TestUtils.SharedComponentAttach(ent1Mat, ent1);
            yield return ent1Mat.routine;

            BoxShape ent1Shape = TestUtils.SharedComponentCreate<BoxShape, BoxShape.Model>(scene, CLASS_ID.BOX_SHAPE, new BoxShape.Model());
            yield return ent1Shape.routine;

            TestUtils.SharedComponentAttach(ent1Shape, ent1);
            yield return new WaitForAllMessagesProcessed();

            // Check the volume
            Assert.AreEqual(videoTexture.GetVolume(), videoTexture.texturePlayer.volume);
        }

        [UnityTest]
        public IEnumerator MuteWhenUserLeavesScene()
        {
            // We disable SceneController monobehaviour to avoid its current scene id update
            sceneController.enabled = false;

            // Set current scene with this scene's id
            CommonScriptableObjects.sceneID.Set(scene.sceneData.id);
            yield return null;

            BLDVideoTexture videoTexture = CreateBLDVideoTexture(scene, "it-wont-load-during-test");
            yield return videoTexture.routine;

            var ent1 = TestUtils.CreateSceneEntity(scene);
            BasicMaterial ent1Mat = TestUtils.SharedComponentCreate<BasicMaterial, BasicMaterial.Model>(scene, CLASS_ID.BASIC_MATERIAL, new BasicMaterial.Model() { texture = videoTexture.id });
            TestUtils.SharedComponentAttach(ent1Mat, ent1);
            yield return ent1Mat.routine;

            BoxShape ent1Shape = TestUtils.SharedComponentCreate<BoxShape, BoxShape.Model>(scene, CLASS_ID.BOX_SHAPE, new BoxShape.Model());
            yield return ent1Shape.routine;

            TestUtils.SharedComponentAttach(ent1Shape, ent1);
            yield return new WaitForAllMessagesProcessed();

            // Set current scene as a different one
            CommonScriptableObjects.sceneID.Set("unexistent-scene");

            // to force the video player to update its volume
            CommonScriptableObjects.playerCoords.Set(new Vector2Int(666, 666));

            yield return null;

            // Check the volume
            Assert.AreEqual(0f, videoTexture.texturePlayer.volume);
        }

        [UnityTest]
        public IEnumerator VolumeIsUnmutedWhenUserEntersScene()
        {
            // We disable SceneController monobehaviour to avoid its current scene id update
            sceneController.enabled = false;

            // Set current scene as a different one
            CommonScriptableObjects.sceneID.Set("unexistent-scene");

            BLDVideoTexture videoTexture = CreateBLDVideoTexture(scene, "it-wont-load-during-test");
            yield return videoTexture.routine;

            var ent1 = TestUtils.CreateSceneEntity(scene);
            BasicMaterial ent1Mat = TestUtils.SharedComponentCreate<BasicMaterial, BasicMaterial.Model>(scene, CLASS_ID.BASIC_MATERIAL, new BasicMaterial.Model() { texture = videoTexture.id });
            TestUtils.SharedComponentAttach(ent1Mat, ent1);
            yield return ent1Mat.routine;

            BoxShape ent1Shape = TestUtils.SharedComponentCreate<BoxShape, BoxShape.Model>(scene, CLASS_ID.BOX_SHAPE, new BoxShape.Model());
            yield return ent1Shape.routine;

            TestUtils.SharedComponentAttach(ent1Shape, ent1);
            yield return new WaitForAllMessagesProcessed();

            // Set current scene with this scene's id
            CommonScriptableObjects.sceneID.Set(scene.sceneData.id);

            // to force the video player to update its volume
            CommonScriptableObjects.playerCoords.Set(new Vector2Int(666, 666));

            yield return null;

            // Check the volume
            Assert.AreEqual(videoTexture.GetVolume(), videoTexture.texturePlayer.volume);
        }

        static BLDVideoClip CreateBLDVideoClip(ParcelScene scn, string url)
        {
            return TestUtils.SharedComponentCreate<BLDVideoClip, BLDVideoClip.Model>
            (
                scn,
                BLD.Models.CLASS_ID.VIDEO_CLIP,
                new BLDVideoClip.Model
                {
                    url = url
                }
            );
        }

        static BLDVideoTexture CreateBLDVideoTexture(ParcelScene scn, BLDVideoClip clip)
        {
            return TestUtils.SharedComponentCreate<BLDVideoTexture, BLDVideoTexture.Model>
            (
                scn,
                BLD.Models.CLASS_ID.VIDEO_TEXTURE,
                new BLDVideoTexture.Model
                {
                    videoClipId = clip.id
                }
            );
        }

        static BLDVideoTexture CreateBLDVideoTextureWithModel(ParcelScene scn, BLDVideoTexture.Model model)
        {
            return TestUtils.SharedComponentCreate<BLDVideoTexture, BLDVideoTexture.Model>
            (
                scn,
                CLASS_ID.VIDEO_TEXTURE,
                model
            );
        }

        static BLDVideoTexture CreateBLDVideoTexture(ParcelScene scn, string url) { return CreateBLDVideoTexture(scn, CreateBLDVideoClip(scn, "http://" + url)); }
        static BLDVideoTexture CreateBLDVideoTextureWithCustomTextureModel(ParcelScene scn, BLDVideoTexture.Model model) { return CreateBLDVideoTextureWithModel(scn, model); }
    }
}