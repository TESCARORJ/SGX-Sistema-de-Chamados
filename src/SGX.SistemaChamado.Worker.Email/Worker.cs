using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.Options;
using SGX.SistemaChamado.Worker.Email.Services;

namespace SGX.SistemaChamado.Worker.Email;

public sealed class Worker(
    ILogger<Worker> logger,
    IServiceScopeFactory scopeFactory,
    IOptions<EmailWorkerOptions> emailWorkerOptions) : BackgroundService
{
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Worker de e-mail iniciando. IntervaloSegundos={IntervaloSegundos} MaxMensagensPorCiclo={MaxMensagensPorCiclo} ImapConfigurado={ImapConfigurado}",
            emailWorkerOptions.Value.IntervaloSegundos,
            emailWorkerOptions.Value.MaxMensagensPorCiclo,
            emailWorkerOptions.Value.Configurado);

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalo = TimeSpan.FromSeconds(Math.Max(5, emailWorkerOptions.Value.IntervaloSegundos));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var emailIngestionService = scope.ServiceProvider.GetRequiredService<EmailIngestionService>();
                await emailIngestionService.ProcessarMensagensAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha no ciclo do worker de e-mail.");
            }

            await Task.Delay(intervalo, stoppingToken);
        }
    }
}
