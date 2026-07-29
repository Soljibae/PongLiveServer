using Unity.Netcode;
using UnityEngine;

public class NetworkControllerManager : NetworkBehaviour
{
    [SerializeField]
    private NetworkKeyboardPaddleInput keyboardInput;

    [SerializeField]
    private NetworkMobileTouchPaddleInput mobileInput;

    public void SetInputEnabledServer(bool enabled)
    {
        if (!IsServer)
            return;

        ClientRpcParams clientRpcParams = new()
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { OwnerClientId }
            }
        };

        SetInputEnabledClientRpc(enabled, clientRpcParams);
    }

    [ClientRpc]
    private void SetInputEnabledClientRpc(bool enabled, ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner)
            return;

        if (Application.isMobilePlatform)
        {
            mobileInput.SetInputEnabled(enabled);
        }
        else
        {
            keyboardInput.SetInputEnabled(enabled);
        }
    }
}
