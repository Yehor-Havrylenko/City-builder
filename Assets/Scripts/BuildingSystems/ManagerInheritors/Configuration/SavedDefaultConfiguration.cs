using UnityEngine;
using Object = SO.Object;

namespace BuildingSystems.ManagerInheritors.Configuration
{
    public class SavedDefaultConfiguration
    {
        public Vector3Int Position { get; private set; }
        public Object Configuration { get; private set; }
        public int Index { get; private set; }

        public SavedDefaultConfiguration(Vector3Int position, Object configuration, int index)
        {
            Position = position;
            Configuration = configuration;
            Index = index;
        }
    }
}