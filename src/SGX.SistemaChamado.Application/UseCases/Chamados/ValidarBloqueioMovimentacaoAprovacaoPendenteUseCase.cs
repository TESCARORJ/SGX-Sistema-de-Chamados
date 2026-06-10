using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Chamados;

public sealed class ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<InstanciaAprovacaoChamado> instanciaAprovacaoRepository,
    IRepository<StatusChamado> statusChamadoRepository) : IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase
{
    private const string OrigemLegado = "AprovacaoChamado";
    private const string OrigemInstancia = "InstanciaAprovacaoChamado";
    private const string MensagemTriagemPermitida = "Existe aprovacao pendente bloqueante, mas a acao consultiva, de evidencia ou triagem segue permitida nesta etapa.";
    private const string MensagemSinalizacao = "Existe aprovacao pendente, porem a acao solicitada segue permitida por nao representar avancao sensivel ou final nesta etapa.";

    private static readonly ValidarBloqueioMovimentacaoAprovacaoPendenteRequestValidator Validator = new();

    public async Task<ValidarBloqueioMovimentacaoAprovacaoPendenteResponse> ExecutarAsync(
        ValidarBloqueioMovimentacaoAprovacaoPendenteRequest request,
        CancellationToken cancellationToken = default)
    {
        await Validator.ValidateAndThrowAsync(request, cancellationToken);

        var chamado = await chamadoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Aprovacoes)
            .FirstOrDefaultAsync(x => x.Id == request.ChamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var bloqueioLegado = ObterBloqueioLegado(chamado);
        var sinalizacaoLegada = ObterSinalizacaoLegada(chamado);
        var bloqueioInstancia = await ObterBloqueioInstanciaAsync(chamado.Id, cancellationToken);
        var sinalizacaoInstancia = await ObterSinalizacaoInstanciaAsync(chamado.Id, cancellationToken);

        var bloqueioAtivo = bloqueioLegado ?? bloqueioInstancia;
        var sinalizacaoAtiva = sinalizacaoLegada ?? sinalizacaoInstancia;

        if (AcaoSemprePermitida(request.TipoAcao))
        {
            if (bloqueioAtivo is not null)
            {
                return CriarSinalizacao(request.TipoAcao, bloqueioAtivo, MensagemTriagemPermitida);
            }

            if (sinalizacaoAtiva is not null && !request.IgnorarSinalizacao)
            {
                return CriarSinalizacao(request.TipoAcao, sinalizacaoAtiva, MensagemSinalizacao);
            }

            return CriarPermitido();
        }

        var acaoSensivelOuFinal = await AcaoEhSensivelOuFinalAsync(request, cancellationToken);
        if (!acaoSensivelOuFinal)
        {
            if (bloqueioAtivo is not null)
            {
                return CriarSinalizacao(request.TipoAcao, bloqueioAtivo, MensagemSinalizacao);
            }

            if (sinalizacaoAtiva is not null && !request.IgnorarSinalizacao)
            {
                return CriarSinalizacao(request.TipoAcao, sinalizacaoAtiva, MensagemSinalizacao);
            }

            return CriarPermitido();
        }

        if (bloqueioAtivo is not null)
        {
            return CriarBloqueio(bloqueioAtivo);
        }

        if (sinalizacaoAtiva is not null && !request.IgnorarSinalizacao)
        {
            return CriarSinalizacao(request.TipoAcao, sinalizacaoAtiva, MensagemSinalizacao);
        }

        return CriarPermitido();
    }

    private async Task<bool> AcaoEhSensivelOuFinalAsync(
        ValidarBloqueioMovimentacaoAprovacaoPendenteRequest request,
        CancellationToken cancellationToken)
    {
        switch (request.TipoAcao)
        {
            case TipoAcaoMovimentacaoChamado.Assumir:
            case TipoAcaoMovimentacaoChamado.Resolver:
            case TipoAcaoMovimentacaoChamado.Encerrar:
            case TipoAcaoMovimentacaoChamado.Reabrir:
            case TipoAcaoMovimentacaoChamado.ExecutarServicoSensivel:
            case TipoAcaoMovimentacaoChamado.AplicarMudanca:
            case TipoAcaoMovimentacaoChamado.LiberarAcesso:
            case TipoAcaoMovimentacaoChamado.Cancelar:
                return true;
            case TipoAcaoMovimentacaoChamado.AlterarStatus:
            {
                var statusDestino = await statusChamadoRepository.Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.StatusDestinoId, cancellationToken)
                    ?? throw new InvalidOperationException("Status de destino nao encontrado para validacao de bloqueio.");

                return StatusRepresentaFechamentoOperacional(statusDestino);
            }
            default:
                return false;
        }
    }

    private static bool AcaoSemprePermitida(TipoAcaoMovimentacaoChamado tipoAcao)
        => tipoAcao is TipoAcaoMovimentacaoChamado.Consultar
            or TipoAcaoMovimentacaoChamado.Comentar
            or TipoAcaoMovimentacaoChamado.AnexarEvidencia
            or TipoAcaoMovimentacaoChamado.Triagem
            or TipoAcaoMovimentacaoChamado.Atribuir
            or TipoAcaoMovimentacaoChamado.Encaminhar;

    private static AvaliacaoBloqueio? ObterBloqueioLegado(Chamado chamado)
    {
        var estado = AprovacaoChamadoHelper.ObterEstado(chamado);
        if (!estado.BloqueiaAvancoAtendimento || !estado.AprovacaoPendente)
        {
            return null;
        }

        return new AvaliacaoBloqueio(
            OrigemLegado,
            estado.AprovacaoChamadoId,
            null,
            null,
            StatusAprovacaoChamado.Pendente.ToString(),
            null,
            Bloqueante: true,
            TipoFluxoAprovacao: null,
            ExigeAprovacao: true,
            estado.MensagemBloqueio ?? AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente,
            AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente,
            PodeContinuarTriagem: true);
    }

    private static AvaliacaoBloqueio? ObterSinalizacaoLegada(Chamado chamado)
    {
        var aprovacao = chamado.Aprovacoes
            .Where(x => x.Ativo && x.Status == StatusAprovacaoChamado.Pendente && !x.BloqueiaAvancoAtendimento)
            .OrderByDescending(x => x.SolicitadaEm)
            .ThenByDescending(x => x.CriadoEm)
            .FirstOrDefault();

        if (aprovacao is null)
        {
            return null;
        }

        return new AvaliacaoBloqueio(
            OrigemLegado,
            aprovacao.Id,
            null,
            null,
            aprovacao.Status.ToString(),
            null,
            Bloqueante: false,
            TipoFluxoAprovacao: null,
            ExigeAprovacao: true,
            "Existe aprovacao legada pendente nao bloqueante para o chamado.",
            "Existe aprovacao legada pendente nao bloqueante para o chamado.",
            PodeContinuarTriagem: true);
    }

    private async Task<AvaliacaoBloqueio?> ObterBloqueioInstanciaAsync(Guid chamadoId, CancellationToken cancellationToken)
    {
        var instancia = await instanciaAprovacaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.ChamadoId == chamadoId && x.Ativo)
            .Where(x => x.Status == StatusInstanciaAprovacaoChamado.Pendente || x.Status == StatusInstanciaAprovacaoChamado.EmReavaliacao)
            .Where(x => x.ExigeAprovacao)
            .Where(x => x.Bloqueante || x.EfeitoOperacional == EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco)
            .OrderByDescending(x => x.SolicitadaEm)
            .ThenByDescending(x => x.CriadoEm)
            .FirstOrDefaultAsync(cancellationToken);

        if (instancia is null)
        {
            return null;
        }

        return new AvaliacaoBloqueio(
            OrigemInstancia,
            null,
            instancia.Id,
            instancia.ConfiguracaoRegraAprovacaoId,
            instancia.Status.ToString(),
            instancia.EfeitoOperacional,
            instancia.Bloqueante,
            instancia.TipoFluxoAprovacao,
            instancia.ExigeAprovacao,
            "Existe instancia de aprovacao pendente bloqueante para o chamado.",
            AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente,
            PodeContinuarTriagem: true);
    }

    private async Task<AvaliacaoBloqueio?> ObterSinalizacaoInstanciaAsync(Guid chamadoId, CancellationToken cancellationToken)
    {
        var instancia = await instanciaAprovacaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.ChamadoId == chamadoId && x.Ativo)
            .Where(x => x.Status == StatusInstanciaAprovacaoChamado.Pendente || x.Status == StatusInstanciaAprovacaoChamado.EmReavaliacao)
            .Where(x => x.ExigeAprovacao)
            .Where(x => !x.Bloqueante && x.EfeitoOperacional != EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco)
            .OrderByDescending(x => x.SolicitadaEm)
            .ThenByDescending(x => x.CriadoEm)
            .FirstOrDefaultAsync(cancellationToken);

        if (instancia is null)
        {
            return null;
        }

        return new AvaliacaoBloqueio(
            OrigemInstancia,
            null,
            instancia.Id,
            instancia.ConfiguracaoRegraAprovacaoId,
            instancia.Status.ToString(),
            instancia.EfeitoOperacional,
            instancia.Bloqueante,
            instancia.TipoFluxoAprovacao,
            instancia.ExigeAprovacao,
            "Existe instancia de aprovacao pendente nao bloqueante para o chamado.",
            "Existe instancia de aprovacao pendente nao bloqueante para o chamado.",
            PodeContinuarTriagem: true);
    }

    private static bool StatusRepresentaFechamentoOperacional(StatusChamado status)
        => status.EhStatusFinal ||
           status.Codigo is StatusChamadoEnum.Resolvido
               or StatusChamadoEnum.Encerrado
               or StatusChamadoEnum.Cancelado
               or StatusChamadoEnum.Concluida;

    private static ValidarBloqueioMovimentacaoAprovacaoPendenteResponse CriarPermitido()
        => new()
        {
            Permitido = true,
            Bloqueado = false,
            ApenasSinalizacao = false,
            PodeContinuarTriagem = true
        };

    private static ValidarBloqueioMovimentacaoAprovacaoPendenteResponse CriarBloqueio(AvaliacaoBloqueio avaliacao)
        => new()
        {
            Permitido = false,
            Bloqueado = true,
            ApenasSinalizacao = false,
            Motivo = avaliacao.Motivo,
            MensagemUsuario = avaliacao.MensagemUsuario,
            OrigemBloqueio = avaliacao.OrigemBloqueio,
            AprovacaoChamadoId = avaliacao.AprovacaoChamadoId,
            InstanciaAprovacaoChamadoId = avaliacao.InstanciaAprovacaoChamadoId,
            ConfiguracaoRegraAprovacaoId = avaliacao.ConfiguracaoRegraAprovacaoId,
            StatusAprovacao = avaliacao.StatusAprovacao,
            EfeitoOperacional = avaliacao.EfeitoOperacional,
            Bloqueante = avaliacao.Bloqueante,
            TipoFluxoAprovacao = avaliacao.TipoFluxoAprovacao,
            ExigeAprovacao = avaliacao.ExigeAprovacao,
            PodeContinuarTriagem = avaliacao.PodeContinuarTriagem
        };

    private static ValidarBloqueioMovimentacaoAprovacaoPendenteResponse CriarSinalizacao(
        TipoAcaoMovimentacaoChamado tipoAcao,
        AvaliacaoBloqueio avaliacao,
        string mensagem)
        => new()
        {
            Permitido = true,
            Bloqueado = false,
            ApenasSinalizacao = true,
            Motivo = $"{avaliacao.Motivo} Acao avaliada: {tipoAcao}.",
            MensagemUsuario = mensagem,
            OrigemBloqueio = avaliacao.OrigemBloqueio,
            AprovacaoChamadoId = avaliacao.AprovacaoChamadoId,
            InstanciaAprovacaoChamadoId = avaliacao.InstanciaAprovacaoChamadoId,
            ConfiguracaoRegraAprovacaoId = avaliacao.ConfiguracaoRegraAprovacaoId,
            StatusAprovacao = avaliacao.StatusAprovacao,
            EfeitoOperacional = avaliacao.EfeitoOperacional,
            Bloqueante = avaliacao.Bloqueante,
            TipoFluxoAprovacao = avaliacao.TipoFluxoAprovacao,
            ExigeAprovacao = avaliacao.ExigeAprovacao,
            PodeContinuarTriagem = avaliacao.PodeContinuarTriagem
        };

    private sealed record AvaliacaoBloqueio(
        string OrigemBloqueio,
        Guid? AprovacaoChamadoId,
        Guid? InstanciaAprovacaoChamadoId,
        Guid? ConfiguracaoRegraAprovacaoId,
        string StatusAprovacao,
        EfeitoOperacionalRegraAprovacao? EfeitoOperacional,
        bool Bloqueante,
        TipoFluxoAprovacao? TipoFluxoAprovacao,
        bool ExigeAprovacao,
        string Motivo,
        string MensagemUsuario,
        bool PodeContinuarTriagem);
}
