using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SGX.SistemaChamado.Tests;

public sealed class UsuariosAdminUseCaseTests
{
    [Fact]
    public async Task CriaUsuarioValido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, departamento, perfilAtendente) = await SeedBaseAsync(context);

        var useCase = CriarUseCase(context, admin);
        var response = await useCase.ExecutarAsync(new CriarUsuarioAdminRequest
        {
            Nome = "Novo Usuario",
            Email = "novo.usuario@empresa.com",
            Login = "novo.usuario",
            DepartamentoId = departamento.Id,
            PerfilIds = [perfilAtendente.Id]
        });

        Assert.Equal("novo.usuario@empresa.com", response.Email);
        Assert.Equal("novo.usuario", response.Login);
        Assert.NotEmpty(response.Perfis);
    }

    [Fact]
    public async Task RejeitaEmailInvalido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, _, perfilAtendente) = await SeedBaseAsync(context);
        var useCase = CriarUseCase(context, admin);

        await Assert.ThrowsAnyAsync<Exception>(() => useCase.ExecutarAsync(new CriarUsuarioAdminRequest
        {
            Nome = "Usuario Invalido",
            Email = "email-invalido",
            PerfilIds = [perfilAtendente.Id]
        }));
    }

    [Fact]
    public async Task RejeitaEmailDuplicado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, departamento, perfilAtendente) = await SeedBaseAsync(context);
        var useCase = CriarUseCase(context, admin);

        await useCase.ExecutarAsync(new CriarUsuarioAdminRequest
        {
            Nome = "Duplicado 1",
            Email = "dup@empresa.com",
            DepartamentoId = departamento.Id,
            PerfilIds = [perfilAtendente.Id]
        });

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarUsuarioAdminRequest
        {
            Nome = "Duplicado 2",
            Email = "dup@empresa.com",
            DepartamentoId = departamento.Id,
            PerfilIds = [perfilAtendente.Id]
        }));
    }

    [Fact]
    public async Task AssociaPerfis()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, _, perfilAtendente) = await SeedBaseAsync(context);
        var perfilSolicitante = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Solicitante);
        var useCase = CriarUseCase(context, admin);

        var response = await useCase.ExecutarAsync(new CriarUsuarioAdminRequest
        {
            Nome = "Usuario MultiPerfil",
            Email = "multi@empresa.com",
            PerfilIds = [perfilAtendente.Id, perfilSolicitante.Id]
        });

        Assert.Equal(2, response.Perfis.Count);
    }

    [Fact]
    public async Task BloqueiaInativacaoDoUltimoAdministradorAtivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Unico", "admin.unico@empresa.com", TipoPerfil.Administrador);

        var useCase = new InativarUsuarioAdminUseCase(
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(admin.Id));
    }

    [Fact]
    public async Task ReativaUsuario()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin.rea@empresa.com", TipoPerfil.Administrador);
        var usuario = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Inativo", "inativo@empresa.com", TipoPerfil.Solicitante);
        usuario.Desativar("teste");
        usuario.AlterarSituacao(SituacaoUsuario.Inativo, "teste");
        await context.SaveChangesAsync();

        var useCase = new ReativarUsuarioAdminUseCase(
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(usuario.Id);
        Assert.True(response.Ativo);
    }

    private static CriarUsuarioAdminUseCase CriarUseCase(SGXSistemaChamadoDbContext context, Usuario admin)
        => new(
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<PerfilAcesso>(context),
            PortalUseCasesTestFactory.Repo<UsuarioPerfilAcesso>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Usuario admin, Departamento departamento, PerfilAcesso perfilAtendente)> SeedBaseAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Administrador", "admin@empresa.com", TipoPerfil.Administrador);
        var departamento = new Departamento("Tecnologia", "TEC", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();
        var perfilAtendente = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Atendente);
        return (admin, departamento, perfilAtendente);
    }
}
