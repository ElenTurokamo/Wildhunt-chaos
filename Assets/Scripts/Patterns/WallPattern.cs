using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Wall")]
public class WallPattern : WavePattern
{
    public int rows = 3;
    public int cols = 5;
    public float spacing = 1.5f;

    public override Transform Spawn(WaveController controller)
    {
        Camera cam = Camera.main;
        float topY = cam.orthographicSize + cam.transform.position.y + 4f;
        float camHalfWidth = cam.orthographicSize * cam.aspect;

        float maxRowWidth = (cols - 1) * spacing / 2f;
        if (maxRowWidth > camHalfWidth - 0.5f)
            spacing = (camHalfWidth - 0.5f) * 2f / (cols - 1);

        var parent = new GameObject("WallGroup").transform;

        // Смещение по Y, чтобы новая волна не накладывалась на предыдущие
        var existingGroups = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (var g in existingGroups)
        {
            if (g.name.StartsWith("WallGroup"))
            {
                float top = float.MinValue;
                foreach (Transform child in g)
                    if (child.position.y > top) top = child.position.y;

                if (top + spacing > topY) topY = top + spacing;
            }
        }

        float startX = cam.transform.position.x - (cols - 1) * spacing / 2f;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (!controller.threat.TrySpend(threatCost)) break;

                float posX = startX + x * spacing;
                float posY = topY + y * spacing;

                Vector3 pos = new Vector3(posX, posY, 0f);
                Instantiate(enemyPrefab, pos, Quaternion.identity, parent);
            }
        }

        return parent; // возвращаем группу для WaveController
    }
}
