using UnityEngine;

public class SinMovement : MonoBehaviour
{
    private float sinCenterX;         
    public float amplitude = 1f;        // амплитуда колебаний
    public float frequency = 1f;        // частота (скорость колебаний)
    public float phaseOffset = 3f;      // сдвиг фазы, если враги должны быть несинхронны

    void Start()
    {
        sinCenterX = transform.position.x;
    }

    void FixedUpdate()
    {
        Vector2 pos = transform.position;

        // Вычисляем новое смещение по синусу
        float sin = Mathf.Sin((Time.time + phaseOffset) * frequency) * amplitude;
        pos.x = sinCenterX + sin;

        transform.position = pos;
    }
}
