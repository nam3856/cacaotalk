public class AccountPasswordSpecification:ISpecification<string>
{
    public bool IsSatisfiedBy(string value)
    {
        if(string.IsNullOrEmpty(value))
        {
            ErrorMessage = "비밀번호는 비어있을 수 없습니다.";
            return false;
        }

        if ((value.Length < 6))
        {
            ErrorMessage = "비밀번호는 6자 이상이어야 합니다.";
            return false;
        }

        return true;
    }

    public string ErrorMessage { get; private set; }
}