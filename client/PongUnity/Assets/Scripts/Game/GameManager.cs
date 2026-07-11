using NUnit.Framework.Constraints;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public int leftScore { get; private set; }
    public int rightScore { get; private set; }

    [SerializeField] private int targetScore;
    [SerializeField] private int countDown;
    [SerializeField] private CountdownUI countdownUI;

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
        CurrentState = GameState.Ready;
        Debug.Log(CurrentState);

        leftScore = 0;
        rightScore = 0;

        //SpawnOBJ


        CurrentState = GameState.Waiting;
        Debug.Log(CurrentState);

        StartCoroutine(WaitingRoutine());
    }

    private IEnumerator WaitingRoutine()
    {
        CurrentState = GameState.Waiting;
        Debug.Log(CurrentState);

        //yield return new WaitForSeconds(countDown);

        for (int i = countDown; i > 0; i--)
        {
            countdownUI.ShowNumber(i);
            yield return new WaitForSeconds(1f);
        }

        countdownUI.Hide();
        CurrentState = GameState.Playing;
        Debug.Log(CurrentState);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
