using Assets.Scripts.Rule;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using static AccountApiClient;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private AccountApiClient accountApiClient;
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button loginRequestButton;
    [SerializeField] private Button multiPlayButton;
    [SerializeField] private TMP_Text loginResultText;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject loginResultPanel;

    private bool isIdValid;
    private bool isPasswordValid;

    private void OnEnable()
    {
        ResetUI();
    }

    private void Awake()
    {
        idInputField.onValueChanged.AddListener(OnIdChanged);

        passwordInputField.onValueChanged.AddListener(OnPasswordChanged);
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

    public void OnOKButtonClicked()
    {
        loginResultPanel.SetActive(false);

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
        loginResultPanel.SetActive(true);
        loginResultText.text = response.message;

        if (response.success)
        {
            loginPanel.SetActive(false);
            loginButton.gameObject.SetActive(false);
            multiPlayButton.gameObject.SetActive(true);

            Debug.Log($"Login successful: {response.nickname}");
            Debug.Log($"Token: {response.token}");
        }
        else
        {
            ResetUI();
        }
    }

    private void OnLoginError(string error)
    {
        loginResultPanel.SetActive(true);
        loginResultText.text = "Login failed.";

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
