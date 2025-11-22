using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Swarm")]
public class SwarmPattern : WavePattern
{
    public int amount = 12;
    public int columns = 4;          
    public float horizontalSpacing = 1f;
    public float verticalSpacing = 1f;   

    public override Transform Spawn(WaveController controller)
    {
        Camera cam = Camera.main;
        float topY = cam.orthographicSize + cam.transform.position.y + 4f;
        float camX = cam.transform.position.x;

        // создаём группу для всех врагов
        var parent = new GameObject("SwarmGroup").transform;

        // смещаем старт по Y, чтобы новая волна не накладывалась на предыдущие
        var existingGroups = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (var g in existingGroups)
        {
            if (g.name.StartsWith("SwarmGroup"))
            {
                float top = float.MinValue;
                foreach (Transform child in g)
                    if (child.position.y > top) top = child.position.y;

                if (top + verticalSpacing > topY) topY = top + verticalSpacing;
            }
        }

        int rows = Mathf.CeilToInt((float)amount / columns);
        float totalWidth = (columns - 1) * horizontalSpacing;
        float startX = camX - totalWidth * 0.5f;

        for (int i = 0; i < amount; i++)
        {
            if (!controller.threat.TrySpend(threatCost)) break;

            int col = i % columns;
            int row = i / columns;

            float x = startX + col * horizontalSpacing;
            float y = topY - row * verticalSpacing;

            Vector3 pos = new Vector3(x, y, 0f);
            Instantiate(enemyPrefab, pos, Quaternion.identity, parent);
        }

        return parent; // возвращаем группу для WaveController
    }
}
