using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class Account
{
    public readonly string Email;
    public readonly string Nickname;


    public Account(string email, string nickname)
    {
        // 규칙을 객체로 캡슐화해서 분리한다.
        // 그래서 도메인과 UI는 모두 "이 규칙을 만족하니?" 물으면된다.
        // 캡슐화한 규칙: 명세(specification)


        // 이메일 검증
        var emailSpecification = new AccountEmailSpecification();
        if (!emailSpecification.IsSatisfiedBy(email))
        {
            throw new Exception(emailSpecification.ErrorMessage);
        }

        // 닉네임 검증
        var nickNameSpecification = new AccountNicknameSpecification();
        if (!nickNameSpecification.IsSatisfiedBy(nickname))
        {
            throw new Exception(nickNameSpecification.ErrorMessage);
        }


        Email = email;
        Nickname = nickname;
    }


    public AccountDTO ToDTO()
    {
        return new AccountDTO(Email, Nickname);
    }
}