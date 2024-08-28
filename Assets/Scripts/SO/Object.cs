using System.Collections.Generic;
using BuildingSystems;
using UnityEngine;

namespace SO
{
    // тут можно было бы и сделать абстрактным классом и наследники Road и Builds
    // но я решил что более целесообразно будет оставить так потому что плодить ради пары не решающих строк будет не очень)
    [CreateAssetMenu(menuName = "Create Object", fileName = "Object", order = 0)]
    public class Object : ScriptableObject, IGridObject
    {
        [SerializeField] private string name;
        [SerializeField] private UnityEngine.Object renderObject;
        [SerializeField] private OccupancyData occupancy;
        [field: SerializeField] public int EmployeesCount { get; private set; }


        public string Name => name;
        public UnityEngine.Object RenderObject => renderObject;
        public OccupancyData Occupancy => occupancy;

        public UnityEngine.Object GetRenderObject() => renderObject;

        public List<Vector2Int> GetOccupiedCells() => occupancy.occupiedCells;
    }
}