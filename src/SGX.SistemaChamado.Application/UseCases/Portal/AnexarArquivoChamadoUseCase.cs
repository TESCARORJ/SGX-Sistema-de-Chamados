using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Application.Options;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class AnexarArquivoChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<AnexoChamado> anexoRepository,
    IRepository<HistoricoChamado> historicoRepository,
    IArquivoStorageService arquivoStorageService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IOptions<ArquivosOptions> arquivosOptions,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAnexarArquivoChamadoUseCase
{
    public async Task<AnexoChamadoResponse> ExecutarAsync(Guid chamadoId, UploadAnexoChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var chamado = await chamadoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        if (!PortalUseCaseHelpers.PodeAcessarChamado(usuarioAtual, chamado))
        {
            throw new UnauthorizedAccessException("Acesso negado ao chamado solicitado.");
        }

        var (nomeOriginalSeguro, extensao) = AnexoAtendimentoHelper.ValidarUpload(
            request.NomeArquivo,
            request.ContentType,
            request.TamanhoBytes,
            arquivosOptions.Value);
        var nomeFisico = AnexoAtendimentoHelper.GerarNomeFisicoSeguro(extensao);

        var resultadoStorage = await arquivoStorageService.SalvarAsync(
            new ArquivoStorageRequest(nomeFisico, request.Conteudo),
            cancellationToken);

        var anexo = new AnexoChamado(
            chamadoId,
            nomeOriginalSeguro,
            nomeFisico,
            request.ContentType,
            request.TamanhoBytes,
            resultadoStorage.CaminhoRelativo,
            usuarioAtual.Id,
            usuarioAtual.Login);

        await anexoRepository.AddAsync(anexo, cancellationToken);

        var historico = new HistoricoChamado(
            chamadoId,
            TipoHistoricoChamado.AnexoAdicionado,
            PortalUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.AnexoAdicionado),
            usuarioAtual.Id,
            usuarioAtual.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);

        chamado.AtualizarAuditoria(usuarioAtual.Login);
        chamadoRepository.Update(chamado);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
            {
                Modulo = "Chamados",
                Entidade = "Chamado",
                EntidadeId = chamadoId.ToString(),
                Acao = TipoAcaoAuditoria.Edicao,
                Descricao = "Anexo adicionado ao chamado.",
                DadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    AnexoId = anexo.Id,
                    anexo.NomeArquivo,
                    anexo.ContentType,
                    anexo.TamanhoBytes
                }),
                Metadados = AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoId.ToString(),
                    codigo: chamado.Codigo,
                    nome: chamado.Titulo,
                    operacao: "AnexoAdicionado",
                    resultado: "Sucesso",
                    observacao: $"AnexoId: {anexo.Id}"),
                Nivel = NivelAuditoria.Informacao,
                Sucesso = true
            }, cancellationToken);
        }

        return new AnexoChamadoResponse(
            anexo.Id,
            anexo.NomeArquivo,
            anexo.ContentType,
            anexo.TamanhoBytes,
            anexo.CriadoEm,
            anexo.UsuarioId,
            usuarioAtual.Nome);
    }
}
