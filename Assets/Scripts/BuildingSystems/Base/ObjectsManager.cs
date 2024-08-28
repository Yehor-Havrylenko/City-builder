using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SO;
using UnityEngine;

namespace BuildingSystems.Base
{
    public abstract class ObjectsManager : MonoBehaviour
    {
        [SerializeField] protected Object objectPrefab;
        [SerializeField] protected GridObjectPlacer placer;
        [SerializeField] protected ObjectsData data;
        protected Object _currentObject;
        protected List<Object> _instances;
        public event Action<IInteractable> SetNewSelected;
        public event Action<IInteractable> SetNewDragged;


        protected virtual void Awake()
        {
            _instances = new List<Object>();
        }

        public abstract void CreateObject(int index, Vector3 startPosition);

        protected virtual void SubcribeObject(Object @object)
        {
            @object.CanPlaceBuilding += placer.CanPlaceObject;
            @object.HighlightCanBeOccupiedArea += placer.HighlightCanBeOccupiedArea;
            @object.ResetHighligth += placer.ResetHighlight;
            @object.PlaceObject += placer.PlaceObject;
            @object.RemoveObject += placer.RemoveObject;
        }

        public abstract void UpdateObjectPosition(Vector3 position);

        public virtual void SetCurrentBuild([CanBeNull] IInteractable interactable)
        {
            _currentObject = interactable as Object;
            OnSetNewSelected(interactable);
        }

        public abstract bool TryInstallCurrentObject(Vector3 position);
        public abstract void UninstallCurrentObject();
        public abstract void ResetToDefault();

        protected virtual void OnSetNewSelected(IInteractable obj)
        {
            SetNewSelected?.Invoke(obj);
        }

        protected virtual void OnSetNewDragged(IInteractable obj)
        {
            SetNewDragged?.Invoke(obj);
        }
    }
}