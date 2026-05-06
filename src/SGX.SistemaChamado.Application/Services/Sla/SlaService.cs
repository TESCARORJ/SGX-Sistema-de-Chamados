using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Services.Sla;

public sealed class SlaService(
    ISlaCalculator slaCalculator,
    IRepository<SlaControle> slaControleRepository) : ISlaService
{
    public async Task InicializarNaAberturaAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default)
    {
        var prazos = await slaCalculator.CalcularPrazosAsync(chamado.PrioridadeId, chamado.CategoriaId, chamado.DepartamentoId, cancellationToken);
        var prazoPrimeiraResposta = agoraUtc.AddHours(prazos.PrazoPrimeiraRespostaHoras);
        var prazoResolucao = agoraUtc.AddHours(prazos.PrazoResolucaoHoras);

        if (chamado.SlaControle is null)
        {
            var controle = new SlaControle(chamado.Id, prazoPrimeiraResposta, prazoResolucao, usuarioLogin);
            await slaControleRepository.AddAsync(controle, cancellationToken);
            return;
        }

        chamado.SlaControle.AtualizarPrazos(prazoPrimeiraResposta, prazoResolucao, usuarioLogin);
    }

    public Task RegistrarPrimeiraRespostaAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default)
    {
        if (chamado.SlaControle is null || chamado.EncerradoEm.HasValue)
        {
            return Task.CompletedTask;
        }

        chamado.SlaControle.RegistrarPrimeiraResposta(agoraUtc, usuarioLogin);
        return Task.CompletedTask;
    }

    public async Task AplicarMudancaPrioridadeAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default)
    {
        if (chamado.SlaControle is null || chamado.EncerradoEm.HasValue || chamado.SlaControle.ResolvidoEm.HasValue)
        {
            return;
        }

        var prazos = await slaCalculator.CalcularPrazosAsync(chamado.PrioridadeId, chamado.CategoriaId, chamado.DepartamentoId, cancellationToken);
        chamado.SlaControle.RecalcularPrazoResolucao(agoraUtc.AddHours(prazos.PrazoResolucaoHoras), usuarioLogin);
    }

    public async Task AplicarMudancaCategoriaAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default)
    {
        if (chamado.SlaControle is null || chamado.EncerradoEm.HasValue || chamado.SlaControle.ResolvidoEm.HasValue)
        {
            return;
        }

        var prazos = await slaCalculator.CalcularPrazosAsync(chamado.PrioridadeId, chamado.CategoriaId, chamado.DepartamentoId, cancellationToken);
        chamado.SlaControle.RecalcularPrazoResolucao(agoraUtc.AddHours(prazos.PrazoResolucaoHoras), usuarioLogin);
    }

    public Task AplicarMudancaStatusAsync(Chamado chamado, StatusChamado statusAnterior, StatusChamado statusAtual, string usuarioLogin, DateTime agoraUtc)
    {
        if (chamado.SlaControle is null)
        {
            return Task.CompletedTask;
        }

        if (!statusAnterior.PausaSla && statusAtual.PausaSla)
        {
            chamado.SlaControle.IniciarPausa(agoraUtc, usuarioLogin);
        }
        else if (statusAnterior.PausaSla && !statusAtual.PausaSla)
        {
            chamado.SlaControle.FinalizarPausa(agoraUtc, usuarioLogin);
        }

        if (statusAtual.EhStatusFinal || statusAtual.Codigo == StatusChamadoEnum.Resolvido)
        {
            chamado.SlaControle.RegistrarResolucao(agoraUtc, usuarioLogin);
        }
        else
        {
            chamado.SlaControle.RecalcularVencimento(usuarioLogin);
        }

        return Task.CompletedTask;
    }

    public Task RegistrarEncerramentoAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc)
    {
        if (chamado.SlaControle is null)
        {
            return Task.CompletedTask;
        }

        chamado.SlaControle.RegistrarResolucao(agoraUtc, usuarioLogin);
        return Task.CompletedTask;
    }

    public async Task ReabrirAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default)
    {
        if (chamado.SlaControle is null)
        {
            return;
        }

        var prazos = await slaCalculator.CalcularPrazosAsync(chamado.PrioridadeId, chamado.CategoriaId, chamado.DepartamentoId, cancellationToken);
        chamado.SlaControle.Reabrir(agoraUtc.AddHours(prazos.PrazoResolucaoHoras), usuarioLogin);
    }

    public bool EstaProximoDoVencimento(SlaControle? slaControle, DateTime agoraUtc)
        => SlaRules.EstaProximoDoVencimento(slaControle, agoraUtc);
}
