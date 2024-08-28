using System;
using BuildingSystems;
using BuildingSystems.Base;
using BuildingSystems.ManagerInheritors;
using UnityEngine;
using Object = BuildingSystems.Base.Object;

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private Road roadManager;
    [SerializeField] private Build buildManager;
    private ObjectsManager _currentManager;
    private InteractableRechanger _rechanger;
    public event Action CreateNewObjectEvent;

    private void Awake()
    {
        _rechanger = new InteractableRechanger();
        buildManager.SetNewDragged += _rechanger.DragNewInteractable;
        buildManager.SetNewSelected += _rechanger.SelectNewInteractable;
        roadManager.SetNewDragged += _rechanger.DragNewInteractable;
        roadManager.SetNewSelected += _rechanger.SelectNewInteractable;
    }

    public void ResetToDefault()
    {
        buildManager.ResetToDefault();
        roadManager.ResetToDefault();
    }

    public void CreateObject(Vector3 position, int index, PlacerType placerType)
    {
        _currentManager = placerType == PlacerType.Road ? roadManager : buildManager;
        _currentManager.CreateObject(index, position);
        OnCreateNewObjectEvent();
    }

    public void SelectObject(IInteractable interactable)
    {
        var @object = interactable as Object;
        if (interactable != null) _currentManager = @object.MyPlacer == PlacerType.Road ? roadManager : buildManager;
        if (_currentManager != null)
        {
            _currentManager.SetCurrentBuild(interactable);
        }
    }

    public void SelectObject(Vector3 position)
    {
        roadManager.FindObjectWithPosition(position);
    }
    public bool TryInstallObject(Vector3 position) =>_currentManager != null && _currentManager.TryInstallCurrentObject(position);

    public void UpdateSelectedObjectPosition(Vector3 position) => _currentManager.UpdateObjectPosition(position);

    private void OnCreateNewObjectEvent() => CreateNewObjectEvent?.Invoke();
}