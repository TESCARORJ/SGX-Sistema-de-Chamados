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

        try
        {
            using var timer = new PeriodicTimer(intervalo);
            using var stopRegistration = stoppingToken.Register(static state =>
            {
                ((PeriodicTimer)state!).Dispose();
            }, timer);

            await ExecutarCicloAsync(stoppingToken);

            while (await timer.WaitForNextTickAsync())
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                await ExecutarCicloAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                logger.LogDebug("Monitoramento de SLA finalizado por cancelamento do host.");
                return;
            }

            logger.LogWarning("Monitoramento de SLA interrompido por cancelamento nao esperado.");
        }
    }

    private async Task ExecutarCicloAsync(CancellationToken cancellationToken)
    {
        var lockAdquirido = false;

        try
        {
            if (!await semaphore.WaitAsync(0, cancellationToken))
            {
                logger.LogWarning("Ciclo de monitoramento de SLA ignorado por execucao concorrente.");
                return;
            }

            lockAdquirido = true;
            using var scope = scopeFactory.CreateScope();
            var monitoringService = scope.ServiceProvider.GetRequiredService<ISlaMonitoringService>();
            await monitoringService.ExecutarVerificacaoAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogDebug("Ciclo de monitoramento de SLA cancelado.");
                return;
            }

            logger.LogWarning("Ciclo de monitoramento de SLA interrompido por cancelamento nao esperado.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha no ciclo de monitoramento de SLA.");
        }
        finally
        {
            if (lockAdquirido)
            {
                semaphore.Release();
            }
        }
    }
}
