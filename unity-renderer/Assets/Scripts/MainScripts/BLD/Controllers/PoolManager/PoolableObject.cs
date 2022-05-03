using System.Collections.Generic;
using BLD.Components;
using UnityEngine;

namespace BLD
{
    public class PoolableObject : IPoolableObject
    {
        public Pool pool;
        public LinkedListNode<PoolableObject> node;
        public GameObject gameObject { get; }

        public bool isInsidePool { get { return node != null; } }

        public event System.Action OnGet;
        public event System.Action OnRelease;

        private IPoolLifecycleHandler[] lifecycleHandlers = null;

        public PoolableObject(Pool poolOwner, GameObject go)
        {
            this.gameObject = go;
            this.pool = poolOwner;

            if (pool.useLifecycleHandlers)
                lifecycleHandlers = gameObject.GetComponents<IPoolLifecycleHandler>();
        }

        public void OnPoolGet()
        {
            if (lifecycleHandlers != null)
            {
                for (var i = 0; i < lifecycleHandlers.Length; i++)
                {
                    var handler = lifecycleHandlers[i];
                    handler.OnPoolGet();
                }
            }

            OnGet?.Invoke();
        }

        public void OnPoolRelease()
        {
            if (lifecycleHandlers != null)
            {
                for (var i = 0; i < lifecycleHandlers.Length; i++)
                {
                    var handler = lifecycleHandlers[i];
                    handler.OnPoolRelease();
                }
            }

            OnRelease?.Invoke();
        }

        public void Release()
        {
            if (this == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Release == null??! This shouldn't happen");
#endif
                return;
            }

            if (pool != null)
            {
                pool.Release(this);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("Pool is null upon release!");
#endif
            }

            OnPoolRelease();
        }

        public void RemoveFromPool() { pool.RemoveFromPool(this); }

        public void OnCleanup(BLD.ICleanableEventDispatcher sender)
        {
            sender.OnCleanupEvent -= this.OnCleanup;
            Release();
        }
    }
}