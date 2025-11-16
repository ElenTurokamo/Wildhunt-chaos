using UnityEngine;

public class TopMovement : MonoBehaviour
{
    public float amplitude = 2f;    // амплитуда горизонтальных колебаний
    public float frequency = 1f;    // частота колебаний
    public float speed = 5f;        // скорость движения вниз
    public float phaseOffset = 0f;  // сдвиг фазы для индивидуальности

    private float camLeftX;
    private float camRightX;
    private float camTopY;

    private bool reachedTop = false;
    private float startY;

    private float halfWidth;  // половина ширины хитбокса
    private float halfHeight; // половина высоты хитбокса

    void Start()
    {
        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        camLeftX = cam.transform.position.x - camWidth;
        camRightX = cam.transform.position.x + camWidth;
        camTopY = cam.transform.position.y + camHeight;

        startY = transform.position.y;

        // вычисляем размеры хитбокса
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
            pos.y -= speed * Time.fixedDeltaTime;

            // Проверяем верхнюю границу хитбокса
            if (pos.y + halfHeight <= camTopY)
            {
                reachedTop = true;
                startY = pos.y;
            }
        }
        else
        {
            // Горизонтальное колебание
            pos.x = camLeftX + halfWidth + Mathf.PingPong(Time.time * amplitude, camRightX - camLeftX - 2*halfWidth);
        }

        transform.position = pos;
    }
}
