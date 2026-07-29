using TMPro;
using UnityEngine;

public class WinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winText;

    public void ShowText(bool isLeft)
    {
        gameObject.SetActive(true);

        if(isLeft)
        {
            winText.text = "Left Win!";
        }
        else
        {
            winText.text = "Right Win!";
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
