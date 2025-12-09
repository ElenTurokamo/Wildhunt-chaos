using UnityEngine;

[CreateAssetMenu(fileName = "SingleEnemyPattern", menuName = "Patterns/Single Enemy", order = 1)]
public class SingleEnemyPattern : WavePattern
{
    public override Transform Spawn(WaveController controller)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab не назначен для паттерна " + patternName);
            return null;
        }

        Camera cam = Camera.main;
        
        float spawnY = cam.orthographicSize + cam.transform.position.y + 2f; 
        
        float spawnX = cam.transform.position.x; 

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        if (!controller.threat.TrySpend(threatCost))
        {
            return null;
        }

        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        return enemyInstance.transform;
    }
}