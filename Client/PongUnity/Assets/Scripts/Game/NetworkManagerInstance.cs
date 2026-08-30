using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkManagerInstance : MonoBehaviour
{
    [SerializeField] private UnityTransport unityTransport;
    [SerializeField] private AccountApiClient accountApiClient;

    public UnityTransport UnityTransport => unityTransport;
    public AccountApiClient AccountApiClient => accountApiClient;

    public static NetworkManagerInstance Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
