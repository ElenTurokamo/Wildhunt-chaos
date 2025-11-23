using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Boss Escort")]
public class ShieldEscortPattern : WavePattern
{
    [Header("Grid Settings")]
    public int rows = 3;
    public int columns = 3;
    public float spacing = 1.5f;     // Distance between minions
    public float groupOffsetX = 5f;  // Distance from center (0) to the center of the side groups

    [Header("Boss Settings")]
    public int bossThreatCost = 10;  // Separate cost for the main enemy
    public float bossOffsetY = 2f;   // Vertical offset for the boss relative to minions start

    public override Transform Spawn(WaveController controller)
    {
        // 1. Validation
        if (enemyPrefabAlt == null)
        {
            Debug.LogError("ShieldEscortPattern: 'Enemy Prefab Alt' is missing! Assign the central enemy prefab.");
            return null;
        }

        var parent = new GameObject("ShieldEscortGroup").transform;
        
        Camera cam = Camera.main;
        float topY = cam.orthographicSize + cam.transform.position.y + 3f;

        // 2. Spawn Boss (Central Unit) - Priority
        // Try to spend points for the boss first
        if (controller.threat.TrySpend(bossThreatCost))
        {
            Vector3 bossPos = new Vector3(0f, topY + bossOffsetY, 0f);
            var boss = Instantiate(enemyPrefabAlt, bossPos, Quaternion.identity, parent);
            
            // Assign threat cost if destructable
            var bossDestructable = boss.GetComponent<EnemyDestructable>();
            if (bossDestructable != null) 
            {
                bossDestructable.threatCost = bossThreatCost;
            }
        }

        // 3. Spawn Minions (Left and Right Grids)
        // We iterate through columns and rows to build the grids
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                // Calculate local grid position (centered relative to its group)
                // (col - (columns - 1) / 2f) creates indices like -1, 0, 1 for a size of 3
                float xOffset = (col - (columns - 1) / 2f) * spacing;
                float yOffset = row * spacing;

                float yPos = topY + yOffset;

                // --- Left Group ---
                if (controller.threat.TrySpend(threatCost))
                {
                    float xLeft = -groupOffsetX + xOffset;
                    SpawnMinion(enemyPrefab, new Vector3(xLeft, yPos, 0f), parent);
                }

                // --- Right Group ---
                if (controller.threat.TrySpend(threatCost))
                {
                    float xRight = groupOffsetX + xOffset;
                    SpawnMinion(enemyPrefab, new Vector3(xRight, yPos, 0f), parent);
                }
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
            destructable.threatCost = threatCost;
        }
    }
}