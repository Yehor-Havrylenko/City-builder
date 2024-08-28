using BuildingSystems;
using BuildingSystems.ManagerInheritors;
using UnityEngine;

public class BuildInteractionController : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    private Camera _mainCamera;
    private bool _isDragging;
    private bool _isNewObject;
    private bool _isSelected;
    private IInteractable _currentInteractable;

    private float _mouseHoldTime;
    private const float DragThreshold = 0.2f;

    private void Start()
    {
        _mainCamera = Camera.main;
        gameController.CreateNewObjectEvent += NewObject;
    }

    private void NewObject()
    {
        _isNewObject = true;
    }

    private void Update()
    {
        if (_isNewObject)
        {
            HandleMouseDrag();
            _isDragging = true;
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseRelease();
            }
        }
        else
        {
            if (_isDragging == false)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    HandleMouseClick();
                    _mouseHoldTime = 0f;
                }

                if (Input.GetMouseButton(0))
                {
                    _mouseHoldTime += Time.deltaTime;

                    if (!_isDragging && _mouseHoldTime >= DragThreshold && _isSelected)
                    {
                        _isDragging = true;
                    }
                }
            }
            else
            {
                HandleMouseDrag();
                if (Input.GetMouseButtonUp(0))
                {
                    HandleMouseRelease();
                }
            }
        }
    }

    private void HandleMouseClick()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            IInteractable clickedInteractable = hit.collider.GetComponentInParent<IInteractable>();
            if (clickedInteractable != null && clickedInteractable != _currentInteractable)
            {
                gameController.SelectObject(clickedInteractable);
                _isSelected = true;
                _isDragging = false;
                _currentInteractable = clickedInteractable;
            }
            else if (clickedInteractable == null)
            {
                if (hit.collider.TryGetComponent<Road>(out _) ||
                    hit.collider.TryGetComponent<BuildingSystems.ObjectInheritors.Road>(out _))
                {
                    gameController.SelectObject(hit.point);
                }
            }
        }
        else
        {
            gameController.SelectObject(null);
            _currentInteractable = null;
            _isSelected = false;
        }
    }

    private void HandleMouseDrag()
    {
        Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        gameController.UpdateSelectedObjectPosition(mousePosition);
    }

    private void HandleMouseRelease()
    {
        if (_isDragging)
        {
            Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            if (gameController.TryInstallObject(mousePosition))
            {
                _isDragging = false;
                _isNewObject = false;
            }
        }
    }
}