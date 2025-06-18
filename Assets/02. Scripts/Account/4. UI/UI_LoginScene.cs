using System;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using RainbowArt.CleanFlatUI;
using Firebase.Auth;
using System.Collections;

[Serializable]
public class UI_InputFields
{
    //public TextMeshProUGUI ResultText;  // 결과 텍스트 - 안써
    public TMP_InputField EmailInputField;
    public TMP_InputField NicknameInputField;
    public TMP_InputField PasswordInputField;
    //public TMP_InputField PasswordComfirmInputField; // 비밀번호 확인 입력 필드 - 안써
    public Button ConfirmButton;   // 로그인 or 회원가입 버튼

    public TextMeshProUGUI EmailResultText;
    public TextMeshProUGUI PasswordResultText;
    public GameObject Spinner;

    public Tooltip EmailErrorTooltip;
    public Tooltip PasswordErrorTooltip;
    public Tooltip NicknameErrorTooltip;
    public NotificationWithButton ErrorNotification;
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
    public Color ErrorColor = Color.red; // 에러 메시지 색상
    private const float TransitionDuration = 0.5f; // 패널 전환 시간

    // 게임 시작하면 로그인 켜주고 회원가입은 꺼주고..
    private void Start()
    {
        LoginPanel.SetActive(true);
        ResisterPanel.SetActive(false);

        //RegisterInputFields.ResultText.text = string.Empty;
    }

    // 회원가입하기 버튼 클릭
    public void OnClickGoToRegisterButton()
    {
        StartCoroutine(TransitionPanels(LoginPanel, ResisterPanel));
    }

    public void OnClickGoToLoginButton()
    {
        StartCoroutine(TransitionPanels(ResisterPanel, LoginPanel));
    }
    private IEnumerator TransitionPanels(GameObject fromPanel, GameObject toPanel)
    {
        RectTransform fromRect = fromPanel.GetComponent<RectTransform>();
        RectTransform toRect = toPanel.GetComponent<RectTransform>();

        CanvasGroup fromCanvasGroup = fromPanel.GetComponent<CanvasGroup>();
        CanvasGroup toCanvasGroup = toPanel.GetComponent<CanvasGroup>();

        Vector2 onScreen = Vector2.zero;
        Vector2 aboveScreen = new Vector2(0, Screen.height);
        Vector2 belowScreen = new Vector2(0, -Screen.height);

        Vector3 normalScale = Vector3.one;
        Vector3 smallScale = Vector3.one * 0.8f;

        // 초기 설정
        fromRect.anchoredPosition = onScreen;
        fromRect.localScale = normalScale;
        fromCanvasGroup.alpha = 1f;

        fromCanvasGroup.interactable = false;
        fromCanvasGroup.blocksRaycasts = false;
        toCanvasGroup.interactable = false;
        toCanvasGroup.blocksRaycasts = false;

        toRect.anchoredPosition = belowScreen;
        toRect.localScale = smallScale;
        toCanvasGroup.alpha = 0f;
        toPanel.SetActive(true);

        float time = 0f;
        while (time < TransitionDuration)
        {
            time += Time.deltaTime;
            float t = time / TransitionDuration;

            // 위치 이동
            fromRect.anchoredPosition = Vector2.Lerp(onScreen, aboveScreen, t);
            toRect.anchoredPosition = Vector2.Lerp(belowScreen, onScreen, t);

            // 크기 변화
            fromRect.localScale = Vector3.Lerp(normalScale, smallScale, t);
            toRect.localScale = Vector3.Lerp(smallScale, normalScale, t);

            // 알파 변화
            fromCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            toCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // 마무리 상태 정리
        fromPanel.SetActive(false);
        fromRect.anchoredPosition = onScreen;
        fromRect.localScale = normalScale;
        fromCanvasGroup.alpha = 1f;

        toRect.anchoredPosition = onScreen;
        toRect.localScale = normalScale;
        toCanvasGroup.alpha = 1f;
        fromCanvasGroup.interactable = true;
        fromCanvasGroup.blocksRaycasts = true;
        toCanvasGroup.interactable = true;
        toCanvasGroup.blocksRaycasts = true;
    }

    // 회원가입
    public async void Register()
    {
        string email = RegisterInputFields.EmailInputField.text;
        var emailSpecification = new AccountEmailSpecification();
        if (!emailSpecification.IsSatisfiedBy(email))
        {
            RegisterInputFields.EmailErrorTooltip.DescriptionValue = emailSpecification.ErrorMessage;
            RegisterInputFields.EmailErrorTooltip.ShowTooltip();
            return;
        }

        string nickname = RegisterInputFields.NicknameInputField.text;
        var nicknameSpecification = new AccountNicknameSpecification();
        if (!nicknameSpecification.IsSatisfiedBy(nickname))
        {
            RegisterInputFields.NicknameErrorTooltip.DescriptionValue = nicknameSpecification.ErrorMessage;
            RegisterInputFields.NicknameErrorTooltip.ShowTooltip();
            return;
        }

        string password = RegisterInputFields.PasswordInputField.text;
        var passwordSpecification = new AccountPasswordSpecification();
        if (!passwordSpecification.IsSatisfiedBy(password))
        {
            RegisterInputFields.PasswordErrorTooltip.DescriptionValue = passwordSpecification.ErrorMessage;
            RegisterInputFields.PasswordErrorTooltip.ShowTooltip();
            return;
        }

        //string password2 = RegisterInputFields.PasswordComfirmInputField.text;
        //if (password != password2)
        //{
        //    RegisterInputFields.ResultText.text = "비밀번호가 다릅니다.";
        //    return;
        //}
        RegisterInputFields.ConfirmButton.interactable = false; // 버튼 비활성화
        RegisterInputFields.Spinner.SetActive(true); // 스피너 활성화

        var result = await AccountManager.Instance.TryRegisterAsync(email, nickname, password);
        if (result.IsSuccess)
        {
            OnClickGoToLoginButton();
            RegisterInputFields.ConfirmButton.interactable = true;
            LoginInputFields.EmailInputField.text = email;
            LoginInputFields.PasswordInputField.text = password;
        }
        else
        {
            switch(result.ErrorCode)
            {
                case (int)AuthError.EmailAlreadyInUse:
                    RegisterInputFields.EmailErrorTooltip.DescriptionValue = result.Message;
                    RegisterInputFields.EmailErrorTooltip.ShowTooltip();
                    break;
                case (int)AuthError.OperationNotAllowed:
                case (int)AuthError.NetworkRequestFailed:
                case (int)AuthError.TooManyRequests:
                default:
                    RegisterInputFields.ErrorNotification.DescriptionValue = result.Message;
                    RegisterInputFields.ErrorNotification.ShowNotification();

                    break;
            }

            RegisterInputFields.ConfirmButton.interactable = true;
            RegisterInputFields.Spinner.SetActive(false); // 스피너 비활성화

        }
    }


    public async void Login()
    {
        LoginInputFields.EmailResultText.text = "이메일";
        LoginInputFields.EmailResultText.color = Color.white;
        LoginInputFields.PasswordResultText.text = "비밀번호";
        LoginInputFields.PasswordResultText.color = Color.white;
        string email = LoginInputFields.EmailInputField.text;
        var emailSpecification = new AccountEmailSpecification();
        if (!emailSpecification.IsSatisfiedBy(email))
        {
            LoginInputFields.EmailResultText.text = emailSpecification.ErrorMessage;
            LoginInputFields.EmailResultText.color = ErrorColor; // 에러 메시지 색상 변경
            return;
        }
        string password = LoginInputFields.PasswordInputField.text;
        var passwordSpecification = new AccountPasswordSpecification();
        if (!passwordSpecification.IsSatisfiedBy(password))
        {
            LoginInputFields.PasswordResultText.text = passwordSpecification.ErrorMessage;
            LoginInputFields.PasswordResultText.color = ErrorColor; // 에러 메시지 색상 변경
            return;
        }
        LoginInputFields.ConfirmButton.interactable = false; // 버튼 비활성화
        LoginInputFields.Spinner.SetActive(true); // 스피너 활성화
        var success = await AccountManager.Instance.TryLoginAsync(email, password);
        if (success.IsSuccess)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            var errorCode = (AuthError)success.ErrorCode;
            switch (errorCode)
            {
                case AuthError.InvalidEmail:
                case AuthError.WrongPassword:
                case AuthError.UserNotFound:
                    LoginInputFields.EmailResultText.text = "이메일 - 유효하지 않은 아이디 또는 비밀번호입니다.";
                    LoginInputFields.EmailResultText.color = ErrorColor; // 에러 메시지 색상 변경
                    LoginInputFields.PasswordResultText.text = "비밀번호 - 유효하지 않은 아이디 또는 비밀번호입니다.";
                    LoginInputFields.PasswordResultText.color = ErrorColor; // 에러 메시지 색상 변경
                    break;
                case AuthError.UserDisabled:
                case AuthError.NetworkRequestFailed:
                case AuthError.TooManyRequests:
                    LoginInputFields.ErrorNotification.DescriptionValue = success.Message;
                    LoginInputFields.ErrorNotification.ShowNotification();
                    break;
                default:
                    LoginInputFields.ErrorNotification.DescriptionValue = $"로그인 중 오류가 발생했습니다: {success.Message}";
                    LoginInputFields.ErrorNotification.ShowNotification();
                    break;
            }
            
        }
        LoginInputFields.ConfirmButton.interactable = true; // 버튼 활성화
        LoginInputFields.Spinner.SetActive(false); // 스피너 비활성화
    }
}