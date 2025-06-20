public class PostContentSpecificaion : ISpecification<string>
{
    public bool IsSatisfiedBy(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ErrorMessage = "게시글 내용은 비어있을 수 없습니다.";
            return false;
        }
        if (value.Length > Post.MaxContentLength)
        {
            ErrorMessage = $"게시글 내용은 최대 {Post.MaxContentLength}자까지 입력할 수 있습니다.";
            return false;
        }
        return true;
    }

    public string ErrorMessage { get; private set; }
}
