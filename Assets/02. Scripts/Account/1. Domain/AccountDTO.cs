using UnityEngine;

public class AccountDTO
{
    public string Email;
    public string Nickname;

    public AccountDTO(string email, string nickname)
    {
        Email = email;
        Nickname = nickname;
    }
}