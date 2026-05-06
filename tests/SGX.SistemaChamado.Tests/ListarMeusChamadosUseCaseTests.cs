using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ListarMeusChamadosUseCaseTests
{
    [Fact]
    public async Task DeveListarApenasChamadosDoUsuarioAutenticado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedChamados(context);

        var useCase = new ListarMeusChamadosUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.Usuario1Contexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosPortalRequest());

        Assert.Single(response.Items);
        Assert.All(response.Items, item => Assert.Contains("U1", item.Titulo));
    }

    [Fact]
    public async Task NaoDeveListarChamadosDeOutroSolicitante()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedChamados(context);

        var useCase = new ListarMeusChamadosUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.Usuario2Contexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosPortalRequest());

        Assert.Single(response.Items);
        Assert.Contains("U2", response.Items.First().Titulo);
    }

    private static async Task<(UsuarioContextoAplicacao Usuario1Contexto, UsuarioContextoAplicacao Usuario2Contexto)> SeedChamados(SGXSistemaChamadoDbContext context)
    {
        var prioridade = context.PrioridadesChamado.First();
        var status = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);
        var categoria = new CategoriaChamado("Categoria", null, null, "teste");

        var usuario1 = new Usuario("Usuario U1", "u1@empresa.com", "u1", "teste");
        var usuario2 = new Usuario("Usuario U2", "u2@empresa.com", "u2", "teste");

        context.CategoriasChamado.Add(categoria);
        context.Usuarios.AddRange(usuario1, usuario2);
        await context.SaveChangesAsync();

        context.Chamados.Add(new Chamado("CH-U1", "Chamado U1", "Descricao U1", usuario1.Id, categoria.Id, prioridade.Id, status.Id, OrigemChamado.Portal, "teste"));
        context.Chamados.Add(new Chamado("CH-U2", "Chamado U2", "Descricao U2", usuario2.Id, categoria.Id, prioridade.Id, status.Id, OrigemChamado.Portal, "teste"));
        await context.SaveChangesAsync();

        return (
            new UsuarioContextoAplicacao(usuario1.Id, usuario1.Nome, usuario1.Email, usuario1.Login, ["Solicitante"]),
            new UsuarioContextoAplicacao(usuario2.Id, usuario2.Nome, usuario2.Email, usuario2.Login, ["Solicitante"]));
    }
}
