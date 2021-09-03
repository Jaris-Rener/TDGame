using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Enemy EnemyPrefab;
    public WorldGrid WorldGrid;

    IEnumerator Start()
    {
        for (int i = 0; i < 12; i++)
        {
            var enemy = Instantiate(EnemyPrefab, transform.position, EnemyPrefab.transform.rotation);
            enemy.AssignPath(WorldGrid.Paths.GetRandom());
            yield return new WaitForSeconds(3f);
        }
    }
}