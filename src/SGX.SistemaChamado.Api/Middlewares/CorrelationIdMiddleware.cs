using Serilog.Context;

namespace SGX.SistemaChamado.Api.Middlewares;

public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId) || string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N");
        }

        var correlationIdValue = correlationId.ToString().Trim();
        context.TraceIdentifier = correlationIdValue;
        context.Items[HeaderName] = correlationIdValue;

        using (LogContext.PushProperty("CorrelationId", correlationIdValue))
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationIdValue;
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
