using System;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class UI_InputFields
{
    public TextMeshProUGUI ResultText;  // 결과 텍스트
    public TMP_InputField EmailInputField;
    public TMP_InputField NicknameInputField;
    public TMP_InputField PasswordInputField;
    public TMP_InputField PasswordComfirmInputField;
    public Button ConfirmButton;   // 로그인 or 회원가입 버튼
}

public class UI_LoginScene : MonoBehaviour
{
    [Header("패널")]
    public GameObject LoginPanel;
    public GameObject ResisterPanel;

    [Header("로그인")]
    public UI_InputFields LoginInputFields;

    [Header("회원가입")]
    public UI_InputFields RegisterInputFields;

    private const string PREFIX = "ID_";
    private const string SALT = "10043420";



    // 게임 시작하면 로그인 켜주고 회원가입은 꺼주고..
    private void Start()
    {
        LoginPanel.SetActive(true);
        ResisterPanel.SetActive(false);

        LoginInputFields.ResultText.text = string.Empty;
        RegisterInputFields.ResultText.text = string.Empty;
    }

    // 회원가입하기 버튼 클릭
    public void OnClickGoToResisterButton()
    {
        LoginPanel.SetActive(false);
        ResisterPanel.SetActive(true);
    }

    public void OnClickGoToLoginButton()
    {
        LoginPanel.SetActive(true);
        ResisterPanel.SetActive(false);
    }


    // 회원가입
    public async void Resister()
    {
        string email = RegisterInputFields.EmailInputField.text;
        var emailSpecification = new AccountEmailSpecification();
        if (!emailSpecification.IsSatisfiedBy(email))
        {
            RegisterInputFields.ResultText.text = emailSpecification.ErrorMessage;
            return;
        }

        string nickname = RegisterInputFields.NicknameInputField.text;
        var nicknameSpecification = new AccountNicknameSpecification();
        if (!nicknameSpecification.IsSatisfiedBy(nickname))
        {
            RegisterInputFields.ResultText.text = nicknameSpecification.ErrorMessage;
            return;
        }

        string password = RegisterInputFields.PasswordInputField.text;
        var passwordSpecification = new AccountPasswordSpecification();
        if (!passwordSpecification.IsSatisfiedBy(password))
        {
            RegisterInputFields.ResultText.text = passwordSpecification.ErrorMessage;
            return;
        }

        string password2 = RegisterInputFields.PasswordComfirmInputField.text;
        if (password != password2)
        {
            RegisterInputFields.ResultText.text = "비밀번호가 다릅니다.";
            return;
        }
        RegisterInputFields.ConfirmButton.interactable = false; // 버튼 비활성화
        RegisterInputFields.ResultText.text = "회원가입 중... 잠시만 기다려주세요.";

        var result = await AccountManager.Instance.TryRegisterAsync(email, nickname, password);
        if (result.IsSuccess)
        {
            OnClickGoToLoginButton();
            RegisterInputFields.ConfirmButton.interactable = true;
        }
        else
        {
            RegisterInputFields.ResultText.text = result.Message;

            RegisterInputFields.ConfirmButton.interactable = true;
        }
    }


    public async void Login()
    {
        string email = LoginInputFields.EmailInputField.text;
        var emailSpecification = new AccountEmailSpecification();
        if (!emailSpecification.IsSatisfiedBy(email))
        {
            LoginInputFields.ResultText.text = emailSpecification.ErrorMessage;
            return;
        }

        string password = LoginInputFields.PasswordInputField.text;
        var passwordSpecification = new AccountPasswordSpecification();
        if (!passwordSpecification.IsSatisfiedBy(password))
        {
            LoginInputFields.ResultText.text = passwordSpecification.ErrorMessage;
            return;
        }

        bool success = await AccountManager.Instance.TryLoginAsync(email, password);
        if (success)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            LoginInputFields.ResultText.text = "아이디 또는 비밀번호가 맞지 않습니다.";
        }
    }
}