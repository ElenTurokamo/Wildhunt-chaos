using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/VFormation")]
public class VFormationPattern : WavePattern
{
    public int rows = 4;           // количество рядов
    public float spacingX = 1.5f;  // горизонтальное расстояние между врагами
    public float spacingY = 1.2f;  // вертикальное расстояние между рядами

    public override void Spawn(WaveController controller)
    {
        Camera cam = Camera.main;
        float camX = cam.transform.position.x;
        float topY = cam.orthographicSize + cam.transform.position.y + 4f;

        var parent = new GameObject("VFormationGroup").transform;

        // Смещение по Y, чтобы новая волна не накладывалась
        var existingV = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (var v in existingV)
        {
            if (v.name.StartsWith("VFormationGroup"))
            {
                float top = float.MinValue;
                foreach (Transform child in v)
                    if (child.position.y > top) top = child.position.y;

                if (top + spacingY > topY) topY = top + spacingY;
            }
        }

        for (int row = 0; row < rows; row++)
        {
            if (!controller.threat.TrySpend(threatCost)) return;

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
    }
}
