public class Result
{
    public readonly bool IsSuccess;
    public readonly string Message;
    public readonly int ErrorCode;
    public Result(bool isSuccess, string message = "", int errorCode = 0)
    {
        IsSuccess = isSuccess;
        Message = message;
        ErrorCode = errorCode;
    }

    public static Result Success() => new Result(true);
    public static Result Fail(string message) => new Result(false, message);
    public static Result Fail(string message, int errorCode) => new Result(false, message, errorCode);
}