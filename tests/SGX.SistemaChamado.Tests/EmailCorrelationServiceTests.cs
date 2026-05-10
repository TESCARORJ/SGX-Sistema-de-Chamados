using SGX.SistemaChamado.Application.Services.Email;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class EmailCorrelationServiceTests
{
    [Fact]
    public async Task IdentificaCodigoSgxNoAssunto()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var chamado = await SeedChamadoAsync(context, "SGX-2026-000001");
        var service = CriarService(context);

        var resultado = await service.CorrelacionarAsync(new Application.Interfaces.Email.EmailMessageData
        {
            Assunto = "Re: [SGX-2026-000001] Falha de acesso",
            RemetenteEmail = "solicitante@sgx.local"
        });

        Assert.NotNull(resultado.Chamado);
        Assert.Equal(chamado.Id, resultado.Chamado!.Id);
        Assert.Equal("SGX-2026-000001", resultado.CodigoDetectado);
    }

    [Fact]
    public async Task IdentificaCodigoComReEncFwdEChm()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var chamado = await SeedChamadoAsync(context, "CHM-2026-000002");
        var service = CriarService(context);

        var re = await service.CorrelacionarAsync(new Application.Interfaces.Email.EmailMessageData
        {
            Assunto = "Re: #CHM-2026-000002",
            RemetenteEmail = "solicitante@sgx.local"
        });

        var enc = await service.CorrelacionarAsync(new Application.Interfaces.Email.EmailMessageData
        {
            Assunto = "Enc: CHM-2026-000002",
            RemetenteEmail = "solicitante@sgx.local"
        });

        var fwd = await service.CorrelacionarAsync(new Application.Interfaces.Email.EmailMessageData
        {
            Assunto = "Fwd: chamado CHM-2026-000002",
            RemetenteEmail = "solicitante@sgx.local"
        });

        Assert.Equal(chamado.Id, re.Chamado!.Id);
        Assert.Equal(chamado.Id, enc.Chamado!.Id);
        Assert.Equal(chamado.Id, fwd.Chamado!.Id);
    }

    [Fact]
    public async Task CorrelacionaPorMessageIdInReplyTo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var chamado = await SeedChamadoAsync(context, "SGX-2026-000004");
        await SeedLogCorrelacaoAsync(context, chamado, "msg-original@sgx.local", DateTime.UtcNow.AddMinutes(-9));

        var service = CriarService(context);
        var resultado = await service.CorrelacionarAsync(new Application.Interfaces.Email.EmailMessageData
        {
            Assunto = "Sem codigo no assunto",
            RemetenteEmail = "solicitante@sgx.local",
            InReplyTo = "<msg-original@sgx.local>"
        });

        Assert.NotNull(resultado.Chamado);
        Assert.Equal(chamado.Id, resultado.Chamado!.Id);
    }

    [Fact]
    public async Task CorrelacionaPorReferencesMaisRecente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var chamadoAntigo = await SeedChamadoAsync(context, "SGX-2026-000010");
        var chamadoRecente = await SeedChamadoAsync(context, "SGX-2026-000011");

        await SeedLogCorrelacaoAsync(context, chamadoAntigo, "msg-ref@sgx.local", DateTime.UtcNow.AddMinutes(-30));
        await SeedLogCorrelacaoAsync(context, chamadoRecente, "msg-ref@sgx.local", DateTime.UtcNow.AddMinutes(-5));

        var service = CriarService(context);
        var resultado = await service.CorrelacionarAsync(new Application.Interfaces.Email.EmailMessageData
        {
            Assunto = "Sem codigo",
            RemetenteEmail = "solicitante@sgx.local",
            References = ["<msg-ref@sgx.local>"]
        });

        Assert.NotNull(resultado.Chamado);
        Assert.Equal(chamadoRecente.Id, resultado.Chamado!.Id);
    }

    [Fact]
    public async Task CodigoNoAssuntoTemPrioridadeSobreHeaders()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var chamadoAssunto = await SeedChamadoAsync(context, "SGX-2026-000100");
        var chamadoHeader = await SeedChamadoAsync(context, "SGX-2026-000101");

        await SeedLogCorrelacaoAsync(context, chamadoHeader, "msg-header@sgx.local", DateTime.UtcNow.AddMinutes(-2));

        var service = CriarService(context);
        var resultado = await service.CorrelacionarAsync(new Application.Interfaces.Email.EmailMessageData
        {
            Assunto = "Re: SGX-2026-000100",
            RemetenteEmail = "solicitante@sgx.local",
            InReplyTo = "<msg-header@sgx.local>"
        });

        Assert.NotNull(resultado.Chamado);
        Assert.Equal(chamadoAssunto.Id, resultado.Chamado!.Id);
    }

    [Fact]
    public async Task SemCorrelacaoMasComIndicadoresMarcaPossuiIndicadorResposta()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        _ = await SeedChamadoAsync(context, "SGX-2026-000003");
        var service = CriarService(context);

        var resultado = await service.CorrelacionarAsync(new Application.Interfaces.Email.EmailMessageData
        {
            Assunto = "Re: SGX-2026-999999",
            RemetenteEmail = "solicitante@sgx.local",
            InReplyTo = "<nao-existe@sgx.local>"
        });

        Assert.Null(resultado.Chamado);
        Assert.True(resultado.PossuiIndicadorResposta);
        Assert.Equal("SGX-2026-999999", resultado.CodigoDetectado);
    }

    [Fact]
    public async Task SemCorrelacaoESemIndicadoresRetornaSemIndicador()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        _ = await SeedChamadoAsync(context, "SGX-2026-000003");
        var service = CriarService(context);

        var resultado = await service.CorrelacionarAsync(new Application.Interfaces.Email.EmailMessageData
        {
            Assunto = "Assunto sem codigo",
            RemetenteEmail = "solicitante@sgx.local"
        });

        Assert.Null(resultado.Chamado);
        Assert.False(resultado.PossuiIndicadorResposta);
    }

    private static EmailCorrelationService CriarService(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
        => new(PortalUseCasesTestFactory.Repo<Chamado>(context), PortalUseCasesTestFactory.Repo<LogIntegracaoEmail>(context));

    private static async Task SeedLogCorrelacaoAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, Chamado chamado, string messageId, DateTime dataProcessamento)
    {
        var log = new LogIntegracaoEmail(
            messageId,
            null,
            null,
            $"fingerprint-{Guid.NewGuid():N}",
            "solicitante@sgx.local",
            null,
            "Solicitante",
            "Assunto inicial",
            dataProcessamento.AddMinutes(-1),
            "teste");

        log.RegistrarTentativa("teste");
        log.MarcarProcessado(chamado.Id, dataProcessamento, "teste");

        context.LogsIntegracaoEmail.Add(log);
        await context.SaveChangesAsync();
    }

    private static async Task<Chamado> SeedChamadoAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, string codigo)
    {
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Correlacao", $"{Guid.NewGuid():N}@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria {Guid.NewGuid():N}");
        var prioridade = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Media);
        var statusAberto = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);

        var chamado = new Chamado(codigo, "Chamado Correlacao", "Descricao", solicitante.Id, categoria.Id, prioridade.Id, statusAberto.Id, OrigemChamado.Email, "teste");
        context.Chamados.Add(chamado);
        await context.SaveChangesAsync();
        return chamado;
    }
}
