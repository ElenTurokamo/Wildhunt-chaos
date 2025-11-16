using UnityEngine;

[CreateAssetMenu(menuName = "Patterns/Cross")]
public class CrossPattern : WavePattern
{
    public float spacing = 2f; 
    public int length = 4;

    public override void Spawn(WaveController controller)
    {
        Camera cam = Camera.main;
        float topY = cam.orthographicSize + cam.transform.position.y + 4f;
        float camHalfWidth = cam.orthographicSize * cam.aspect;

        float maxSpacing = Mathf.Min(spacing, camHalfWidth / length - 0.5f);

        var parent = new GameObject("CrossGroup").transform;

        var existingCrosses = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (var cross in existingCrosses)
        {
            if (cross.name.StartsWith("CrossGroup"))
            {
                float crossTop = float.MinValue;
                foreach (Transform child in cross)
                {
                    if (child.position.y > crossTop) crossTop = child.position.y;
                }
                if (crossTop + maxSpacing > topY) topY = crossTop + maxSpacing;
            }
        }

        for (int i = -length; i <= length; i++)
        {
            if (!controller.threat.TrySpend(threatCost)) return;

            // Горизонтальная линия
            float xH = i * maxSpacing;
            Vector3 spawnPosH = new Vector3(xH, topY, 0);
            Instantiate(enemyPrefab, spawnPosH, Quaternion.identity, parent);

            if (!controller.threat.TrySpend(threatCost)) return;

            // Вертикальная линия
            float xV = 0f;
            float yV = topY - i * maxSpacing;
            Vector3 spawnPosV = new Vector3(xV, yV, 0);
            Instantiate(enemyPrefab, spawnPosV, Quaternion.identity, parent);
        }
    }
}
