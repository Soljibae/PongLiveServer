using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class NetworkGameManager : NetworkBehaviour
{
    [Header("Game Setting")]
    [SerializeField] private int targetScore;
    [SerializeField] private int countDown;

    [Header("UI")]
    [SerializeField] private NetworkInGameUI networkInGameUI;

    [Header("Prefabs")]
    [SerializeField] private Paddle paddlePrefab;
    [SerializeField] private Ball ballPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform leftPaddleSpawnPoint;
    [SerializeField] private Transform rightPaddleSpawnPoint;
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private Transform runtimeObjectsParent;

    [Header("Input Controllers")]
    [SerializeField] private KeyboardPaddleInput keyboardInput;
    [SerializeField] private MobileTouchPaddleInput mobileTouchInput;

    public static NetworkGameManager Instance { get; private set; }

    private const ulong EmptyClientId = ulong.MaxValue;

    private readonly NetworkVariable<ulong> leftClientId = new NetworkVariable<ulong>(EmptyClientId);

    private readonly NetworkVariable<ulong> rightClientId = new NetworkVariable<ulong>(EmptyClientId);

    private readonly NetworkVariable<int> playerCount = new NetworkVariable<int>(0);

    private readonly NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.None);

    private int requiredPlayerCount = 2;

    /*
    public int LeftScore { get; private set; }
    public int RightScore { get; private set; } 

    private Paddle leftPaddle;
    private Paddle rightPaddle;
    private Ball ball;

    private bool isMobile;
    */
    public enum GameState : byte
    {
        None,
        Waiting,
        Countdown,
        Playing,
        End
    }

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        leftClientId.OnValueChanged += OnPlayerIdChanged;
        rightClientId.OnValueChanged += OnPlayerIdChanged;
        playerCount.OnValueChanged += OnPlayerCountChanged;
        gameState.OnValueChanged += OnGameStateChanged;

        if (IsServer)
        {
            gameState.Value = GameState.Waiting;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            AssignAlreadyConnectedClients();
            RefreshPlayerCountAndState();
        }

        if (IsClient)
        {
            RefreshPlayerSideUI();
            RefreshGameStateUI(gameState.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        leftClientId.OnValueChanged -= OnPlayerIdChanged;
        rightClientId.OnValueChanged -= OnPlayerIdChanged;
        playerCount.OnValueChanged -= OnPlayerCountChanged;
        gameState.OnValueChanged -= OnGameStateChanged;

        if (NetworkManager.Singleton == null)
            return;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void AssignAlreadyConnectedClients()
    {
        if (!IsServer)
            return;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            AssignPlayer(clientId);
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer)
            return;

        Debug.Log($"Client connected: {clientId}");

        AssignPlayer(clientId);
        RefreshPlayerCountAndState();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        //ºÎÀü½Â ·ÎÁ÷
    }

    private void AssignPlayer(ulong clientId)
    {
        if (!IsServer)
            return;

        if (leftClientId.Value == clientId || rightClientId.Value == clientId)
            return;

        if (leftClientId.Value == EmptyClientId)
        {
            leftClientId.Value = clientId;
            Debug.Log($"Client {clientId} assigned to Left.");
            return;
        }

        if (rightClientId.Value == EmptyClientId)
        {
            rightClientId.Value = clientId;
            Debug.Log($"Client {clientId} assigned to Right.");
            return;
        }

        Debug.LogWarning($"Room is full. Client {clientId} cannot be assigned.");
        NetworkManager.Singleton.DisconnectClient(clientId);
    }

    private void RefreshPlayerCountAndState()
    {
        if (!IsServer)
            return;

        int count = 0;

        if (leftClientId.Value != EmptyClientId)
            count++;

        if (rightClientId.Value != EmptyClientId)
            count++;

        playerCount.Value = count;

        if (count < requiredPlayerCount)
        {
            gameState.Value = GameState.Waiting;
            Debug.Log("Waiting for another player.");
        }
        else
        {
            gameState.Value = GameState.Countdown;
            Debug.Log("Both players joined. Countdown.");
        }
    }

    public PlayerSide GetLocalPlayerSide()
    {
        if (NetworkManager.Singleton == null)
            return PlayerSide.None;

        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        if (localClientId == leftClientId.Value)
            return PlayerSide.Left;

        if (localClientId == rightClientId.Value)
            return PlayerSide.Right;

        return PlayerSide.None;
    }

    private void OnPlayerIdChanged(ulong previousValue, ulong newValue)
    {
        RefreshPlayerSideUI();
    }

    private void OnPlayerCountChanged(int previousValue, int newValue)
    {
        Debug.Log($"Player count changed: {previousValue} -> {newValue}");
    }

    private void OnGameStateChanged(GameState previousValue, GameState newValue)
    {
        Debug.Log($"Game state changed: {previousValue} -> {newValue}");

        RefreshGameStateUI(newValue);

        ApplyGameState(newValue);
    }

    private void RefreshPlayerSideUI()
    {
        if (!IsClient)
            return;

        PlayerSide mySide = GetLocalPlayerSide();

        networkInGameUI.leftPlayerName.gameObject.SetActive(mySide == PlayerSide.Left);
        networkInGameUI.rightPlayerName.gameObject.SetActive(mySide == PlayerSide.Right);

        Debug.Log($"PlayerSide UI On localSide : {mySide}");
    }

    private void RefreshGameStateUI(GameState state)
    {
        if (!IsClient)
            return;

        switch (state)
        {
            case GameState.Waiting:
                networkInGameUI.watingText.gameObject.SetActive(true);
                break;
            case GameState.Countdown:
                networkInGameUI.countdownUI.ShowNumber(countDown);
                break;
            case GameState.Playing:
                networkInGameUI.countdownUI.gameObject.SetActive(false);
                networkInGameUI.scoreboardUI.SetScoreboardText(0, 0);
                break;
            case GameState.End:
                networkInGameUI.countdownUI.gameObject.SetActive(false);
                //
                break;
        }

        Debug.Log($"RefreshGameStateUI: {state}");
    }

    private void ApplyGameState(GameState state)
    {
        if (!IsServer)
            return;

        switch (state)
        {
            case GameState.Countdown:
                StartCoroutine(CountdownRoutine());
                break;
        }
    }

    private IEnumerator CountdownRoutine()
    {
        //yield return new WaitForSeconds(countDown);

        for (int i = countDown; i > 0; i--)
        {
            ShowCountdownClientRpc(i);
            yield return new WaitForSeconds(1f);
        }

        gameState.Value = GameState.Playing;
    }

    [ClientRpc]
    private void ShowCountdownClientRpc(int count)
    {
        if (!IsClient)
            return;

        if (count == 0)
            return;

        networkInGameUI.countdownUI.ShowNumber(count);
    }
}
