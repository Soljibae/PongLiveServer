using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Paddle : MonoBehaviour
{
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;

    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public float CurrentSpeed { get; private set; }
    public float MaxSpeed => maxSpeed;
    public float Height => height;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        transform.localScale = new Vector3(width, height, 1f);

        if (boxCollider != null)
        {
            boxCollider.size = Vector2.one;
            boxCollider.offset = Vector2.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
