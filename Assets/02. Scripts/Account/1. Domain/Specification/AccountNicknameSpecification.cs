using System;
using System.Text.RegularExpressions;

public class AccountNicknameSpecification:ISpecification<string>
{
    private static readonly Regex NicknameRegex = new Regex(@"^[가-힣a-zA-Z]{2,7}$", RegexOptions.Compiled);


    private static readonly string[] ForbiddenNicknames = { "admin", "root", "운영자", "바보" };

    public bool IsSatisfiedBy(string value)
    {
        // 닉네임 검증
        if (string.IsNullOrEmpty(value))
        {
            ErrorMessage = "닉네임은 비어있을 수 없습니다.";
            return false;
        }
        if (!NicknameRegex.IsMatch(value))
        {
            ErrorMessage = "닉네임은 한글 또는 영어로 구성되어야 하며, 2~7자 사이여야 합니다.";
            return false;
        }
        if (IsForbiddenNickname(value))
        {
            ErrorMessage = "사용할 수 없는 닉네임입니다.";
            return false;
        }
        return true;
    }
    private bool IsForbiddenNickname(string nickname)
    {
        foreach (var forbidden in ForbiddenNicknames)
        {
            if (nickname.Equals(forbidden, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
    public string ErrorMessage { get; private set; }

}