namespace DevLog.Shared;

public class Result<T>
{
    public bool IsSuccess {get; set;}
    public T? Value {get; set;}
    public List<string> Errors {get; set;}

    public Result(bool isSuccess , T? value, List<string>? error = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = error ?? new List<string>();
    }

    public static Result<T> Success (T value) => new (true, value);
    public static Result<T> Fail (string error) => new (false, default, new List<string>{error});
    public static Result<T> Fail (List<string> errors) => new (false, default, errors);
}

// public class ApiError
// {
//     public string Message {get; set;} = string.Empty;
//     public string Code {get; set;} = string.Empty;

//     public ApiError(string message, string code)
//     {
//         Message = message;
//         Code = code;
//     }
// }