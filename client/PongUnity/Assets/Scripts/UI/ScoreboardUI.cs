using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreboardText;

    public void SetScoreboardText(int left, int right)
    {
        gameObject.SetActive(true);
        scoreboardText.text = left.ToString() + " : " + right.ToString();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
