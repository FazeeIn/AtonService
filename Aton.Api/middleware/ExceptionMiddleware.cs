using System.Security.Authentication;
using Aton.Core.Exceptions;
using Aton.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Aton.Api.middleware;

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
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case NotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Type = ex.GetType().Name,
                        Title = ex.Message,
                        Detail = ex.Message,
                        Instance = $"{context.Request.Method} {context.Request.Path}"
                    });
                    break;
                
                case ConflictException:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Type = ex.GetType().Name,
                        Title = ex.Message,
                        Detail = ex.Message,
                        Instance = $"{context.Request.Method} {context.Request.Path}"
                    });
                    break;
                
                case AuthenticationException:
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Type = ex.GetType().Name,
                        Title = ex.Message,
                        Detail = ex.Message,
                        Instance = $"{context.Request.Method} {context.Request.Path}"
                    });
                    break;
                
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Type = ex.GetType().Name,
                        Title = "Internal server error",
                        Detail = "Internal server error",
                        Instance = $"{context.Request.Method} {context.Request.Path}"
                    });
                    break;
            }
        }
    }
}