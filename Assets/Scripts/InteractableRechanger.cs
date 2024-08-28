using BuildingSystems;

public class InteractableRechanger
{
    private IInteractable _currentSelectInteractable;
    private IInteractable _currentDragInteractable;

    public void SelectNewInteractable(IInteractable interactable)
    {
        if(interactable==_currentSelectInteractable)return;
        _currentSelectInteractable?.Deselect();
        _currentSelectInteractable = interactable;
        _currentSelectInteractable?.Select();
    }

    public void DragNewInteractable(IInteractable interactable)
    {
        if(interactable==_currentDragInteractable)return;
        _currentDragInteractable?.EndDrag();
        _currentDragInteractable = interactable;
        _currentDragInteractable?.StartDrag();
    }
}