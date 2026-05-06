using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class CodigoChamadoServiceTests
{
    [Fact]
    public async Task GeraCodigoNoFormatoSgxAnoSequencial()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var service = new CodigoChamadoService(PortalUseCasesTestFactory.Repo<Chamado>(context));

        var codigo = await service.GerarAsync();
        var anoAtual = DateTime.UtcNow.Year;

        Assert.StartsWith($"SGX-{anoAtual}-", codigo);
        Assert.Matches(@"^SGX-\d{4}-\d{6}$", codigo);
    }

    [Fact]
    public async Task MantemPaddingDeSeisDigitos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedChamadoAsync(context, $"SGX-{DateTime.UtcNow.Year}-000009");
        var service = new CodigoChamadoService(PortalUseCasesTestFactory.Repo<Chamado>(context));

        var codigo = await service.GerarAsync();

        Assert.EndsWith("-000010", codigo);
    }

    [Fact]
    public async Task NaoDuplicaCodigoEmSequenciaSimples()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        await SeedChamadoAsync(context, $"SGX-{DateTime.UtcNow.Year}-000001");
        var service = new CodigoChamadoService(PortalUseCasesTestFactory.Repo<Chamado>(context));

        var codigo1 = await service.GerarAsync();
        await SeedChamadoAsync(context, codigo1);
        var codigo2 = await service.GerarAsync();

        Assert.NotEqual(codigo1, codigo2);
    }

    private static async Task SeedChamadoAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, string codigo)
    {
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Codigo", $"{Guid.NewGuid():N}@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria Codigo {Guid.NewGuid():N}");
        var prioridade = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Media);
        var statusAberto = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);

        context.Chamados.Add(new Chamado(codigo, "Teste Codigo", "Descricao", solicitante.Id, categoria.Id, prioridade.Id, statusAberto.Id, OrigemChamado.Portal, "teste"));
        await context.SaveChangesAsync();
    }
}
