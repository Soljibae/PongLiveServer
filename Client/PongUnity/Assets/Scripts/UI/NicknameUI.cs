using TMPro;
using UnityEngine;

public class NicknameUI : MonoBehaviour
{
    [SerializeField] public TMP_Text nickNameText;
    [SerializeField] public GameObject backGroundWhite;
    [SerializeField] public GameObject backGroundBlack;

    private void Awake()
    {
        AuthManager.Instance.nicknameUI = this;

        if (AuthManager.Instance.IsLoggedIn)
        {
            SetNickname(AuthManager.Instance.Nickname);
        }
    }

    public void SetNickname(string nickname)
    {
        nickNameText.text = nickname;
        nickNameText.gameObject.SetActive(true);
        backGroundWhite.SetActive(true);
        backGroundBlack.SetActive(true);
    }
}
