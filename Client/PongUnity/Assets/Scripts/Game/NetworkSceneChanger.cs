using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkManager))]
public class NetworkSceneChanger : MonoBehaviour
{
    private NetworkManager networkManager;

    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();

        networkManager.OnClientStopped += OnLocalClientStopped;
    }

    private void OnDestroy()
    {
        if (networkManager != null)
        {
            networkManager.OnClientStopped -= OnLocalClientStopped;
        }
    }

    private void OnLocalClientStopped(bool wasHost)
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
