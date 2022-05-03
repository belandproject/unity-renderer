using BLD.Helpers;
using System.Collections;
using BLD.Controllers;
using UnityEngine;
using BLD.Models;
using BLD.SettingsCommon;
using AudioSettings = BLD.SettingsCommon.AudioSettings;

namespace BLD.Components
{
    public class BLDAudioSource : BaseComponent, IOutOfSceneBoundariesHandler
    {
        [System.Serializable]
        public class Model : BaseModel
        {
            public string audioClipId;
            public bool playing = false;
            public float volume = 1f;
            public bool loop = false;
            public float pitch = 1f;
            public long playedAtTimestamp = 0;

            public override BaseModel GetDataFromJSON(string json) { return Utils.SafeFromJson<Model>(json); }
        }

        public float playTime => audioSource.time;
        internal AudioSource audioSource;
        BLDAudioClip lastBLDAudioClip;

        private bool isDestroyed = false;
        public long playedAtTimestamp = 0;
        private bool isOutOfBoundaries = false;

        private void Awake()
        {
            audioSource = gameObject.GetOrCreateComponent<AudioSource>();
            model = new Model();

            Settings.i.audioSettings.OnChanged += OnAudioSettingsChanged;
            DataStore.i.virtualAudioMixer.sceneSFXVolume.OnChange += OnVirtualAudioMixerChangedValue;
        }

        public void InitBLDAudioClip(BLDAudioClip bldAudioClip)
        {
            if (lastBLDAudioClip != null)
            {
                lastBLDAudioClip.OnLoadingFinished -= DclAudioClip_OnLoadingFinished;
            }

            lastBLDAudioClip = bldAudioClip;
        }

        public double volume => ((Model) model).volume;

        public override IEnumerator ApplyChanges(BaseModel baseModel)
        {
            yield return new WaitUntil(() => CommonScriptableObjects.rendererState.Get());

            //If the scene creates and destroy an audiosource before our renderer has been turned on bad things happen!
            //TODO: Analyze if we can catch this upstream and stop the IEnumerator
            if (isDestroyed)
                yield break;

            CommonScriptableObjects.sceneID.OnChange -= OnCurrentSceneChanged;
            CommonScriptableObjects.sceneID.OnChange += OnCurrentSceneChanged;

            ApplyCurrentModel();

            yield return null;
        }

        private void ApplyCurrentModel()
        {
            if (audioSource == null)
            {
                Debug.LogWarning("AudioSource is null!.");
                return;
            }

            Model model = (Model) this.model;
            UpdateAudioSourceVolume();
            audioSource.loop = model.loop;
            audioSource.pitch = model.pitch;
            audioSource.spatialBlend = 1;
            audioSource.dopplerLevel = 0.1f;

            if (model.playing)
            {
                BLDAudioClip bldAudioClip = scene.GetSharedComponent(model.audioClipId) as BLDAudioClip;

                if (bldAudioClip != null)
                {
                    InitBLDAudioClip(bldAudioClip);
                    //NOTE(Brian): Play if finished loading, otherwise will wait for the loading to complete (or fail).
                    if (bldAudioClip.loadingState == BLDAudioClip.LoadState.LOADING_COMPLETED)
                    {
                        ApplyLoadedAudioClip(bldAudioClip);
                    }
                    else
                    {
                        bldAudioClip.OnLoadingFinished -= DclAudioClip_OnLoadingFinished;
                        bldAudioClip.OnLoadingFinished += DclAudioClip_OnLoadingFinished;
                    }
                }
                else
                {
                    Debug.LogError("Wrong audio clip type when playing audiosource!!");
                }
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }

        private void OnAudioSettingsChanged(AudioSettings settings)
        {
            UpdateAudioSourceVolume();
        }

        private void OnVirtualAudioMixerChangedValue(float currentValue, float previousValue)
        {
            UpdateAudioSourceVolume();
        }

        private void UpdateAudioSourceVolume()
        {
            AudioSettings audioSettingsData = Settings.i.audioSettings.Data;
            float newVolume = ((Model)model).volume * Utils.ToVolumeCurve(DataStore.i.virtualAudioMixer.sceneSFXVolume.Get() * audioSettingsData.sceneSFXVolume * audioSettingsData.masterVolume);

            if (scene is GlobalScene globalScene && globalScene.isPortableExperience)
            {
                audioSource.volume = newVolume;
                return;
            }

            if (isOutOfBoundaries)
            {
                audioSource.volume = 0;
                return;
            }

            audioSource.volume = scene.sceneData.id == CommonScriptableObjects.sceneID.Get() ? newVolume : 0f;
        }

        private void OnCurrentSceneChanged(string currentSceneId, string previousSceneId)
        {
            if (audioSource != null)
            {
                Model model = (Model) this.model;
                float volume = 0;
                if ((scene.sceneData.id == currentSceneId) || (scene is GlobalScene globalScene && globalScene.isPortableExperience))
                {
                    volume = model.volume;
                }

                audioSource.volume = volume;
            }
        }

        private void OnDestroy()
        {
            isDestroyed = true;
            CommonScriptableObjects.sceneID.OnChange -= OnCurrentSceneChanged;

            //NOTE(Brian): Unsuscribe events.
            InitBLDAudioClip(null);

            Settings.i.audioSettings.OnChanged -= OnAudioSettingsChanged;
            DataStore.i.virtualAudioMixer.sceneSFXVolume.OnChange -= OnVirtualAudioMixerChangedValue;
        }

        public void UpdateOutOfBoundariesState(bool isEnabled)
        {
            isOutOfBoundaries = !isEnabled;
            UpdateAudioSourceVolume();
        }

        private void DclAudioClip_OnLoadingFinished(BLDAudioClip obj)
        {
            if (obj.loadingState == BLDAudioClip.LoadState.LOADING_COMPLETED && audioSource != null)
            {
                ApplyLoadedAudioClip(obj);
            }

            obj.OnLoadingFinished -= DclAudioClip_OnLoadingFinished;
        }

        private void ApplyLoadedAudioClip(BLDAudioClip clip)
        {
            if (audioSource.clip != clip.audioClip)
            {
                audioSource.clip = clip.audioClip;
            }

            Model model = (Model) this.model;
            bool shouldPlay = playedAtTimestamp != model.playedAtTimestamp ||
                              (model.playing && !audioSource.isPlaying);

            if (audioSource.enabled && model.playing && shouldPlay)
            {
                //To remove a pesky and quite unlikely warning when the audiosource is out of scenebounds
                audioSource.Play();
            }

            playedAtTimestamp = model.playedAtTimestamp;
        }

        public override int GetClassId() { return (int) CLASS_ID_COMPONENT.AUDIO_SOURCE; }
    }
}