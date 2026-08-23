using Assets.Scripts.Rule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpUI : MonoBehaviour
{
    [SerializeField] private AccountApiClient accountApiClient;

    [Header("ID")]
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_Text idStatusText;
    [SerializeField] private Button idDuplicateCheckButton;

    [Header("Password")]
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField passwordConfirmInputField;
    [SerializeField] private TMP_Text passwordStatusText;

    [Header("Nickname")]
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private TMP_Text nicknameStatusText;
    [SerializeField] private Button nicknameDuplicateCheckButton;

    [Header("Sign Up")]
    [SerializeField] private Button signUpButton;
    [SerializeField] private Button signUpSubmitButton;
    [SerializeField] private TMP_Text signUpResultText;

    [SerializeField] private GameObject signUpPanel;
    [SerializeField] private GameObject signUpResultPanel;

    private enum InputStatus
    {
        Default,
        Valid,
        Invalid
    }

    private Color defaultColor = Color.black;
    private Color validColor = Color.green;
    private Color invalidColor = Color.red;

    private bool isIdValid;
    private bool isIdDuplicateChecked;

    private bool isPasswordValid;
    private bool isPasswordConfirmed;

    private bool isNicknameValid;
    private bool isNicknameDuplicateChecked;

    private void Awake()
    {
        idInputField.onValueChanged.AddListener(OnIdChanged);

        passwordInputField.onValueChanged.AddListener(OnPasswordChanged);
        passwordConfirmInputField.onValueChanged.AddListener(OnPasswordConfirmChanged);

        nicknameInputField.onValueChanged.AddListener(OnNicknameChanged);
    }

    private void OnEnable()
    {
        ResetUI();
    }

    private void OnDestroy()
    {
        idInputField.onValueChanged.RemoveListener(OnIdChanged);

        passwordInputField.onValueChanged.RemoveListener(OnPasswordChanged);
        passwordConfirmInputField.onValueChanged.RemoveListener(OnPasswordConfirmChanged);

        nicknameInputField.onValueChanged.RemoveListener(OnNicknameChanged);
    }

    private void OnIdChanged(string value)
    {
        isIdValid = IsAlphaNumeric(
            value,
            AccountRules.IdMinLength,
            AccountRules.IdMaxLength
        );

        isIdDuplicateChecked = false;

        if (string.IsNullOrEmpty(value))
        {
            SetStatusText(
                idStatusText,
                "Check ID availability.",
                InputStatus.Default
            );
        }
        else if (!isIdValid)
        {
            SetStatusText(
                idStatusText,
                "ID does not meet the requirements.",
                InputStatus.Invalid
            );
        }
        else
        {
            SetStatusText(
                idStatusText,
                "Check ID availability.",
                InputStatus.Default
            );
        }

        idDuplicateCheckButton.interactable = isIdValid;

        UpdateSignUpButtonState();
    }

    public void SetIdDuplicateCheckResult(bool isAvailable)
    {
        isIdDuplicateChecked = isAvailable;

        if (isAvailable)
        {
            SetStatusText(
                idStatusText,
                "ID is available.",
                InputStatus.Valid
            );
        }
        else
        {
            SetStatusText(
                idStatusText,
                "ID is already in use.",
                InputStatus.Invalid
            );
        }

        UpdateSignUpButtonState();
    }

    private void OnPasswordChanged(string value)
    {
        UpdatePasswordStatus();
        UpdateSignUpButtonState();
    }

    private void OnPasswordConfirmChanged(string value)
    {
        UpdatePasswordStatus();
        UpdateSignUpButtonState();
    }

    private void UpdatePasswordStatus()
    {
        string password = passwordInputField.text;
        string confirmPassword = passwordConfirmInputField.text;

        isPasswordValid = IsAlphaNumeric(
            password,
            AccountRules.PasswordMinLength,
            AccountRules.PasswordMaxLength
        );
        isPasswordConfirmed = false;

        if (string.IsNullOrEmpty(password) &&
            string.IsNullOrEmpty(confirmPassword))
        {
            SetStatusText(
                passwordStatusText,
                "Confirm your password.",
                InputStatus.Default
            );

            return;
        }

        if (!isPasswordValid)
        {
            SetStatusText(
                passwordStatusText,
                "Password does not meet the requirements.",
                InputStatus.Invalid
            );

            return;
        }

        if (string.IsNullOrEmpty(confirmPassword))
        {
            SetStatusText(
                passwordStatusText,
                "Confirm your password.",
                InputStatus.Default
            );

            return;
        }

        if (password == confirmPassword)
        {
            isPasswordConfirmed = true;

            SetStatusText(
                passwordStatusText,
                "Passwords match.",
                InputStatus.Valid
            );
        }
        else
        {
            SetStatusText(
                passwordStatusText,
                "Passwords do not match.",
                InputStatus.Invalid
            );
        }
    }

    private void OnNicknameChanged(string value)
    {
        isNicknameValid = IsAlphaNumeric(
            value,
            AccountRules.NicknameMinLength,
            AccountRules.NicknameMaxLength
        );

        isNicknameDuplicateChecked = false;

        if (string.IsNullOrEmpty(value))
        {
            SetStatusText(
                nicknameStatusText,
                "Check nickname availability.",
                InputStatus.Default
            );
        }
        else if (!isNicknameValid)
        {
            SetStatusText(
                nicknameStatusText,
                "Nickname does not meet the requirements.",
                InputStatus.Invalid
            );
        }
        else
        {
            SetStatusText(
                nicknameStatusText,
                "Check nickname availability.",
                InputStatus.Default
            );
        }

        nicknameDuplicateCheckButton.interactable = isNicknameValid;

        UpdateSignUpButtonState();
    }

    public void SetNicknameDuplicateCheckResult(bool isAvailable)
    {
        isNicknameDuplicateChecked = isAvailable;

        if (isAvailable)
        {
            SetStatusText(
                nicknameStatusText,
                "Nickname is available.",
                InputStatus.Valid
            );
        }
        else
        {
            SetStatusText(
                nicknameStatusText,
                "Nickname is already in use.",
                InputStatus.Invalid
            );
        }

        UpdateSignUpButtonState();
    }

    public void OnSignUpButtonClicked()
    {
        signUpPanel.SetActive(true);
    }

    public void OnSignUpOffButtonClicked()
    {
        signUpPanel.SetActive(false);

        ResetUI();
    }

    public void OnOKButtonClicked()
    {
        signUpResultPanel.SetActive(false);

        ResetUI();
    }

    public void OnIdDuplicateCheckClicked()
    {
        string id = idInputField.text;

        StartCoroutine(
            accountApiClient.CheckId(
                id,
                OnIdCheckCompleted,
                OnIdCheckError
            )
        );
    }

    private void OnIdCheckCompleted(
    bool available,
    string message)
    {
        isIdDuplicateChecked = available;

        if (available)
        {
            idDuplicateCheckButton.interactable = false;
        }

        SetStatusText(
            idStatusText,
            message,
            available
                ? InputStatus.Valid
                : InputStatus.Invalid
        );

        UpdateSignUpButtonState();
    }

    private void OnIdCheckError(string error)
    {
        isIdDuplicateChecked = false;

        SetStatusText(
            idStatusText,
            "Failed to check ID.",
            InputStatus.Invalid
        );

        Debug.LogError(error);

        UpdateSignUpButtonState();
    }

    public void OnNicknameDuplicateCheckClicked()
    {
        string nickname = nicknameInputField.text;

        StartCoroutine(
            accountApiClient.CheckNickname(
                nickname,
                OnNicknameCheckCompleted,
                OnNicknameCheckError
            )
        );
    }

    private void OnNicknameCheckCompleted(
    bool available,
    string message)
    {
        isNicknameDuplicateChecked = available;

        if(available)
        {
            nicknameDuplicateCheckButton.interactable = false;
        }

        SetStatusText(
            nicknameStatusText,
            message,
            available
                ? InputStatus.Valid
                : InputStatus.Invalid
        );

        UpdateSignUpButtonState();
    }

    private void OnNicknameCheckError(string error)
    {
        isNicknameDuplicateChecked = false;

        SetStatusText(
            nicknameStatusText,
            "Failed to check nickname.",
            InputStatus.Invalid
        );

        Debug.LogError(error);

        UpdateSignUpButtonState();
    }

    public void OnSignUpSubmitClicked()
    {
        signUpSubmitButton.interactable = false;

        string id = idInputField.text;
        string password = passwordInputField.text;
        string nickname = nicknameInputField.text;

        StartCoroutine(
            accountApiClient.SignUp(
                id,
                password,
                nickname,
                OnSignUpCompleted,
                OnSignUpError
            )
        );
    }

    private void OnSignUpCompleted(
    bool success,
    string message)
    {
        signUpResultPanel.SetActive(true);
        signUpResultText.text = message;

        if (success)
        {
            signUpResultText.color = validColor;
            signUpPanel.SetActive(false);
        }
        else
        {
            signUpResultText.color = invalidColor;

            ResetUI();
        }
    }

    private void OnSignUpError(string error)
    {
        signUpResultPanel.SetActive(true);
        signUpResultText.text = "Sign up failed.";

        ResetUI();

        Debug.LogError(error);
    }

    private void UpdateSignUpButtonState()
    {
        signUpButton.interactable =
            isIdValid &&
            isIdDuplicateChecked &&
            isPasswordValid &&
            isPasswordConfirmed &&
            isNicknameValid &&
            isNicknameDuplicateChecked;
    }

    private void SetStatusText(TMP_Text statusText, string message, InputStatus status)
    {
        statusText.text = message;

        switch (status)
        {
            case InputStatus.Default:
                statusText.color = defaultColor;
                break;

            case InputStatus.Valid:
                statusText.color = validColor;
                break;

            case InputStatus.Invalid:
                statusText.color = invalidColor;
                break;
        }
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
        passwordConfirmInputField.text = string.Empty;

        nicknameInputField.text = string.Empty;

        SetStatusText(
            idStatusText,
            "Check ID availability.",
            InputStatus.Default
        );

        SetStatusText(
            passwordStatusText,
            "Confirm your password.",
            InputStatus.Default
        );

        SetStatusText(
            nicknameStatusText,
            "Check nickname availability.",
            InputStatus.Default
        );

        if (signUpResultText != null)
        {
            signUpResultText.text = string.Empty;
            signUpResultText.color = defaultColor;
        }

        isIdValid = false;
        isIdDuplicateChecked = false;

        isPasswordValid = false;
        isPasswordConfirmed = false;

        isNicknameValid = false;
        isNicknameDuplicateChecked = false;

        idDuplicateCheckButton.interactable = false;
        nicknameDuplicateCheckButton.interactable = false;

        signUpButton.interactable = false;
    }
}
