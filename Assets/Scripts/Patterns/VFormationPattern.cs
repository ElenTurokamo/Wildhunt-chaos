using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/VFormation")]
public class VFormationPattern : WavePattern
{
    public int rows = 4;           // количество рядов
    public float spacingX = 1.5f;  // горизонтальное расстояние между врагами
    public float spacingY = 1.2f;  // вертикальное расстояние между рядами

    public override Transform Spawn(WaveController controller)
    {
        Camera cam = Camera.main;
        float camX = cam.transform.position.x;
        float topY = cam.orthographicSize + cam.transform.position.y + 4f;

        // Создаём родительский объект для этой формации
        var parent = new GameObject("VFormationGroup").transform;

        // Сдвигаем старт по Y, чтобы новая волна не накладывалась на предыдущую
        var existingGroups = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (var g in existingGroups)
        {
            if (g.name.StartsWith("VFormationGroup"))
            {
                float groupTop = float.MinValue;
                foreach (Transform child in g)
                {
                    if (child.position.y > groupTop) groupTop = child.position.y;
                }
                if (groupTop + spacingY > topY) topY = groupTop + spacingY;
            }
        }

        for (int row = rows - 1; row >= 0; row--)
        {
            if (!controller.threat.TrySpend(threatCost)) break;

            float posY = topY - (rows - 1 - row) * spacingY;

            if (row == 0)
            {
                Vector3 pos = new Vector3(camX, posY, 0f);
                Instantiate(enemyPrefab, pos, Quaternion.identity, parent);
            }
            else
            {
                Vector3 leftPos = new Vector3(camX - row * spacingX, posY, 0f);
                Vector3 rightPos = new Vector3(camX + row * spacingX, posY, 0f);

                Instantiate(enemyPrefab, leftPos, Quaternion.identity, parent);
                Instantiate(enemyPrefab, rightPos, Quaternion.identity, parent);
            }
        }

        return parent; // возвращаем группу для WaveController
    }
}
