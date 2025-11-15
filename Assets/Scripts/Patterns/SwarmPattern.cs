using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Swarm")]
public class SwarmPattern : WavePattern
{
    public int amount = 12;
    public float spread = 3f;

    public override void Spawn(WaveController controller)
    {
        float topY = Camera.main.orthographicSize + Camera.main.transform.position.y + 2f;
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        float maxSpread = Mathf.Min(spread, camHalfWidth - 0.5f);

        for (int i = 0; i < amount; i++)
        {
            if (!controller.threat.TrySpend(threatCost)) return;

            float x = Random.Range(-maxSpread, maxSpread);
            Vector3 pos = new Vector3(x, topY, 0);

            Instantiate(enemyPrefab, pos, Quaternion.identity);
        }
    }
}
