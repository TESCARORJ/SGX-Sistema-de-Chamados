using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class ReabrirChamadoUseCaseTests
{
    [Fact]
    public async Task ReabreChamadoEncerrado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedEncerradoAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new ReabrirChamadoRequest { Mensagem = "Reabrindo" });

        Assert.Equal("Em Atendimento", response.Status);
        Assert.Null(context.Chamados.Single().EncerradoEm);
    }

    [Fact]
    public async Task CriaHistorico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedEncerradoAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        await useCase.ExecutarAsync(dados.Chamado.Id, new ReabrirChamadoRequest { Mensagem = "Reabrindo" });

        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.Reaberto);
    }

    [Fact]
    public async Task BloqueiaReabrirChamadoAberto()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAbertoAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new ReabrirChamadoRequest { Mensagem = "Teste" }));
    }

    private static ReabrirChamadoUseCase CriarUseCase(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Chamado Chamado, UsuarioContextoAplicacao AdminContexto)> SeedEncerradoAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Encerrado, null, "REA1");
        chamado.Encerrar(context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Encerrado).Id, "teste");
        context.Chamados.Update(chamado);
        await context.SaveChangesAsync();

        return (chamado, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }

    private static async Task<(Chamado Chamado, UsuarioContextoAplicacao AdminContexto)> SeedAbertoAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin2", "admin2@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante2", "sol2@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Sistemas");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, null, "REA2");

        return (chamado, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}

