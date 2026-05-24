using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class InventarioAtivosChamadosUseCasesTests
{
    [Fact]
    public async Task VincularAtivoAChamadoExistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var vincularUseCase = CriarVincularUseCase(context, seed.Admin);

        var response = await vincularUseCase.ExecutarAsync(seed.ChamadoSemAtivo.Id, seed.AtivoAtivo.Id);

        Assert.Equal(seed.AtivoAtivo.Id, response.InventarioAtivoId);
        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == seed.ChamadoSemAtivo.Id && x.Tipo == TipoHistoricoChamado.AtivoVinculado);
        Assert.Contains(context.HistoricosInventarioAtivo, x => x.InventarioAtivoId == seed.AtivoAtivo.Id && x.TipoMovimentacao == TipoMovimentacaoAtivo.VinculoChamado);
    }

    [Fact]
    public async Task ImpedirVinculoComAtivoInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var vincularUseCase = CriarVincularUseCase(context, seed.Admin);

        await Assert.ThrowsAsync<InvalidOperationException>(() => vincularUseCase.ExecutarAsync(seed.ChamadoSemAtivo.Id, seed.AtivoInativo.Id));
    }

    [Fact]
    public async Task RemoverVinculoDeAtivoDoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var vincularUseCase = CriarVincularUseCase(context, seed.Admin);
        var removerUseCase = CriarRemoverUseCase(context, seed.Admin);

        await vincularUseCase.ExecutarAsync(seed.ChamadoSemAtivo.Id, seed.AtivoAtivo.Id);
        var response = await removerUseCase.ExecutarAsync(seed.ChamadoSemAtivo.Id);

        Assert.Null(response.InventarioAtivoId);
        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == seed.ChamadoSemAtivo.Id && x.Tipo == TipoHistoricoChamado.AtivoRemovido);
        Assert.Contains(context.HistoricosInventarioAtivo, x => x.InventarioAtivoId == seed.AtivoAtivo.Id && x.TipoMovimentacao == TipoMovimentacaoAtivo.RemocaoVinculoChamado);
    }

    [Fact]
    public async Task ListarChamadosPorAtivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var vincularUseCase = CriarVincularUseCase(context, seed.Admin);
        var inventarioUseCase = CriarInventarioUseCase(context, seed.Admin);

        await vincularUseCase.ExecutarAsync(seed.ChamadoSemAtivo.Id, seed.AtivoAtivo.Id);

        var response = await inventarioUseCase.ListarChamadosAsync(seed.AtivoAtivo.Id, new FiltroChamadosRelacionadosInventarioAtivoRequest());

        Assert.Single(response.Items);
        Assert.Equal(seed.ChamadoSemAtivo.Id, response.Items.Single().ChamadoId);
    }

    [Fact]
    public async Task ListarChamadosDeAtivoInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var vincularUseCase = CriarVincularUseCase(context, seed.Admin);
        var inventarioUseCase = CriarInventarioUseCase(context, seed.Admin);

        await vincularUseCase.ExecutarAsync(seed.ChamadoSemAtivo.Id, seed.AtivoAtivo.Id);
        seed.AtivoAtivo.Inativar(seed.Admin.Id, seed.Admin.Login);
        await context.SaveChangesAsync();

        var response = await inventarioUseCase.ListarChamadosAsync(seed.AtivoAtivo.Id, new FiltroChamadosRelacionadosInventarioAtivoRequest());

        Assert.Single(response.Items);
        Assert.Equal(seed.ChamadoSemAtivo.Id, response.Items.Single().ChamadoId);
    }

    private static VincularInventarioAtivoChamadoUseCase CriarVincularUseCase(SGXSistemaChamadoDbContext context, Usuario admin)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<InventarioAtivo>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoInventarioAtivo>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

    private static RemoverInventarioAtivoChamadoUseCase CriarRemoverUseCase(SGXSistemaChamadoDbContext context, Usuario admin)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<InventarioAtivo>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoInventarioAtivo>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

    private static InventarioAtivosAdminUseCases CriarInventarioUseCase(SGXSistemaChamadoDbContext context, Usuario admin)
        => new(
            PortalUseCasesTestFactory.Repo<InventarioAtivo>(context),
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoInventarioAtivo>(context),
            PortalUseCasesTestFactory.Repo<TipoAtivoInventario>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<SeedContexto> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Admin Inventario Chamado",
            $"admin.inventario.chamado.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Administrador);

        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante Inventario",
            $"solicitante.inventario.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Solicitante);

        var departamento = new Departamento("Tecnologia", "TI", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infraestrutura", departamento.Id);
        var chamadoSemAtivo = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            solicitante,
            categoria,
            StatusChamadoEnum.Aberto,
            sufixoCodigo: "INV-ATIVO");

        var tipoAtivo = new TipoAtivoInventario("Notebook", null, "teste");
        context.TiposAtivoInventario.Add(tipoAtivo);
        await context.SaveChangesAsync();

        var ativoAtivo = new InventarioAtivo("INV-001", "Notebook Operacional", tipoAtivo.Id, admin.Id, admin.Login);
        ativoAtivo.DefinirDepartamento(departamento.Id);
        ativoAtivo.DefinirStatusOperacional(StatusOperacionalAtivo.Operacional);
        ativoAtivo.DefinirStatusPatrimonial(StatusPatrimonialAtivo.EmUso);

        var ativoInativo = new InventarioAtivo("INV-002", "Notebook Inativo", tipoAtivo.Id, admin.Id, admin.Login);
        ativoInativo.DefinirDepartamento(departamento.Id);
        ativoInativo.DefinirStatusOperacional(StatusOperacionalAtivo.ComDefeito);
        ativoInativo.DefinirStatusPatrimonial(StatusPatrimonialAtivo.EmUso);
        ativoInativo.Inativar(admin.Id, admin.Login);

        context.InventarioAtivos.AddRange(ativoAtivo, ativoInativo);
        await context.SaveChangesAsync();

        return new SeedContexto(admin, chamadoSemAtivo, ativoAtivo, ativoInativo);
    }

    private sealed record SeedContexto(
        Usuario Admin,
        Chamado ChamadoSemAtivo,
        InventarioAtivo AtivoAtivo,
        InventarioAtivo AtivoInativo);
}
