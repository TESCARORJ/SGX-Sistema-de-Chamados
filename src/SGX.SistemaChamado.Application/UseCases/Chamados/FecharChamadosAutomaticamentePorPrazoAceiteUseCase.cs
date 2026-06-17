using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Chamados;

public sealed class FecharChamadosAutomaticamentePorPrazoAceiteUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<StatusChamado> statusChamadoRepository,
    IRepository<HistoricoChamado> historicoChamadoRepository,
    IRepository<ParametroSistema> parametroSistemaRepository,
    IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase validarBloqueioUseCase,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IFecharChamadosAutomaticamentePorPrazoAceiteUseCase
{
    public async Task<FecharChamadosAutomaticamentePorPrazoAceiteResponse> ExecutarAsync(
        FecharChamadosAutomaticamentePorPrazoAceiteRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.LimiteProcessamento.HasValue && request.LimiteProcessamento.Value <= 0)
        {
            throw new ArgumentException("O limite de processamento deve ser um valor positivo.", nameof(request.LimiteProcessamento));
        }

        var response = new FecharChamadosAutomaticamentePorPrazoAceiteResponse();
        var prazoAceiteHoras = await ResolverPrazoAceiteHorasAsync(request, cancellationToken);
        var prazoAceite = TimeSpan.FromHours(prazoAceiteHoras);

        var statusResolvido = await statusChamadoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == StatusChamadoEnum.Resolvido, cancellationToken)
            ?? throw new InvalidOperationException("Status 'Resolvido' nao encontrado no sistema.");

        var statusEncerrado = await statusChamadoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == StatusChamadoEnum.Encerrado, cancellationToken)
            ?? throw new InvalidOperationException("Status 'Encerrado' nao encontrado no sistema.");

        var dataLimiteExpiracao = request.DataReferencia.AddHours(-prazoAceiteHoras);

        var queryChamados = chamadoRepository.Query()
            .Where(x => x.Ativo &&
                        x.StatusId == statusResolvido.Id &&
                        x.ResolvidoEm.HasValue &&
                        x.ResolvidoEm.Value <= dataLimiteExpiracao);

        if (request.LimiteProcessamento.HasValue)
        {
            queryChamados = queryChamados.Take(request.LimiteProcessamento.Value);
        }

        var chamadosElegiveis = await queryChamados.ToListAsync(cancellationToken);
        response.TotalAnalisados = chamadosElegiveis.Count;

        if (response.TotalAnalisados == 0)
        {
            return response;
        }

        var usuarioHistoricoId = request.UsuarioSistemaId;
        const string usuarioHistorico = "Sistema";
        var auditoriasPendentes = new List<RegistrarEventoAuditoriaRequest>();

        foreach (var chamado in chamadosElegiveis)
        {
            var resolvidoEmOriginal = chamado.ResolvidoEm;
            var requestValidacao = new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
            {
                ChamadoId = chamado.Id,
                TipoAcao = TipoAcaoMovimentacaoChamado.FecharAutomaticamentePorPrazoAceite
            };

            var validacaoBloqueio = await validarBloqueioUseCase.ExecutarAsync(requestValidacao, cancellationToken);
            if (validacaoBloqueio.Bloqueado)
            {
                response.TotalBloqueadosPorAprovacao++;
                response.TotalIgnorados++;
                response.ChamadosIgnorados.Add(new FechamentoAutomaticoChamadoResultadoResponse
                {
                    ChamadoId = chamado.Id,
                    CodigoChamado = chamado.Codigo,
                    StatusAnterior = statusResolvido.Nome,
                    StatusNovo = statusResolvido.Nome,
                    ResolvidoEm = resolvidoEmOriginal,
                    EncerradoEm = chamado.EncerradoEm,
                    Motivo = validacaoBloqueio.MensagemUsuario ?? "Chamado ignorado por aprovacao pendente bloqueante.",
                    BloqueadoPorAprovacao = true
                });
                continue;
            }

            var descricaoHistorico =
                $"Chamado fechado automaticamente em {request.DataReferencia:O} por ausencia de manifestacao do solicitante no prazo de {prazoAceiteHoras} horas. Resolvido em {resolvidoEmOriginal:O}. Status anterior: {statusResolvido.Nome}. Status novo: {statusEncerrado.Nome}.";

            chamado.FecharAutomaticamentePorPrazoAceite(
                statusEncerrado.Id,
                StatusChamadoEnum.Resolvido,
                request.DataReferencia,
                prazoAceite,
                usuarioHistorico);

            await historicoChamadoRepository.AddAsync(new HistoricoChamado(
                chamado.Id,
                TipoHistoricoChamado.FechamentoAutomatico,
                descricaoHistorico,
                usuarioHistoricoId,
                usuarioHistorico), cancellationToken);

            response.TotalFechados++;
            response.ChamadosFechados.Add(new FechamentoAutomaticoChamadoResultadoResponse
            {
                ChamadoId = chamado.Id,
                CodigoChamado = chamado.Codigo,
                StatusAnterior = statusResolvido.Nome,
                StatusNovo = statusEncerrado.Nome,
                ResolvidoEm = resolvidoEmOriginal,
                EncerradoEm = chamado.EncerradoEm,
                Motivo = descricaoHistorico
            });

            if (auditoriaService is not null)
            {
                auditoriasPendentes.Add(new RegistrarEventoAuditoriaRequest
                {
                    Modulo = "Chamados",
                    Entidade = "Chamado",
                    EntidadeId = chamado.Id.ToString(),
                    Acao = TipoAcaoAuditoria.FecharChamadoAutomaticamentePorPrazoAceite,
                    Descricao = "Chamado fechado automaticamente por prazo de aceite.",
                    DadosAntes = AuditoriaDiffHelper.SerializarSeguro(new
                    {
                        ChamadoId = chamado.Id,
                        UsuarioExecutorId = usuarioHistoricoId,
                        UsuarioExecutorLogin = usuarioHistorico,
                        DataEventoUtc = chamado.EncerradoEm,
                        StatusAnterior = statusResolvido.Nome,
                        StatusNovo = statusEncerrado.Nome,
                        ResolvidoEm = resolvidoEmOriginal,
                        EncerradoEm = (DateTime?)null,
                        chamado.AceitoEm,
                        chamado.AceitoPorUsuarioId
                    }),
                    DadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
                    {
                        ChamadoId = chamado.Id,
                        UsuarioExecutorId = usuarioHistoricoId,
                        UsuarioExecutorLogin = usuarioHistorico,
                        DataEventoUtc = chamado.EncerradoEm,
                        StatusAnterior = statusResolvido.Nome,
                        StatusNovo = statusEncerrado.Nome,
                        chamado.ResolvidoEm,
                        chamado.EncerradoEm,
                        chamado.AceitoEm,
                        chamado.AceitoPorUsuarioId,
                        PrazoAceiteHoras = prazoAceiteHoras,
                        FechamentoAutomatico = true,
                        OrigemFechamento = "Automatica"
                    }),
                    Metadados = AuditoriaDiffHelper.CriarMetadadosPadrao(
                        origem: "sistema",
                        modulo: "Chamados",
                        entidade: "Chamado",
                        entidadeId: chamado.Id.ToString(),
                        codigo: chamado.Codigo,
                        nome: chamado.Titulo,
                        operacao: "FechamentoAutomaticoPrazoAceite",
                        resultado: "Sucesso",
                        observacao: $"Resolvido em {resolvidoEmOriginal:O}; encerrado em {chamado.EncerradoEm:O}."),
                    Nivel = NivelAuditoria.Informacao,
                    Sucesso = true,
                    UsuarioId = usuarioHistoricoId,
                    UsuarioLogin = usuarioHistorico
                });
            }
        }

        if (response.TotalFechados > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);

            if (auditoriaService is not null)
            {
                foreach (var auditoriaPendente in auditoriasPendentes)
                {
                    await auditoriaService.RegistrarAsync(auditoriaPendente, cancellationToken);
                }
            }
        }

        return response;
    }

    private async Task<int> ResolverPrazoAceiteHorasAsync(
        FecharChamadosAutomaticamentePorPrazoAceiteRequest request,
        CancellationToken cancellationToken)
    {
        if (request.PrazoAceiteHoras.HasValue)
        {
            var prazoExplicito = request.PrazoAceiteHoras.Value;
            if (prazoExplicito <= 0)
            {
                throw new ArgumentException("O prazo de aceite deve ser um valor positivo.", nameof(request.PrazoAceiteHoras));
            }

            ObterConfiguracaoAutoFechamentoChamadoUseCase.ValidarPrazo(prazoExplicito, "request");
            return prazoExplicito;
        }

        var parametro = await parametroSistemaRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Ativo && x.Chave == ConfiguracaoAutoFechamentoChamadoConstantes.ChaveParametroPrazoAceiteHoras,
                cancellationToken);

        return ObterConfiguracaoAutoFechamentoChamadoUseCase.ResolverPrazo(parametro);
    }
}
