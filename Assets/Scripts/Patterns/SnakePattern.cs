using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Snake")]
public class SnakePattern : WavePattern
{
    public int count = 8;
    public float amplitude = 2f;
    public float spacing = 1f;
    
    [Tooltip("Горизонтальное смещение для каждой линии врагов.")]
    public float lineOffset = 2f; // Смещение x=-2 и x=+2

    public override Transform Spawn(WaveController controller)
    {
        // Обязательная проверка, если забыли назначить второй префаб
        if (enemyPrefabAlt == null)
        {
            Debug.LogError("В SnakePattern не назначен enemyPrefabAlt! Правая линия не будет создана или будет использовать enemyPrefab.");
            // Для безопасности можно использовать основной префаб, если альтернативный не задан
            enemyPrefabAlt = enemyPrefab; 
        }

        Camera cam = Camera.main;
        float camHalfWidth = cam.orthographicSize * cam.aspect;
        float maxAmplitude = Mathf.Min(amplitude, camHalfWidth - lineOffset - 0.5f);

        // Создаём родительский объект для этой змеи
        var parent = new GameObject("SnakeGroup").transform;

        // --- Логика поиска верхней точки остается без изменений ---
        
        float topY = cam.orthographicSize + cam.transform.position.y + 4f;
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
                if (snakeTop + spacing > topY) topY = snakeTop + spacing;
            }
        }
        
        // --- Логика спауна двух линий с разными префабами ---

        for (int i = 0; i < count; i++)
        {
            if (!controller.threat.TrySpend(threatCost * 2)) break; // Тратим в 2 раза больше угрозы

            float sineWaveX = Mathf.Sin(i * 0.5f) * maxAmplitude; 
            float y = topY + i * spacing;

            // 1. Левая линия: Обычное движение (+sineWaveX) и Основной префаб (enemyPrefab)
            float xLeft = -lineOffset + sineWaveX;
            Vector3 posLeft = new Vector3(xLeft, y, 0f);
            
            var enemyLeft = Instantiate(enemyPrefab, posLeft, Quaternion.identity, parent);
            var destructableLeft = enemyLeft.GetComponent<EnemyDestructable>();
            if (destructableLeft != null) destructableLeft.threatCost = threatCost;

            // 2. Правая линия: Зеркальное движение (-sineWaveX) и Альтернативный префаб (enemyPrefabAlt)
            float xRight = lineOffset + (-sineWaveX); 
            Vector3 posRight = new Vector3(xRight, y, 0f);

            var enemyRight = Instantiate(enemyPrefabAlt, posRight, Quaternion.identity, parent); // <-- Используем enemyPrefabAlt
            var destructableRight = enemyRight.GetComponent<EnemyDestructable>();
            if (destructableRight != null) destructableRight.threatCost = threatCost;
        }

        return parent;
    }
}