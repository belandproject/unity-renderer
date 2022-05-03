using System.Collections;

namespace BLD.Huds.QuestsTracker
{
    public interface IQuestNotification
    {
        void Show();
        void Dispose();
        IEnumerator Waiter();
    }
}