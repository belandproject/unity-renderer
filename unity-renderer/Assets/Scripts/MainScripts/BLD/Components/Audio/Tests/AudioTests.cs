using BLD.Components;
using BLD.Controllers;
using BLD.Helpers;
using BLD.Models;
using NUnit.Framework;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using BLD;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class AudioTests : IntegrationTestSuite_Legacy
    {
        private ParcelScene scene;
        private ISceneController sceneController => BLD.Environment.i.world.sceneController;

        [UnitySetUp]
        protected override IEnumerator SetUp()
        {
            yield return base.SetUp();
            scene = TestUtils.CreateTestScene();
            CommonScriptableObjects.rendererState.Set(true);
        }

        public BLDAudioClip CreateAudioClip(string url, bool loop, bool shouldTryToLoad, double volume)
        {
            BLDAudioClip.Model model = new BLDAudioClip.Model
            {
                url = url,
                loop = loop,
                shouldTryToLoad = shouldTryToLoad,
                volume = volume
            };

            return CreateAudioClip(model);
        }

        public BLDAudioClip CreateAudioClip(BLDAudioClip.Model model) { return TestUtils.SharedComponentCreate<BLDAudioClip, BLDAudioClip.Model>(scene, CLASS_ID.AUDIO_CLIP, model); }

        public IEnumerator CreateAndLoadAudioClip(bool waitForLoading = true)
        {
            var entity = TestUtils.CreateSceneEntity(scene);
            yield return null;

            yield return TestUtils.CreateAudioSourceWithClipForEntity(entity);

            BLDAudioSource bldAudioSource = entity.gameObject.GetComponentInChildren<BLDAudioSource>();
            AudioSource unityAudioSource = bldAudioSource.GetComponentInChildren<AudioSource>();

            Assert.IsTrue(scene.entities.ContainsKey(entity.entityId), "Entity was not created correctly!");
            Assert.IsTrue(bldAudioSource != null, "BLDAudioSource Creation Failure!");
            Assert.IsTrue(unityAudioSource != null, "Unity AudioSource Creation Failure!");

            yield return bldAudioSource.routine;

            Assert.IsTrue(unityAudioSource.isPlaying, "Audio Source is not playing when it should!");

            //NOTE(Brian): Stop test
            yield return TestUtils.CreateAudioSource(scene,
                entityId: entity.entityId,
                audioClipId: "audioClipTest",
                playing: false);

            yield return null;

            Assert.IsTrue(!unityAudioSource.isPlaying, "Audio Source is playing when it should NOT play!");
        }

        /// <summary>
        /// This should test creating a audioclip/audiosource couple, wait for audioClip load and send playing:true afterwards.
        /// </summary>
        [UnityTest]
        public IEnumerator CreateAndLoadAudioClipTest() { yield return CreateAndLoadAudioClip(waitForLoading: true); }

        /// <summary>
        /// This should test creating a audioclip/audiosource couple but send playing:true before the audioClip finished loading.
        /// </summary>
        [UnityTest]
        public IEnumerator PlayAudioTestWithoutFinishLoading() { yield return CreateAndLoadAudioClip(waitForLoading: false); }

        [UnityTest]
        public IEnumerator AudioComponentMissingValuesGetDefaultedOnUpdate() { yield return TestUtils.TestEntityComponentDefaultsOnUpdate<BLDAudioSource.Model, BLDAudioSource>(scene); }

        [UnityTest]
        public IEnumerator AudioClipMissingValuesGetDefaultedOnUpdate()
        {
            // 1. Create component with non-default configs
            BLDAudioClip.Model componentModel = new BLDAudioClip.Model
            {
                loop = true,
                shouldTryToLoad = false,
                volume = 0.8f
            };

            BLDAudioClip audioClip = CreateAudioClip(componentModel);

            yield return audioClip.routine;

            // 2. Check configured values
            Assert.IsTrue(audioClip.isLoop);
            Assert.IsFalse(audioClip.shouldTryLoad);
            Assert.AreEqual(0.8f, audioClip.volume);

            // 3. Update component with missing values
            componentModel = new BLDAudioClip.Model { };

            scene.SharedComponentUpdate(audioClip.id, JsonUtility.ToJson(componentModel));

            yield return audioClip.routine;

            // 4. Check defaulted values
            Assert.IsFalse(audioClip.isLoop);
            Assert.IsTrue(audioClip.shouldTryLoad);
            Assert.AreEqual(1f, audioClip.volume);
        }

        [UnityTest]
        public IEnumerator AudioIsLooped()
        {
            var entity = TestUtils.CreateSceneEntity(scene);
            yield return null;

            yield return TestUtils.LoadAudioClip(scene, "1", TestAssetsUtils.GetPath() + "/Audio/short_effect.ogg", false, true, 1);

            yield return TestUtils.CreateAudioSource(scene, entity.entityId, "1", true, loop: true);

            BLDAudioSource bldAudioSource = entity.components.Values.FirstOrDefault(x => x is BLDAudioSource) as BLDAudioSource;

            Assert.IsTrue(bldAudioSource.audioSource.loop);
        }

        [UnityTest]
        public IEnumerator AudioIsNotLooped()
        {
            var entity = TestUtils.CreateSceneEntity(scene);
            yield return null;

            yield return TestUtils.LoadAudioClip(scene, "1", TestAssetsUtils.GetPath() + "/Audio/short_effect.ogg", false, true, 1);

            yield return TestUtils.CreateAudioSource(scene, entity.entityId, "1", true, loop: false);

            BLDAudioSource bldAudioSource = entity.components.Values.FirstOrDefault(x => x is BLDAudioSource) as BLDAudioSource;
            bldAudioSource.audioSource.time = bldAudioSource.audioSource.clip.length - 0.05f;
            yield return new WaitForSeconds(0.1f);

            Assert.AreEqual(0, bldAudioSource.playTime);
        }

        [UnityTest]
        public IEnumerator AudioClipAttachedGetsReplacedOnNewAttachment()
        {
            yield return TestUtils.TestAttachedSharedComponentOfSameTypeIsReplaced<BLDAudioClip.Model, BLDAudioClip>(
                scene, CLASS_ID.AUDIO_CLIP);
        }

        [UnityTest]
        public IEnumerator VolumeWhenAudioCreatedWithNoUserInScene()
        {
            // We disable SceneController monobehaviour to avoid its current scene id update
            sceneController.enabled = false;

            // Set current scene as a different one
            CommonScriptableObjects.sceneID.Set("unexistent-scene");

            var entity = TestUtils.CreateSceneEntity(scene);
            yield return null;

            yield return TestUtils.CreateAudioSourceWithClipForEntity(entity);

            BLDAudioSource bldAudioSource = entity.gameObject.GetComponentInChildren<BLDAudioSource>();
            yield return bldAudioSource.routine;

            AudioSource unityAudioSource = bldAudioSource.GetComponentInChildren<AudioSource>();

            // Check the volume
            Assert.AreEqual(0f, unityAudioSource.volume);
        }

        [UnityTest]
        public IEnumerator VolumeWhenAudioCreatedWithUserInScene()
        {
            // We disable SceneController monobehaviour to avoid its current scene id update
            sceneController.enabled = false;

            // Set current scene with this scene's id
            CommonScriptableObjects.sceneID.Set(scene.sceneData.id);

            var entity = TestUtils.CreateSceneEntity(scene);
            yield return null;

            yield return TestUtils.CreateAudioSourceWithClipForEntity(entity);

            BLDAudioSource bldAudioSource = entity.gameObject.GetComponentInChildren<BLDAudioSource>();
            yield return bldAudioSource.routine;

            AudioSource unityAudioSource = bldAudioSource.GetComponentInChildren<AudioSource>();

            // Check the volume
            Assert.AreEqual(unityAudioSource.volume, bldAudioSource.volume);
        }

        [UnityTest]
        public IEnumerator VolumeIsMutedWhenUserLeavesScene()
        {
            // We disable SceneController monobehaviour to avoid its current scene id update
            sceneController.enabled = false;

            // Set current scene with this scene's id
            CommonScriptableObjects.sceneID.Set(scene.sceneData.id);

            var entity = TestUtils.CreateSceneEntity(scene);
            yield return null;

            yield return TestUtils.CreateAudioSourceWithClipForEntity(entity);

            BLDAudioSource bldAudioSource = entity.gameObject.GetComponentInChildren<BLDAudioSource>();
            yield return bldAudioSource.routine;

            AudioSource unityAudioSource = bldAudioSource.GetComponentInChildren<AudioSource>();

            // Set current scene as a different one
            CommonScriptableObjects.sceneID.Set("unexistent-scene");

            // Check the volume
            Assert.AreEqual(unityAudioSource.volume, 0f);
        }

        [UnityTest]
        public IEnumerator VolumeIsUnmutedWhenUserEntersScene()
        {
            // We disable SceneController monobehaviour to avoid its current scene id update
            sceneController.enabled = false;

            // Set current scene as a different one
            CommonScriptableObjects.sceneID.Set("unexistent-scene");

            var entity = TestUtils.CreateSceneEntity(scene);
            yield return null;

            yield return TestUtils.CreateAudioSourceWithClipForEntity(entity);

            BLDAudioSource bldAudioSource = entity.gameObject.GetComponentInChildren<BLDAudioSource>();
            yield return bldAudioSource.routine;

            AudioSource unityAudioSource = bldAudioSource.GetComponentInChildren<AudioSource>();

            // Set current scene with this scene's id
            CommonScriptableObjects.sceneID.Set(scene.sceneData.id);

            // Check the volume
            Assert.AreEqual(unityAudioSource.volume, bldAudioSource.volume);
        }

        [UnityTest]
        public IEnumerator AudioStreamComponentCreation()
        {
            var entity = TestUtils.CreateSceneEntity(scene);
            BLDAudioStream.Model model = new BLDAudioStream.Model()
            {
                url = "https://audio.bld.guru/radio/8110/radio.mp3",
                playing = false,
                volume = 1f
            };
            BLDAudioStream component = TestUtils.EntityComponentCreate<BLDAudioStream, BLDAudioStream.Model>(scene, entity, model );

            yield return component.routine;
            Assert.IsFalse(component.GetModel().playing);

            model.playing = true;
            component.UpdateFromModel(model);
            yield return component.routine;
            Assert.IsTrue(component.GetModel().playing);

            model.playing = false;
            component.UpdateFromModel(model);
            yield return component.routine;
            Assert.IsFalse(component.GetModel().playing);
        }

        [Test]
        public void AudioClip_OnReadyBeforeLoading()
        {
            BLDAudioClip bldAudioClip = CreateAudioClip(TestAssetsUtils.GetPath() + "/Audio/short_effect.ogg", true, true, 1);
            bool isOnReady = false;
            bldAudioClip.CallWhenReady((x) => { isOnReady = true; });

            Assert.IsTrue(isOnReady); //BLDAudioClip is ready on creation
        }

        [UnityTest]
        public IEnumerator AudioClip_OnReadyWaitLoading()
        {
            BLDAudioClip bldAudioClip = CreateAudioClip(TestAssetsUtils.GetPath() + "/Audio/short_effect.ogg", true, true, 1);
            bool isOnReady = false;
            bldAudioClip.CallWhenReady((x) => { isOnReady = true; });
            yield return bldAudioClip.routine;

            Assert.IsTrue(isOnReady);
        }

        [UnityTest]
        public IEnumerator AudioClip_OnReadyAfterLoadingInstantlyCalled()
        {
            BLDAudioClip bldAudioClip = CreateAudioClip(TestAssetsUtils.GetPath() + "/Audio/short_effect.ogg", true, true, 1);
            yield return bldAudioClip.routine;
            bool isOnReady = false;
            bldAudioClip.CallWhenReady((x) => { isOnReady = true; });

            Assert.IsTrue(isOnReady);
        }
    }
}