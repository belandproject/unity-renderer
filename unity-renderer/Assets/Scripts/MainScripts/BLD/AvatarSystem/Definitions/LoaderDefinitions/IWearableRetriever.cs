using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using BLD;
using UnityEngine;

namespace AvatarSystem
{
    public interface IWearableRetriever : IDisposable
    {
        Rendereable rendereable { get; }
        UniTask<Rendereable> Retrieve(GameObject container, ContentProvider contentProvider, string baseUrl, string mainFile, CancellationToken ct = default);
    }
}