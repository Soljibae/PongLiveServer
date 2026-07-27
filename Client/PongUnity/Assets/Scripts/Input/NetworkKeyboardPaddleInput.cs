using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkKeyboardPaddleInput : NetworkBehaviour
{
    [SerializeField] private Key upKey = Key.W;
    [SerializeField] private Key downKey = Key.S;

    private bool isInputEnabled;
    private float currentInput;
    public override void OnNetworkSpawn()
    {
        bool shouldEnable = IsOwner && !Application.isMobilePlatform;

        enabled = shouldEnable;

        if (!shouldEnable)
            return;
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

        if (Keyboard.current[upKey].isPressed)
            currentInput += 1f;

        if (Keyboard.current[downKey].isPressed)
            currentInput -= 1f;

        currentInput = Mathf.Clamp(currentInput, -1f, 1f);
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        if (!isInputEnabled)
            return;

        SendInputServerRpc(currentInput);
    }

    [ServerRpc]
    private void SendInputServerRpc(float input, ServerRpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;

        NetworkGameManager.Instance.SetPlayerInput(senderClientId, Mathf.Clamp(input, -1f, 1f));
    }
}
