using UnityEngine;

public class SinMovement : MonoBehaviour
{
    public float amplitude = 1f;    // амплитуда колебаний
    public float frequency = 1f;    // частота колебаний
    public float speed = 1f;        // скорость движения по Y
    public float phaseOffset = 0f;  // сдвиг фазы, чтобы враги были несинхронны

    private float camCenterX;

    void Start()
    {
        camCenterX = Camera.main.transform.position.x; 
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;

        pos.y -= speed * Time.fixedDeltaTime;

        pos.x = camCenterX + Mathf.Sin(pos.y * frequency + phaseOffset) * amplitude;

        transform.position = pos;
    }
}
