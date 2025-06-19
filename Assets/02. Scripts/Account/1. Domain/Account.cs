using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class Account
{
    public readonly string Email;
    public readonly string Nickname;
    public readonly int ImageIndex;

    public Account(string email, string nickname)
    {
        var emailSpec = new AccountEmailSpecification();
        if (!emailSpec.IsSatisfiedBy(email))
            throw new Exception(emailSpec.ErrorMessage);

        var nickSpec = new AccountNicknameSpecification();
        if (!nickSpec.IsSatisfiedBy(nickname))
            throw new Exception(nickSpec.ErrorMessage);

        Email = email;
        Nickname = nickname;
        ImageIndex = UnityEngine.Random.Range(0, 5); // 0~4 랜덤
    }

    public Account(string email, string nickname, int imageIndex)
    {
        var emailSpec = new AccountEmailSpecification();
        if (!emailSpec.IsSatisfiedBy(email))
            throw new System.Exception(emailSpec.ErrorMessage);

        var nickSpec = new AccountNicknameSpecification();
        if (!nickSpec.IsSatisfiedBy(nickname))
            throw new System.Exception(nickSpec.ErrorMessage);

        Email = email;
        Nickname = nickname;
        ImageIndex = Mathf.Clamp(imageIndex, 0, 4); // 안정성
    }
    public AccountDTO ToDTO()
    {
        return new AccountDTO(Email, Nickname, ImageIndex);
    }
}