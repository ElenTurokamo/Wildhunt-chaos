using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Cross")]
public class CrossPattern : WavePattern
{
    public float spacing = 1.2f;
    public int length = 4;

    public override void Spawn(WaveController controller)
    {
        float topY = Camera.main.orthographicSize + Camera.main.transform.position.y + 4f;
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        float maxSpacing = Mathf.Min(spacing, camHalfWidth / length - 0.5f);

        for (int i = -length; i <= length; i++)
        {
            if (!controller.threat.TrySpend(threatCost)) return;

            // Горизонтальная линия сверху
            float xH = i * maxSpacing;
            Vector3 spawnPosH = new Vector3(xH, topY, 0);
            Instantiate(enemyPrefab, spawnPosH, Quaternion.identity);

            if (!controller.threat.TrySpend(threatCost)) return;

            // Вертикальная линия сверху
            float xV = 0f;
            float yV = topY - i * maxSpacing;
            Vector3 spawnPosV = new Vector3(xV, yV, 0);
            Instantiate(enemyPrefab, spawnPosV, Quaternion.identity);
        }
    }
}
