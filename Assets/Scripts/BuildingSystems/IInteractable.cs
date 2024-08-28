namespace BuildingSystems
{
    public interface IInteractable
    {
        void Select();
        void Deselect();
        void StartDrag();
        void EndDrag();
    }
}