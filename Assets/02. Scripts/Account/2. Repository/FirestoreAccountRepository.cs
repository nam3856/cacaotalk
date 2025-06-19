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
            { "nickname", accountDto.Nickname },
            { "imageIndex", accountDto.ImageIndex }
        };

        await _db.Collection("users").Document(user.UserId).SetAsync(userData);
    }

    public async Task<AccountDTO> FindAsync(string email)
    {
        var user = _auth.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("No user logged in.");
            return null;
        }

        var snapshot = await _db.Collection("users").Document(user.UserId).GetSnapshotAsync();
        if (!snapshot.Exists)
        {
            Debug.LogWarning("사용자 정보 없음.");
            return null;
        }

        string nickname = snapshot.GetValue<string>("nickname");
        int imageIndex = snapshot.ContainsField("imageIndex") ? snapshot.GetValue<int>("imageIndex") : 0;

        return new AccountDTO(email, nickname, imageIndex);
    }
}
