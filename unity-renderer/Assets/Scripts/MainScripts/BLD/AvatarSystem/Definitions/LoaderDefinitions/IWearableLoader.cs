using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using BLD;
using UnityEngine;

namespace AvatarSystem
{
    public interface IWearableLoader : IDisposable
    {
        public enum Status
        {
            Idle,
            Succeeded,
            Defaulted,
            Failed
        }

        WearableItem wearable { get; }
        Rendereable rendereable { get; }
        Status status { get; }
        UniTask Load(GameObject container, AvatarSettings avatarSettings, CancellationToken ct = default);
    }
}