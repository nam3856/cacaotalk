using UnityEngine;

public class AccountDTO
{
    public string Email { get; private set; }
    public string Nickname { get; private set; }
    public int ImageIndex { get; private set; }

    public AccountDTO(string email, string nickname, int imageIndex)
    {
        Email = email;
        Nickname = nickname;
        ImageIndex = imageIndex;
    }
}