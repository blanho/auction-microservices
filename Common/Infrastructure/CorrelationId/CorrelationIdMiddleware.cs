using Microsoft.AspNetCore.Http;
using Common.Application.Abstractions;

namespace Common.Infrastructure.CorrelationId;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeaderName = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICorrelationIdService correlationIdService)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId))
        {
            correlationIdService.CorrelationId = correlationId.ToString();
        }
        else
        {
            correlationIdService.GenerateCorrelationId();
        }

        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CorrelationIdHeaderName))
            {
                context.Response.Headers.Append(CorrelationIdHeaderName, correlationIdService.CorrelationId);
            }
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
