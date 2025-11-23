using UnityEngine;

public class TopMovement : MonoBehaviour
{
    [Header("Вертикальное движение")]
    public float speedY = 5f;
    public float extraOffsetY = 1.5f;

    [Header("Горизонтальное движение")]
    public float amplitudeX = 2f;
    public float xSpeed = 2f;
    public float leftBoundX = -4.5f;
    public float rightBoundX = 4.5f;
    public float phaseOffset = 0f;

    private float camTopY;
    private bool reachedInitialTop = false;
    private bool startedOscillation = false;

    private float targetY;
    private float startX;

    private float oscillationStartTime;

    private float halfWidth;
    private float halfHeight;

    void Start()
    {
        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize;

        camTopY = cam.transform.position.y + camHeight;

        startX = transform.position.x;
        
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            halfWidth = col.bounds.size.x / 2f;
            halfHeight = col.bounds.size.y / 2f;
        }
        else
        {
            halfWidth = 0.5f;
            halfHeight = 0.5f;
        }
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;

        if (!reachedInitialTop)
        {
            // Phase 1: Moving down to the camera top boundary
            pos.y -= speedY * Time.fixedDeltaTime;

            if (pos.y + halfHeight <= camTopY)
            {
                reachedInitialTop = true;
                targetY = pos.y - extraOffsetY;
                startX = pos.x; 
            }
        }
        else if (!startedOscillation)
        {
            // Phase 2: Sinking to the target Y level
            
            pos.y -= speedY * Time.fixedDeltaTime;

            if (pos.y <= targetY)
            {
                pos.y = targetY;
                startedOscillation = true;
                oscillationStartTime = Time.time; 
            }
        }
        else
        {
            // Phase 3: Horizontal oscillation
            
            float timeSinceStart = (Time.time - oscillationStartTime) + phaseOffset;

            pos.x = startX + Mathf.Sin(timeSinceStart * xSpeed) * amplitudeX;
            pos.y = targetY;
        }

        // X-axis clamping
        pos.x = Mathf.Clamp(pos.x, leftBoundX + halfWidth, rightBoundX - halfWidth);

        transform.position = pos;
    }
}