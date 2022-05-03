using System;
using BLD.Models;
using UnityEngine;

namespace BLD
{
    public interface ISceneMetricsCounter : IDisposable
    {
        event System.Action<ISceneMetricsCounter> OnMetricsUpdated;
        SceneMetricsModel maxCount { get; }
        SceneMetricsModel currentCount { get; }

        void Enable();

        void Disable();

        void SendEvent();

        bool IsInsideTheLimits();

        void Configure(string sceneId, Vector2Int scenePosition, int sceneParcelCount);
    }
}