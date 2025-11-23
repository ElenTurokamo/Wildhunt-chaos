using UnityEngine;

public class SinMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float amplitude = 1f; 
    public float frequency = 1f;
    public float speed = 1f; 
    public float phaseOffset = 0f;

    [Header("Тип синусоиды")]
    [Tooltip("Зеркальное отражение движения.")]
    public bool isMirrored = false;

    private float camCenterX;

    void Start()
    {
        if (Camera.main != null)
        {
            camCenterX = Camera.main.transform.position.x; 
        }
        else
        {
            camCenterX = 0f;
        }
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;

        pos.y -= speed * Time.fixedDeltaTime;

        float mirroredMultiplier = isMirrored ? -1f : 1f;

        float sineOffset = Mathf.Sin(pos.y * frequency + phaseOffset) * amplitude * mirroredMultiplier;

        pos.x = camCenterX + sineOffset;

        transform.position = pos;
    }
}