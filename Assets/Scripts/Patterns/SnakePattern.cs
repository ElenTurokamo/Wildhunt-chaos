using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Snake")]
public class SnakePattern : WavePattern
{
    public int count = 8;
    public float amplitude = 2f;
    public float spacing = 1f;

    public override void Spawn(WaveController controller)
    {
        float topY = Camera.main.orthographicSize + Camera.main.transform.position.y + 1f;
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float maxAmplitude = Mathf.Min(amplitude, camHalfWidth - 0.5f); // ограничиваем, чтобы не вылетало

        for (int i = 0; i < count; i++)
        {
            if (!controller.threat.TrySpend(threatCost)) return;

            float x = Mathf.Sin(i * spacing) * maxAmplitude;
            Vector3 pos = new Vector3(x, topY, 0);

            Instantiate(enemyPrefab, pos, Quaternion.identity);
        }
    }
}
