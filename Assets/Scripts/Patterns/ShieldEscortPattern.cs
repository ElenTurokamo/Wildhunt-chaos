using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Boss Escort")]
public class ShieldEscortPattern : WavePattern
{
    [Header("Grid Settings")]
    public int rows = 3;
    public int columns = 3;
    public float spacing = 1.5f;
    public float groupOffsetX = 5f;

    [Header("Boss Settings")]
    public int bossThreatCost = 10;
    public float bossOffsetY = 2f;

    public override Transform Spawn(WaveController controller)
    {
        if (enemyPrefabAlt == null)
        {
            Debug.LogError("ShieldEscortPattern: 'Enemy Prefab Alt' missing.");
            return null;
        }

        // 1. Calculate Total Cost (All or Nothing)
        int minionsPerSide = rows * columns;
        int totalMinions = minionsPerSide * 2;
        int totalCost = bossThreatCost + (totalMinions * threatCost);

        // 2. Try to spend for the WHOLE wave at once
        if (!controller.threat.TrySpend(totalCost))
        {
            return null; // Not enough points for the full formation
        }

        var parent = new GameObject("ShieldEscortGroup").transform;
        
        Camera cam = Camera.main;
        float topY = cam.orthographicSize + cam.transform.position.y + 3f;

        // 3. Spawn Boss (Guaranteed)
        Vector3 bossPos = new Vector3(0f, topY + bossOffsetY, 0f);
        var boss = Instantiate(enemyPrefabAlt, bossPos, Quaternion.identity, parent);
        
        var bossDestructable = boss.GetComponent<EnemyDestructable>();
        if (bossDestructable != null) 
        {
            bossDestructable.threatCost = bossThreatCost; // Set score value
        }

        // 4. Spawn Minions (Guaranteed)
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                float xOffset = (col - (columns - 1) / 2f) * spacing;
                float yOffset = row * spacing;
                float yPos = topY + yOffset;

                // Left Group
                float xLeft = -groupOffsetX + xOffset;
                SpawnMinion(enemyPrefab, new Vector3(xLeft, yPos, 0f), parent);

                // Right Group
                float xRight = groupOffsetX + xOffset;
                SpawnMinion(enemyPrefab, new Vector3(xRight, yPos, 0f), parent);
            }
        }

        return parent;
    }

    private void SpawnMinion(GameObject prefab, Vector3 position, Transform parent)
    {
        var minion = Instantiate(prefab, position, Quaternion.identity, parent);
        var destructable = minion.GetComponent<EnemyDestructable>();
        if (destructable != null)
        {
            destructable.threatCost = threatCost; // Set score value only
        }
    }
}