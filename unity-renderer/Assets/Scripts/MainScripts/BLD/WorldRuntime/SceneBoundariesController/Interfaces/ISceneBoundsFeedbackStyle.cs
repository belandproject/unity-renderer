using System.Collections.Generic;
using BLD.Models;
using UnityEngine;

namespace BLD.Controllers
{
    public interface ISceneBoundsFeedbackStyle
    {
        void CleanFeedback();
        void ApplyFeedback(BLD.Models.MeshesInfo meshesInfo, bool isInsideBoundaries);
        List<Material> GetOriginalMaterials(BLD.Models.MeshesInfo meshesInfo);
    }
}