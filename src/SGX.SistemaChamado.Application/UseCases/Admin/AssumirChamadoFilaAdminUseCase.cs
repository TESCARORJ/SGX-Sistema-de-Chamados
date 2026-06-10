using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class AssumirChamadoFilaAdminUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<MembroGrupoTecnico> membroGrupoTecnicoRepository,
    IRepository<HistoricoChamado> historicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase? validarBloqueioMovimentacaoUseCase = null) : IAssumirChamadoFilaAdminUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AssumirChamadoFilaRequest request, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        if (request.UsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario que assume o chamado e obrigatorio.", nameof(request));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        if (request.UsuarioId != usuario.Id)
        {
            throw new InvalidOperationException("Chamado da fila so pode ser assumido pelo proprio usuario autenticado.");
        }

        var chamado = await chamadoRepository.Query()
            .Include(x => x.Responsavel)
            .Include(x => x.GrupoTecnico)
            .Include(x => x.FilaAtendimento)
            .Include(x => x.Aprovacoes)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        await GarantirMovimentacaoPermitidaAsync(chamado, cancellationToken);

        if (chamado.ResponsavelId.HasValue)
        {
            throw new InvalidOperationException("Chamado da fila ja possui responsavel individual.");
        }

        if (!chamado.GrupoTecnicoId.HasValue)
        {
            throw new InvalidOperationException("Chamado precisa estar vinculado a um grupo tecnico para ser assumido da fila.");
        }

        if (chamado.GrupoTecnico is null || !chamado.GrupoTecnico.Ativo)
        {
            throw new InvalidOperationException("Grupo tecnico do chamado nao encontrado ou inativo.");
        }

        if (!chamado.FilaAtendimentoId.HasValue)
        {
            throw new InvalidOperationException("Chamado precisa estar vinculado a uma fila de atendimento para ser assumido da fila.");
        }

        if (chamado.FilaAtendimento is null || !chamado.FilaAtendimento.Ativo)
        {
            throw new InvalidOperationException("Fila de atendimento do chamado nao encontrada ou inativa.");
        }

        if (chamado.FilaAtendimento.GrupoTecnicoId != chamado.GrupoTecnicoId.Value)
        {
            throw new InvalidOperationException("Fila de atendimento do chamado nao pertence ao grupo tecnico do chamado.");
        }

        var membroAtivo = await membroGrupoTecnicoRepository.Query()
            .AsNoTracking()
            .AnyAsync(
                x => x.GrupoTecnicoId == chamado.GrupoTecnicoId.Value &&
                    x.UsuarioId == usuario.Id &&
                    x.Ativo,
                cancellationToken);

        if (!membroAtivo)
        {
            throw new InvalidOperationException("Usuario nao e membro ativo do grupo tecnico do chamado.");
        }

        chamado.AtribuirResponsavel(usuario.Id, usuario.Login);
        chamadoRepository.Update(chamado);

        var descricao = $"Chamado assumido da fila {chamado.FilaAtendimento.Nome} por {usuario.Nome}.";
        if (!string.IsNullOrWhiteSpace(request.Observacao))
        {
            descricao = $"{descricao} Observacao: {request.Observacao.Trim()}";
        }

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.ChamadoAssumidoDaFila,
            AdminUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.ChamadoAssumidoDaFila, descricao),
            usuario.Id,
            usuario.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstAsync(x => x.Id == chamadoId, cancellationToken);

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }

    private async Task GarantirMovimentacaoPermitidaAsync(Chamado chamado, CancellationToken cancellationToken)
    {
        if (validarBloqueioMovimentacaoUseCase is null)
        {
            var estadoAprovacao = AprovacaoChamadoHelper.ObterEstado(chamado);
            if (estadoAprovacao.BloqueiaAvancoAtendimento)
            {
                throw new InvalidOperationException(estadoAprovacao.MensagemBloqueio ?? AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente);
            }

            return;
        }

        var avaliacao = await validarBloqueioMovimentacaoUseCase.ExecutarAsync(
            new()
            {
                ChamadoId = chamado.Id,
                TipoAcao = TipoAcaoMovimentacaoChamado.Assumir
            },
            cancellationToken);

        if (avaliacao.Bloqueado)
        {
            throw new InvalidOperationException(avaliacao.MensagemUsuario ?? AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente);
        }
    }
}
