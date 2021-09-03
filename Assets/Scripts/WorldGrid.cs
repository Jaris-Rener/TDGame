using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WorldGrid
    : MonoBehaviour
{
    public Vector2Int GridSize = new Vector2Int(15, 10);
    public Vector2Int GridOffset;
    public GridPoint[] Grid;

    public List<GridPath> Paths;

    [ContextMenu("Reset Grid")]
    private void GenerateGrid()
    {
        Grid = new GridPoint[GridSize.x * GridSize.y];
        for (var i = 0; i < Grid.Length; i++)
        {
            Grid[i] = new GridPoint(this, i);
        }
    }

    public int IndexFromPoint(int x, int y)
    {
        return x + y * GridSize.x;
    }

    public Vector2Int PointFromIndex(int i)
    {
        return new Vector2Int(i%GridSize.x, Mathf.FloorToInt((float)i/GridSize.x));
    }

    public Vector3 GridPointToWorldPoint(GridPoint gridPoint)
    {
        var point = PointFromIndex(gridPoint.Index);
        point += GridOffset;
        return new Vector3(point.x, 0, point.y);
    }

    public GridPoint WorldPointToGridPoint(Vector3 worldPoint)
    {
        var rounded = Vector3Int.RoundToInt(worldPoint);
        rounded.x -= GridOffset.x;
        rounded.z -= GridOffset.y;
        return Grid[IndexFromPoint(rounded.x, rounded.z)];
    }

    [ContextMenu("Test")]
    void Test()
    {
        for(int i = 0; i < Grid.Length; i++)
            print($"{i} " + PointFromIndex(i));
    }

    private void OnDrawGizmosSelected()
    {
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int z = 0; z < GridSize.y; z++)
            {
                var point = Grid[IndexFromPoint(x, z)];
                Gizmos.color = point.TowerPlacement == ETowerPlacement.Allowed ? new Color(0f, 1f, 0f, 0.1f) : new Color(1f, 0f, 0f, 0.1f);
                var pos = new Vector3(x + GridOffset.x, 0, z + GridOffset.y);
                Gizmos.DrawCube(pos, new Vector3(1, 0.01f, 1));
                Gizmos.color = point.TowerPlacement == ETowerPlacement.Allowed ? new Color(0f, 1f, 0f, 1f) : new Color(1f, 0f, 0f, 1f);
                Gizmos.DrawWireCube(pos, new Vector3(1, 0.01f, 1));
            }
        }
    }
}

[Serializable]
public class GridPath
{
    public GridPath()
    {
    }

    public GridPath(string name)
    {
        Name = name;
    }

    public List<GridPoint> Path = new List<GridPoint>();
    public string Name;
}