using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services;
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

    [Fact]
    public async Task BloqueiaStatusIncompativelComNaturezaEventoAlerta()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, NaturezaChamadoEnum.EventoAlerta, "STA-EA");
        var statusAguardandoSolicitanteId = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.AguardandoSolicitante).Id;

        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new AlterarStatusChamadoRequest { StatusId = statusAguardandoSolicitanteId }));

        Assert.Contains("nao e permitido", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PermiteAlterarParaStatusNovoCompativelComNaturezaMudanca()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, NaturezaChamadoEnum.Mudanca, "STA-MUD");
        var statusEmAnaliseId = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.EmAnalise).Id;
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new AlterarStatusChamadoRequest { StatusId = statusEmAnaliseId });

        Assert.Equal("Em Analise", response.Status);
    }

    [Fact]
    public async Task BloqueiaStatusNovoIncompativelComNaturezaIncidente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, NaturezaChamadoEnum.Incidente, "STA-INC");
        var statusPlanejadaId = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Planejada).Id;
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new AlterarStatusChamadoRequest { StatusId = statusPlanejadaId }));

        Assert.Contains("nao e permitido", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static AlterarStatusChamadoUseCase CriarUseCase(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Chamado Chamado, Guid StatusEmAtendimentoId, Usuario Admin, UsuarioContextoAplicacao AdminContexto)> SeedAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        NaturezaChamadoEnum naturezaChamado = NaturezaChamadoEnum.Requisicao,
        string sufixoCodigo = "STA1")
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            solicitante,
            categoria,
            StatusChamadoEnum.Aberto,
            null,
            sufixoCodigo,
            naturezaChamado: naturezaChamado);
        var status = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.EmAtendimento);

        return (chamado, status.Id, admin, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}

