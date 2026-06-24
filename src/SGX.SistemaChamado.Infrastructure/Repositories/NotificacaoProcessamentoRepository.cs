using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Infrastructure.Repositories;

public sealed class NotificacaoProcessamentoRepository(SGXSistemaChamadoDbContext context) : INotificacaoProcessamentoRepository
{
    public async Task<bool> TentarIniciarProcessamentoAsync(
        Guid notificacaoId,
        DateTime iniciadaEm,
        string atualizadoPor,
        Guid? atualizadoPorUsuarioId,
        int limiteTentativas,
        CancellationToken cancellationToken = default)
    {
        var iniciadaEmUtc = iniciadaEm.Kind == DateTimeKind.Utc
            ? iniciadaEm
            : iniciadaEm.ToUniversalTime();

        var afetadas = await context.Notificacoes
            .Where(x => x.Id == notificacaoId)
            .Where(x => x.Ativo)
            .Where(x => x.QuantidadeTentativas < limiteTentativas)
            .Where(x =>
                x.Status == StatusNotificacao.Pendente
                || (x.Status == StatusNotificacao.Agendada
                    && x.AgendadaEm.HasValue
                    && x.AgendadaEm.Value <= iniciadaEmUtc))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Status, StatusNotificacao.EmProcessamento)
                .SetProperty(x => x.ProcessadaEm, iniciadaEmUtc)
                .SetProperty(x => x.QuantidadeTentativas, x => x.QuantidadeTentativas + 1)
                .SetProperty(x => x.FalhouEm, (DateTime?)null)
                .SetProperty(x => x.UltimoErro, (string?)null)
                .SetProperty(x => x.MotivoCancelamento, (string?)null)
                .SetProperty(x => x.AtualizadoEm, iniciadaEmUtc)
                .SetProperty(x => x.AtualizadoPor, atualizadoPor)
                .SetProperty(x => x.AtualizadoPorUsuarioId, atualizadoPorUsuarioId),
                cancellationToken);

        return afetadas == 1;
    }

    public async Task<bool> TentarRegistrarSucessoAsync(
        Guid notificacaoId,
        DateTime enviadaEm,
        string atualizadoPor,
        Guid? atualizadoPorUsuarioId,
        CancellationToken cancellationToken = default)
    {
        var enviadaEmUtc = enviadaEm.Kind == DateTimeKind.Utc
            ? enviadaEm
            : enviadaEm.ToUniversalTime();

        var afetadas = await context.Notificacoes
            .Where(x => x.Id == notificacaoId)
            .Where(x => x.Status == StatusNotificacao.EmProcessamento)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Status, StatusNotificacao.Enviada)
                .SetProperty(x => x.EnviadaEm, enviadaEmUtc)
                .SetProperty(x => x.AgendadaEm, (DateTime?)null)
                .SetProperty(x => x.FalhouEm, (DateTime?)null)
                .SetProperty(x => x.UltimoErro, (string?)null)
                .SetProperty(x => x.MotivoCancelamento, (string?)null)
                .SetProperty(x => x.AtualizadoEm, enviadaEmUtc)
                .SetProperty(x => x.AtualizadoPor, atualizadoPor)
                .SetProperty(x => x.AtualizadoPorUsuarioId, atualizadoPorUsuarioId),
                cancellationToken);

        return afetadas == 1;
    }
}
