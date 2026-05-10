using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.Interfaces.Email;
using SGX.SistemaChamado.Application.Options;
using SGX.SistemaChamado.Application.Services.Email;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class EmailMessageProcessorTests
{
    [Fact]
    public async Task EmailNovoCriaChamadoComOrigemStatusHistoricoECamposPadrao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);
        var messageId = "msg-novo-1";

        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), new FakeArquivoStorageService());
        var resultado = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "1",
            MessageId = messageId,
            InReplyTo = "origem-anterior@sgx.local",
            References = ["origem-anterior@sgx.local", "origem-principal@sgx.local"],
            RemetenteEmail = "novo@sgx.local",
            Destinatario = "suporte@sgx.local",
            RemetenteNome = "Novo",
            Assunto = "Falha no sistema",
            CorpoTexto = "Descricao do problema",
            DataRecebimento = DateTime.UtcNow
        });

        var chamado = context.Chamados.Single(x => x.Origem == OrigemChamado.Email);
        var statusAberto = context.StatusChamado.Single(x => x.Codigo == StatusChamadoEnum.Aberto);
        var prioridadeMedia = context.PrioridadesChamado.Single(x => x.Nivel == PrioridadeChamadoEnum.Media);
        var categoria = context.CategoriasChamado.Single(x => x.Nome == "Suporte Tecnico");
        var historico = context.HistoricosChamado.Single(x => x.ChamadoId == chamado.Id && x.Tipo == TipoHistoricoChamado.IntegracaoEmail);
        var log = context.LogsIntegracaoEmail.Single(x => x.MessageId == messageId);

        Assert.Equal(EmailMensagemProcessamentoStatus.Processado, resultado.Status);
        Assert.Equal(statusAberto.Id, chamado.StatusId);
        Assert.Equal(categoria.Id, chamado.CategoriaId);
        Assert.Equal(prioridadeMedia.Id, chamado.PrioridadeId);
        Assert.Equal(categoria.DepartamentoId, chamado.DepartamentoId);
        Assert.Equal("Chamado criado a partir de e-mail", historico.Descricao);
        Assert.Equal("suporte@sgx.local", log.Destinatario);
        Assert.Equal("origem-anterior@sgx.local", log.InReplyTo);
        Assert.Contains("origem-principal@sgx.local", log.References);
        Assert.Equal(chamado.Id, log.ChamadoId);
        Assert.Equal(StatusProcessamentoEmail.Processado, log.StatusProcessamento);
    }

    [Fact]
    public async Task AssuntoVazioUsaTituloPadrao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);

        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), new FakeArquivoStorageService());
        var _ = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "assunto-vazio",
            MessageId = "msg-assunto-vazio",
            RemetenteEmail = "solicitante@sgx.local",
            Assunto = "   ",
            CorpoTexto = "Corpo valido",
            DataRecebimento = DateTime.UtcNow
        });

        var chamado = context.Chamados.Single(x => x.Origem == OrigemChamado.Email);
        Assert.Equal("Chamado aberto por e-mail", chamado.Titulo);
    }

    [Fact]
    public async Task CorpoHtmlEhTratadoSemScript()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);

        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), new FakeArquivoStorageService());
        var _ = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "html-1",
            MessageId = "msg-html-1",
            RemetenteEmail = "html@sgx.local",
            Assunto = "HTML",
            CorpoHtml = "<h1>Teste</h1><script>alert('x')</script><p>Texto final</p>",
            DataRecebimento = DateTime.UtcNow
        });

        var chamado = context.Chamados.Single(x => x.Origem == OrigemChamado.Email);
        Assert.DoesNotContain("script", chamado.Descricao, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Teste", chamado.Descricao);
        Assert.Contains("Texto final", chamado.Descricao);
    }

    [Fact]
    public async Task RemetenteInexistenteCriaSolicitante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);

        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), new FakeArquivoStorageService());
        var _ = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "novo-remetente",
            MessageId = "msg-novo-remetente",
            RemetenteEmail = "novo.remetente@sgx.local",
            RemetenteNome = "Novo Remetente",
            Assunto = "Novo remetente",
            CorpoTexto = "Corpo",
            DataRecebimento = DateTime.UtcNow
        });

        var usuario = context.Usuarios.Single(x => x.Email == "novo.remetente@sgx.local");
        var perfilSolicitante = context.PerfisAcesso.Single(x => x.TipoPerfil == TipoPerfil.Solicitante);
        var possuiVinculo = context.UsuariosPerfisAcesso.Any(x => x.UsuarioId == usuario.Id && x.PerfilAcessoId == perfilSolicitante.Id);

        Assert.True(possuiVinculo);
    }

    [Fact]
    public async Task RemetenteExistenteEhReutilizadoComoSolicitante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);
        var usuarioExistente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Remetente Existente", "existente@sgx.local", TipoPerfil.Solicitante);

        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), new FakeArquivoStorageService());
        var _ = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "existente-remetente",
            MessageId = "msg-existente-remetente",
            RemetenteEmail = "existente@sgx.local",
            Assunto = "Assunto",
            CorpoTexto = "Corpo",
            DataRecebimento = DateTime.UtcNow
        });

        var chamado = context.Chamados.Single(x => x.Origem == OrigemChamado.Email);
        Assert.Equal(usuarioExistente.Id, chamado.SolicitanteId);
    }

    [Fact]
    public async Task DepartamentoPadraoConfiguradoEhAplicado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);

        var departamentoPadrao = new Departamento("Financeiro", "FIN", null, "teste");
        context.Departamentos.Add(departamentoPadrao);
        await context.SaveChangesAsync();

        var options = CriarEmailWorkerOptions(departamentoPadraoId: departamentoPadrao.Id);
        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), new FakeArquivoStorageService(), options);

        var _ = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "dep-padrao",
            MessageId = "msg-dep-padrao",
            RemetenteEmail = "dep@sgx.local",
            Assunto = "Departamento padrao",
            CorpoTexto = "Corpo",
            DataRecebimento = DateTime.UtcNow
        });

        var chamado = context.Chamados.Single(x => x.Origem == OrigemChamado.Email);
        Assert.Equal(departamentoPadrao.Id, chamado.DepartamentoId);
    }

    [Fact]
    public async Task RespostaCorrelacionadaAdicionaComentarioPublicoEHistorico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var chamado = await SeedChamadoAsync(context, "SGX-2026-009999");

        var processor = CriarProcessor(context, new FakeEmailCorrelationService(chamado), new FakeArquivoStorageService());
        var resultado = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "resp-1",
            MessageId = "msg-resposta-1",
            RemetenteEmail = "resposta@sgx.local",
            Assunto = "Re: SGX-2026-009999",
            CorpoTexto = "Atualizacao do chamado",
            DataRecebimento = DateTime.UtcNow
        });

        var comentario = context.ComentariosChamado.Single(x => x.ChamadoId == chamado.Id);
        var historico = context.HistoricosChamado.Single(x => x.ChamadoId == chamado.Id && x.Descricao == "Resposta recebida por e-mail");
        var log = context.LogsIntegracaoEmail.Single(x => x.MessageId == "msg-resposta-1");

        Assert.Equal(EmailMensagemProcessamentoStatus.Processado, resultado.Status);
        Assert.False(comentario.Interno);
        Assert.Equal("Atualizacao do chamado", comentario.Mensagem);
        Assert.NotNull(historico);
        Assert.Equal(chamado.Id, log.ChamadoId);
    }

    [Fact]
    public async Task RespostaCorrelacionadaComCorpoVazioUsaTextoPadrao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var chamado = await SeedChamadoAsync(context, "SGX-2026-009998");

        var processor = CriarProcessor(context, new FakeEmailCorrelationService(chamado), new FakeArquivoStorageService());
        var _ = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "resp-vazia-1",
            MessageId = "msg-resposta-vazia-1",
            RemetenteEmail = "resposta@sgx.local",
            Assunto = "Re: SGX-2026-009998",
            CorpoTexto = "   ",
            CorpoHtml = "<div>   </div>",
            DataRecebimento = DateTime.UtcNow
        });

        var comentario = context.ComentariosChamado.Single(x => x.ChamadoId == chamado.Id);
        Assert.Equal("Resposta recebida por e-mail sem conteudo textual.", comentario.Mensagem);
    }

    [Fact]
    public async Task RespostaSemCorrelacaoComIndicadoresMarcaNaoCorrelacionado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);

        var processor = CriarProcessor(
            context,
            new FakeEmailCorrelationService(null, possuiIndicadorResposta: true, codigoDetectado: "SGX-2026-888888"),
            new FakeArquivoStorageService());

        var resultado = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "nao-correl-1",
            MessageId = "msg-nao-correl-1",
            RemetenteEmail = "resposta@sgx.local",
            Assunto = "Re: SGX-2026-888888",
            InReplyTo = "<inexistente@sgx.local>",
            CorpoTexto = "Texto",
            DataRecebimento = DateTime.UtcNow
        });

        var log = context.LogsIntegracaoEmail.Single(x => x.MessageId == "msg-nao-correl-1");
        Assert.Equal(EmailMensagemProcessamentoStatus.NaoCorrelacionado, resultado.Status);
        Assert.Equal(StatusProcessamentoEmail.NaoCorrelacionado, log.StatusProcessamento);
        Assert.Empty(context.Chamados.Where(x => x.Origem == OrigemChamado.Email));
    }

    [Fact]
    public async Task DuplicidadePorMessageIdEhBloqueada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);
        var chamado = await SeedChamadoAsync(context, "SGX-2026-001111");
        var storage = new FakeArquivoStorageService();
        var processor = CriarProcessor(context, new FakeEmailCorrelationService(chamado), storage);

        _ = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "3",
            MessageId = "msg-duplicada-id",
            RemetenteEmail = "dup.id@sgx.local",
            Assunto = "Re: SGX-2026-001111",
            CorpoTexto = "Mensagem",
            DataRecebimento = DateTime.UtcNow,
            Anexos =
            [
                new EmailAttachmentData("arquivo.pdf", "application/pdf", [1, 2, 3, 4], 4, null)
            ]
        });

        var duplicada = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "4",
            MessageId = "msg-duplicada-id",
            RemetenteEmail = "dup.id@sgx.local",
            Assunto = "Re: SGX-2026-001111",
            CorpoTexto = "Outra mensagem",
            DataRecebimento = DateTime.UtcNow
        });

        Assert.Equal(EmailMensagemProcessamentoStatus.Duplicado, duplicada.Status);
        Assert.Single(context.ComentariosChamado.Where(x => x.ChamadoId == chamado.Id));
        Assert.Single(context.AnexosChamado.Where(x => x.ChamadoId == chamado.Id));
        Assert.Single(storage.Salvos);
    }

    [Fact]
    public async Task RemetenteForaDominioPermitidoEhIgnoradoELogado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);

        var options = CriarEmailWorkerOptions(dominiosPermitidos: ["sgx.local"]);
        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), new FakeArquivoStorageService(), options);

        var resultado = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "dominio-bloqueado",
            MessageId = "msg-dominio-bloqueado",
            RemetenteEmail = "externo@fora.com",
            Assunto = "Nao permitido",
            CorpoTexto = "Corpo",
            DataRecebimento = DateTime.UtcNow
        });

        var log = context.LogsIntegracaoEmail.Single(x => x.MessageId == "msg-dominio-bloqueado");
        Assert.Equal(EmailMensagemProcessamentoStatus.Ignorado, resultado.Status);
        Assert.Empty(context.Chamados.Where(x => x.Origem == OrigemChamado.Email));
        Assert.Equal(StatusProcessamentoEmail.Ignorado, log.StatusProcessamento);
        Assert.Contains("dominios permitidos", log.Erro ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CategoriaPadraoInvalidaGeraErroENaoCriaChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);

        var options = CriarEmailWorkerOptions(categoriaPadraoId: Guid.NewGuid());
        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), new FakeArquivoStorageService(), options);

        var resultado = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "cat-invalida",
            MessageId = "msg-cat-invalida",
            RemetenteEmail = "cat@sgx.local",
            Assunto = "Categoria invalida",
            CorpoTexto = "Corpo",
            DataRecebimento = DateTime.UtcNow
        });

        var log = context.LogsIntegracaoEmail.Single(x => x.MessageId == "msg-cat-invalida");
        Assert.Equal(EmailMensagemProcessamentoStatus.Erro, resultado.Status);
        Assert.Empty(context.Chamados.Where(x => x.Origem == OrigemChamado.Email));
        Assert.Equal(StatusProcessamentoEmail.Erro, log.StatusProcessamento);
    }

    [Fact]
    public async Task DuplicidadePorFingerprintEhBloqueada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);
        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), new FakeArquivoStorageService());
        var referencia = DateTime.UtcNow;

        _ = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "5",
            MessageId = null,
            RemetenteEmail = "dup.fp@sgx.local",
            Assunto = "Mesmo assunto",
            CorpoTexto = "Mesmo corpo",
            DataRecebimento = referencia
        });

        var duplicada = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "6",
            MessageId = null,
            RemetenteEmail = "dup.fp@sgx.local",
            Assunto = "Mesmo assunto",
            CorpoTexto = "Mesmo corpo",
            DataRecebimento = referencia
        });

        Assert.Equal(EmailMensagemProcessamentoStatus.Duplicado, duplicada.Status);
    }

    [Fact]
    public async Task ErroEmUmaMensagemEhRegistradoSemExceptionFatal()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);
        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), new FakeArquivoStorageService());

        var resultado = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "7",
            MessageId = "msg-erro-1",
            RemetenteEmail = "",
            Assunto = "Mensagem invalida",
            CorpoTexto = "Sem remetente",
            DataRecebimento = DateTime.UtcNow
        });

        var log = context.LogsIntegracaoEmail.Single(x => x.MessageId == "msg-erro-1");
        Assert.Equal(EmailMensagemProcessamentoStatus.Erro, resultado.Status);
        Assert.Equal(StatusProcessamentoEmail.Erro, log.StatusProcessamento);
    }

    [Fact]
    public async Task AnexoPermitidoEhSalvo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);
        var storage = new FakeArquivoStorageService();
        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), storage);

        var resultado = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "8",
            MessageId = "msg-anexo-ok-1",
            RemetenteEmail = "anexo.ok@sgx.local",
            Assunto = "Com anexo",
            CorpoTexto = "Segue arquivo",
            DataRecebimento = DateTime.UtcNow,
            Anexos =
            [
                new EmailAttachmentData("arquivo.pdf", "application/pdf", [1, 2, 3, 4], 4, null)
                
            ]
        });

        Assert.Equal(EmailMensagemProcessamentoStatus.Processado, resultado.Status);
        Assert.Single(storage.Salvos);
        Assert.Single(context.AnexosChamado);
    }

    [Fact]
    public async Task AnexoInvalidoEhIgnoradoSemInterromperProcessamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);
        var storage = new FakeArquivoStorageService();
        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), storage);

        var resultado = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "9",
            MessageId = "msg-anexo-invalido-1",
            RemetenteEmail = "anexo.invalido@sgx.local",
            Assunto = "Anexo invalido",
            CorpoTexto = "Corpo",
            DataRecebimento = DateTime.UtcNow,
            Anexos =
            [
                new EmailAttachmentData("arquivo.exe", "application/x-msdownload", [1, 2, 3], 3, null)
            ]
        });

        Assert.Equal(EmailMensagemProcessamentoStatus.Processado, resultado.Status);
        Assert.Empty(storage.Salvos);
        Assert.Empty(context.AnexosChamado);
        var log = context.LogsIntegracaoEmail.Single(x => x.MessageId == "msg-anexo-invalido-1");
        Assert.Contains("rejeitado", log.Erro ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task NomeAnexoComPathTraversalEhSanitizado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);
        var storage = new FakeArquivoStorageService();
        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), storage);

        var _ = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "10",
            MessageId = "msg-anexo-traversal-1",
            RemetenteEmail = "anexo.traversal@sgx.local",
            Assunto = "Anexo traversal",
            CorpoTexto = "Corpo",
            DataRecebimento = DateTime.UtcNow,
            Anexos =
            [
                new EmailAttachmentData("..\\..\\secret.pdf", "application/pdf", [1, 2, 3], 3, null)
            ]
        });

        var anexo = context.AnexosChamado.Single();
        Assert.DoesNotContain("..", anexo.NomeArquivo);
        Assert.DoesNotContain("\\", anexo.NomeArquivo);
        Assert.DoesNotContain("/", anexo.NomeArquivo);
    }

    private static EmailMessageProcessor CriarProcessor(
        SGXSistemaChamadoDbContext context,
        IEmailCorrelationService correlationService,
        FakeArquivoStorageService arquivoStorageService,
        IOptions<EmailWorkerOptions>? workerOptions = null)
    {
        return new EmailMessageProcessor(
            PortalUseCasesTestFactory.Repo<LogIntegracaoEmail>(context),
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<AnexoChamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<UsuarioPerfilAcesso>(context),
            PortalUseCasesTestFactory.Repo<PerfilAcesso>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<ParametroSistema>(context),
            correlationService,
            arquivoStorageService,
            new FakeCodigoChamadoService(),
            SlaTestFactory.CriarService(context),
            PortalUseCasesTestFactory.ArquivosOptionsPadrao,
            workerOptions ?? PortalUseCasesTestFactory.EmailWorkerOptionsPadrao,
            PortalUseCasesTestFactory.Uow(context),
            NullLogger<EmailMessageProcessor>.Instance);
    }

    private static IOptions<EmailWorkerOptions> CriarEmailWorkerOptions(
        Guid? categoriaPadraoId = null,
        Guid? prioridadePadraoId = null,
        Guid? departamentoPadraoId = null,
        string[]? dominiosPermitidos = null)
    {
        return Options.Create(new EmailWorkerOptions
        {
            ImapHost = "imap.sgx.local",
            ImapPorta = 993,
            Usuario = "worker@sgx.local",
            Senha = "segredo",
            Pasta = "INBOX",
            IntervaloSegundos = 60,
            MaxMensagensPorCiclo = 20,
            CategoriaPadraoId = categoriaPadraoId,
            PrioridadePadraoId = prioridadePadraoId,
            DepartamentoPadraoId = departamentoPadraoId,
            DominiosPermitidos = dominiosPermitidos ?? []
        });
    }

    private static async Task SeedCategoriaPadraoAsync(SGXSistemaChamadoDbContext context)
    {
        if (await context.CategoriasChamado.AnyAsync(x => x.Nome == "Suporte Tecnico"))
        {
            return;
        }

        var departamento = new Departamento("TI Email", "TIE", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        context.CategoriasChamado.Add(new CategoriaChamado("Suporte Tecnico", null, departamento.Id, "teste"));
        await context.SaveChangesAsync();
    }

    private static async Task<Chamado> SeedChamadoAsync(SGXSistemaChamadoDbContext context, string codigo)
    {
        await SeedCategoriaPadraoAsync(context);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Processor", $"{Guid.NewGuid():N}@sgx.local", TipoPerfil.Solicitante);
        var categoria = context.CategoriasChamado.First(x => x.Nome == "Suporte Tecnico");
        var prioridade = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Media);
        var statusAberto = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);

        var chamado = new Chamado(codigo, "Chamado existente", "Descricao", solicitante.Id, categoria.Id, prioridade.Id, statusAberto.Id, OrigemChamado.Email, "teste", categoria.DepartamentoId);
        context.Chamados.Add(chamado);
        await context.SaveChangesAsync();
        return chamado;
    }

    private sealed class FakeEmailCorrelationService(
        Chamado? chamado,
        bool possuiIndicadorResposta = false,
        string? codigoDetectado = null) : IEmailCorrelationService
    {
        public Task<Chamado?> TryFindChamadoAsync(EmailMessageData emailMessage, CancellationToken cancellationToken = default)
            => Task.FromResult(chamado);

        public Task<EmailCorrelationResult> CorrelacionarAsync(EmailMessageData emailMessage, CancellationToken cancellationToken = default)
            => Task.FromResult(new EmailCorrelationResult(chamado, chamado is not null || possuiIndicadorResposta, codigoDetectado, []));
    }
}
