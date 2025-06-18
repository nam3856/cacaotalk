public class Result
{
    public readonly bool IsSuccess;
    public readonly string Message;

    public Result(bool isSuccess, string message = "")
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static Result Success() => new Result(true);
    public static Result Fail(string message) => new Result(false, message);
}