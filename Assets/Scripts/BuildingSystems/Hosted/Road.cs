using System;
using UnityEngine;

namespace BuildingSystems.Hosted
{
    [Serializable]
    public class Road : HostedBase
    {
        [field: SerializeField] public Vector3Int GridPosition { get; private set; }
    }
}