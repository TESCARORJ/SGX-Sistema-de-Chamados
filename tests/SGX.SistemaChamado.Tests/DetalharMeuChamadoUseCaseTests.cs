using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class DetalharMeuChamadoUseCaseTests
{
    [Fact]
    public async Task SolicitanteVeProprioChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedChamados(context);

        var useCase = new DetalharMeuChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.SolicitanteContexto));

        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Equal(dados.ChamadoSolicitante.Id, response.Id);
    }

    [Fact]
    public async Task SolicitanteNaoVeChamadoDeOutroUsuario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedChamados(context);

        var useCase = new DetalharMeuChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.SolicitanteContexto));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(dados.ChamadoOutro.Id));
    }

    [Fact]
    public async Task AdministradorPodeVisualizarConformeRegra()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedChamados(context);

        var useCase = new DetalharMeuChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AdminContexto));

        var response = await useCase.ExecutarAsync(dados.ChamadoOutro.Id);

        Assert.Equal(dados.ChamadoOutro.Id, response.Id);
    }

    private static async Task<(Chamado ChamadoSolicitante, Chamado ChamadoOutro, UsuarioContextoAplicacao SolicitanteContexto, UsuarioContextoAplicacao AdminContexto)> SeedChamados(SGXSistemaChamadoDbContext context)
    {
        var prioridade = context.PrioridadesChamado.First();
        var status = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);
        var categoria = new CategoriaChamado("Categoria", null, null, "teste");

        var solicitante = new Usuario("Usuario Solicitante", "solicitante@empresa.com", "solicitante", "teste");
        var outro = new Usuario("Outro Usuario", "outro@empresa.com", "outro", "teste");

        context.CategoriasChamado.Add(categoria);
        context.Usuarios.AddRange(solicitante, outro);
        await context.SaveChangesAsync();

        var chamadoSolicitante = new Chamado("CH-PROP", "Chamado Proprio", "Descricao", solicitante.Id, categoria.Id, prioridade.Id, status.Id, OrigemChamado.Portal, "teste");
        var chamadoOutro = new Chamado("CH-OUTRO", "Chamado Outro", "Descricao", outro.Id, categoria.Id, prioridade.Id, status.Id, OrigemChamado.Portal, "teste");
        context.Chamados.AddRange(chamadoSolicitante, chamadoOutro);
        await context.SaveChangesAsync();

        return (
            chamadoSolicitante,
            chamadoOutro,
            new UsuarioContextoAplicacao(solicitante.Id, solicitante.Nome, solicitante.Email, solicitante.Login, ["Solicitante"]),
            new UsuarioContextoAplicacao(Guid.NewGuid(), "Admin", "admin@empresa.com", "admin", ["Administrador"]));
    }
}
