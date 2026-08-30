using TMPro;
using UnityEngine;

public class PopUpUI : MonoBehaviour
{
    [SerializeField] public TMP_Text popUpText;

    public void OnOKButtonClicked()
    {
        this.gameObject.SetActive(false);
        popUpText.text = string.Empty;
    }
}
