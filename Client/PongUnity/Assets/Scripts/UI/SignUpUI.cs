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

    private bool isIdValid;
    private bool isIdDuplicateChecked;

    private bool isPasswordValid;
    private bool isPasswordConfirmed;

    private bool isNicknameValid;
    private bool isNicknameDuplicateChecked;

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

    private void ResetUI()
    {
        idInputField.text = "";
        passwordInputField.text = "";
        passwordConfirmInputField.text = "";
        nicknameInputField.text = "";
    }
}
