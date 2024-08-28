using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BuildingSystems.Base
{
    public class GridInitializer : MonoBehaviour
    {
        [SerializeField] private GridObjectPlacer gridObjectPlacer;
        [SerializeField] private Tilemap tilemapPlacer;
        [SerializeField] private Vector2Int gridSize;

        private void Awake()
        {
            gridObjectPlacer.Initialize(tilemapPlacer, gridSize, InitializeGrid());
        }

        private List<Vector3Int> InitializeGrid()
        {
            List<Vector3Int> allCells = new List<Vector3Int>();

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3Int cellPosition = new Vector3Int(x, y, 0);
                    allCells.Add(cellPosition);
                }
            }

            return allCells;
        }
    }
}