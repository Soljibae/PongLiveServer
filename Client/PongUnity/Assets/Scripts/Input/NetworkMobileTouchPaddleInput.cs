using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkMobileTouchPaddleInput : NetworkBehaviour
{
    [SerializeField] private float stopDistance = 0.08f;

    private NetworkPaddle targetPaddle;
    private Camera targetCamera;

    private readonly NetworkVariable<NetworkObjectReference> targetPaddleReference = new();

    private bool isInputEnabled;
    private float currentInput;

    public void ConfigureTargetPaddleServer(NetworkPaddle paddle)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        targetPaddleReference.Value = new NetworkObjectReference(paddle.NetworkObject);
    }

    public override void OnNetworkSpawn()
    {

        targetPaddleReference.OnValueChanged += HandleTargetPaddleChanged;

        bool shouldEnable = IsOwner && Application.isMobilePlatform;

        enabled = shouldEnable;

        if (!shouldEnable)
            return;

        targetCamera = Camera.main;

        if (targetCamera == null)
        {
            Debug.LogError("Main Camera not found", this);
        }

        ResolveTargetPaddle(targetPaddleReference.Value);
    }

    public override void OnNetworkDespawn()
    {
        targetPaddleReference.OnValueChanged -= HandleTargetPaddleChanged;
    }

    private void HandleTargetPaddleChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
    {
        if (!IsOwner || !Application.isMobilePlatform)
        {
            return;
        }

        ResolveTargetPaddle(newValue);
    }

    private void ResolveTargetPaddle(NetworkObjectReference paddleReference)
    {
        if (!paddleReference.TryGet(out NetworkObject paddleObject))
        {
            targetPaddle = null;
            return;
        }

        targetPaddle = paddleObject.GetComponent<NetworkPaddle>();

        if (targetPaddle == null)
        {
            Debug.LogError("Paddle not found", paddleObject);
        }
    }

    public void SetInputEnabled(bool enabled)
    {
        if (!IsOwner)
            return;

        isInputEnabled = enabled;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        currentInput = 0f;

        if (!isInputEnabled)
            return;

        if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
            NetworkGameManager.Instance.ToggleLeaveUIState();

        if (targetPaddle == null)
            return;

        if (targetCamera == null)
            return;

        if (!TryGetPointerScreenPosition(out Vector2 screenPosition))
            return;

        float distanceFromCamera = Mathf.Abs(targetCamera.transform.position.z - targetPaddle.transform.position.z);

        Vector3 worldPosition = targetCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, distanceFromCamera));

        float yDifference = worldPosition.y - targetPaddle.transform.position.y;

        if (Mathf.Abs(yDifference) <= stopDistance)
        {
            currentInput = 0f;
        }
        else if (yDifference > 0f)
        {
            currentInput = 1f;
        }
        else
        {
            currentInput = -1f;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        if (!isInputEnabled)
            return;

        if (targetPaddle == null)
            return;

        SendInputServerRpc(currentInput);
    }

    private bool TryGetPointerScreenPosition(out Vector2 screenPosition)
    {
        screenPosition = Vector2.zero;

        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.isPressed)
            {
                screenPosition = touch.position.ReadValue();
                return true;
            }
        }

#if UNITY_EDITOR
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                screenPosition = Mouse.current.position.ReadValue();
                return true;
            }
        }
#endif

        return false;
    }

    [ServerRpc]
    private void SendInputServerRpc(float input, ServerRpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;

        NetworkGameManager.Instance.SetPlayerInput(senderClientId, Mathf.Clamp(input, -1f, 1f));
    }
}
