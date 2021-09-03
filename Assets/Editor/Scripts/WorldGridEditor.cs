using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldGrid))]
public class WorldGridEditor
    : Editor
{
    private WorldGrid _worldGrid;
    private EEditMode _editMode;
    private GridPath _path;

    public enum EEditMode
    {
        None,
        Towers,
        Path
    }

    public override void OnInspectorGUI()
    {
        _worldGrid = (WorldGrid)target;

        EditorGUI.BeginChangeCheck();
        var gridSize = EditorGUILayout.Vector2IntField("Grid Size", _worldGrid.GridSize);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_worldGrid, "Change Grid Size");
            _worldGrid.GridSize = gridSize;
        }

        EditorGUI.BeginChangeCheck();
        var gridOffset = EditorGUILayout.Vector2IntField("Grid Offset", _worldGrid.GridOffset);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_worldGrid, "Change Grid Offset");
            _worldGrid.GridOffset = gridOffset;
        }

        _editMode = (EEditMode)EditorGUILayout.EnumPopup("Edit Mode", _editMode);

        if (_editMode == EEditMode.Path)
            HandlePaths();
    }

    private void HandlePaths()
    {
        GUILayout.BeginVertical("OL box flat");
        _worldGrid.Paths ??= new List<GridPath>();
        foreach (var path in _worldGrid.Paths)
        {
            GUI.contentColor = _path == path ? Color.white : new Color(0.8f, 0.8f, 0.8f);
            GUI.backgroundColor = _path == path ? Color.white : new Color(0.8f, 0.8f, 0.8f);
            if (GUILayout.Button(path.Name, new GUIStyle("LODBlackBox") { contentOffset = new Vector2(8, 0), alignment = TextAnchor.MiddleLeft }, GUILayout.Height(20)))
                _path = path;
        }

        GUI.contentColor = Color.white;

        GUILayout.EndVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Path", EditorStyles.miniButton, GUILayout.Width(100)))
        {
            Undo.RecordObject(_worldGrid, "Add Path");
            _worldGrid.Paths.Add(new GridPath("New Path"));
        }

        GUI.backgroundColor = new Color(1f, 0.25f, 0.25f);
        if (_path != null)
        {
            if (GUILayout.Button("Delete Path", EditorStyles.miniButton, GUILayout.Width(100)))
            {
                Undo.RecordObject(_worldGrid, "Delete Path");
                _worldGrid.Paths.Remove(_path);
                _path = null;
            }
        }

        GUI.backgroundColor = Color.white;

        GUILayout.EndHorizontal();

        if (_path?.Path != null)
        {
            GUILayout.Label(_path.Path.Count.ToString());
            foreach (var point in _path.Path)
            {
                GUILayout.Label(point.Index.ToString());
            }
        }
    }

    private void OnSceneGUI()
    {
        if (_editMode == EEditMode.None)
            return;

        var e = Event.current;
        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(0);
            return;
        }

        var didHit = RaycastGrid(out var hit);
        GridPoint gridPoint = null;

        if (didHit)
            gridPoint = _worldGrid.WorldPointToGridPoint(hit.point);

        switch (_editMode)
        {
            case EEditMode.Towers:
                if (e.type == EventType.MouseDown && e.button == 0 && didHit)
                    gridPoint.TowerPlacement = gridPoint.TowerPlacement == ETowerPlacement.Allowed ? ETowerPlacement.Blocked : ETowerPlacement.Allowed;
                break;
            case EEditMode.Path:
                Tools.current = Tool.None;
                if (e.type == EventType.MouseDown && e.button == 0 && didHit)
                    AddPathPoint(gridPoint);
                if (_path != null)
                    DrawPath(gridPoint);
                break;
        }


        if (e.type != EventType.MouseMove && e.type != EventType.MouseDown)
            return;

        e.Use();
    }

    private bool RaycastGrid(out RaycastHit hit)
    {
        var e = Event.current;
        var scene = SceneView.currentDrawingSceneView;
        Vector3 mousePos = e.mousePosition;
        var ppp = EditorGUIUtility.pixelsPerPoint;
        mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
        mousePos.x *= ppp;
        var ray = scene.camera.ScreenPointToRay(mousePos);

        return Physics.Raycast(ray, out hit);
    }

    private void DrawPath(GridPoint hovered)
    {
        if (_path?.Path == null || _path.Path.Count <= 0)
            return;

        int hoveredIndex = -1;
        if (hovered != null)
            hoveredIndex = _path.Path.FindIndex(x => x.Index == hovered.Index);

        Handles.color = hoveredIndex == 0 ? Color.red : Color.white;
        Handles.DrawWireCube(_worldGrid.GridPointToWorldPoint(_path.Path[0]), new Vector3(0.8f, 0, 0.8f));
        for (int i = 0; i < _path.Path.Count - 1; i++)
        {
            var b = _worldGrid.GridPointToWorldPoint(_path.Path[i + 1]);
            var a = _worldGrid.GridPointToWorldPoint(_path.Path[i]);

            Handles.color = i < hoveredIndex - 1 || hoveredIndex < 0 ? Color.white : Color.red;
            Handles.DrawWireCube(b, new Vector3(0.8f, 0, 0.8f));

            Handles.color = i < hoveredIndex || hoveredIndex < 0 ? Color.white : Color.red;
            Handles.DrawDottedLine(a, b, 8);
            
            var dist = Vector3.Distance(a, b);
            const float pathRenderDist = 0.75f;
            for (int j = 0; j < (int)(dist / pathRenderDist); j++)
            {
                Handles.DrawSolidDisc(Vector3.Lerp(a, b, ((float)EditorApplication.timeSinceStartup+j*pathRenderDist) / dist % 1), Vector3.up, 0.035f);
            }
        }

        Handles.color = Color.white;

        SceneView.RepaintAll();
    }

    private void AddPathPoint(GridPoint point)
    {
        if (_path.Path.Any(x => x.Index == point.Index))
        {
            var index = _path.Path.FindIndex(x => x.Index == point.Index);
            var i = _path.Path.Count - index;
            _path.Path.RemoveRange(index, i);
        }
        else
        {
            _path.Path.Add(point);
        }
    }

    private EDirection GetGridSide(Vector3 pos)
    {
        var angle = Vector3.SignedAngle(Vector3.forward, Vector3Int.RoundToInt(pos) - pos, Vector3.up);
        var abs = Mathf.Abs(angle);
        if (abs < 45)
            return EDirection.South;
        else if (abs > 135)
            return EDirection.North;
        else if (Mathf.Sign(angle) < 0)
            return EDirection.East;

        return EDirection.West;
    }
}
