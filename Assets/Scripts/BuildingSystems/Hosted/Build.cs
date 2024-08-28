using System;
using UnityEngine;

namespace BuildingSystems.Hosted
{
    [Serializable]
    public class Build : HostedBase
    {
        [field: SerializeField] public Base.Object Object { get; private set; }

        public Vector3Int GridPosition => Vector3Int.RoundToInt(Object.transform.position);
    }
}