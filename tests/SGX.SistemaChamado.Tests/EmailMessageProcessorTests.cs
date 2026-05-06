using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.Interfaces.Email;
using SGX.SistemaChamado.Application.Services.Email;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class EmailMessageProcessorTests
{
    [Fact]
    public async Task EmailNovoCriaChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);

        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), new FakeArquivoStorageService());
        var resultado = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "1",
            MessageId = "msg-novo-1",
            RemetenteEmail = "novo@sgx.local",
            RemetenteNome = "Novo",
            Assunto = "Falha no sistema",
            CorpoTexto = "Descricao do problema",
            DataRecebimento = DateTime.UtcNow
        });

        Assert.Equal(EmailMensagemProcessamentoStatus.Processado, resultado.Status);
        Assert.Single(context.Chamados.Where(x => x.Origem == OrigemChamado.Email));
    }

    [Fact]
    public async Task EmailCorrelacionadoCriaComentario()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var chamado = await SeedChamadoAsync(context, "SGX-2026-000500");

        var processor = CriarProcessor(context, new FakeEmailCorrelationService(chamado), new FakeArquivoStorageService());
        var resultado = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "2",
            MessageId = "msg-correlacionado-1",
            RemetenteEmail = "resposta@sgx.local",
            RemetenteNome = "Resposta",
            Assunto = "Re: SGX-2026-000500",
            CorpoTexto = "Complementando informacoes",
            DataRecebimento = DateTime.UtcNow
        });

        Assert.Equal(EmailMensagemProcessamentoStatus.Processado, resultado.Status);
        Assert.Contains(context.ComentariosChamado, x => x.ChamadoId == chamado.Id && !x.Interno);
    }

    [Fact]
    public async Task DuplicidadePorMessageIdEhBloqueada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedCategoriaPadraoAsync(context);
        var processor = CriarProcessor(context, new FakeEmailCorrelationService(null), new FakeArquivoStorageService());

        _ = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "3",
            MessageId = "msg-duplicada-id",
            RemetenteEmail = "dup.id@sgx.local",
            Assunto = "Assunto",
            CorpoTexto = "Mensagem",
            DataRecebimento = DateTime.UtcNow
        });

        var duplicada = await processor.ProcessarAsync(new EmailMessageData
        {
            Identificador = "4",
            MessageId = "msg-duplicada-id",
            RemetenteEmail = "dup.id@sgx.local",
            Assunto = "Assunto alterado",
            CorpoTexto = "Outra mensagem",
            DataRecebimento = DateTime.UtcNow
        });

        Assert.Equal(EmailMensagemProcessamentoStatus.IgnoradoDuplicado, duplicada.Status);
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

        Assert.Equal(EmailMensagemProcessamentoStatus.IgnoradoDuplicado, duplicada.Status);
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
                new EmailAttachmentData("arquivo.pdf", "application/pdf", [1, 2, 3, 4])
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
                new EmailAttachmentData("arquivo.exe", "application/x-msdownload", [1, 2, 3])
            ]
        });

        Assert.Equal(EmailMensagemProcessamentoStatus.Processado, resultado.Status);
        Assert.Empty(storage.Salvos);
        Assert.Empty(context.AnexosChamado);
    }

    private static EmailMessageProcessor CriarProcessor(
        SGXSistemaChamadoDbContext context,
        IEmailCorrelationService correlationService,
        FakeArquivoStorageService arquivoStorageService)
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
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<ParametroSistema>(context),
            correlationService,
            arquivoStorageService,
            new FakeCodigoChamadoService(),
            SlaTestFactory.CriarService(context),
            PortalUseCasesTestFactory.ArquivosOptionsPadrao,
            PortalUseCasesTestFactory.Uow(context),
            NullLogger<EmailMessageProcessor>.Instance);
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

    private sealed class FakeEmailCorrelationService(Chamado? chamado) : IEmailCorrelationService
    {
        public Task<Chamado?> TryFindChamadoAsync(EmailMessageData emailMessage, CancellationToken cancellationToken = default)
            => Task.FromResult(chamado);
    }
}
