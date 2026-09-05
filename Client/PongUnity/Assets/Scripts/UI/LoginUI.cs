using Assets.Scripts.Rule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AccountApiClient;

public class LoginUI : MonoBehaviour
{
    private AccountApiClient accountApiClient;
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button loginRequestButton;
    [SerializeField] private Button multiPlayButton;
    [SerializeField] private Button signUpButton;
    [SerializeField] private Button rankingButton;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private PopUpUI popUpUI;

    private bool isIdValid;
    private bool isPasswordValid;

    private void OnEnable()
    {
        ResetUI();
    }

    private void Awake()
    {
        accountApiClient = NetworkManagerInstance.Instance.AccountApiClient;

        idInputField.onValueChanged.AddListener(OnIdChanged);

        passwordInputField.onValueChanged.AddListener(OnPasswordChanged);

        if(AuthManager.Instance.IsLoggedIn)
        {
            loginButton.gameObject.SetActive(false);
            signUpButton.gameObject.SetActive(false);
            multiPlayButton.gameObject.SetActive(true);
            rankingButton.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        idInputField.onValueChanged.RemoveListener(OnIdChanged);

        passwordInputField.onValueChanged.RemoveListener(OnPasswordChanged);
    }
    public void OnLoginButtonClicked()
    {
        loginPanel.SetActive(true);
    }

    public void OnLoginOffButtonClicked()
    {
        loginPanel.SetActive(false);

        ResetUI();
    }

    public void OnLoginRequestButtonClicked()
    {
        loginRequestButton.interactable = false;

        string id = idInputField.text;
        string password = passwordInputField.text;

        StartCoroutine(
           accountApiClient.Login(
               id,
               password,
               OnLoginCompleted,
               OnLoginError
           )
       );
    }

    private void OnLoginCompleted(LoginResponse response)
    {
        popUpUI.gameObject.SetActive(true);
        popUpUI.popUpText.text = response.message;

        if (response.success)
        {
            loginPanel.SetActive(false);
            loginButton.gameObject.SetActive(false);
            signUpButton.gameObject.SetActive(false);
            multiPlayButton.gameObject.SetActive(true);
            rankingButton.gameObject.SetActive(true);

            Debug.Log($"Login successful: {response.nickname}");
            Debug.Log($"Token: {response.token}");

            AuthManager.Instance.SetLoginInfo(
                response.token,
                response.nickname
            );
        }
        else
        {
            ResetUI();
        }
    }

    private void OnLoginError(string error)
    {
        popUpUI.gameObject.SetActive(true);
        popUpUI.popUpText.text = "Login failed.";

        ResetUI();

        Debug.LogError(error);
    }

    private void OnIdChanged(string value)
    {
        isIdValid = IsAlphaNumeric(
            value,
            AccountRules.IdMinLength,
            AccountRules.IdMaxLength
        );

        UpdateLoginButtonState();
    }

    private void OnPasswordChanged(string value)
    {
        isPasswordValid = IsAlphaNumeric(
            value,
            AccountRules.PasswordMinLength,
            AccountRules.PasswordMaxLength
        );

        UpdateLoginButtonState();
    }

    private void UpdateLoginButtonState()
    {
        loginRequestButton.interactable =
            isIdValid &&
            isPasswordValid;
    }

    private bool IsAlphaNumeric(string value, int minLength, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        if (value.Length < minLength || value.Length > maxLength)
            return false;

        foreach (char c in value)
        {
            bool isUpper = c >= 'A' && c <= 'Z';
            bool isLower = c >= 'a' && c <= 'z';
            bool isNumber = c >= '0' && c <= '9';

            if (!isUpper && !isLower && !isNumber)
                return false;
        }

        return true;
    }

    private void ResetUI()
    {
        idInputField.text = string.Empty;

        passwordInputField.text = string.Empty;
       
        isIdValid = false;

        isPasswordValid = false;
    }
}
