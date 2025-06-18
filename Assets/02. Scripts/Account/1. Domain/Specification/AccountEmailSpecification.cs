using System.Text.RegularExpressions;
using UnityEngine;

public class AccountEmailSpecification : ISpecification<string>
{
    // 이메일 정규표현식
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public bool IsSatisfiedBy(string value)
    {
        // 이메일 검증
        if (string.IsNullOrEmpty(value))
        {
            ErrorMessage = "이메일은 비어있을 수 없습니다.";
            return false;
        }

        if (!EmailRegex.IsMatch(value))
        {
            ErrorMessage = "올바른 이메일 형식이 아닙니다.";
            return false;
        }


        return true;
    }

    public string ErrorMessage { get; private set; }
}