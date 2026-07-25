using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class ConnectionApprovalHandler : MonoBehaviour
{
    [SerializeField] private int maxPlayerCount = 2;

    [SerializeField]  private NetworkManager networkManager;

    private bool isMatchLocked;

    private void Awake()
    {
        networkManager.ConnectionApprovalCallback = ApprovalCheck;
        networkManager.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDestroy()
    {
        if (networkManager == null)
            return;

        networkManager.ConnectionApprovalCallback = null;
        networkManager.OnClientConnectedCallback -= OnClientConnected;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        int connectedPlayerCount = networkManager.ConnectedClientsIds.Count;

        bool canJoin = !isMatchLocked && connectedPlayerCount < maxPlayerCount;

        response.Approved = canJoin;
        response.CreatePlayerObject = canJoin;
        response.Pending = false;
        response.Reason = canJoin ? string.Empty : "The match is full or already in progress.";

        Debug.Log($"ApprovalCheck | " + $"ClientId: {request.ClientNetworkId}, " + $"Connected: {connectedPlayerCount}, " + $"Approved: {canJoin}");
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!networkManager.IsServer)
            return;

        if (networkManager.ConnectedClientsIds.Count >= maxPlayerCount)
        {
            CloseMatchServer();
        }
    }

    public void OpenMatchServer()
    {
        if (!networkManager.IsServer)
            return;

        isMatchLocked = false;

        Debug.Log("Match connection opened.");
    }

    public void CloseMatchServer()
    {
        if (!networkManager.IsServer)
            return;

        isMatchLocked = true;

        Debug.Log("Match connection locked.");
    }
}
