using System.Text.Json;
using FluentValidation;
using SGX.SistemaChamado.Api.Exceptions;

namespace SGX.SistemaChamado.Api.Middlewares;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    IWebHostEnvironment environment,
    ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation(
                "Requisicao cancelada pelo cliente. CorrelationId={CorrelationId} TraceId={TraceId}",
                context.Items[CorrelationIdMiddleware.HeaderName],
                context.TraceIdentifier);

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = MapException(exception);

        logger.LogError(
            exception,
            "Falha nao tratada. StatusCode={StatusCode} CorrelationId={CorrelationId} TraceId={TraceId}",
            statusCode,
            context.Items[CorrelationIdMiddleware.HeaderName],
            context.TraceIdentifier);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var detail = environment.IsDevelopment()
            ? exception.Message
            : "Consulte o suporte tecnico com o correlationId informado.";

        var response = new
        {
            traceId = context.TraceIdentifier,
            correlationId = context.Items[CorrelationIdMiddleware.HeaderName]?.ToString() ?? context.TraceIdentifier,
            status = statusCode,
            title,
            detail,
            errors = BuildErrors(exception)
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static (int StatusCode, string Title) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Requisicao invalida"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Requisicao invalida"),
            AcessoNegadoException => (StatusCodes.Status403Forbidden, "Acesso negado"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Nao autorizado"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso nao encontrado"),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Conflito de regra de negocio"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno")
        };
    }

    private static object[] BuildErrors(Exception exception)
    {
        if (exception is ValidationException validationException)
        {
            return validationException.Errors
                .Select(error => new
                {
                    campo = error.PropertyName,
                    mensagem = error.ErrorMessage
                })
                .Cast<object>()
                .ToArray();
        }

        return [];
    }
}
