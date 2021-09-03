using System;
using UnityEngine;

[Serializable]
public class GridPoint
{
    public GridPoint(WorldGrid grid, int index)
    {
        Index = index;
        Grid = grid;
    }

    public Vector3 SnappedPos => Grid.GridPointToWorldPoint(this);

    public int Index;
    public EDirection Navigation = EDirection.None;
    public ETowerPlacement TowerPlacement = ETowerPlacement.Allowed;
    public Tower PlacedTower;

    public WorldGrid Grid;

    public void PlaceTower(Tower tower)
    {
        PlacedTower = tower;
        PlacedTower.transform.position = SnappedPos;
    }
}