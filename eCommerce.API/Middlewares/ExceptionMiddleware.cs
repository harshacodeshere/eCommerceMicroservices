namespace eCommerce.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError($"{ex.GetType()} : {ex.Message}");

            if (ex.InnerException is not null)
            {
                _logger.LogError($"{ex.InnerException.GetType()} : {ex.InnerException.Message}");
            }

            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { Message = ex.Message, Type = ex.GetType().ToString() });
        }
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}
