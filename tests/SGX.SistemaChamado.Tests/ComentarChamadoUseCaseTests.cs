using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ComentarChamadoUseCaseTests
{
    [Fact]
    public async Task DeveAdicionarComentarioAoProprioChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedDados(context);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);
        var response = await useCase.ExecutarAsync(dados.ChamadoProprio.Id, new ComentarioChamadoRequest { Mensagem = "Comentario teste" });

        Assert.Equal("Comentario teste", response.Mensagem);
        Assert.Single(context.ComentariosChamado);
    }

    [Fact]
    public async Task DeveCriarHistorico()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedDados(context);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);
        await useCase.ExecutarAsync(dados.ChamadoProprio.Id, new ComentarioChamadoRequest { Mensagem = "Comentario" });

        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.ComentarioAdicionado);
    }

    [Fact]
    public async Task DeveBloquearComentarioEmChamadoDeOutroUsuario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedDados(context);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(
            dados.ChamadoOutro.Id,
            new ComentarioChamadoRequest { Mensagem = "Nao pode" }));
    }

    private static ComentarChamadoUseCase CriarUseCase(SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao usuarioContexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(usuarioContexto),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Chamado ChamadoProprio, Chamado ChamadoOutro, UsuarioContextoAplicacao UsuarioContexto)> SeedDados(SGXSistemaChamadoDbContext context)
    {
        var prioridade = context.PrioridadesChamado.First();
        var status = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);
        var categoria = new CategoriaChamado("Categoria", null, null, "teste");

        var usuario = new Usuario("Usuario 1", "u1@empresa.com", "u1", "teste");
        var outro = new Usuario("Usuario 2", "u2@empresa.com", "u2", "teste");
        context.CategoriasChamado.Add(categoria);
        context.Usuarios.AddRange(usuario, outro);
        await context.SaveChangesAsync();

        var chamadoProprio = new Chamado("CH-C1", "Chamado C1", "Descricao", usuario.Id, categoria.Id, prioridade.Id, status.Id, OrigemChamado.Portal, "teste");
        var chamadoOutro = new Chamado("CH-C2", "Chamado C2", "Descricao", outro.Id, categoria.Id, prioridade.Id, status.Id, OrigemChamado.Portal, "teste");
        context.Chamados.AddRange(chamadoProprio, chamadoOutro);
        await context.SaveChangesAsync();

        return (
            chamadoProprio,
            chamadoOutro,
            new UsuarioContextoAplicacao(usuario.Id, usuario.Nome, usuario.Email, usuario.Login, ["Solicitante"]));
    }
}
