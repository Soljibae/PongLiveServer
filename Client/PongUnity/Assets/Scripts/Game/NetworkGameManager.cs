using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private NetworkPaddle paddlePrefab;
    [SerializeField] private NetworkBall ballPrefab;
    [SerializeField] private NetworkObject paddleControllerPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform leftPaddleSpawnPoint;
    [SerializeField] private Transform rightPaddleSpawnPoint;
    [SerializeField] private Transform ballSpawnPoint;

    [Header("Input Controllers")]
    [SerializeField] private KeyboardPaddleInput keyboardInput;
    [SerializeField] private MobileTouchPaddleInput mobileTouchInput;

    [Header("Camera Setting")]
    [SerializeField] private float cameraHalfHeight = 5f;
    public float CameraHalfHeight => cameraHalfHeight;
    public static NetworkGameManager Instance { get; private set; }

    private const ulong EmptyClientId = ulong.MaxValue;

    private readonly NetworkVariable<ulong> leftClientId = new NetworkVariable<ulong>(EmptyClientId);

    private readonly NetworkVariable<ulong> rightClientId = new NetworkVariable<ulong>(EmptyClientId);

    private readonly NetworkVariable<int> playerCount = new NetworkVariable<int>(0);

    private readonly NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.None);

    private readonly NetworkVariable<int> leftScore = new NetworkVariable<int>(0);

    private readonly NetworkVariable<int> rightScore = new NetworkVariable<int>(0);

    private readonly NetworkVariable<MatchResult> matchResult = new NetworkVariable<MatchResult>(MatchResult.None);

    private ConnectionApprovalHandler connectionApprovalHandler;

    private readonly Dictionary<ulong, NetworkObject> spawnedControllers = new();

    private int requiredPlayerCount = 2;

    private NetworkPaddle leftPaddle;
    private NetworkPaddle rightPaddle;
    private NetworkBall ball;

    /*
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

    public enum MatchResult : byte
    {
        None,
        LeftWinByScore,
        RightWinByScore,
        LeftWinByForfeit,
        RightWinByForfeit
    }

    private void Awake()
    {
        Instance = this;

        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            mainCamera.orthographicSize = cameraHalfHeight;
        }
    }

    public override void OnNetworkSpawn()
    {
        leftClientId.OnValueChanged += OnPlayerIdChanged;
        rightClientId.OnValueChanged += OnPlayerIdChanged;
        playerCount.OnValueChanged += OnPlayerCountChanged;
        gameState.OnValueChanged += OnGameStateChanged;
        leftScore.OnValueChanged += OnScoreChanged;
        rightScore.OnValueChanged += OnScoreChanged;
        matchResult.OnValueChanged += OnMatchResultChanged;

        if (IsServer)
        {
            connectionApprovalHandler = NetworkManager.Singleton.GetComponent<ConnectionApprovalHandler>();

            gameState.Value = GameState.Waiting;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            ball = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
            leftPaddle = Instantiate(paddlePrefab, leftPaddleSpawnPoint.position, Quaternion.identity);
            rightPaddle = Instantiate(paddlePrefab, rightPaddleSpawnPoint.position, Quaternion.identity);

            ball.NetworkObject.Spawn();
            leftPaddle.NetworkObject.Spawn();
            rightPaddle.NetworkObject.Spawn();

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
        leftScore.OnValueChanged -= OnScoreChanged;
        rightScore.OnValueChanged -= OnScoreChanged;
        matchResult.OnValueChanged -= OnMatchResultChanged;


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
        if (!IsServer)
            return;

        bool leftDisconnected = leftClientId.Value == clientId;
        bool rightDisconnected = rightClientId.Value == clientId;

        if (!leftDisconnected && !rightDisconnected)
            return;

        GameState stateBeforeDisconnect = gameState.Value;

        if (leftDisconnected)
        {
            leftClientId.Value = EmptyClientId;
            leftPaddle.ResetPositionServer();
        }
        else
        {
            rightClientId.Value = EmptyClientId;
            rightPaddle.ResetPositionServer();
        }

        spawnedControllers.Remove(clientId);

        if (stateBeforeDisconnect == GameState.Countdown)
        {
            RefreshPlayerCountAndState();
            connectionApprovalHandler?.OpenMatchServer();
            return;
        }

        if (stateBeforeDisconnect == GameState.Playing)
        {
            MatchResult result = leftDisconnected ? MatchResult.RightWinByForfeit : MatchResult.LeftWinByForfeit;

            EndMatchServer(result);

            playerCount.Value = 1;
            return;
        }

        RefreshPlayerCountAndState();
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
            leftPaddle.NetworkObject.ChangeOwnership(clientId);
            SpawnController(clientId, leftPaddle);
            return;
        }

        if (rightClientId.Value == EmptyClientId)
        {
            rightClientId.Value = clientId;
            Debug.Log($"Client {clientId} assigned to Right.");
            rightPaddle.NetworkObject.ChangeOwnership(clientId);
            SpawnController(clientId, rightPaddle);
            return;
        }

        Debug.LogWarning($"Room is full. Client {clientId} cannot be assigned.");
        NetworkManager.Singleton.DisconnectClient(clientId);
    }

    private void SpawnController(ulong clientId, NetworkPaddle targetPaddle)
    {
        if (!IsServer)
            return;

        if (spawnedControllers.ContainsKey(clientId))
            return;

        NetworkObject controllerObject = Instantiate(paddleControllerPrefab);

        controllerObject.SpawnWithOwnership(clientId);

        NetworkMobileTouchPaddleInput mobileInput = controllerObject.GetComponent<NetworkMobileTouchPaddleInput>();

        mobileInput.ConfigureTargetPaddleServer(targetPaddle);

        spawnedControllers.Add(clientId, controllerObject);
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

        ApplyGameStateServer(newValue);

        RefreshGameStateUI(newValue);
    }

    private void OnScoreChanged(int previousValue, int newValue)
    {
        RefreshScoreboardUI();
    }
    private void OnMatchResultChanged(MatchResult previousValue, MatchResult newValue)
    {
        Debug.Log($"Match Resule: {newValue}");


        if (!IsClient)
            return;

        if (gameState.Value == GameState.End)
        {
            RefreshGameStateUI(gameState.Value);
        }
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

    private void RefreshScoreboardUI()
    {
        if (!IsClient)
            return;

        networkInGameUI.scoreboardUI.SetScoreboardText(leftScore.Value, rightScore.Value);
    }

    private void RefreshGameStateUI(GameState state)
    {
        if (!IsClient)
            return;

        switch (state)
        {
            case GameState.Waiting:
                networkInGameUI.watingText.gameObject.SetActive(true);
                networkInGameUI.countdownUI.gameObject.SetActive(false);
                break;
            case GameState.Countdown:
                networkInGameUI.countdownUI.ShowNumber(countDown);
                networkInGameUI.watingText.gameObject.SetActive(false);
                break;
            case GameState.Playing:
                networkInGameUI.countdownUI.gameObject.SetActive(false);
                networkInGameUI.scoreboardUI.SetScoreboardText(0, 0);
                break;
            case GameState.End:
                networkInGameUI.countdownUI.gameObject.SetActive(false);
                switch(matchResult.Value)
                {
                    case MatchResult.LeftWinByScore:
                        networkInGameUI.winUI.ShowText(true);
                        break;
                    case MatchResult.RightWinByScore:
                        networkInGameUI.winUI.ShowText(false);
                        break;
                    case MatchResult.LeftWinByForfeit:
                        networkInGameUI.winUI.ShowText(true);
                        break;
                    case MatchResult.RightWinByForfeit:
                        networkInGameUI.winUI.ShowText(false);
                        break;
                }
                break;
        }

        Debug.Log($"RefreshGameStateUI: {state}");
    }

    private void ApplyGameStateServer(GameState state)
    {
        if (!IsServer)
            return;

        Debug.Log(state);

        switch (state)
        {
            case GameState.Countdown:
                StartCoroutine(CountdownRoutineServer());
                break;
            case GameState.Playing:
                ball.SetIsPlayingServer(true);
                ball.ResetBallServer();
                ball.LaunchServer();
                leftPaddle.SetIsPlayingServer(true);
                rightPaddle.SetIsPlayingServer(true);
                SetAllControllerInputEnabled(true);
                break;
            case GameState.End:
                ball.SetIsPlayingServer(false);
                leftPaddle.SetIsPlayingServer(false);
                rightPaddle.SetIsPlayingServer(false);
                SetAllControllerInputEnabled(false);
                break;
        }
    }

    public void SetAllControllerInputEnabled(bool enabled)
    {
        if (!IsServer)
            return;

        foreach (NetworkObject controllerObject in spawnedControllers.Values)
        {
            if (controllerObject == null)
                continue;

            if (!controllerObject.IsSpawned)
                continue;

            if (!controllerObject.TryGetComponent(out NetworkControllerManager controller))
            {
                Debug.LogError("NetworkControllerManager not found", controllerObject);

                continue;
            }

            controller.SetInputEnabledServer(enabled);
        }
    }

    private IEnumerator CountdownRoutineServer()
    {
        //yield return new WaitForSeconds(countDown);

        for (int i = countDown; i > 0; i--)
        {
            if (gameState.Value != GameState.Countdown)
                yield break;

            ShowCountdownClientRpc(i);
            yield return new WaitForSeconds(1f);
        }

        if (gameState.Value == GameState.Countdown)
        {
            gameState.Value = GameState.Playing;
        }
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

    public void AddScoreServer(bool scoreForLeft)
    {
        if (!IsServer)
            return;

        if (gameState.Value != GameState.Playing)
            return;

        if (scoreForLeft)
        {
            leftScore.Value++;

            if (leftScore.Value >= targetScore)
            {
                EndMatchServer(MatchResult.LeftWinByScore);
                return;
            }
        }
        else
        {
            rightScore.Value++;

            if (rightScore.Value >= targetScore)
            {
                EndMatchServer(MatchResult.RightWinByScore);
                return;
            }
        }

        ball.ResetBallServer();
        ball.LaunchServer();
    }

    private void EndMatchServer(MatchResult result)
    {
        if (!IsServer)
            return;

        if (gameState.Value == GameState.End)
            return;

        matchResult.Value = result;
        gameState.Value = GameState.End;
    }

    public void SetPlayerInput(ulong clientId, float input)
    {
        if (!IsServer)
            return;

        input = Mathf.Clamp(input, -1f, 1f);

        if (clientId == leftClientId.Value)
        {
            leftPaddle.MoveServer(input);
        }
        else if (clientId == rightClientId.Value)
        {
            rightPaddle.MoveServer(input);
        }
    }
}
