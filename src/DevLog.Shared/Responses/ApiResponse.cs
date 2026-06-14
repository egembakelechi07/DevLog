namespace DevLog.Shared.Responses;

public class ApiResponse<T>
{
    public bool IsSuccess {get; set;}
    public string Message {get; set;} = string.Empty;
    public T? Data {get; set;}
    public List<string> Errors {get; set;} = new();

    public static ApiResponse<T> Success(T data, string message= "Success")
    {
        return new ApiResponse<T>
        {
            IsSuccess= true,
            Message = message,
            Data = data,
            Errors = new List<string>()
        };
    }

    public static ApiResponse<T> Failure(string message, List<string>? errors)
    {
        return new ApiResponse<T>
        {
            IsSuccess= false,
            Message = message,
            Data = default,
            Errors = errors ?? new List<string>()
        };
    }
}