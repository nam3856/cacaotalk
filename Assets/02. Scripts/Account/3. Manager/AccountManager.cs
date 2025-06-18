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

            // Firestore에 닉네임 저장
            var db = FirebaseFirestore.DefaultInstance;
            var userData = new Dictionary<string, object>
        {
            { "email", email },
            { "nickname", nickname }
        };

            await db.Collection("users").Document(user.UserId).SetAsync(userData);
            return Result.Success();
        }
        catch (FirebaseException fe)
        {
            Debug.LogError($"Firebase Error: {fe.ErrorCode} - {fe.Message}");
            return Result.Fail("Firebase 회원가입 오류");
        }
    }

    public async Task<bool> TryLoginAsync(string email, string password)
    {
        try
        {
            var auth = FirebaseAuth.DefaultInstance;
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            var user = result.User;

            if (user == null)
            {
                Debug.LogWarning("Firebase 로그인 실패: 사용자 없음");
                return false;
            }

            // Firestore에서 추가 정보 불러오기 (예: 닉네임)
            var db = FirebaseFirestore.DefaultInstance;
            var snapshot = await db.Collection("users").Document(user.UserId).GetSnapshotAsync();

            if (snapshot.Exists && snapshot.ContainsField("nickname"))
            {
                string nickname = snapshot.GetValue<string>("nickname");
                _myAccount = new Account(user.Email, nickname); // 비밀번호 저장 X
            }
            else
            {
                _myAccount = new Account(user.Email, "Unknown");
            }

            return true;
        }
        catch (FirebaseException fe)
        {
            Debug.LogError($"로그인 실패: {fe.ErrorCode} - {fe.Message}");
            return false;
        }
    }


    public string GetNicknameByPlayerId(string playerId)
    {
        // Firestore 저장 구조에 따라 이 메서드는 다르게 동작해야 할 수도 있음
        var findTask = _repository.FindAsync(playerId);
        findTask.Wait();

        var saveData = findTask.Result;
        return saveData != null ? saveData.Nickname : "Unknown";
    }

    public string GetMyNickname() => _myAccount?.Nickname ?? string.Empty;
    public string GetMyEmail() => _myAccount?.Email ?? string.Empty;
}
