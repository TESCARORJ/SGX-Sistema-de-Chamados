using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Options;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Chamados;

public sealed class ListarAnexosChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<AnexoChamado> anexoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarAnexosChamadoUseCase
{
    public async Task<IReadOnlyCollection<AnexoChamadoResponse>> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        ComentariosChamadoUseCaseHelpers.ValidarPerfilPermitido(usuario);

        var chamado = await chamadoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        if (!ComentariosChamadoUseCaseHelpers.PodeAcessarChamado(usuario, chamado))
        {
            throw new KeyNotFoundException("Chamado nao encontrado.");
        }

        var anexos = await anexoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Usuario)
            .Where(x => x.Ativo && x.ChamadoId == chamadoId)
            .OrderBy(x => x.CriadoEm)
            .Select(x => new AnexoChamadoResponse(
                x.Id,
                x.NomeArquivo,
                x.ContentType,
                x.TamanhoBytes,
                x.CriadoEm,
                x.UsuarioId,
                x.Usuario.Nome))
            .ToArrayAsync(cancellationToken);

        return anexos;
    }
}

public sealed class AdicionarAnexoChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<AnexoChamado> anexoRepository,
    IRepository<HistoricoChamado> historicoRepository,
    IArquivoStorageService arquivoStorageService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IOptions<ArquivosOptions> arquivosOptions,
    IUnitOfWork unitOfWork) : IAdicionarAnexoChamadoUseCase
{
    public async Task<UploadAnexoChamadoResponse> ExecutarAsync(Guid chamadoId, CriarAnexoChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        ComentariosChamadoUseCaseHelpers.ValidarPerfilPermitido(usuario);

        var chamado = await chamadoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        if (!ComentariosChamadoUseCaseHelpers.PodeAcessarChamado(usuario, chamado))
        {
            throw new KeyNotFoundException("Chamado nao encontrado.");
        }

        var (nomeArquivoOriginalSeguro, extensao) = AnexoAtendimentoHelper.ValidarUpload(
            request.NomeArquivo,
            request.ContentType,
            request.TamanhoBytes,
            arquivosOptions.Value);
        var nomeFisico = AnexoAtendimentoHelper.GerarNomeFisicoSeguro(extensao);

        var resultadoStorage = await arquivoStorageService.SalvarAsync(
            new ArquivoStorageRequest(nomeFisico, request.Conteudo),
            cancellationToken);

        var anexo = new AnexoChamado(
            chamado.Id,
            nomeArquivoOriginalSeguro,
            nomeFisico,
            request.ContentType,
            request.TamanhoBytes,
            resultadoStorage.CaminhoRelativo,
            usuario.Id,
            usuario.Login);

        await anexoRepository.AddAsync(anexo, cancellationToken);

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.AnexoAdicionado,
            ComentariosChamadoUseCaseHelpers.EhAdministradorOuAtendente(usuario)
                ? "Anexo adicionado no atendimento"
                : "Anexo adicionado",
            usuario.Id,
            usuario.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);

        chamado.AtualizarAuditoria(usuario.Login);
        chamadoRepository.Update(chamado);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UploadAnexoChamadoResponse(
            anexo.Id,
            anexo.NomeArquivo,
            anexo.ContentType,
            anexo.TamanhoBytes,
            anexo.CriadoEm,
            anexo.UsuarioId,
            usuario.Nome);
    }
}

public sealed class BaixarAnexoChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<AnexoChamado> anexoRepository,
    IArquivoStorageService arquivoStorageService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IBaixarAnexoChamadoUseCase
{
    public async Task<DownloadAnexoChamadoResponse> ExecutarAsync(Guid chamadoId, Guid anexoId, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        if (anexoId == Guid.Empty)
        {
            throw new ArgumentException("Id do anexo invalido.", nameof(anexoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        ComentariosChamadoUseCaseHelpers.ValidarPerfilPermitido(usuario);

        var chamado = await chamadoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        if (!ComentariosChamadoUseCaseHelpers.PodeAcessarChamado(usuario, chamado))
        {
            throw new KeyNotFoundException("Chamado nao encontrado.");
        }

        var anexo = await anexoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == anexoId && x.ChamadoId == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Anexo nao encontrado.");

        var stream = await arquivoStorageService.AbrirLeituraAsync(anexo.Caminho, cancellationToken);

        return new DownloadAnexoChamadoResponse
        {
            Conteudo = stream,
            NomeArquivo = anexo.NomeArquivo,
            ContentType = anexo.ContentType
        };
    }
}
