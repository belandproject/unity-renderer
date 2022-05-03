using BLD.Components;
using BLD.Models;

namespace MainScripts.BLD.WorldRuntime
{
    public interface ISceneMessageProcessor
    {
        IBLDEntity CreateEntity(string id);
        void RemoveEntity(string id, bool removeImmediatelyFromEntitiesList = true);
        void SetEntityParent(string entityId, string parentId);
        void SharedComponentAttach(string entityId, string id);
        IEntityComponent EntityComponentCreateOrUpdateWithModel(string entityId, CLASS_ID_COMPONENT classId, object data);
        IEntityComponent EntityComponentCreateOrUpdate(string entityId, CLASS_ID_COMPONENT classId, string data) ;
        IEntityComponent EntityComponentUpdate(IBLDEntity entity, CLASS_ID_COMPONENT classId,
            string componentJson);
        ISharedComponent SharedComponentCreate(string id, int classId);
        void SharedComponentDispose(string id);
        void EntityComponentRemove(string entityId, string name);
        ISharedComponent SharedComponentUpdate(string id, string json);
    }
}