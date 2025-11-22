using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Lambda")]
public class LambdaPattern : WavePattern
{
    public int rows = 4;           // количество рядов
    public float spacingX = 1.5f;  // горизонтальное расстояние между врагами
    public float spacingY = 1.2f;  // вертикальное расстояние между рядами

    public override Transform Spawn(WaveController controller)
    {
        Camera cam = Camera.main;
        float camX = cam.transform.position.x;
        float topY = cam.orthographicSize + cam.transform.position.y + 4f;

        // создаём группу для всех врагов
        var parent = new GameObject("LambdaGroup").transform;

        // Смещение по Y, чтобы новая волна не накладывалась
        var existingGroups = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (var g in existingGroups)
        {
            if (g.name.StartsWith("LambdaGroup"))
            {
                float top = float.MinValue;
                foreach (Transform child in g)
                    if (child.position.y > top) top = child.position.y;

                if (top + spacingY > topY) topY = top + spacingY;
            }
        }

        // Первый ряд (один враг) — вершина V
        if (controller.threat.TrySpend(threatCost))
        {
            Vector3 centerPos = new Vector3(camX, topY, 0f);
            Instantiate(enemyPrefab, centerPos, Quaternion.identity, parent);
        }

        // Остальные ряды (по бокам)
        for (int row = 1; row < rows; row++)
        {
            if (!controller.threat.TrySpend(threatCost)) break;

            float posY = topY - row * spacingY;

            // Левый враг
            float leftX = camX - row * spacingX;
            Vector3 leftPos = new Vector3(leftX, posY, 0f);
            Instantiate(enemyPrefab, leftPos, Quaternion.identity, parent);

            // Правый враг
            float rightX = camX + row * spacingX;
            Vector3 rightPos = new Vector3(rightX, posY, 0f);
            Instantiate(enemyPrefab, rightPos, Quaternion.identity, parent);
        }

        return parent; // возвращаем группу для WaveController
    }
}
