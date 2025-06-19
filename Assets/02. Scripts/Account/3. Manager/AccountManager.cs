using Firebase.Auth;
using Firebase.Firestore;
using Firebase;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public static AccountManager Instance;

    private Account _myAccount;
    public AccountDTO CurrentAccount => _myAccount?.ToDTO();

    private IAccountRepository _repository;

    private const string SALT = "123456";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        _repository = new FirestoreAccountRepository();
    }

    public async Task<Result> TryRegisterAsync(string email, string nickname, string password)
    {
        try
        {
            var auth = FirebaseAuth.DefaultInstance;
            var authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);

            var user = authResult.User;
            if (user == null)
            {
                return Result.Fail("회원가입에 실패했습니다.");
            }
            int imageIndex = UnityEngine.Random.Range(0, 5);
            // Firestore에 닉네임 저장
            var db = FirebaseFirestore.DefaultInstance;
            var userData = new Dictionary<string, object>
            {
                { "email", email },
                { "nickname", nickname },
                { "imageIndex", imageIndex }
            };

            await db.Collection("users").Document(user.UserId).SetAsync(userData);
            return Result.Success();
        }
        catch (FirebaseException fae) // FirebaseAuthException으로 변경하여 AuthError에 접근
        {
            Debug.LogError($"Firebase Auth Error: {fae.ErrorCode} - {fae.Message}");

            string errorMessage = "알 수 없는 회원가입 오류가 발생했습니다."; // Default error message

            switch (fae.ErrorCode)
            {
                case (int)AuthError.EmailAlreadyInUse:
                    errorMessage = "이미 사용 중인 이메일 주소입니다.";
                    break;
                case (int)AuthError.OperationNotAllowed:
                    errorMessage = "이메일/비밀번호 로그인이 활성화되어 있지 않습니다. Firebase 콘솔에서 설정을 확인해주세요.";
                    break;
                case (int)AuthError.NetworkRequestFailed:
                    errorMessage = "네트워크 연결에 실패했습니다. 인터넷 연결을 확인해주세요.";
                    break;
                case (int)AuthError.TooManyRequests:
                    errorMessage = "너무 많은 요청이 발생했습니다. 잠시 후 다시 시도해주세요.";
                    break;
                default:
                    errorMessage = $"회원가입 중 오류가 발생했습니다: {fae.Message}";
                    break;
            }
            return Result.Fail(errorMessage);
        }
    }

    public async Task<Result> TryLoginAsync(string email, string password)
    {
        try
        {
            var auth = FirebaseAuth.DefaultInstance;
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            var user = result.User;

            if (user == null)
            {
                // 이 경우는 FirebaseException에서 이미 처리될 가능성이 높지만, 혹시 모를 경우를 대비
                return Result.Fail("로그인 실패: 사용자 정보를 가져올 수 없습니다.");
            }

            // Firestore에서 추가 정보 불러오기 (예: 닉네임)
            var db = FirebaseFirestore.DefaultInstance;
            var snapshot = await db.Collection("users").Document(user.UserId).GetSnapshotAsync();

            if (snapshot.Exists && snapshot.ContainsField("nickname"))
            {
                string nickname = snapshot.GetValue<string>("nickname");
                int imageIndex = snapshot.ContainsField("imageIndex")
                ? snapshot.GetValue<int>("imageIndex") : 0;
                _myAccount = new Account(user.Email, nickname, imageIndex);
            }
            else
            {
                _myAccount = new Account(user.Email, "Unknown", 0);
            }

            return Result.Success(); // 로그인 성공
        }
        catch (FirebaseException fae)
        {
            Debug.LogError($"로그인 실패: {fae.ErrorCode} - {fae.Message}");

            string errorMessage = "알 수 없는 로그인 오류가 발생했습니다.";
            var errorCode = (AuthError)fae.ErrorCode;
            switch (errorCode)
            {
                case AuthError.InvalidEmail:
                case AuthError.WrongPassword:
                case AuthError.UserNotFound:
                    errorMessage = "이메일 또는 비밀번호가 올바르지 않습니다.";
                    break;
                case AuthError.UserDisabled:
                    errorMessage = "이 계정은 비활성화되었습니다. 관리자에게 문의하세요.";
                    break;
                case AuthError.NetworkRequestFailed:
                    errorMessage = "네트워크 연결에 실패했습니다. 인터넷 연결을 확인해주세요.";
                    break;
                case AuthError.TooManyRequests:
                    errorMessage = "너무 많은 로그인 시도가 있었습니다. 잠시 후 다시 시도해주세요.";
                    break;
                default:
                    errorMessage = $"로그인 중 오류가 발생했습니다: {fae.Message}";
                    break;
            }
            return Result.Fail(errorMessage, fae.ErrorCode);
        }
    }



    public string GetMyNickname() => _myAccount?.Nickname ?? string.Empty;
    public string GetMyEmail() => _myAccount?.Email ?? string.Empty;
}
