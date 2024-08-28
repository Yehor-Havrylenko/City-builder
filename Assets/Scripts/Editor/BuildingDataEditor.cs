using BuildingSystems;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(BuildingData))]
public class BuildingDataEditor : Editor
{
    private BuildingData buildingData;
    private bool editMode = false;
    private Tilemap tilemap;

    private void OnEnable()
    {
        buildingData = (BuildingData)target;
        tilemap = buildingData.GetComponent<Tilemap>();

        if (tilemap == null)
        {
            Debug.LogError("Tilemap component not found on the GameObject.");
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button(editMode ? "Stop Editing" : "Edit Occupied Cells"))
        {
            editMode = !editMode;
            SceneView.RepaintAll();
        }

        if (editMode)
        {
            GUILayout.Label("Click on the tilemap in Scene view to select occupied cells.");
        }
    }

    private void OnSceneGUI()
    {
        if (!editMode || tilemap == null) return;

        Event e = Event.current;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            Vector3 mousePosition = Event.current.mousePosition;
            mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
            Ray ray = SceneView.currentDrawingSceneView.camera.ScreenPointToRay(mousePosition);

            Plane plane = new Plane(tilemap.transform.forward, tilemap.transform.position);
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 localHit = tilemap.transform.InverseTransformPoint(hitPoint);
                Vector3Int cellPosition = tilemap.LocalToCell(localHit);

                Vector2Int cell = new Vector2Int(cellPosition.x, cellPosition.y);

                if (!buildingData.occupancyData.occupiedCells.Contains(cell))
                {
                    buildingData.occupancyData.occupiedCells.Add(cell);
                }
                else
                {
                    buildingData.occupancyData.occupiedCells.Remove(cell);
                }

                e.Use();
                SceneView.RepaintAll();
            }
        }

        Handles.color = Color.green;
        foreach (var cell in buildingData.occupancyData.occupiedCells)
        {
            Vector3 worldPos = tilemap.CellToWorld(new Vector3Int(cell.x, cell.y, 0));
            Vector3[] verts = new Vector3[4];

            verts[0] = worldPos + tilemap.CellToLocalInterpolated(new Vector3(0, 0, 0));
            verts[1] = worldPos + tilemap.CellToLocalInterpolated(new Vector3(1, 0, 0));
            verts[2] = worldPos + tilemap.CellToLocalInterpolated(new Vector3(1, 1, 0));
            verts[3] = worldPos + tilemap.CellToLocalInterpolated(new Vector3(0, 1, 0));

            Handles.DrawSolidRectangleWithOutline(verts, new Color(0, 1, 0, 0.25f), Color.green);
        }
    }
}
