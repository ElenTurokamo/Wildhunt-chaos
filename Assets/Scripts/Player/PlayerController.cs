using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float movementSpeed;
    Vector2 normalizedInput;
    Rigidbody2D rb;

    [SerializeField] float minPosX, maxPosX, minPosY, maxPosY;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        normalizedInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")).normalized;

        rb.linearVelocity = normalizedInput * movementSpeed;
        
        Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(transform.position.x, minPosX, maxPosX),
            Mathf.Clamp(transform.position.y, minPosY, maxPosY));

        rb.position = clampedPosition;
    }
}
