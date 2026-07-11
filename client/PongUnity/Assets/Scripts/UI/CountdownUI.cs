using TMPro;
using UnityEngine;

public class CountdownUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI countdownText;
    public void ShowNumber(int number)
    {
        gameObject.SetActive(true);
        countdownText.text = number.ToString();
    }

    public void Hide()
    { 
        gameObject.SetActive(false); 
    }
}

