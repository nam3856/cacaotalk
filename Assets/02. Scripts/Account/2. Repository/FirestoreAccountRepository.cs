using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class FirestoreAccountRepository : IAccountRepository
{
    private FirebaseFirestore _db;
    private FirebaseAuth _auth;

    public FirestoreAccountRepository()
    {
        _db = FirebaseFirestore.DefaultInstance;
        _auth = FirebaseAuth.DefaultInstance;
    }

    public async void Save(AccountDTO accountDto)
    {
        var user = _auth.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("No user is currently logged in.");
            return;
        }

        var userData = new Dictionary<string, object>
        {
            { "email", accountDto.Email },
            { "nickname", accountDto.Nickname }
        };

        await _db.Collection("users").Document(user.UserId).SetAsync(userData);
    }

    public async Task<AccountDTO> FindAsync(string email)
    {
        // email 기반 검색은 Firestore에서 비효율적이므로 보통 UID 기반으로 저장하고 사용
        // 구현 필요 시 이메일 → UID 매핑 테이블 또는 Firebase Auth에서 직접 로그인 후 UID 사용
        return null;
    }
}
