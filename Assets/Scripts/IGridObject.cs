using System.Collections.Generic;
using UnityEngine;

public interface IGridObject
{
    Object GetRenderObject();
    List<Vector2Int> GetOccupiedCells();
}