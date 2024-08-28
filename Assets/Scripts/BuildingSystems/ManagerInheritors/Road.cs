using System;
using System.Collections.Generic;
using BuildingSystems.Base;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = BuildingSystems.Base.Object;
using Random = UnityEngine.Random;

namespace BuildingSystems.ManagerInheritors
{
    public class Road : ObjectsManager
    {
        [SerializeField] private Tilemap roadTilemap;
        [SerializeField] private int roadIndexStart = 100;
        private Vector3Int _previousPosition;
        private List<Configuration.Road> _defaultRoadConfigurations;
        private List<Vector3Int> _roadPositions;
        private int _lastIndex;

        private event Action<Sprite> _setRoadSprite;

        protected override void Awake()
        {
            base.Awake();
            _currentObject = Instantiate(objectPrefab, Vector3.zero, Quaternion.identity, transform);
            SubcribeObject(_currentObject);
            _roadPositions = new List<Vector3Int>();
            _defaultRoadConfigurations = new List<Configuration.Road>();
            placer.InitializeFinished += Initialize;
        }

        public Vector3 GetRandomRoadPosition() =>
            placer.CellToWorld(_roadPositions[Random.Range(0, _roadPositions.Count)]);

        public override void SetCurrentBuild(IInteractable interactable)
        {
            OnSetNewSelected(null);
        }

        protected override void SubcribeObject(Object @object)
        {
            base.SubcribeObject(@object);
            ObjectInheritors.Road road = (ObjectInheritors.Road)@object;
            _setRoadSprite += road.SetRoadSprite;
        }

        private SO.Object FindGridObject(RuleTile ruleTile)
        {
            foreach (var @object in data.Builds)
            {
                if (@object.RenderObject.Equals(ruleTile))
                {
                    return @object;
                }
            }

            return null;
        }

        public void FindObjectWithPosition(Vector3 position)
        {
            var gridPosition = placer.WorldToCell(position);
            var alignPosition = placer.CellToWorld(gridPosition);
            var index = placer.TakeObjectIndexFromCell(gridPosition);
            var tile = roadTilemap.GetTile(gridPosition);
            var ruleTile = tile as RuleTile;
            var @object = data.Builds.Find(obj => (RuleTile)obj.RenderObject == ruleTile);
            SelectNewRoad(alignPosition, gridPosition, @object, index);
            _roadPositions.Remove(gridPosition);
        }

        private void SelectNewRoad(Vector3 alignPosition, Vector3Int gridPosition, SO.Object @object, int index,
            bool useView = true)
        {
            if (useView) OnSetNewSelected(null);
            _currentObject.SetConfiguration(@object, index);
            _currentObject.AlignToGrid(alignPosition);
            _currentObject.UpdatePosition(gridPosition);
            if (useView)
            {
                OnSetNewDragged(null);
                OnSetNewSelected(_currentObject);
                _setRoadSprite?.Invoke(roadTilemap.GetSprite(gridPosition));
            }
        }

        private void Initialize()
        {
            var gridSize = placer.GridSize;
            var cellIndex = 0;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    var cellPosition = new Vector3Int(x, y, 0);
                    var tile = roadTilemap.GetTile(cellPosition);
                    if (tile != null)
                    {
                        if (tile is RuleTile)
                        {
                            _lastIndex = roadIndexStart + cellIndex;
                            var ruleTile = tile as RuleTile;
                            var gridObject = FindGridObject(ruleTile);
                            _currentObject.SetConfiguration(gridObject, _lastIndex);
                            _currentObject.UpdatePosition(cellPosition);
                            Vector3 alignedPosition = placer.CellToWorld(cellPosition);
                            _currentObject.AlignToGrid(alignedPosition);
                            if (_currentObject.TryInstallObject(cellPosition))
                            {
                                _roadPositions.Add(cellPosition);
                                var configuration = new Configuration.Road(cellPosition, tile as RuleTile,
                                    gridObject, _lastIndex);
                                _defaultRoadConfigurations.Add(configuration);
                                cellIndex++;
                                Debug.Log($"Road installed index {_lastIndex}");
                            }
                            else
                            {
                                Debug.LogError($"Road installing failed for index {_lastIndex}");
                            }
                        }
                    }
                }
            }
        }

        public override void CreateObject(int index, Vector3 startPosition)
        {
            _currentObject.SetConfiguration(data.Builds[index], _lastIndex++);
            OnSetNewSelected(null);
            OnSetNewDragged(_currentObject);
        }


        public override void UpdateObjectPosition(Vector3 position)
        {
            var gridPosition = placer.WorldToCell(position);
            var alignedPosition = placer.CellToWorld(gridPosition);
            placer.ShowPlacer();
            OnSetNewSelected(null);
            OnSetNewDragged(_currentObject);
            _currentObject.UpdatePosition(gridPosition);
            _setRoadSprite?.Invoke(roadTilemap.GetSprite(gridPosition));
            _currentObject.AlignToGrid(alignedPosition);
            if (_previousPosition != gridPosition &&
                _roadPositions.Contains(_previousPosition) == false)
                roadTilemap.SetTile(_previousPosition, null);
            roadTilemap.SetTile(gridPosition, (RuleTile)data.Builds[0].RenderObject);
            _previousPosition = gridPosition;
        }

        public override bool TryInstallCurrentObject(Vector3 position)
        {
            var gridPosition = placer.WorldToCell(position);

            if (_currentObject.TryInstallObject(gridPosition))
            {
                OnSetNewDragged(null);
                OnSetNewSelected(_currentObject);
                placer.HidePlacer();
                _roadPositions.Add(gridPosition);
                return true;
            }

            return false;
        }

        public override void UninstallCurrentObject()
        {
            _currentObject.Remove();
            var gridPosition = placer.WorldToCell(_currentObject.transform.position);
            roadTilemap.SetTile(gridPosition, null);
            _lastIndex--;
        }

        public override void ResetToDefault()
        {
            foreach (var position in _roadPositions)
            {
                var alignPosition = placer.CellToWorld(position);
                var index = placer.TakeObjectIndexFromCell(position);
                var tile = roadTilemap.GetTile(position);
                var ruleTile = tile as RuleTile;
                var @object = data.Builds.Find(obj => (RuleTile)obj.RenderObject == ruleTile);
                SelectNewRoad(alignPosition, position, @object, index, false);
                _currentObject.Remove();
                roadTilemap.SetTile(position, null);
            }

            _roadPositions.Clear();
            foreach (var configuration in _defaultRoadConfigurations)
            {
                roadTilemap.SetTile(configuration.Position, configuration.Tile);
                _currentObject.SetConfiguration(configuration.Configuration, configuration.Index);
                _currentObject.UpdatePosition(configuration.Position);
                Vector3 alignedPosition = placer.CellToWorld(configuration.Position);
                _currentObject.AlignToGrid(alignedPosition);
                if (_currentObject.TryInstallObject(configuration.Position))
                {
                    _roadPositions.Add(configuration.Position);
                }

                _lastIndex = configuration.Index;
            }
        }
    }
}