using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Snake")]
public class SnakePattern : WavePattern
{
    public int count = 8;
    public float amplitude = 2f;
    public float spacing = 1f;

    public override void Spawn(WaveController controller)
    {
        float topY = Camera.main.orthographicSize + Camera.main.transform.position.y + 4f;

        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float maxAmplitude = Mathf.Min(amplitude, camHalfWidth - 0.5f);

        for (int i = 0; i < count; i++)
        {
            if (!controller.threat.TrySpend(threatCost)) return;

            float x = Mathf.Sin(i * 0.5f) * maxAmplitude;
            float y = topY + i * spacing;

            Vector3 pos = new Vector3(x, y, 0);
            var enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

            // назначаем threatCost для ScoreSystem
            var destructable = enemy.GetComponent<EnemyDestructable>();
            if (destructable != null) destructable.threatCost = threatCost;
        }
    }
}
