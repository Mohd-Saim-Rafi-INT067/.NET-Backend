// Middleware/GlobalExceptionMiddleware.cs
using System.Net;
using AuthenticationAuthorization.DTOs;

namespace AuthenticationAuthorization.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = ApiResponseDto.Fail("An unexpected error occurred. Please try again later.");
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}