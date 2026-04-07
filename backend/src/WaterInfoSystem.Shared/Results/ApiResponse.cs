namespace WaterInfoSystem.Shared.Results;

public class ApiResponse<T>
{
    public int Code { get; init; }

    public string Message { get; init; } = "success";

    public T? Data { get; init; }

    public static ApiResponse<T> Success(T? data, string message = "success")
    {
        return new ApiResponse<T>
        {
            Code = 200,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Failure(int code, string message)
    {
        return new ApiResponse<T>
        {
            Code = code,
            Message = message,
            Data = default
        };
    }
}
