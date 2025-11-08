using Unity.VisualScripting;
using UnityEngine;

public class SinMovement : MonoBehaviour
{
    float sinCenterX;

    void Start()
    {
        sinCenterX = transform.position.x;
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        float sin = Mathf.Cos(pos.y);
        pos.x = sinCenterX + sin;

        transform.position = pos;
    }
}
