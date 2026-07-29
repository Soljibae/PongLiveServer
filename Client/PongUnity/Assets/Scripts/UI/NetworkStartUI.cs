using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkStartUI : MonoBehaviour
{
    private bool shouldLoadGameScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager is missing.");
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
    }

    public void StartHost()
    {
        shouldLoadGameScene = true;

        bool result = NetworkManager.Singleton.StartHost();

        if (!result)
            shouldLoadGameScene = false;


        Debug.Log($"StartHost result: {result}");
    }

    public void StartClient()
    {
        bool result = NetworkManager.Singleton.StartClient();

        Debug.Log($"StartClient result: {result}");
    }

    private void OnServerStarted()
    {
        Debug.Log("Server started.");

        if (shouldLoadGameScene)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("NetworkGameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
            Debug.Log("Load Scene.");
        }      
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected. ClientId: {clientId}");

        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log($"Host ½ÇÇà Áß / Client ¿¬°áµÊ: {clientId}");
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log($"Client Á¢¼Ó ¼º°ø / ³» ClientId: {NetworkManager.Singleton.LocalClientId}");
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected. ClientId: {clientId}");
        Debug.Log($"Client ¿¬°á ²÷±è: {clientId}");
    }
}
