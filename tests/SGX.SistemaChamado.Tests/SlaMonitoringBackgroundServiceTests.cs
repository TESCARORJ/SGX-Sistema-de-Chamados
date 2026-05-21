using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Application.Options;

namespace SGX.SistemaChamado.Tests;

public sealed class SlaMonitoringBackgroundServiceTests
{
    [Fact]
    public async Task StopAsyncNaoPropagaExcecaoQuandoCancelado()
    {
        var monitoringService = new MonitoringServiceCancelavelFake();
        using var serviceProvider = BuildServiceProvider(monitoringService);
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

        var options = Options.Create(new SlaMonitoringOptions
        {
            Enabled = true,
            IntervalMinutes = 1
        });

        var backgroundService = new SlaMonitoringBackgroundService(
            scopeFactory,
            options,
            NullLogger<SlaMonitoringBackgroundService>.Instance);

        await backgroundService.StartAsync(CancellationToken.None);

        await monitoringService.CicloIniciado.Task.WaitAsync(TimeSpan.FromSeconds(5));

        var exception = await Record.ExceptionAsync(() => backgroundService.StopAsync(CancellationToken.None));

        Assert.Null(exception);
    }

    private static ServiceProvider BuildServiceProvider(ISlaMonitoringService monitoringService)
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => monitoringService);
        return services.BuildServiceProvider();
    }

    private sealed class MonitoringServiceCancelavelFake : ISlaMonitoringService
    {
        public TaskCompletionSource<bool> CicloIniciado { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task ExecutarVerificacaoAsync(CancellationToken cancellationToken = default)
        {
            CicloIniciado.TrySetResult(true);
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        }
    }
}
