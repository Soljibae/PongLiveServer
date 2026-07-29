using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(NetworkRigidbody2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class NetworkPaddle : NetworkBehaviour
{
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;

    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private BoxCollider2D boxCollider;

    private float maxY, minY;
    public float CurrentSpeed { get; private set; }
    public float MaxSpeed => maxSpeed;
    public float Height => height;

    private bool isPlaying;

    private void Awake()
    {
        isPlaying = false;
        CurrentSpeed = 0;

        transform.localScale = new Vector3(width, height, 1f);

        if (boxCollider != null)
        {
            boxCollider.size = Vector2.one;
            boxCollider.offset = Vector2.zero;
        }
    }
    public override void OnNetworkSpawn()
    {
        float halfCameraHeight = NetworkGameManager.Instance.CameraHalfHeight;

        maxY = halfCameraHeight - height / 2;
        minY = -maxY;
    }
    public void ResetPositionServer()
    {
        if (!IsServer)
            return;

        CurrentSpeed = 0f;

        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;

        Vector3 currentPosition = transform.position;
        currentPosition.y = 0f;

        transform.position = currentPosition;
    }

    public void SetIsPlayingServer(bool isPlaying)
    {
        if (!IsServer)
            return;

        this.isPlaying = isPlaying;

        if (!isPlaying)
        {
            CurrentSpeed = 0;
            rigidBody.linearVelocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;
        }
    }

    public void MoveServer(float input)
    {
        if (!IsServer)
            return;

        if (!isPlaying)
            return;

        input = Mathf.Clamp(input, -1f, 1f);

        if (Mathf.Approximately(input, 0f))
        {
            CurrentSpeed = 0f;
        }
        else
        {
            float targetSpeed = input * maxSpeed;

            CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }

        Vector2 nextPosition = rigidBody.position + Vector2.up * CurrentSpeed * Time.fixedDeltaTime;

        nextPosition.y = Mathf.Clamp(nextPosition.y, minY, maxY);

        rigidBody.MovePosition(nextPosition);
    }
}
