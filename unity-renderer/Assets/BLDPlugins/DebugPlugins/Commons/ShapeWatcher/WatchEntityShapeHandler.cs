using BLD.Models;

namespace BLDPlugins.DebugPlugins.Commons
{
    public class WatchEntityShapeHandler
    {
        private readonly IBLDEntity entity;
        private readonly IShapeListener listener;

        public WatchEntityShapeHandler(IBLDEntity entity, IShapeListener listener)
        {
            this.entity = entity;
            this.listener = listener;

            if (entity.meshesInfo.currentShape != null)
            {
                OnShapeUpdated(entity);
            }

            entity.OnMeshesInfoUpdated += OnShapeUpdated;
            entity.OnMeshesInfoCleaned += OnShapeCleaned;
        }

        public void Dispose()
        {
            entity.OnMeshesInfoUpdated -= OnShapeUpdated;
            entity.OnMeshesInfoCleaned -= OnShapeCleaned;

            listener.Dispose();
        }

        private void OnShapeUpdated(IBLDEntity entity)
        {
            if (entity.meshesInfo.currentShape != null
                && entity.meshesInfo.meshRootGameObject != null)
            {
                listener.OnShapeUpdated(entity);
            }
        }

        private void OnShapeCleaned(IBLDEntity entity)
        {
            listener.OnShapeCleaned(entity);
        }
    }
}