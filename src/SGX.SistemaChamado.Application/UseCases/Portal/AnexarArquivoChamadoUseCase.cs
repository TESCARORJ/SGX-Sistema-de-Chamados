using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
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
    IUnitOfWork unitOfWork) : IAnexarArquivoChamadoUseCase
{
    private static readonly HashSet<string> ExtensoesPermitidas =
    [
        ".pdf", ".png", ".jpg", ".jpeg", ".txt", ".doc", ".docx", ".xls", ".xlsx"
    ];

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

        ValidarAnexo(request);

        var extensao = Path.GetExtension(request.NomeArquivo).ToLowerInvariant();
        var nomeFisico = $"{Guid.NewGuid():N}{extensao}";

        var resultadoStorage = await arquivoStorageService.SalvarAsync(
            new ArquivoStorageRequest(nomeFisico, request.Conteudo),
            cancellationToken);

        var anexo = new AnexoChamado(
            chamadoId,
            request.NomeArquivo,
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

        return new AnexoChamadoResponse(
            anexo.Id,
            anexo.NomeArquivo,
            anexo.ContentType,
            anexo.TamanhoBytes,
            anexo.CriadoEm,
            anexo.UsuarioId,
            usuarioAtual.Nome);
    }

    private void ValidarAnexo(UploadAnexoChamadoRequest request)
    {
        var options = arquivosOptions.Value;

        if (request.TamanhoBytes <= 0)
        {
            throw new InvalidOperationException("Arquivo invalido: tamanho deve ser maior que zero.");
        }

        if (request.TamanhoBytes > options.TamanhoMaximoBytes)
        {
            throw new InvalidOperationException($"Arquivo excede o limite maximo de {options.TamanhoMaximoBytes} bytes.");
        }

        if (string.IsNullOrWhiteSpace(request.NomeArquivo))
        {
            throw new InvalidOperationException("Nome do arquivo obrigatorio.");
        }

        var nomeArquivo = Path.GetFileName(request.NomeArquivo);
        if (!string.Equals(nomeArquivo, request.NomeArquivo, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Nome de arquivo invalido.");
        }

        var extensao = Path.GetExtension(nomeArquivo).ToLowerInvariant();
        if (!ExtensoesPermitidas.Contains(extensao))
        {
            throw new InvalidOperationException("Extensao de arquivo nao permitida.");
        }

        var contentType = (request.ContentType ?? string.Empty).Trim().ToLowerInvariant();
        var mediaType = contentType.Split(';', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault() ?? string.Empty;
        var contentTypesPermitidos = options.ContentTypesPermitidos
            .Select(x => x.Trim().ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var isOctetStream = string.Equals(mediaType, "application/octet-stream", StringComparison.OrdinalIgnoreCase);
        if (!contentTypesPermitidos.Contains(mediaType) && !isOctetStream)
        {
            throw new InvalidOperationException("Content type nao permitido.");
        }
    }
}
