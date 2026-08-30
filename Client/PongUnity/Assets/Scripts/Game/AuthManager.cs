using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public NicknameUI nicknameUI;

    public static AuthManager Instance { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public string Nickname { get; private set; } = string.Empty;

    public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void SetLoginInfo(
        string token,
        string nickname)
    {
        Token = token;
        Nickname = nickname;

        if(nicknameUI)
        {
            nicknameUI.SetNickname(nickname);
        }
    }

    public void Logout()
    {
        Token = string.Empty;
        Nickname = string.Empty;
    }
}
