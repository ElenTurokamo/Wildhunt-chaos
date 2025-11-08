using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Gun[] guns;

    public float movementSpeed;
    Vector2 normalizedInput;
    Rigidbody2D rb;

    bool shoot;
    public float shootDelay = 0.15f; 
    private float nextShootTime = 0f;

    bool speedUp;

    [SerializeField] float minPosX, maxPosX, minPosY, maxPosY;

    void Start()
    {
        guns = transform.GetComponentsInChildren<Gun>();
    }

    void Update()
    {
        shoot = Input.GetKey(KeyCode.Mouse0);

        if (shoot && Time.time >= nextShootTime)
        {
            foreach (Gun gun in guns)
            {
                gun.Shoot();
            }

            nextShootTime = Time.time + shootDelay;
        }
    }

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
