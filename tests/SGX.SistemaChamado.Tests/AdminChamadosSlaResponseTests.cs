using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AdminChamadosSlaResponseTests
{
    [Fact]
    public async Task FilaAdministrativaRetornaSlaVencidoEProximoVencimento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (contexto, _) = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { Pagina = 1, TamanhoPagina = 20 });

        Assert.Contains(response.Items, x => x.SlaVencido);
        Assert.Contains(response.Items, x => x.SlaProximoVencimento);
    }

    [Fact]
    public async Task DetalheAdministrativoRetornaDadosDeSla()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (contexto, chamadoId) = await SeedAsync(context);

        var useCase = new DetalharChamadoAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            new FakeUsuarioContextoAplicacaoService(contexto));

        var response = await useCase.ExecutarAsync(chamadoId);

        Assert.NotNull(response.Sla);
        Assert.NotEqual(default, response.Sla!.PrazoResolucaoEm);
    }

    private static async Task<(UsuarioContextoAplicacao ContextoAdmin, Guid ChamadoId)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin SLA", "admin.sla@sgx.local", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante SLA", "sol.sla@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Categoria SLA");
        var prioridade = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Alta);
        var statusAberto = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);

        var chamadoVencido = new Chamado("CH-ASLA-1", "Vencido", "Descricao", solicitante.Id, categoria.Id, prioridade.Id, statusAberto.Id, OrigemChamado.Portal, "teste");
        var chamadoProximo = new Chamado("CH-ASLA-2", "Proximo", "Descricao", solicitante.Id, categoria.Id, prioridade.Id, statusAberto.Id, OrigemChamado.Portal, "teste");
        context.Chamados.AddRange(chamadoVencido, chamadoProximo);
        await context.SaveChangesAsync();

        var slaVencido = new ChamadoSla(
            chamadoVencido.Id,
            null,
            prioridade.Id,
            DateTime.UtcNow.AddHours(-8),
            DateTime.UtcNow.AddHours(-6),
            DateTime.UtcNow.AddHours(-1),
            true,
            false,
            null,
            "teste");
        slaVencido.RegistrarResolucao(DateTime.UtcNow, "teste");
        var slaProximo = new ChamadoSla(
            chamadoProximo.Id,
            null,
            prioridade.Id,
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddMinutes(30),
            DateTime.UtcNow.AddMinutes(45),
            true,
            false,
            null,
            "teste");
        context.ChamadosSla.AddRange(slaVencido, slaProximo);
        await context.SaveChangesAsync();

        return (AdminUseCasesTestFactory.Contexto(admin, "Administrador"), chamadoProximo.Id);
    }
}
