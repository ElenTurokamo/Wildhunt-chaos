using UnityEngine;

public class TopMovement : MonoBehaviour
{
    [Header("Вертикальное движение")]
    public float speedY = 5f;           // скорость движения вниз
    public float extraOffsetY = 1.5f;   // дополнительное смещение вниз после достижения верхней границы

    [Header("Горизонтальное движение")]
    public float amplitudeX = 2f;       // амплитуда колебаний по X
    public float xSpeed = 2f;           // скорость колебаний по X
    public float leftBoundX = -4.5f;    // левый предел движения
    public float rightBoundX = 4.5f;    // правый предел движения
    public float phaseOffset = 0f;      // индивидуальный сдвиг фазы

    private float camTopY;
    private bool reachedTop = false;
    private float startY;
    private float startX;

    private float halfWidth;
    private float halfHeight;

    void Start()
    {
        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize;

        camTopY = cam.transform.position.y + camHeight;

        startX = transform.position.x;
        startY = transform.position.y;

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

        if (!reachedTop)
        {
            // Движение вниз
            pos.y -= speedY * Time.fixedDeltaTime;

            // Проверка достижения верхней границы камеры
            if (pos.y + halfHeight <= camTopY)
            {
                reachedTop = true;
                startY = pos.y - extraOffsetY; // Y-центр колебаний чуть ниже
                startX = pos.x;                 // X-центр колебаний
            }
        }
        else
        {
            // Горизонтальное колебание по синусу с отдельной скоростью
            pos.x = startX + Mathf.Sin((Time.time + phaseOffset) * xSpeed) * amplitudeX;
            pos.y = startY;
        }

        // Ограничение по X
        pos.x = Mathf.Clamp(pos.x, leftBoundX + halfWidth, rightBoundX - halfWidth);

        transform.position = pos;
    }
}
