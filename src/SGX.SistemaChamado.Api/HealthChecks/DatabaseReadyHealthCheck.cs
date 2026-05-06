using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Api.HealthChecks;

public sealed class DatabaseReadyHealthCheck(
    SGXSistemaChamadoDbContext dbContext,
    ILogger<DatabaseReadyHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
            if (canConnect)
            {
                return HealthCheckResult.Healthy("Conexao com PostgreSQL validada.");
            }

            return HealthCheckResult.Unhealthy("Nao foi possivel conectar ao PostgreSQL.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao executar health check de prontidao do banco.");
            return HealthCheckResult.Unhealthy("Falha ao validar prontidao do banco.", ex);
        }
    }
}
