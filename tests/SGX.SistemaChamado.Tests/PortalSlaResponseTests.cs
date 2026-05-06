using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class PortalSlaResponseTests
{
    [Fact]
    public async Task PortalRetornaIndicacaoAmigavelDeSla()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarMeusChamadosUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.ContextoSolicitante));

        var response = await useCase.ExecutarAsync(new FiltroChamadosPortalRequest { Pagina = 1, TamanhoPagina = 20 });

        Assert.Single(response.Items);
        Assert.True(response.Items.First().SlaProximoVencimento || response.Items.First().SlaVencido);
    }

    [Fact]
    public async Task SolicitanteContinuaVendoApenasOsPropriosChamados()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarMeusChamadosUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.ContextoSolicitante));

        var response = await useCase.ExecutarAsync(new FiltroChamadosPortalRequest());

        Assert.Single(response.Items);
        Assert.Equal(dados.ChamadoSolicitanteId, response.Items.First().Id);
    }

    private static async Task<(UsuarioContextoAplicacao ContextoSolicitante, Guid ChamadoSolicitanteId)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Portal Solicitante", "portal.sol@sgx.local", TipoPerfil.Solicitante);
        var outro = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Outro Solicitante", "portal.outro@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Portal Categoria");
        var prioridade = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Alta);
        var statusAberto = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);

        var meuChamado = new Chamado("CH-PSLA-1", "Meu chamado", "Descricao", solicitante.Id, categoria.Id, prioridade.Id, statusAberto.Id, OrigemChamado.Portal, "teste");
        var outroChamado = new Chamado("CH-PSLA-2", "Outro chamado", "Descricao", outro.Id, categoria.Id, prioridade.Id, statusAberto.Id, OrigemChamado.Portal, "teste");
        context.Chamados.AddRange(meuChamado, outroChamado);
        await context.SaveChangesAsync();

        var meuSla = new SlaControle(meuChamado.Id, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(2), "teste");
        var outroSla = new SlaControle(outroChamado.Id, DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(10), "teste");
        context.SlaControles.AddRange(meuSla, outroSla);
        await context.SaveChangesAsync();

        return (new UsuarioContextoAplicacao(solicitante.Id, solicitante.Nome, solicitante.Email, solicitante.Login, ["Solicitante"]), meuChamado.Id);
    }
}
