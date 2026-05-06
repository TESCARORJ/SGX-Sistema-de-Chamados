using SGX.SistemaChamado.Application.Interfaces.Email;
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

        var encontrado = await service.TryFindChamadoAsync(new EmailMessageData
        {
            Assunto = "Re: [SGX-2026-000001] Falha de acesso",
            RemetenteEmail = "solicitante@sgx.local"
        });

        Assert.NotNull(encontrado);
        Assert.Equal(chamado.Id, encontrado!.Id);
    }

    [Fact]
    public async Task IdentificaCodigoComReOuEnc()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var chamado = await SeedChamadoAsync(context, "SGX-2026-000002");
        var service = CriarService(context);

        var re = await service.TryFindChamadoAsync(new EmailMessageData
        {
            Assunto = "Re: SGX-2026-000002",
            RemetenteEmail = "solicitante@sgx.local"
        });

        var enc = await service.TryFindChamadoAsync(new EmailMessageData
        {
            Assunto = "ENC: chamado SGX-2026-000002",
            RemetenteEmail = "solicitante@sgx.local"
        });

        Assert.NotNull(re);
        Assert.NotNull(enc);
        Assert.Equal(chamado.Id, re!.Id);
        Assert.Equal(chamado.Id, enc!.Id);
    }

    [Fact]
    public async Task RetornaNullQuandoNaoHaCorrelacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        _ = await SeedChamadoAsync(context, "SGX-2026-000003");
        var service = CriarService(context);

        var encontrado = await service.TryFindChamadoAsync(new EmailMessageData
        {
            Assunto = "Assunto sem codigo",
            RemetenteEmail = "solicitante@sgx.local"
        });

        Assert.Null(encontrado);
    }

    [Fact]
    public async Task CorrelacionaPorMessageIdInReplyTo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var chamado = await SeedChamadoAsync(context, "SGX-2026-000004");

        var log = new LogIntegracaoEmail(
            "msg-original@sgx.local",
            "fingerprint-original",
            "solicitante@sgx.local",
            "Solicitante",
            "Assunto inicial",
            DateTime.UtcNow.AddMinutes(-10),
            "teste");
        log.RegistrarTentativa("teste");
        log.MarcarProcessado(chamado.Id, DateTime.UtcNow.AddMinutes(-9), "teste");

        context.LogsIntegracaoEmail.Add(log);
        await context.SaveChangesAsync();

        var service = CriarService(context);
        var encontrado = await service.TryFindChamadoAsync(new EmailMessageData
        {
            Assunto = "Sem codigo no assunto",
            RemetenteEmail = "solicitante@sgx.local",
            InReplyTo = "<msg-original@sgx.local>"
        });

        Assert.NotNull(encontrado);
        Assert.Equal(chamado.Id, encontrado!.Id);
    }

    private static EmailCorrelationService CriarService(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
        => new(PortalUseCasesTestFactory.Repo<Chamado>(context), PortalUseCasesTestFactory.Repo<LogIntegracaoEmail>(context));

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
