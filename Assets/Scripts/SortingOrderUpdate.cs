using UnityEngine;

public class SortingOrderUpdate : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Update()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.z * 100);
    }
}