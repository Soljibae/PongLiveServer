using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkManager))]
[RequireComponent(typeof(UnityTransport))]
public class DedicatedServerBootstrap : MonoBehaviour
{
    [SerializeField]
    private ushort defaultPort = 7777;

    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private UnityTransport unityTransport;

#if UNITY_SERVER

    private ushort currentPort;
    private bool isShuttingDown = false;

    private void Start()
    {
        StartDedicatedServer();
    }

    private void OnDestroy()
    {
        if (networkManager == null)
            return;

        networkManager.OnServerStarted -= OnServerStarted;
    }

    private void StartDedicatedServer()
    {
        if (networkManager.IsListening)
        {
            Debug.LogWarning("NetworkManager is already listening.");
            return;
        }

        currentPort = ReadPortArgument(defaultPort);

        unityTransport.SetConnectionData("127.0.0.1", currentPort,"0.0.0.0");

        networkManager.OnServerStarted += OnServerStarted;

        bool result = networkManager.StartServer();

        Debug.Log( $"StartServer result: {result}, Port: {currentPort}");

        if (result)
            return;

        networkManager.OnServerStarted -= OnServerStarted;

        Debug.LogError($"Dedicated Server start failed. Port: {currentPort}");

        Application.Quit(1);
    }

    private void OnServerStarted()
    {
        Debug.Log($"Dedicated Server started. Port: {currentPort}");

        networkManager.SceneManager.LoadScene("NetworkGameScene", LoadSceneMode.Single);
    }

    private static ushort ReadPortArgument(ushort fallbackPort)
    {
        string[] arguments = Environment.GetCommandLineArgs();

        for (int i = 0; i < arguments.Length - 1; i++)
        {
            bool isPortArgument = arguments[i].Equals("-port", StringComparison.OrdinalIgnoreCase);

            if (!isPortArgument)
                continue;

            bool parsed = ushort.TryParse(arguments[i + 1], out ushort port);

            if (parsed)
                return port;

            Debug.LogWarning($"Invalid port argument: {arguments[i + 1]}");

            return fallbackPort;
        }

        return fallbackPort;
    }

    public void ShutdownDedicatedServer()
    {
        if (isShuttingDown)
            return;

        isShuttingDown = true;

        Debug.Log("Dedicated Server shutdown requested.");

        if (networkManager != null && networkManager.IsListening)
        {
            networkManager.Shutdown();
        }

        Debug.Log("Dedicated Server process will exit.");

        Application.Quit(0);
    }

#endif

}
