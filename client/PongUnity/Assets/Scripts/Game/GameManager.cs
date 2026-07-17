using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public int LeftScore { get; private set; }
    public int RightScore { get; private set; }

    [Header("Game Setting")]
    [SerializeField] private int targetScore;
    [SerializeField] private int countDown;

    [Header("UI")]
    [SerializeField] private CountdownUI countdownUI;
    [SerializeField] private ScoreboardUI scoreboardUI;
    [SerializeField] private WinUI winUI;

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

    [SerializeField] private PlayerSide localPlayerSide = PlayerSide.Left; //로컬은 left 고정, 멀티 환경에서 수정

    private Paddle leftPaddle;
    private Paddle rightPaddle;
    private Ball ball;

    private bool isMobile;
    public enum GameState
    {
        Ready,
        Waiting,
        Playing,
        End
    }
    public GameState CurrentState { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitGame();
    }

    private void InitGame()
    {
        CurrentState = GameState.Ready;
        Debug.Log(CurrentState);

        LeftScore = 0;
        RightScore = 0;

        isMobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;

        //SpawnOBJ
        ball = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity, runtimeObjectsParent);
        leftPaddle = Instantiate(paddlePrefab, leftPaddleSpawnPoint.position, Quaternion.identity, runtimeObjectsParent);
        rightPaddle = Instantiate(paddlePrefab, rightPaddleSpawnPoint.position, Quaternion.identity, runtimeObjectsParent);

        SetupLocalPlayerInput();

        WaitGame();
    }

    private void WaitGame()
    {
        CurrentState = GameState.Waiting;
        Debug.Log(CurrentState);

        StartCoroutine(CountdownWaitingRoutine());
    }

    private IEnumerator CountdownWaitingRoutine()
    {
        //yield return new WaitForSeconds(countDown);

        for (int i = countDown; i > 0; i--)
        {
            countdownUI.ShowNumber(i);
            yield return new WaitForSeconds(1f);
        }

        StartGame();
    }

    private void StartGame()
    {
        CurrentState = GameState.Playing;
        Debug.Log(CurrentState);

        countdownUI.Hide();
        scoreboardUI.SetScoreboardText(LeftScore, RightScore);

        if (isMobile)
        { }
        else
            keyboardInput.SetInputEnabled(true);

            //activate obj
        ball.SetIsPlaying(true);
        ball.ResetBall();
        ball.Launch();

        leftPaddle.SetIsPlaying(true);
        rightPaddle.SetIsPlaying(true);
    }
    private void StopGame()
    {
        CurrentState = GameState.End;
        Debug.Log(CurrentState);

        if (isMobile)
        { }
        else
            keyboardInput.SetInputEnabled(false);

        //deactivate obj
        ball.SetIsPlaying(false);
        leftPaddle.SetIsPlaying(false);
        rightPaddle.SetIsPlaying(false);

        StartCoroutine(ReturnWaitingRoutine());
    }

    private IEnumerator ReturnWaitingRoutine()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("MainMenuScene");
    }

    public void AddScore(bool scoreForLeft)
    {
        if (CurrentState != GameState.Playing)
            return;

        if (scoreForLeft)
        {
            scoreboardUI.SetScoreboardText(++LeftScore, RightScore);
        }
        else
        {
            scoreboardUI.SetScoreboardText(LeftScore, ++RightScore);
        }

        if(LeftScore >= targetScore || RightScore >= targetScore)
        {
            winUI.ShowText(scoreForLeft);
            StopGame();
            return;
        }

        ball.ResetBall();
        ball.Launch();
    }

    private Paddle GetLocalPlayerPaddle()
    {
        if (localPlayerSide == PlayerSide.Left)
            return leftPaddle;

        return rightPaddle;
    }

    private void SetupLocalPlayerInput()
    {
        Paddle controlledPaddle = GetLocalPlayerPaddle();

        if (keyboardInput != null)
        {
            keyboardInput.gameObject.SetActive(true);

            keyboardInput.Init(controlledPaddle, Key.W, Key.S);
        }
    }
}
