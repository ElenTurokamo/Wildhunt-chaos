using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Snake")]
public class SnakePattern : WavePattern
{
    public int count = 8;
    public float amplitude = 2f;
    public float spacing = 1f;

    public override Transform Spawn(WaveController controller)
    {
        Camera cam = Camera.main;
        float camHalfWidth = cam.orthographicSize * cam.aspect;
        float maxAmplitude = Mathf.Min(amplitude, camHalfWidth - 0.5f);

        // Создаём родительский объект для этой змеи
        var parent = new GameObject("SnakeGroup").transform;

        // Смещаем старт по Y, чтобы новая волна не накладывалась на предыдущую
        float topY = cam.orthographicSize + cam.transform.position.y + 4f;

        // Если уже есть змеи на сцене, найдём самую верхнюю точку
        var existingSnakes = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (var snake in existingSnakes)
        {
            if (snake.name.StartsWith("SnakeGroup"))
            {
                float snakeTop = float.MinValue;
                foreach (Transform child in snake)
                {
                    if (child.position.y > snakeTop) snakeTop = child.position.y;
                }
                // Сдвигаем новую волну выше предыдущей
                if (snakeTop + spacing > topY) topY = snakeTop + spacing;
            }
        }

        for (int i = 0; i < count; i++)
        {
            if (!controller.threat.TrySpend(threatCost)) break;

            float x = Mathf.Sin(i * 0.5f) * maxAmplitude;
            float y = topY + i * spacing;

            Vector3 pos = new Vector3(x, y, 0f);
            var enemy = Instantiate(enemyPrefab, pos, Quaternion.identity, parent);

            // назначаем threatCost для ScoreSystem
            var destructable = enemy.GetComponent<EnemyDestructable>();
            if (destructable != null) destructable.threatCost = threatCost;
        }

        return parent; // возвращаем группу для WaveController
    }
}
