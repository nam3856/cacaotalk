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
        await Task.Yield();
        return null;
    }
}
