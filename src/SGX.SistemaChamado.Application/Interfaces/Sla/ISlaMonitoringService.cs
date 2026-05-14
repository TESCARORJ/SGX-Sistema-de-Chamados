namespace SGX.SistemaChamado.Application.Interfaces.Sla;

public interface ISlaMonitoringService
{
    Task ExecutarVerificacaoAsync(CancellationToken cancellationToken = default);
}
