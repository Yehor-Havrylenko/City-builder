using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingSystems
{
    [Serializable]
    public class OccupancyData
    {
        public List<Vector2Int> occupiedCells = new();
    }
}