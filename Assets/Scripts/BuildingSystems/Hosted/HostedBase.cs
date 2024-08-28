using System;
using UnityEngine;

namespace BuildingSystems.Hosted
{
    [Serializable]
    public class HostedBase
    {
        [field: SerializeField] public SO.Object Configuration { get; private set; }
    }
}