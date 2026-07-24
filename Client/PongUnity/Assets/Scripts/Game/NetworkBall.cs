using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(NetworkRigidbody2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class NetworkBall : NetworkBehaviour
{
    [SerializeField] private float radius;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float minSpeed;
    [SerializeField] private float speedDecay;
    [SerializeField, Range(0f, 75f)] private float launchMaxAngle;
    [SerializeField, Range(0f, 75f)] private float paddleBounceMaxAngle;

    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private CircleCollider2D circleCollider;

    private Vector2 currentDirection;
    private float currentSpeed;

    private bool isPlaying;
  
    void Awake()
    {
        isPlaying = false;
        currentDirection = Vector2.zero;
        currentSpeed = 0f;

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

        if (!IsServer)
            return;

        if (currentSpeed > minSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, minSpeed, speedDecay * Time.fixedDeltaTime);
        }

        ApplyVelocityServer();
    }

    public void SetIsPlayingServer(bool isPlaying)
    {
        if (!IsServer)
            return;

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

    public void ResetBallServer()
    {
        if (!IsServer)
            return;

        transform.position = Vector3.zero;

        currentDirection = Vector2.zero;
        currentSpeed = 0f;
    }

    public void LaunchServer()
    {
        if (!IsServer)
            return;

        int xSign = Random.value < 0.5f ? -1 : 1;

        float angle = Random.Range(-launchMaxAngle, launchMaxAngle);

        float angleRad = angle * Mathf.Deg2Rad;

        currentDirection = new Vector2(xSign * Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;
        currentSpeed = minSpeed;

        ApplyVelocityServer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isPlaying)
            return;

        if (!IsServer)
            return;

        NetworkPaddle paddle = collision.gameObject.GetComponentInParent<NetworkPaddle>();

        if (paddle != null)
        {
            float paddleHalfHeight = paddle.Height / 2;

            float hitOffset = (transform.position.y - paddle.transform.position.y) / paddleHalfHeight;

            hitOffset = Mathf.Clamp(hitOffset, -1f, 1f);

            int xSign = transform.position.x >= paddle.transform.position.x ? 1 : -1;

            float bounceAngle = hitOffset * paddleBounceMaxAngle;

            float angleRad = bounceAngle * Mathf.Deg2Rad;

            currentDirection = new Vector2(xSign * Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

            float paddleSpeedRatio = Mathf.InverseLerp(0f, paddle.MaxSpeed, Mathf.Abs(paddle.CurrentSpeed));

            currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, paddleSpeedRatio);
        }
        else
        {
            if (collision.contactCount <= 0)
                return;

            Vector2 normal = collision.GetContact(0).normal;
            Vector2 baseDirection = currentDirection;

            currentDirection = Vector2.Reflect(baseDirection, normal).normalized;
        }

        ApplyVelocityServer();
    }

    private void ApplyVelocityServer()
    {
        if (!IsServer)
            return;

        rigidBody.linearVelocity = currentDirection * currentSpeed;
    }
}
