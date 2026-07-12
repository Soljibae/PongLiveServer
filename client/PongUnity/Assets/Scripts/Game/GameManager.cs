using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public int LeftScore { get; private set; }
    public int RightScore { get; private set; }

    [SerializeField] private int targetScore;
    [SerializeField] private int countDown;
    [SerializeField] private CountdownUI countdownUI;
    [SerializeField] private ScoreboardUI scoreboardUI;

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

        //SpawnOBJ

        WaitGame();
    }

    private void WaitGame()
    {
        CurrentState = GameState.Waiting;
        Debug.Log(CurrentState);

        StartCoroutine(WaitingRoutine());
    }

    private IEnumerator WaitingRoutine()
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

        //activate obj
    }
    private void StopGame()
    {
        CurrentState = GameState.End;
        Debug.Log(CurrentState);

        //deactivate obj
    }

    public void AddScore(bool isLeft)
    {
        if (CurrentState != GameState.Playing)
            return;

        if (isLeft)
        {
            scoreboardUI.SetScoreboardText(++LeftScore, RightScore);
        }
        else
        {
            scoreboardUI.SetScoreboardText(LeftScore, ++RightScore);
        }

        if(LeftScore >= targetScore || RightScore >= targetScore)
            StopGame();
    }
}
