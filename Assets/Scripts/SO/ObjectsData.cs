using System.Collections.Generic;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "ListObjects", menuName = "Grid objects/objects", order = 0)]
    public class ObjectsData : ScriptableObject
    {
        [field: SerializeField] public List<Object> Builds { get; private set; }
    }
}