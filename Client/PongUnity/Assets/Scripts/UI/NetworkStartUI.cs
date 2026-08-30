using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkStartUI : MonoBehaviour
{
    //[Header("Client Connection Settings")]
    //[SerializeField] private string serverAddress = "127.0.0.1";
    //[SerializeField] private ushort serverPort = 7777;

    private UnityTransport unityTransport;
    [SerializeField] private GameObject ServerAddressInputOBJ;
    [SerializeField] private TMP_InputField addressInputField;
    [SerializeField] private TMP_InputField portInputField;

    private void Awake()
    {
        unityTransport = NetworkManagerInstance.Instance.UnityTransport;
    }

    void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager is missing.");
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        //NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        //NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
    }
    public void OnClickStartButton()
    {
        ServerAddressInputOBJ.gameObject.SetActive(true);
    }

    public void OnClickOffButton()
    {
        ServerAddressInputOBJ.gameObject.SetActive(false);
    }

    public void OnClickConnectButton()
    {
        if (addressInputField == null || portInputField == null)
        {
            Debug.LogError("Server address or port input field is missing.");
            return;
        }

        string address = addressInputField.text.Trim();

        if (string.IsNullOrWhiteSpace(address))
        {
            Debug.LogError("Server address is empty.");
            return;
        }

        if (!ushort.TryParse(portInputField.text.Trim(), out ushort port))
        {
            Debug.LogError("Server port must be between 0 and 65535.");
            return;
        }

        Connect(address, port);
    }

    public bool Connect(string address, ushort port)
    {
        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager == null)
        {
            Debug.LogError("NetworkManager is missing.");
            return false;
        }

        if (networkManager.IsListening)
        {
            Debug.LogWarning("NetworkManager is already running.");

            return false;
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            Debug.LogError("Server address is empty.");
            return false;
        }

        if (unityTransport == null)
        {
            Debug.LogError("UnityTransport is missing.");
            return false;
        }

        string trimmedAddress = address.Trim();

        unityTransport.SetConnectionData(trimmedAddress, port);

        bool result = networkManager.StartClient();

        Debug.Log($"StartClient result: {result}, " + $"Server: {trimmedAddress}:{port}");

        return result;
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

    //private bool shouldLoadGameScene;

    //public void StartHost()
    //{
    //    shouldLoadGameScene = true;

    //    bool result = NetworkManager.Singleton.StartHost();

    //    if (!result)
    //        shouldLoadGameScene = false;

    //    Debug.Log($"StartHost result: {result}");
    //}
    //private void OnServerStarted()
    //{
    //    Debug.Log("Server started.");

    //    if (shouldLoadGameScene)
    //    {
    //        NetworkManager.Singleton.SceneManager.LoadScene("NetworkGameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    //        Debug.Log("Load Scene.");
    //    }
    //}
}
