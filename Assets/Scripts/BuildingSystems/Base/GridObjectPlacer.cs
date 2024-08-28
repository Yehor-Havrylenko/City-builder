using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BuildingSystems.Base
{
    public sealed class GridObjectPlacer : MonoBehaviour
    {
        [SerializeField] private TileBase freeTile;
        [SerializeField] private TileBase canBeOccupiedTile;
        [SerializeField] private TileBase occupiedTile;
        [SerializeField] private Material gridMaterial;
        private Dictionary<int, List<Vector3Int>> _occupiedCellsRegistry = new Dictionary<int, List<Vector3Int>>();
        private List<Vector3Int> _freeCells = new List<Vector3Int>();
        private Tilemap _tilemap;
        private Vector2Int _gridSize;
        private Color32 _gridShowColor;
        private Color32 _gridHideColor;
        private Color32 _currentGridColor;
        private MaterialPropertyBlock _materialPropertyBlock;
        public event Action InitializeFinished;
        public Vector2Int GridSize => _gridSize;

        public void Initialize(Tilemap tilemap, Vector2Int gridSize, List<Vector3Int> allCells)
        {
            _tilemap = tilemap;
            _gridSize = gridSize;
            _freeCells = allCells;
            _gridShowColor = new Color32(255, 255, 255, 255);
            _gridHideColor = new Color32(255, 255, 255, 0);
            HidePlacer();
            InitializeFinished?.Invoke();
        }

        private enum CellStatus
        {
            Free,
            CanBeOccupied,
            Occupied
        }

        public Vector3Int WorldToCell(Vector3 worldPosition) => _tilemap.WorldToCell(worldPosition);
        public Vector3 CellToWorld(Vector3Int cellPosition) => _tilemap.CellToWorld(cellPosition);

        public int TakeObjectIndexFromCell(Vector3Int cellPosition)
        {
            var list = new List<Vector3Int> { cellPosition };
            return FindKeyByValue(list);
        }

        private int FindKeyByValue(List<Vector3Int> targetList)
        {
            var result = _occupiedCellsRegistry.FirstOrDefault(x => x.Value.SequenceEqual(targetList));

            if (!EqualityComparer<int>.Default.Equals(result.Key, default(int)) ||
                _occupiedCellsRegistry.ContainsKey(result.Key))
            {
                return result.Key;
            }

            return -1;
        }

        public bool CanPlaceObject(Vector3Int position, List<Vector2Int> occupiedCells)
        {
            foreach (var cell in occupiedCells)
            {
                Vector3Int cellPosition = position + new Vector3Int(cell.x, cell.y, 0);
                bool isCellWithinGrid = !IsCellWithinGrid(cellPosition);
                bool isCellOccupied = IsCellOccupied(cellPosition);
                if (isCellWithinGrid || isCellOccupied)
                {
                    return false;
                }
            }

            return true;
        }

        public void PlaceObject(int buildId, Vector3Int position, List<Vector2Int> occupiedCells)
        {
            if (CanPlaceObject(position, occupiedCells))
            {
                if (_occupiedCellsRegistry.ContainsKey(buildId))
                {
                    RemoveObject(buildId);
                }

                List<Vector3Int> newOccupiedCells = new List<Vector3Int>();

                foreach (var cell in occupiedCells)
                {
                    Vector3Int cellPosition = position + new Vector3Int(cell.x, cell.y, 0);
                    SetCellStatus(cellPosition, CellStatus.Occupied);
                    newOccupiedCells.Add(cellPosition);
                }

                _occupiedCellsRegistry[buildId] = newOccupiedCells;
                _freeCells.RemoveAll(cell => newOccupiedCells.Contains(cell));
            }
            else
            {
                Debug.LogWarning("Cannot place object: Some tiles are already occupied or out of bounds.");
            }
        }

        public void ShowPlacer()
        {
            if (_currentGridColor.Equals(_gridShowColor)) return;
            _currentGridColor = _gridShowColor;
            gridMaterial.DOColor(_gridShowColor, 0.2f);
        }

        public void HidePlacer()
        {
            if (_currentGridColor.Equals(_gridHideColor)) return;
            _currentGridColor = _gridHideColor;
            gridMaterial.DOColor(_gridHideColor, 0.2f);
        }

        public void RemoveObject(int buildId)
        {
            if (_occupiedCellsRegistry.ContainsKey(buildId))
            {
                foreach (var cell in _occupiedCellsRegistry[buildId])
                {
                    SetCellStatus(cell, CellStatus.Free);
                    _freeCells.Add(cell);
                }

                _occupiedCellsRegistry.Remove(buildId);
            }
        }

        private bool IsCellOccupied(Vector3Int position)
        {
            return _occupiedCellsRegistry.Values.Any(cells => cells.Contains(position));
        }

        private void SetCellStatus(Vector3Int position, CellStatus status)
        {
            switch (status)
            {
                case CellStatus.Free:
                    _tilemap.SetTile(position, freeTile);
                    break;
                case CellStatus.CanBeOccupied:
                    _tilemap.SetTile(position, canBeOccupiedTile);
                    break;
                case CellStatus.Occupied:
                    _tilemap.SetTile(position, occupiedTile);
                    break;
            }
        }

        private bool IsCellWithinGrid(Vector3Int position)
        {
            return position.x >= 0 && position.x < _gridSize.x && position.y >= 0 && position.y < _gridSize.y &&
                   position.z == 0;
        }

        public void HighlightCanBeOccupiedArea(Vector3Int position, List<Vector2Int> occupiedCells)
        {
            ResetHighlight();

            foreach (var cell in occupiedCells)
            {
                Vector3Int cellPosition = position + new Vector3Int(cell.x, cell.y, 0);
                if (IsCellWithinGrid(cellPosition))
                {
                    if (!IsCellOccupied(cellPosition))
                    {
                        SetCellStatus(cellPosition, CellStatus.CanBeOccupied);
                        _freeCells.Add(cellPosition);
                    }
                }
            }
        }


        public void ResetHighlight()
        {
            foreach (var cell in _freeCells)
            {
                SetCellStatus(cell, CellStatus.Free);
            }

            foreach (var occupiedCell in _occupiedCellsRegistry.Values.SelectMany(cells => cells))
            {
                SetCellStatus(occupiedCell, CellStatus.Occupied);
            }

            _freeCells.Clear();
        }
    }
}