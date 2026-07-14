using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Ball : MonoBehaviour
{
    [SerializeField] private float radius;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float minSpeed;
    [SerializeField] private float speedDecay;
    [SerializeField, Range(0f, 75f)] private float launchMaxAngle = 35f;

    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 currentDirection;
    private float currentSpeed;

    private bool isPlaying;

    void Awake()
    {
        isPlaying = false;
        currentDirection = Vector2.zero;
        currentSpeed = 0f;

        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody2D>();

        if(circleCollider == null)
            circleCollider = GetComponent<CircleCollider2D>();

        if(spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);

        if (circleCollider != null)
        {
            circleCollider.radius = 0.5f;
            circleCollider.offset = Vector2.zero;
        }
    }
    void FixedUpdate()
    {
        if (!isPlaying)
            return;

        if (currentSpeed > minSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, minSpeed, speedDecay * Time.fixedDeltaTime);
        }

        ApplyVelocity();
    }

    public void SetIsPlaying(bool isPlaying)
    {
        this.isPlaying = isPlaying;

        if (rigidBody != null)
        {
            rigidBody.simulated = isPlaying;

            if (!isPlaying)
            {
                rigidBody.linearVelocity = Vector2.zero;
                rigidBody.angularVelocity = 0f;
            }
        }

        if (circleCollider != null)
            circleCollider.enabled = isPlaying;

        if (!isPlaying)
        {
            currentDirection = Vector2.zero;
            currentSpeed = 0f;
        }
    }

    public void ResetBall()
    {
        transform.position = Vector3.zero;

        currentDirection = Vector2.zero;
        currentSpeed = 0f;
    }

    public void Launch()
    {
        int xSign = Random.value < 0.5f ? -1 : 1;

        float angle = Random.Range(-launchMaxAngle, launchMaxAngle);

        float angleRad = angle * Mathf.Deg2Rad;

        currentDirection = new Vector2(xSign * Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;
        currentSpeed = minSpeed;

        ApplyVelocity();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isPlaying)
            return;

        Paddle paddle = collision.gameObject.GetComponentInParent<Paddle>();

        if (paddle != null)
        {
            
        }
        else
        {
            if (collision.contactCount <= 0)
                return;

            Vector2 normal = collision.GetContact(0).normal;
            Vector2 baseDirection = currentDirection;

            currentDirection = Vector2.Reflect(baseDirection, normal).normalized;

            ApplyVelocity();
        }

        
    }

    private void ApplyVelocity()
    {
        rigidBody.linearVelocity = currentDirection * currentSpeed;
    }
}
