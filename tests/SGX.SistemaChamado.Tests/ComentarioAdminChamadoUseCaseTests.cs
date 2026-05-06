using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class ComentarioAdminChamadoUseCaseTests
{
    [Fact]
    public async Task AdicionaComentarioInterno()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new ComentarioAdminChamadoRequest { Mensagem = "Comentario interno", Interno = true });

        Assert.True(response.Interno);
        Assert.Contains(context.ComentariosChamado, x => x.Interno);
    }

    [Fact]
    public async Task AdicionaComentarioPublico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new ComentarioAdminChamadoRequest { Mensagem = "Comentario publico", Interno = false });

        Assert.False(response.Interno);
        Assert.Contains(context.ComentariosChamado, x => !x.Interno);
    }

    [Fact]
    public async Task ComentarioInternoNaoApareceNoPortalDoSolicitante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var comentarioUseCase = CriarUseCase(context, dados.AdminContexto);
        await comentarioUseCase.ExecutarAsync(dados.Chamado.Id, new ComentarioAdminChamadoRequest { Mensagem = "Somente interno", Interno = true });

        var detalharPortal = new DetalharMeuChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.SolicitanteContexto));

        var detalhe = await detalharPortal.ExecutarAsync(dados.Chamado.Id);

        Assert.Empty(detalhe.Comentarios);
    }

    private static ComentarChamadoAdminUseCase CriarUseCase(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Chamado Chamado, UsuarioContextoAplicacao AdminContexto, UsuarioContextoAplicacao SolicitanteContexto)> SeedAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "COM1");

        return (
            chamado,
            AdminUseCasesTestFactory.Contexto(admin, "Administrador"),
            AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"));
    }
}

