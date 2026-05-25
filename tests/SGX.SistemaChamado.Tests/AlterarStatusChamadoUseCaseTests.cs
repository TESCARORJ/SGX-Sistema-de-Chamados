using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class AlterarStatusChamadoUseCaseTests
{
    [Fact]
    public async Task AlteraStatus()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new AlterarStatusChamadoRequest { StatusId = dados.StatusEmAtendimentoId });

        Assert.Equal("Em Atendimento", response.Status);
    }

    [Fact]
    public async Task CriaHistorico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        await useCase.ExecutarAsync(dados.Chamado.Id, new AlterarStatusChamadoRequest { StatusId = dados.StatusEmAtendimentoId });

        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.StatusAlterado);
    }

    [Fact]
    public async Task RejeitaStatusInexistenteInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new AlterarStatusChamadoRequest { StatusId = Guid.NewGuid() }));
    }

    [Fact]
    public async Task BloqueiaAvancoQuandoChamadoTemAprovacaoPendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var aprovacao = new AprovacaoChamado(
            dados.Chamado.Id,
            TipoOrigemAprovacaoChamado.CatalogoServico,
            dados.Admin.Id,
            dados.Admin.Login,
            dados.Chamado.SolicitanteId,
            "Servico catalogo",
            "Aguarda aprovacao");
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new AlterarStatusChamadoRequest { StatusId = dados.StatusEmAtendimentoId }));

        Assert.Contains("aguarda aprovacao", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AvancoFuncionaAposAprovacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var aprovacao = new AprovacaoChamado(
            dados.Chamado.Id,
            TipoOrigemAprovacaoChamado.CatalogoServico,
            dados.Admin.Id,
            dados.Admin.Login,
            dados.Chamado.SolicitanteId,
            "Servico catalogo",
            "Aguarda aprovacao");
        aprovacao.Aprovar(dados.Admin.Id, dados.Admin.Id, dados.Admin.Login, "Aprovado");
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new AlterarStatusChamadoRequest { StatusId = dados.StatusEmAtendimentoId });

        Assert.Equal("Em Atendimento", response.Status);
    }

    [Fact]
    public async Task AvancoPermaneceBloqueadoAposReprovacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var aprovacao = new AprovacaoChamado(
            dados.Chamado.Id,
            TipoOrigemAprovacaoChamado.CatalogoServico,
            dados.Admin.Id,
            dados.Admin.Login,
            dados.Chamado.SolicitanteId,
            "Servico catalogo",
            "Aguarda aprovacao");
        aprovacao.Reprovar(dados.Admin.Id, dados.Admin.Id, dados.Admin.Login, "Reprovado");
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new AlterarStatusChamadoRequest { StatusId = dados.StatusEmAtendimentoId }));

        Assert.Contains("reprovado", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AvancoFuncionaAposCancelamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var aprovacao = new AprovacaoChamado(
            dados.Chamado.Id,
            TipoOrigemAprovacaoChamado.CatalogoServico,
            dados.Admin.Id,
            dados.Admin.Login,
            dados.Chamado.SolicitanteId,
            "Servico catalogo",
            "Aguarda aprovacao");
        aprovacao.Cancelar(dados.Admin.Id, dados.Admin.Login, "Cancelado");
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new AlterarStatusChamadoRequest { StatusId = dados.StatusEmAtendimentoId });

        Assert.Equal("Em Atendimento", response.Status);
    }

    private static AlterarStatusChamadoUseCase CriarUseCase(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Chamado Chamado, Guid StatusEmAtendimentoId, Usuario Admin, UsuarioContextoAplicacao AdminContexto)> SeedAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, null, "STA1");
        var status = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.EmAtendimento);

        return (chamado, status.Id, admin, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}

