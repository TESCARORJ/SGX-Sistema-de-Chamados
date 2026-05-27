using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Email;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Application.Options;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Services.Email;

public sealed class EmailMessageProcessor(
    IRepository<LogIntegracaoEmail> logIntegracaoEmailRepository,
    IRepository<Chamado> chamadoRepository,
    IRepository<ComentarioChamado> comentarioRepository,
    IRepository<HistoricoChamado> historicoRepository,
    IRepository<AnexoChamado> anexoRepository,
    IRepository<Usuario> usuarioRepository,
    IRepository<UsuarioPerfilAcesso> usuarioPerfilRepository,
    IRepository<PerfilAcesso> perfilAcessoRepository,
    IRepository<Departamento> departamentoRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<PrioridadeChamado> prioridadeRepository,
    IRepository<StatusChamado> statusRepository,
    IRepository<ParametroSistema> parametroRepository,
    IEmailCorrelationService emailCorrelationService,
    IArquivoStorageService arquivoStorageService,
    ICodigoChamadoService codigoChamadoService,
    IPrioridadeChamadoMatrizService prioridadeChamadoMatrizService,
    ISlaService slaService,
    IOptions<ArquivosOptions> arquivosOptions,
    IOptions<EmailWorkerOptions> emailWorkerOptions,
    IUnitOfWork unitOfWork,
    ILogger<EmailMessageProcessor> logger) : IEmailMessageProcessor
{
    private const string UsuarioIntegracao = "integracao.email.worker";
    private const int MaxTitulo = 180;
    private const int MaxDescricao = 4000;
    private const int MaxComentario = 3000;
    private static readonly HashSet<string> ExtensoesBloqueadas = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".bat", ".cmd", ".ps1", ".js", ".vbs", ".msi", ".scr"
    };
    private static readonly Regex RegexScript = new(@"<script[\s\S]*?</script>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex RegexTagsHtml = new(@"<[^>]+>", RegexOptions.Compiled);

    public async Task<EmailMensagemProcessamentoResultado> ProcessarAsync(EmailMessageData mensagem, CancellationToken cancellationToken = default)
    {
        var agora = DateTime.UtcNow;
        var messageId = NormalizarHeader(mensagem.MessageId);
        var fingerprint = CalcularFingerprint(mensagem);

        var logExistente = await BuscarLogExistenteAsync(messageId, fingerprint, cancellationToken);
        if (logExistente is not null)
        {
            logExistente.RegistrarTentativa(UsuarioIntegracao);
            logExistente.MarcarDuplicado(logExistente.ChamadoId, agora, UsuarioIntegracao);
            logIntegracaoEmailRepository.Update(logExistente);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new EmailMensagemProcessamentoResultado(EmailMensagemProcessamentoStatus.Duplicado, logExistente.ChamadoId, null);
        }

        var log = new LogIntegracaoEmail(
            messageId,
            Limitar(NormalizarHeader(mensagem.InReplyTo), 600),
            Limitar(NormalizarReferences(mensagem.References), 4000),
            fingerprint,
            Limitar(mensagem.RemetenteEmail?.Trim().ToLowerInvariant(), 320) ?? "desconhecido@local",
            Limitar(mensagem.Destinatario, 1200),
            Limitar(mensagem.RemetenteNome, 180),
            Limitar(mensagem.Assunto, 600),
            mensagem.DataRecebimento == default ? agora : mensagem.DataRecebimento,
            UsuarioIntegracao);

        log.RegistrarTentativa(UsuarioIntegracao);
        await logIntegracaoEmailRepository.AddAsync(log, cancellationToken);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogWarning(dbEx, "Mensagem ignorada por conflito de chave unica (deduplicacao concorrente). MessageId={MessageId} Fingerprint={Fingerprint}", messageId, fingerprint);
            return new EmailMensagemProcessamentoResultado(EmailMensagemProcessamentoStatus.Duplicado, null, null);
        }

        Chamado? chamado = null;
        try
        {
            if (!RemetentePermitido(mensagem.RemetenteEmail))
            {
                log.MarcarIgnorado(DateTime.UtcNow, UsuarioIntegracao, "Remetente fora dos dominios permitidos.");
                logIntegracaoEmailRepository.Update(log);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return new EmailMensagemProcessamentoResultado(EmailMensagemProcessamentoStatus.Ignorado, null, "Remetente fora dos dominios permitidos.");
            }

            var correlationResult = await emailCorrelationService.CorrelacionarAsync(mensagem, cancellationToken);
            chamado = correlationResult.Chamado;
            var usuario = await ObterOuCriarUsuarioAsync(mensagem, cancellationToken);

            if (chamado is null && correlationResult.PossuiIndicadorResposta)
            {
                const string mensagemNaoCorrelacionada = "Nao foi possivel correlacionar a resposta com um chamado existente.";
                log.MarcarNaoCorrelacionado(DateTime.UtcNow, UsuarioIntegracao, mensagemNaoCorrelacionada);
                logIntegracaoEmailRepository.Update(log);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return new EmailMensagemProcessamentoResultado(EmailMensagemProcessamentoStatus.NaoCorrelacionado, null, mensagemNaoCorrelacionada);
            }

            if (chamado is null)
            {
                chamado = await CriarNovoChamadoAsync(mensagem, usuario, cancellationToken);
            }
            else
            {
                await AdicionarComentarioAsync(chamado, usuario, mensagem, cancellationToken);
            }

            var avisosAnexos = await ProcessarAnexosAsync(mensagem, chamado, usuario, cancellationToken);

            log.MarcarProcessado(chamado.Id, DateTime.UtcNow, UsuarioIntegracao);
            if (avisosAnexos.Count > 0)
            {
                log.AtualizarObservacao(
                    $"Mensagem processada, mas um ou mais anexos foram rejeitados. {string.Join(" | ", avisosAnexos.Take(5))}",
                    UsuarioIntegracao);
            }

            logIntegracaoEmailRepository.Update(log);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new EmailMensagemProcessamentoResultado(EmailMensagemProcessamentoStatus.Processado, chamado.Id, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao processar mensagem de e-mail. MessageId={MessageId} Fingerprint={Fingerprint}", messageId, fingerprint);
            log.MarcarErro(CriarMensagemErroControlada(ex), DateTime.UtcNow, UsuarioIntegracao, chamado?.Id);
            logIntegracaoEmailRepository.Update(log);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new EmailMensagemProcessamentoResultado(EmailMensagemProcessamentoStatus.Erro, chamado?.Id, CriarMensagemErroControlada(ex));
        }
    }

    private async Task<LogIntegracaoEmail?> BuscarLogExistenteAsync(string? messageId, string fingerprint, CancellationToken cancellationToken)
    {
        return await logIntegracaoEmailRepository.Query()
            .FirstOrDefaultAsync(
                x => x.Ativo && (x.Fingerprint == fingerprint || (messageId != null && x.MessageId == messageId)),
                cancellationToken);
    }

    private async Task<Usuario> ObterOuCriarUsuarioAsync(EmailMessageData mensagem, CancellationToken cancellationToken)
    {
        var remetente = (mensagem.RemetenteEmail ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(remetente))
        {
            throw new InvalidOperationException("Nao foi possivel identificar o remetente da mensagem.");
        }

        var usuario = await usuarioRepository.Query()
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstOrDefaultAsync(x => x.Email == remetente, cancellationToken);

        if (usuario is null)
        {
            var nome = string.IsNullOrWhiteSpace(mensagem.RemetenteNome)
                ? remetente.Split('@')[0]
                : mensagem.RemetenteNome.Trim();

            var login = await GerarLoginDisponivelAsync(remetente, cancellationToken);
            usuario = new Usuario(nome, remetente, login, UsuarioIntegracao);
            await usuarioRepository.AddAsync(usuario, cancellationToken);
        }

        await GarantirPerfilSolicitanteAsync(usuario, cancellationToken);
        return usuario;
    }

    private async Task GarantirPerfilSolicitanteAsync(Usuario usuario, CancellationToken cancellationToken)
    {
        var perfilSolicitante = await perfilAcessoRepository.Query()
            .FirstOrDefaultAsync(x => x.Ativo && x.TipoPerfil == TipoPerfil.Solicitante, cancellationToken)
            ?? throw new InvalidOperationException("Perfil Solicitante nao encontrado para associacao da integracao de e-mail.");

        var possuiPerfil = await usuarioPerfilRepository.Query()
            .AnyAsync(x => x.UsuarioId == usuario.Id && x.PerfilAcessoId == perfilSolicitante.Id, cancellationToken);

        if (!possuiPerfil)
        {
            await usuarioPerfilRepository.AddAsync(new UsuarioPerfilAcesso(usuario.Id, perfilSolicitante.Id, UsuarioIntegracao), cancellationToken);
        }
    }

    private async Task<string> GerarLoginDisponivelAsync(string email, CancellationToken cancellationToken)
    {
        var baseLogin = email.Split('@')[0].Trim().ToLowerInvariant();
        baseLogin = string.IsNullOrWhiteSpace(baseLogin)
            ? "email"
            : Regex.Replace(baseLogin, @"[^a-z0-9._-]", string.Empty);

        if (string.IsNullOrWhiteSpace(baseLogin))
        {
            baseLogin = "email";
        }

        var candidato = baseLogin;
        var contador = 1;

        while (await usuarioRepository.Query().AnyAsync(x => x.Login == candidato, cancellationToken))
        {
            contador++;
            candidato = $"{baseLogin}.{contador}";
        }

        return candidato;
    }

    private async Task<Chamado> CriarNovoChamadoAsync(EmailMessageData mensagem, Usuario solicitante, CancellationToken cancellationToken)
    {
        var categoria = await ObterCategoriaPadraoAsync(cancellationToken);
        var naturezaChamado = DeterminarNaturezaChamado(mensagem);
        var impactoChamado = ImpactoChamadoEnum.Baixo;
        var urgenciaChamado = naturezaChamado == NaturezaChamadoEnum.Incidente
            ? UrgenciaChamadoEnum.Media
            : UrgenciaChamadoEnum.Baixa;

        var prioridade = await prioridadeChamadoMatrizService.ObterPrioridadeAsync(impactoChamado, urgenciaChamado, cancellationToken)
            ?? await ObterPrioridadePadraoAsync(cancellationToken);
        var statusAberto = await statusRepository.Query()
            .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == StatusChamadoEnum.Aberto, cancellationToken)
            ?? throw new InvalidOperationException("Status 'Aberto' nao encontrado.");

        var titulo = Limitar(SanitizarTexto(mensagem.Assunto), MaxTitulo);
        if (string.IsNullOrWhiteSpace(titulo))
        {
            titulo = "Chamado aberto por e-mail";
        }

        var descricao = Limitar(SanitizarCorpoMensagem(mensagem), MaxDescricao);
        if (string.IsNullOrWhiteSpace(descricao))
        {
            descricao = "Mensagem recebida por integracao de e-mail sem conteudo textual.";
        }

        var codigo = await codigoChamadoService.GerarAsync(cancellationToken);
        var departamentoId = await ObterDepartamentoPadraoIdAsync(categoria.DepartamentoId, cancellationToken);
        var chamado = new Chamado(
            codigo,
            titulo,
            descricao,
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            statusAberto.Id,
            OrigemChamado.Email,
            UsuarioIntegracao,
            departamentoId,
            naturezaChamado: naturezaChamado,
            impactoChamado: impactoChamado,
            urgenciaChamado: urgenciaChamado);

        await chamadoRepository.AddAsync(chamado, cancellationToken);

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.IntegracaoEmail,
            "Chamado criado a partir de e-mail",
            solicitante.Id,
            UsuarioIntegracao);

        await historicoRepository.AddAsync(historico, cancellationToken);
        await slaService.InicializarNaAberturaAsync(chamado, UsuarioIntegracao, DateTime.UtcNow, cancellationToken);
        return chamado;
    }

    private async Task AdicionarComentarioAsync(Chamado chamado, Usuario autor, EmailMessageData mensagem, CancellationToken cancellationToken)
    {
        if (!chamado.Ativo)
        {
            throw new InvalidOperationException("Chamado correlacionado esta inativo e nao pode receber comentarios.");
        }

        var comentarioTexto = Limitar(SanitizarCorpoMensagem(mensagem), MaxComentario);
        if (string.IsNullOrWhiteSpace(comentarioTexto))
        {
            comentarioTexto = "Resposta recebida por e-mail sem conteudo textual.";
        }

        var comentario = new ComentarioChamado(
            chamado.Id,
            autor.Id,
            comentarioTexto,
            false,
            UsuarioIntegracao);

        await comentarioRepository.AddAsync(comentario, cancellationToken);

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.IntegracaoEmail,
            "Resposta recebida por e-mail",
            autor.Id,
            UsuarioIntegracao);

        await historicoRepository.AddAsync(historico, cancellationToken);
        chamado.AtualizarAuditoria(UsuarioIntegracao);
        chamadoRepository.Update(chamado);
    }

    private async Task<IReadOnlyCollection<string>> ProcessarAnexosAsync(EmailMessageData mensagem, Chamado chamado, Usuario usuario, CancellationToken cancellationToken)
    {
        var avisos = new List<string>();
        var options = arquivosOptions.Value;
        var workerOptions = emailWorkerOptions.Value;
        var contentTypesPermitidos = options.ContentTypesPermitidos
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var extensoesPermitidas = workerOptions.ExtensoesPermitidas
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().StartsWith('.') ? x.Trim().ToLowerInvariant() : $".{x.Trim().ToLowerInvariant()}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var tamanhoMaximoBytesWorker = Math.Max(1, workerOptions.TamanhoMaximoAnexoMb) * 1024L * 1024L;
        var tamanhoMaximoBytes = Math.Min(options.TamanhoMaximoBytes, tamanhoMaximoBytesWorker);

        foreach (var anexo in mensagem.Anexos)
        {
            try
            {
                if (anexo.Conteudo.Length <= 0)
                {
                    logger.LogWarning("Anexo ignorado (tamanho invalido). MessageId={MessageId} Nome={Nome}", mensagem.MessageId, anexo.NomeArquivo);
                    avisos.Add($"Anexo '{anexo.NomeArquivo}' rejeitado por tamanho invalido.");
                    continue;
                }

                var tamanhoAnexo = anexo.TamanhoBytes > 0 ? anexo.TamanhoBytes : anexo.Conteudo.LongLength;
                if (tamanhoAnexo > tamanhoMaximoBytes)
                {
                    logger.LogWarning("Anexo ignorado por exceder limite de tamanho. MessageId={MessageId} Nome={Nome} Tamanho={Tamanho}", mensagem.MessageId, anexo.NomeArquivo, tamanhoAnexo);
                    avisos.Add($"Anexo '{anexo.NomeArquivo}' rejeitado por exceder tamanho maximo.");
                    continue;
                }

                var nomeArquivoOriginal = SanitizarNomeArquivo(anexo.NomeArquivo);
                if (string.IsNullOrWhiteSpace(nomeArquivoOriginal))
                {
                    logger.LogWarning("Anexo ignorado por nome invalido. MessageId={MessageId}", mensagem.MessageId);
                    avisos.Add("Anexo rejeitado por nome invalido.");
                    continue;
                }

                var extensao = Path.GetExtension(nomeArquivoOriginal).Trim().ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(extensao))
                {
                    logger.LogWarning("Anexo ignorado por nao possuir extensao. MessageId={MessageId} Nome={Nome}", mensagem.MessageId, nomeArquivoOriginal);
                    avisos.Add($"Anexo '{nomeArquivoOriginal}' rejeitado por nao possuir extensao.");
                    continue;
                }

                if (ExtensoesBloqueadas.Contains(extensao))
                {
                    logger.LogWarning("Anexo ignorado por extensao bloqueada. MessageId={MessageId} Nome={Nome} Extensao={Extensao}", mensagem.MessageId, nomeArquivoOriginal, extensao);
                    avisos.Add($"Anexo '{nomeArquivoOriginal}' rejeitado por extensao bloqueada.");
                    continue;
                }

                if (extensoesPermitidas.Count > 0 && !extensoesPermitidas.Contains(extensao))
                {
                    logger.LogWarning("Anexo ignorado por extensao nao permitida. MessageId={MessageId} Nome={Nome} Extensao={Extensao}", mensagem.MessageId, nomeArquivoOriginal, extensao);
                    avisos.Add($"Anexo '{nomeArquivoOriginal}' rejeitado por extensao nao permitida.");
                    continue;
                }

                var contentType = (anexo.ContentType ?? string.Empty).Trim().ToLowerInvariant();
                var mediaType = contentType.Split(';', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault() ?? string.Empty;
                var isOctetStream = string.Equals(mediaType, "application/octet-stream", StringComparison.OrdinalIgnoreCase);

                if (!contentTypesPermitidos.Contains(mediaType) && !isOctetStream)
                {
                    logger.LogWarning("Anexo ignorado por content type nao permitido. MessageId={MessageId} Nome={Nome} ContentType={ContentType}", mensagem.MessageId, nomeArquivoOriginal, contentType);
                    avisos.Add($"Anexo '{nomeArquivoOriginal}' rejeitado por content type nao permitido.");
                    continue;
                }

                var nomeFisico = $"{Guid.NewGuid():N}{extensao}";
                await using var stream = new MemoryStream(anexo.Conteudo, writable: false);

                var resultadoStorage = await arquivoStorageService.SalvarAsync(
                    new ArquivoStorageRequest(nomeFisico, stream),
                    cancellationToken);

                var anexoChamado = new AnexoChamado(
                    chamado.Id,
                    nomeArquivoOriginal,
                    nomeFisico,
                    string.IsNullOrWhiteSpace(mediaType) ? "application/octet-stream" : mediaType,
                    tamanhoAnexo,
                    resultadoStorage.CaminhoRelativo,
                    usuario.Id,
                    UsuarioIntegracao);

                await anexoRepository.AddAsync(anexoChamado, cancellationToken);

                var historico = new HistoricoChamado(
                    chamado.Id,
                    TipoHistoricoChamado.AnexoAdicionado,
                    $"Anexo recebido por e-mail: {nomeArquivoOriginal}",
                    usuario.Id,
                    UsuarioIntegracao);

                await historicoRepository.AddAsync(historico, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Falha ao salvar anexo da integracao de e-mail. MessageId={MessageId} Nome={Nome}", mensagem.MessageId, anexo.NomeArquivo);
                avisos.Add($"Falha ao salvar anexo '{anexo.NomeArquivo}'.");
            }
        }

        return avisos;
    }

    private async Task<CategoriaChamado> ObterCategoriaPadraoAsync(CancellationToken cancellationToken)
    {
        var categoriaPadraoId = emailWorkerOptions.Value.CategoriaPadraoId;
        if (categoriaPadraoId.HasValue && categoriaPadraoId.Value != Guid.Empty)
        {
            var categoriaPorConfiguracao = await categoriaRepository.Query()
                .FirstOrDefaultAsync(x => x.Ativo && x.Id == categoriaPadraoId.Value, cancellationToken);
            if (categoriaPorConfiguracao is not null)
            {
                return categoriaPorConfiguracao;
            }

            throw new InvalidOperationException("CategoriaPadraoId configurada no EmailWorker nao foi encontrada ou esta inativa.");
        }

        var categoriaIdParam = await ObterParametroAtivoAsync("email.integracao.categoriaPadraoId", cancellationToken);
        if (Guid.TryParse(categoriaIdParam, out var categoriaId))
        {
            var categoriaPorId = await categoriaRepository.Query()
                .FirstOrDefaultAsync(x => x.Ativo && x.Id == categoriaId, cancellationToken);
            if (categoriaPorId is not null)
            {
                return categoriaPorId;
            }
        }

        var categoriaNomeParam = await ObterParametroAtivoAsync("email.integracao.categoriaPadrao", cancellationToken);
        if (!string.IsNullOrWhiteSpace(categoriaNomeParam))
        {
            var categoriaPorNome = await categoriaRepository.Query()
                .FirstOrDefaultAsync(x => x.Ativo && x.Nome == categoriaNomeParam, cancellationToken);
            if (categoriaPorNome is not null)
            {
                return categoriaPorNome;
            }
        }

        return await categoriaRepository.Query()
            .FirstOrDefaultAsync(x => x.Ativo && x.Nome == "Suporte Tecnico", cancellationToken)
            ?? throw new InvalidOperationException("Categoria padrao para integracao de e-mail nao encontrada.");
    }

    private async Task<PrioridadeChamado> ObterPrioridadePadraoAsync(CancellationToken cancellationToken)
    {
        var prioridadePadraoId = emailWorkerOptions.Value.PrioridadePadraoId;
        if (prioridadePadraoId.HasValue && prioridadePadraoId.Value != Guid.Empty)
        {
            var prioridadePorConfiguracao = await prioridadeRepository.Query()
                .FirstOrDefaultAsync(x => x.Ativo && x.Id == prioridadePadraoId.Value, cancellationToken);
            if (prioridadePorConfiguracao is not null)
            {
                return prioridadePorConfiguracao;
            }

            throw new InvalidOperationException("PrioridadePadraoId configurada no EmailWorker nao foi encontrada ou esta inativa.");
        }

        var prioridadeIdParam = await ObterParametroAtivoAsync("email.integracao.prioridadePadraoId", cancellationToken);
        if (Guid.TryParse(prioridadeIdParam, out var prioridadeId))
        {
            var prioridadePorId = await prioridadeRepository.Query()
                .FirstOrDefaultAsync(x => x.Ativo && x.Id == prioridadeId, cancellationToken);
            if (prioridadePorId is not null)
            {
                return prioridadePorId;
            }
        }

        var prioridadeNomeParam = await ObterParametroAtivoAsync("email.integracao.prioridadePadrao", cancellationToken);
        if (!string.IsNullOrWhiteSpace(prioridadeNomeParam))
        {
            var prioridadePorNome = await prioridadeRepository.Query()
                .FirstOrDefaultAsync(x => x.Ativo && x.Nome == prioridadeNomeParam, cancellationToken);
            if (prioridadePorNome is not null)
            {
                return prioridadePorNome;
            }
        }

        return await prioridadeRepository.Query()
            .FirstOrDefaultAsync(x => x.Ativo && x.Nivel == PrioridadeChamadoEnum.Media, cancellationToken)
            ?? throw new InvalidOperationException("Prioridade padrao para integracao de e-mail nao encontrada.");
    }

    private async Task<string?> ObterParametroAtivoAsync(string chave, CancellationToken cancellationToken)
    {
        var parametro = await parametroRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ativo && x.Chave == chave, cancellationToken);

        return parametro?.Valor?.Trim();
    }

    private async Task<Guid?> ObterDepartamentoPadraoIdAsync(Guid? departamentoCategoria, CancellationToken cancellationToken)
    {
        var departamentoPadraoId = emailWorkerOptions.Value.DepartamentoPadraoId;
        if (!departamentoPadraoId.HasValue || departamentoPadraoId.Value == Guid.Empty)
        {
            return departamentoCategoria;
        }

        var departamentoAtivo = await departamentoRepository.Query()
            .AnyAsync(x => x.Ativo && x.Id == departamentoPadraoId.Value, cancellationToken);
        if (!departamentoAtivo)
        {
            throw new InvalidOperationException("DepartamentoPadraoId configurado no EmailWorker nao foi encontrado ou esta inativo.");
        }

        return departamentoPadraoId.Value;
    }

    private static NaturezaChamadoEnum DeterminarNaturezaChamado(EmailMessageData mensagem)
    {
        var assunto = SanitizarTexto(mensagem.Assunto);
        var corpo = SanitizarCorpoMensagem(mensagem);
        var texto = $"{assunto} {corpo}".Trim().ToLowerInvariant();
        return ContemIndicadorIncidente(texto)
            ? NaturezaChamadoEnum.Incidente
            : NaturezaChamadoEnum.Requisicao;
    }

    private static bool ContemIndicadorIncidente(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return false;
        }

        string[] termosIncidente =
        [
            "erro",
            "falha",
            "indisponibilidade",
            "indisponivel",
            "travamento",
            "travou",
            "sem acesso",
            "queda",
            "fora do ar"
        ];

        return termosIncidente.Any(texto.Contains);
    }

    private static string? Limitar(string? valor, int maximo)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        var texto = valor.Trim();
        return texto.Length <= maximo ? texto : texto[..maximo];
    }

    private static string SanitizarCorpoMensagem(EmailMessageData mensagem)
    {
        var baseTexto = !string.IsNullOrWhiteSpace(mensagem.CorpoTexto)
            ? mensagem.CorpoTexto!
            : mensagem.CorpoHtml ?? string.Empty;

        var texto = SanitizarTexto(baseTexto);
        var linhas = texto.Split('\n')
            .Select(l => l.TrimEnd())
            .ToList();

        var corte = linhas.Count;
        for (var i = 0; i < linhas.Count; i++)
        {
            if (linhas[i].StartsWith(">", StringComparison.Ordinal) ||
                linhas[i].StartsWith("--", StringComparison.Ordinal) ||
                linhas[i].Contains(" escreveu:", StringComparison.OrdinalIgnoreCase))
            {
                corte = i;
                break;
            }
        }

        var linhasFiltradas = linhas.Take(corte)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        return string.Join(Environment.NewLine, linhasFiltradas).Trim();
    }

    private static string SanitizarTexto(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return string.Empty;
        }

        var semScript = RegexScript.Replace(texto, string.Empty);
        var semTags = RegexTagsHtml.Replace(semScript, " ");
        var normalizado = System.Net.WebUtility.HtmlDecode(semTags);
        return Regex.Replace(normalizado, @"\s+", " ").Trim();
    }

    private static string CalcularFingerprint(EmailMessageData mensagem)
    {
        var raw = new StringBuilder()
            .Append((mensagem.RemetenteEmail ?? string.Empty).Trim().ToLowerInvariant())
            .Append('|')
            .Append((mensagem.Assunto ?? string.Empty).Trim())
            .Append('|')
            .Append((mensagem.CorpoTexto ?? mensagem.CorpoHtml ?? string.Empty).Trim())
            .Append('|')
            .Append(mensagem.DataRecebimento.ToUniversalTime().ToString("yyyyMMddHHmm"))
            .ToString();

        var bytes = Encoding.UTF8.GetBytes(raw);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string? NormalizarHeader(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim().Trim('<', '>');
    }

    private static string? NormalizarReferences(IReadOnlyCollection<string> references)
    {
        if (references.Count == 0)
        {
            return null;
        }

        var normalizados = references
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => NormalizarHeader(x)!)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return normalizados.Length == 0 ? null : string.Join(';', normalizados);
    }

    private static string CriarMensagemErroControlada(Exception ex)
    {
        var mensagem = ex.Message;
        if (string.IsNullOrWhiteSpace(mensagem))
        {
            mensagem = "Erro ao processar mensagem de e-mail.";
        }

        return Limitar(mensagem, 8000) ?? "Erro ao processar mensagem de e-mail.";
    }

    private static string? SanitizarNomeArquivo(string? nomeArquivo)
    {
        if (string.IsNullOrWhiteSpace(nomeArquivo))
        {
            return null;
        }

        var nomeBase = Path.GetFileName(nomeArquivo.Trim());
        if (string.IsNullOrWhiteSpace(nomeBase))
        {
            return null;
        }

        var nomeSemControle = new string(nomeBase.Where(ch => !char.IsControl(ch)).ToArray()).Trim();
        if (string.IsNullOrWhiteSpace(nomeSemControle))
        {
            return null;
        }

        foreach (var invalido in Path.GetInvalidFileNameChars())
        {
            nomeSemControle = nomeSemControle.Replace(invalido, '_');
        }

        return nomeSemControle.Trim();
    }

    private bool RemetentePermitido(string? remetenteEmail)
    {
        var dominios = emailWorkerOptions.Value.DominiosPermitidos
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToLowerInvariant())
            .ToArray();

        if (dominios.Length == 0)
        {
            return true;
        }

        var remetente = (remetenteEmail ?? string.Empty).Trim().ToLowerInvariant();
        var arrobaIndex = remetente.LastIndexOf('@');
        if (arrobaIndex <= 0 || arrobaIndex == remetente.Length - 1)
        {
            return false;
        }

        var dominio = remetente[(arrobaIndex + 1)..];
        return dominios.Contains(dominio, StringComparer.OrdinalIgnoreCase);
    }
}
