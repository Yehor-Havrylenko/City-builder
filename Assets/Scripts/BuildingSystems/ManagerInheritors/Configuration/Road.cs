using UnityEngine;
using Object = SO.Object;

namespace BuildingSystems.ManagerInheritors.Configuration
{
    public class Road : SavedDefaultConfiguration
    {
        public Road(Vector3Int position, RuleTile tile, Object configuration, int index) : base(position,
            configuration, index)
        {
            Tile = tile;
        }

        public RuleTile Tile { get; private set; }
    }
}