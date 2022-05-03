using System.Collections.Generic;
using UnityEngine;

namespace BLD
{
    public interface ISceneHandler
    {
        HashSet<Vector2Int> GetAllLoadedScenesCoords();
    }
}