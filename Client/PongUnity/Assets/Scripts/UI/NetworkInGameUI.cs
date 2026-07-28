using UnityEngine;
using TMPro;
public class NetworkInGameUI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] public TextMeshProUGUI watingText;
    [SerializeField] public TextMeshProUGUI leftPlayerName;
    [SerializeField] public TextMeshProUGUI rightPlayerName;
    [SerializeField] public CountdownUI countdownUI;
    [SerializeField] public ScoreboardUI scoreboardUI;
    [SerializeField] public WinUI winUI;
    [SerializeField] public LeaveUI leaveUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
