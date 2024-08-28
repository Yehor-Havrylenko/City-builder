using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingSystems.Base
{
    public abstract class Object : MonoBehaviour, IInteractable
    {
        [SerializeField] protected SpriteRenderer renderer;
        [SerializeField] private ObjectAnimation animation;
        [SerializeField] private PlacerType myPlacer;
        private Vector3Int _lastGridPosition;
        private IGridObject _gridObject;
        private bool _isPlaced;
        private bool _isDragging;
        protected int _id;

        public PlacerType MyPlacer => myPlacer;
        public event Func<Vector3Int, List<Vector2Int>, bool> CanPlaceBuilding;
        public event Action<Vector3Int, List<Vector2Int>> HighlightCanBeOccupiedArea;
        public event Action<int, Vector3Int, List<Vector2Int>> PlaceObject;
        public event Action<int> RemoveObject;
        public event Action ResetHighligth;

        public virtual void SetConfiguration(IGridObject gridObject, int id = 0)
        {
            _gridObject = gridObject;
        }

        void IInteractable.Select()
        {
            if (!_isPlaced) return;

            animation.PlaySelect();
        }

        void IInteractable.Deselect()
        {
            if (!_isPlaced) return;

            animation.StopSelect();
        }

        void IInteractable.StartDrag()
        {
            if (_isPlaced)
            {
                OnRemoveObject(_id);
                _isDragging = true;
                animation.StopSelect();
                animation.PlayDrag();
            }
        }

        void IInteractable.EndDrag()
        {
            if (_isDragging)
            {
                _isDragging = false;
                animation.StopDrag();
                animation.PlaySelect();
            }
        }

        public void Remove()
        {
            OnRemoveObject(_id);
        }
        public void UpdatePosition(Vector3Int gridPosition)
        {
            if (!_isPlaced || _isDragging)
            {
                if (gridPosition != _lastGridPosition)
                {
                    _lastGridPosition = gridPosition;
                    UpdateHighlight(gridPosition);
                }
            }
        }

        public bool TryInstallObject(Vector3Int gridPosition)
        {
            if (OnCanPlaceBuilding(gridPosition, _gridObject.GetOccupiedCells()))
            {
                _isPlaced = true;
                OnResetHighligth();
                OnPlaceObject(gridPosition, _gridObject.GetOccupiedCells(), _id);
                return true;
            }

            return false;
        }

        public void AlignToGrid(Vector3 alignedPosition)
        {
            transform.position = alignedPosition;
        }

        private void UpdateHighlight(Vector3Int gridPosition)
        {
            OnResetHighligth();
            OnHighlightCanBeOccupiedArea(gridPosition, _gridObject.GetOccupiedCells());
        }

        protected virtual bool OnCanPlaceBuilding(Vector3Int arg1, List<Vector2Int> arg2)
        {
            return CanPlaceBuilding?.Invoke(arg1, arg2) ?? true;
        }

        protected virtual void OnResetHighligth()
        {
            ResetHighligth?.Invoke();
        }

        protected virtual void OnHighlightCanBeOccupiedArea(Vector3Int arg1, List<Vector2Int> arg2)
        {
            HighlightCanBeOccupiedArea?.Invoke(arg1, arg2);
        }

        protected virtual void OnPlaceObject(Vector3Int arg1, List<Vector2Int> arg2, int id)
        {
            PlaceObject?.Invoke(id, arg1, arg2);
        }

        protected virtual void OnRemoveObject(int obj)
        {
            RemoveObject?.Invoke(obj);
        }
    }
}