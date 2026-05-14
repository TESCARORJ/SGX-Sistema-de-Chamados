using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Application.Options;

namespace SGX.SistemaChamado.Api.Services;

public sealed class SlaMonitoringBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<SlaMonitoringOptions> options,
    ILogger<SlaMonitoringBackgroundService> logger) : BackgroundService
{
    private readonly SemaphoreSlim semaphore = new(1, 1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.Enabled)
        {
            logger.LogInformation("Monitoramento de SLA desabilitado por configuracao.");
            return;
        }

        var intervalo = TimeSpan.FromMinutes(Math.Max(1, options.Value.IntervalMinutes));

        using var timer = new PeriodicTimer(intervalo);
        await ExecutarCicloAsync(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ExecutarCicloAsync(stoppingToken);
        }
    }

    private async Task ExecutarCicloAsync(CancellationToken cancellationToken)
    {
        if (!await semaphore.WaitAsync(0, cancellationToken))
        {
            logger.LogWarning("Ciclo de monitoramento de SLA ignorado por execucao concorrente.");
            return;
        }

        try
        {
            using var scope = scopeFactory.CreateScope();
            var monitoringService = scope.ServiceProvider.GetRequiredService<ISlaMonitoringService>();
            await monitoringService.ExecutarVerificacaoAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha no ciclo de monitoramento de SLA.");
        }
        finally
        {
            semaphore.Release();
        }
    }
}
