using Assets.Scripts.Rule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpUI : MonoBehaviour
{
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
