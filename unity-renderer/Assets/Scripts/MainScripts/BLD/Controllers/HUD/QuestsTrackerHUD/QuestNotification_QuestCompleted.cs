using System.Collections;
using BLD.Huds.QuestsTracker;
using TMPro;
using UnityEngine;

namespace BLD.Huds.QuestsTracker
{
    public class QuestNotification_QuestCompleted : MonoBehaviour, IQuestNotification
    {
        [SerializeField] internal TextMeshProUGUI questName;
        [SerializeField] internal AudioEvent questCompleteAudioEvent;

        public void Populate(QuestModel questModel) { questName.text = questModel.name; }

        public void Show()
        {
            gameObject.SetActive(true);
            questCompleteAudioEvent.Play();
        }

        public void Dispose() { Destroy(gameObject); }
        public IEnumerator Waiter() { yield return WaitForSecondsCache.Get(QuestsNotificationsController.DEFAULT_NOTIFICATION_DURATION); }
    }
}