using System.Net;
using Common.Core.Constants;
using Common.Core.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Common.OpenApi.Middleware;

/// <summary>
/// Exception handling middleware for mapping AppException types to ProblemDetails.
/// Can be used across all services for consistent error handling.
/// </summary>
public static class ExceptionHandlingMiddleware
{
    /// <summary>
    /// Handles AppException types and maps them to ProblemDetails with appropriate HTTP status codes.
    /// Also handles common framework exceptions (ArgumentException, UnauthorizedAccessException).
    /// </summary>
    public static IApplicationBuilder UseAppExceptionHandling(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (AppException ex)
            {
                await WriteProblemDetailsAsync(context, ex);
            }
            catch (Exception ex)
            {
                await WriteGenericExceptionAsync(context, ex);
            }
        });
        return app;
    }

    /// <summary>
    /// Legacy method name for backward compatibility. Use UseAppExceptionHandling instead.
    /// </summary>
    public static IApplicationBuilder UseCommonExceptionHandling(this IApplicationBuilder app)
        => UseAppExceptionHandling(app);

    private static Task WriteProblemDetailsAsync(HttpContext context, AppException ex)
    {
        var status = ex switch
        {
            ValidationAppException => HttpStatusCode.BadRequest,
            NotFoundException => HttpStatusCode.NotFound,
            ConflictException => HttpStatusCode.Conflict,
            UnauthorizedAppException => HttpStatusCode.Unauthorized,
            ForbiddenAppException => HttpStatusCode.Forbidden,
            _ => HttpStatusCode.InternalServerError
        };

        var problem = new ProblemDetails
        {
            Title = ex.Message,
            Detail = ex.Details,
            Status = (int)status,
            Type = $"https://httpstatuses.com/{(int)status}",
            Instance = context.Request.Path
        };

        // Include validation errors when present
        if (ex is ValidationAppException vex && vex.Errors.Count > 0)
        {
            problem.Extensions["errors"] = vex.Errors;
        }

        // Attach correlation id when available
        if (context.Request.Headers.TryGetValue(HeaderConstants.CorrelationId, out var cid))
        {
            problem.Extensions["correlationId"] = cid.ToString();
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsJsonAsync(problem);
    }

    private static Task WriteGenericExceptionAsync(HttpContext context, Exception ex)
    {
        // Default to 500 Internal Server Error for unexpected exceptions
        var status = HttpStatusCode.InternalServerError;
        var title = "An unexpected error occurred.";
        var detail = ex.Message;

        // Map common exception types
        if (ex is ArgumentException or ArgumentNullException)
        {
            status = HttpStatusCode.BadRequest;
            title = "Invalid argument";
        }
        else if (ex is UnauthorizedAccessException)
        {
            status = HttpStatusCode.Unauthorized;
            title = "Unauthorized";
        }

        var problem = new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = (int)status,
            Type = $"https://httpstatuses.com/{(int)status}",
            Instance = context.Request.Path
        };

        // Attach correlation id when available
        if (context.Request.Headers.TryGetValue(HeaderConstants.CorrelationId, out var cid))
        {
            problem.Extensions["correlationId"] = cid.ToString();
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsJsonAsync(problem);
    }
}
