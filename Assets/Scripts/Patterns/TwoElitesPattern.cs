using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Two Elites")]
public class TwoElitesPattern : WavePattern
{
    [Header("Настройки позиции")]
    [Tooltip("Расстояние между двумя щитовиками")]
    public float spacing = 2.0f;

    [Header("Настройки очков")]
    public int minionBaseScore = 200;

    [Tooltip("Смещение всей группы по X (если 0, то по центру экрана)")]
    public float centerX = 0f;

    public override Transform Spawn(WaveController controller)
    {
        if (!controller.threat.TrySpend(threatCost * 2)) return null;

        var parent = new GameObject("TwoElitesGroup").transform;

        Camera cam = Camera.main;
        float topY = cam.orthographicSize + cam.transform.position.y + 2f;

        float xLeft = centerX - (spacing / 2f);
        float xRight = centerX + (spacing / 2f);

        SpawnMinion(enemyPrefab, new Vector3(xLeft, topY, 0f), parent, controller);
        SpawnMinion(enemyPrefab, new Vector3(xRight, topY, 0f), parent, controller);

        return parent;
    }

    private void SpawnMinion(GameObject prefab, Vector3 position, Transform parent, WaveController controller)
    {
        if (prefab == null) return;

        var enemy = Instantiate(prefab, position, Quaternion.identity, parent);
        var destructable = enemy.GetComponent<EnemyDestructable>();

        if (destructable != null)
        {
            float minutesAlive = controller.threat.GetTimeAlive() / 60f;
            float multiplier = 1f + (minutesAlive * 0.01f);
            
            int calculatedScore = Mathf.RoundToInt(minionBaseScore * multiplier);
            
            destructable.finalScore = calculatedScore;
        }
    }
}