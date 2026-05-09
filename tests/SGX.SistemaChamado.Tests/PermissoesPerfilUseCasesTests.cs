using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class PermissoesPerfilUseCasesTests
{
    [Fact]
    public async Task AdministradorAtualizaPermissoesDoPerfil()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var perfilAtendente = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Atendente);

        var useCase = new AtualizarPermissoesPerfilUseCase(
            PortalUseCasesTestFactory.Repo<PerfilAcesso>(context),
            PortalUseCasesTestFactory.Repo<PermissaoSistema>(context),
            PortalUseCasesTestFactory.Repo<PerfilAcessoPermissao>(context),
            new FakeUsuarioContextoAplicacaoService(new Application.Interfaces.UsuarioContextoAplicacao(
                Guid.NewGuid(),
                "Admin",
                "admin@empresa.com",
                "admin",
                ["Administrador"])),
            PortalUseCasesTestFactory.Uow(context));

        var request = new AtualizarPermissoesPerfilRequest
        {
            CodigosPermissoes = ["Chamados.Visualizar", "Chamados.Assumir", "Dashboard.Visualizar"]
        };

        var response = await useCase.ExecutarAsync(perfilAtendente.Id, request);

        var vinculadas = response.PermissoesVinculadas.Select(x => x.Codigo).OrderBy(x => x).ToArray();
        Assert.Equal(["Chamados.Assumir", "Chamados.Visualizar", "Dashboard.Visualizar"], vinculadas);

        var perfilAtualizado = await context.PerfisAcesso
            .Include(x => x.PerfilPermissoes)
            .ThenInclude(x => x.PermissaoSistema)
            .FirstAsync(x => x.Id == perfilAtendente.Id);

        Assert.Equal(3, perfilAtualizado.PerfilPermissoes.Count);
        Assert.Equal(3, perfilAtualizado.PerfilPermissoes.Select(x => x.PermissaoSistemaId).Distinct().Count());
    }

    [Fact]
    public async Task AtendenteNaoPodeAtualizarPermissoesDoPerfil()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var perfilAtendente = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Atendente);

        var useCase = new AtualizarPermissoesPerfilUseCase(
            PortalUseCasesTestFactory.Repo<PerfilAcesso>(context),
            PortalUseCasesTestFactory.Repo<PermissaoSistema>(context),
            PortalUseCasesTestFactory.Repo<PerfilAcessoPermissao>(context),
            new FakeUsuarioContextoAplicacaoService(new Application.Interfaces.UsuarioContextoAplicacao(
                Guid.NewGuid(),
                "Atendente",
                "atendente@empresa.com",
                "atendente",
                ["Atendente"])),
            PortalUseCasesTestFactory.Uow(context));

        var request = new AtualizarPermissoesPerfilRequest
        {
            CodigosPermissoes = ["Chamados.Visualizar"]
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(perfilAtendente.Id, request));
    }

    [Fact]
    public async Task ObterPermissoesUsuarioAtualRetornaUniaoDePerfisAtivos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var perfilAdmin = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Administrador);
        var perfilAtendente = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Atendente);
        perfilAtendente.Desativar("teste");
        await context.SaveChangesAsync();

        var usuario = new Usuario("Usuario Permissoes", "permissoes@empresa.com", "permissoes", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilAdmin.Id, "teste"));
        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilAtendente.Id, "teste"));
        await context.SaveChangesAsync();

        var permissaoInativa = await context.PermissoesSistema.FirstAsync(x => x.Codigo == "Chamados.VisualizarTodos");
        permissaoInativa.Desativar("teste");
        await context.SaveChangesAsync();

        var useCase = new ObterPermissoesUsuarioAtualUseCase(
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            new FakeUsuarioContextoAplicacaoService(new Application.Interfaces.UsuarioContextoAplicacao(
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.Login,
                ["Administrador", "Atendente"])));

        var permissoes = await useCase.ExecutarAsync();

        Assert.Contains("Usuarios.Gerenciar", permissoes);
        Assert.DoesNotContain("Chamados.VisualizarTodos", permissoes);
        Assert.Equal(permissoes.Count, permissoes.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }
}
