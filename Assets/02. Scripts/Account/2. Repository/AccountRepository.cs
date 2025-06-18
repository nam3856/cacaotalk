using UnityEngine;

public class AccountRepository
{
    public const string SAVE_PREFIX = "ACCOUNT_";

    public void Save(AccountDTO accountDto)
    {
        AccountSaveData data = new AccountSaveData(accountDto);
        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString(SAVE_PREFIX + data.Email, json);
    }

    public AccountSaveData Find(string email)
    {
        if (!PlayerPrefs.HasKey(SAVE_PREFIX + email))
        {
            return null;
        }

        return JsonUtility.FromJson<AccountSaveData>(PlayerPrefs.GetString(SAVE_PREFIX + email));
    }
}

public class AccountSaveData
{
    public string Email;
    public string Nickname;

    public AccountSaveData(AccountDTO accountDto)
    {
        Email = accountDto.Email;
        Nickname = accountDto.Nickname;
    }
}