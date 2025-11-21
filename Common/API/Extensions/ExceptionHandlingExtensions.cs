using System.Net;
using Common.Application.Errors;
using Common.Application.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Common.API.Extensions;

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseCommonExceptionHandling(this IApplicationBuilder app)
    {
        var webApp = (WebApplication)app;
        webApp.Use(async (context, next) =>
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
                await WriteProblemDetailsAsync(context, new AppUnexpectedWrapper(ex));
            }
        });
        return app;
    }

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
        if (context.Request.Headers.TryGetValue(HeaderNames.CorrelationId, out var cid))
        {
            problem.Extensions["correlationId"] = cid.ToString();
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsJsonAsync(problem);
    }

    // Internal wrapper to represent unexpected exceptions as AppException
    private sealed class AppUnexpectedWrapper : AppException
    {
        public AppUnexpectedWrapper(Exception inner)
            : base("An unexpected error occurred.", ErrorCode.Unknown, inner: inner) { }
    }
}
