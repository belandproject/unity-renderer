using System.Collections.Generic;

namespace BLD
{
    public interface IMessageQueueHandler
    {
        void EnqueueSceneMessage(QueuedSceneMessage_Scene message);
        Queue<QueuedSceneMessage_Scene> sceneMessagesPool { get; }
    }
}