using System.Collections.Generic;
using BuildingSystems.Base;
using BuildingSystems.ManagerInheritors.Configuration;
using NPC;
using UnityEngine;
using UnityEngine.Serialization;
using Object = BuildingSystems.Base.Object;

namespace BuildingSystems.ManagerInheritors
{
    public sealed class Build : ObjectsManager
    {
        [SerializeField] private List<Hosted.Build> hostedBuilds;
        [SerializeField] private List<Object> objects;
        [SerializeField] private Creator npcCreator;
        private List<SavedDefaultConfiguration> _defaultConfigurations;

        protected override void Awake()
        {
            base.Awake();
            _defaultConfigurations = new List<SavedDefaultConfiguration>();
            placer.InitializeFinished += Initialize;
        }

        private void Initialize()
        {
            foreach (var hostedObject in hostedBuilds)
            {
                hostedObject.Object.SetConfiguration(hostedObject.Configuration);
                var position = hostedObject.GridPosition;
                position.z = 0;
                Vector3Int gridPosition = placer.WorldToCell(position);
                Vector3 alignedPosition = placer.CellToWorld(gridPosition);
                _defaultConfigurations.Add(new SavedDefaultConfiguration(position, hostedObject.Configuration, 0));
                _instances.Add(hostedObject.Object);
                InstallHostedObject(hostedObject.Object, gridPosition, alignedPosition);
                npcCreator.Spawn(hostedObject.Configuration.EmployeesCount);
            }
        }

        public override void CreateObject(int index, Vector3 startPosition)
        {
            var position = placer.WorldToCell(startPosition);
            var gridObject = data.Builds[index];
            if (_currentObject != null)
            {
                Destroy(_currentObject);
            }

            _currentObject = Instantiate(objectPrefab, position, Quaternion.identity, transform);
            _currentObject.SetConfiguration(gridObject);
            npcCreator.Spawn(gridObject.EmployeesCount);
            _instances.Add(_currentObject);
            SubcribeObject(_currentObject);
            OnSetNewSelected(null);
            OnSetNewDragged(_currentObject);
        }

        public override void UpdateObjectPosition(Vector3 position)
        {
            var gridPosition = placer.WorldToCell(position);
            var alignedPosition = placer.CellToWorld(gridPosition);
            if (_currentObject != null)
            {
                placer.ShowPlacer();
                OnSetNewSelected(null);
                OnSetNewDragged(_currentObject);
                _currentObject.UpdatePosition(gridPosition);
                _currentObject.AlignToGrid(alignedPosition);
            }
        }

        public override bool TryInstallCurrentObject(Vector3 position)
        {
            var gridPosition = placer.WorldToCell(position);
            if (_currentObject != null)
            {
                if (_currentObject.TryInstallObject(gridPosition))
                {
                    OnSetNewDragged(null);
                    OnSetNewSelected(_currentObject);
                    _currentObject = null;
                    placer.HidePlacer();
                    return true;
                }
            }

            return false;
        }

        public override void UninstallCurrentObject()
        {
            _currentObject.Remove();
            if (_instances.Contains(_currentObject))
            {
                _instances.Remove(_currentObject);
            }

            Destroy(_currentObject);
            _currentObject = null;
        }

        public override void ResetToDefault()
        {
            foreach (var instance in _instances)
            {
                instance.Remove();
                Destroy(instance.gameObject);
            }

            npcCreator.RemoveAllCharacters();
            _instances.Clear();
            foreach (var configuration in _defaultConfigurations)
            {
                _currentObject = Instantiate(objectPrefab, configuration.Position, Quaternion.identity, transform);
                _currentObject.SetConfiguration(configuration.Configuration);
                _instances.Add(_currentObject);
                SubcribeObject(_currentObject);
                var position = configuration.Position;
                position.z = 0;
                Vector3Int gridPosition = placer.WorldToCell(position);
                Vector3 alignedPosition = placer.CellToWorld(gridPosition);
                InstallHostedObject(_currentObject, gridPosition, alignedPosition);
                npcCreator.Spawn(configuration.Configuration.EmployeesCount);
            }
        }

        private void InstallHostedObject(Object @object, Vector3Int gridPosition, Vector3 alignedPosition)
        {
            if (@object != null)
            {
                SubcribeObject(@object);
                @object.UpdatePosition(gridPosition);
                @object.AlignToGrid(alignedPosition);
                if (@object.TryInstallObject(gridPosition))
                {
                    Debug.Log("Success install hosted Object");
                }
                else
                {
                    Debug.LogError("Failed to install hosted object");
                }
            }
        }
    }
}