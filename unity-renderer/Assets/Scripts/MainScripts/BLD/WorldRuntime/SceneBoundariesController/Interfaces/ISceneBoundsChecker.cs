using System;
using System.Collections.Generic;
using BLD.Models;
using UnityEngine;

namespace BLD.Controllers
{
    public interface ISceneBoundsChecker : IService
    {
        event Action<IBLDEntity, bool> OnEntityBoundsCheckerStatusChanged;

        float timeBetweenChecks { get; set; }
        bool enabled { get; }
        int entitiesToCheckCount { get; }
        int highPrioEntitiesToCheckCount { get; }
        void SetFeedbackStyle(ISceneBoundsFeedbackStyle feedbackStyle);
        ISceneBoundsFeedbackStyle GetFeedbackStyle();
        List<Material> GetOriginalMaterials(MeshesInfo meshesInfo);
        void Start();
        void Stop();
        void AddEntityToBeChecked(IBLDEntity entity);

        /// <summary>
        /// Add an entity that will be consistently checked, until manually removed from the list.
        /// </summary>
        void AddPersistent(IBLDEntity entity);

        /// <summary>
        /// Returns whether an entity was added to be consistently checked
        /// </summary>
        ///
        bool WasAddedAsPersistent(IBLDEntity entity);

        void RemoveEntityToBeChecked(IBLDEntity entity);
        void EvaluateEntityPosition(IBLDEntity entity);
        bool IsEntityInsideSceneBoundaries(IBLDEntity entity);
    }
}