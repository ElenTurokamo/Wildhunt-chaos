using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Wall")]
public class WallPattern : WavePattern
{
    public int rows = 3;
    public int cols = 5;
    public float spacing = 1.5f;

    public override void Spawn(WaveController controller)
    {
        float topY = Camera.main.orthographicSize + Camera.main.transform.position.y + 1f;
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float maxRowWidth = (cols - 1) * spacing / 2f;

        if (maxRowWidth > camHalfWidth - 0.5f) spacing = (camHalfWidth - 0.5f) * 2f / (cols - 1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (!controller.threat.TrySpend(threatCost)) return;

                float posX = (x - cols / 2f) * spacing;
                float posY = topY + y * spacing;

                Vector3 pos = new Vector3(posX, posY, 0);

                Instantiate(enemyPrefab, pos, Quaternion.identity);
            }
        }
    }
}
