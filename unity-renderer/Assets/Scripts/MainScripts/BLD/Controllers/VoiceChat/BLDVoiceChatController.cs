using UnityEngine;
using UnityEngine.Analytics;

namespace BLD
{
    public class BLDVoiceChatController : MonoBehaviour
    {
        [Header("InputActions")]
        public InputAction_Hold voiceChatAction;
        public InputAction_Trigger voiceChatToggleAction;

        private InputAction_Hold.Started voiceChatStartedDelegate;
        private InputAction_Hold.Finished voiceChatFinishedDelegate;
        private InputAction_Trigger.Triggered voiceChatToggleDelegate;

        private bool firstTimeVoiceRecorded = true;

        void Awake()
        {
            voiceChatStartedDelegate = (action) => StartVoiceChatRecording();
            voiceChatFinishedDelegate = (action) => BLD.Interface.WebInterface.SendSetVoiceChatRecording(false);
            voiceChatToggleDelegate = (action) => BLD.Interface.WebInterface.ToggleVoiceChatRecording();
            voiceChatAction.OnStarted += voiceChatStartedDelegate;
            voiceChatAction.OnFinished += voiceChatFinishedDelegate;
            voiceChatToggleAction.OnTriggered += voiceChatToggleDelegate;

            KernelConfig.i.EnsureConfigInitialized().Then(config => EnableVoiceChat(config.comms.voiceChatEnabled));
            KernelConfig.i.OnChange += OnKernelConfigChanged;
        }
        void OnDestroy()
        {
            voiceChatAction.OnStarted -= voiceChatStartedDelegate;
            voiceChatAction.OnFinished -= voiceChatFinishedDelegate;
            KernelConfig.i.OnChange -= OnKernelConfigChanged;
        }

        void OnKernelConfigChanged(KernelConfigModel current, KernelConfigModel previous) { EnableVoiceChat(current.comms.voiceChatEnabled); }

        void EnableVoiceChat(bool enable) { CommonScriptableObjects.voiceChatDisabled.Set(!enable); }

        private void StartVoiceChatRecording()
        {
            BLD.Interface.WebInterface.SendSetVoiceChatRecording(true);
            if (firstTimeVoiceRecorded)
            {
                AnalyticsHelper.SendVoiceChatStartedAnalytic();
                firstTimeVoiceRecorded = false;
            }
        }
    }
}