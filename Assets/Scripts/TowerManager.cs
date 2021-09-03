using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public WorldGrid Grid;
    public Tower TowerPrefab;

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                var gridPoint = Grid.WorldPointToGridPoint(hit.point);

                if(gridPoint.TowerPlacement == ETowerPlacement.Blocked)
                    return;

                if(gridPoint.PlacedTower != null)
                    return;

                gridPoint.PlaceTower(Instantiate(TowerPrefab));
            }
        }
    }
}
