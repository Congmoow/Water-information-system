using System.Text.Json;
using WaterInfoSystem.Shared.Exceptions;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            await WriteErrorAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, 500, "服务器内部错误");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int code, string message)
    {
        context.Response.StatusCode = code;
        context.Response.ContentType = "application/json; charset=utf-8";
        var payload = ApiResponse<object>.Failure(code, message);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
