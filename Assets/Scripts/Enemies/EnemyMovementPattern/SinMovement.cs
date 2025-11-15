using UnityEngine;

public class SinMovement : MonoBehaviour
{
    private float sinCenterX;
    public float amplitude = 1f;    // амплитуда колебаний
    public float frequency = 1f;    // частота колебаний
    public float phaseOffset = 0f;  // сдвиг фазы, чтобы враги были несинхронны

    void Start()
    {
        sinCenterX = transform.position.x;
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;

        // X колеблется в зависимости от Y
        pos.x = sinCenterX + Mathf.Sin(pos.y * frequency + phaseOffset) * amplitude;

        transform.position = pos;
    }
}
