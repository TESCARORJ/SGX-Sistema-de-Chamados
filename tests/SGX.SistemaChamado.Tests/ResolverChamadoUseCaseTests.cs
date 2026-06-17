using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using System.Text.Json.Nodes;

namespace SGX.SistemaChamado.Tests;

public sealed class ResolverChamadoUseCaseTests
{
    [Fact]
    public async Task DeveResolverChamadoComSolucaoTecnicaValida()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new ResolverChamadoRequest { Solucao = "Problema resolvido", ComentarioInterno = false });

        Assert.Equal("Resolvido", response.Status);
        Assert.NotNull(context.Chamados.Single().ResolvidoEm);
    }

    [Fact]
    public async Task NaoDeveResolverChamadoComSolucaoTecnicaVazia()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new ResolverChamadoRequest { Solucao = "", ComentarioInterno = false }));
    }

    [Fact]
    public async Task NaoDeveResolverChamadoComSolucaoTecnicaNula()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new ResolverChamadoRequest { Solucao = null!, ComentarioInterno = false }));
    }

    [Fact]
    public async Task NaoDeveResolverChamadoComSolucaoTecnicaSomenteEspacos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new ResolverChamadoRequest { Solucao = "   ", ComentarioInterno = false }));
    }

    [Fact]
    public async Task NaoDeveAlterarStatusQuandoSolucaoTecnicaInvalida()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var statusOriginal = dados.Chamado.Status.Codigo;

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new ResolverChamadoRequest { Solucao = "", ComentarioInterno = false }));
        
        var chamadoDb = context.Chamados.Single();
        Assert.Equal(statusOriginal, chamadoDb.Status.Codigo);
    }

    [Fact]
    public async Task NaoDevePreencherResolvidoEmQuandoSolucaoTecnicaInvalida()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new ResolverChamadoRequest { Solucao = "  ", ComentarioInterno = false }));
        
        var chamadoDb = context.Chamados.Single();
        Assert.Null(chamadoDb.ResolvidoEm);
    }

    [Fact]
    public async Task NaoDeveRegistrarHistoricoQuandoSolucaoTecnicaInvalida()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new ResolverChamadoRequest { Solucao = "", ComentarioInterno = false }));
        
        Assert.DoesNotContain(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.Resolvido);
    }

    [Fact]
    public async Task NaoDeveRegistrarAuditoriaQuandoSolucaoTecnicaInvalida()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var auditoria = new FakeAuditoriaService();

        var useCase = CriarUseCase(context, dados.AdminContexto, auditoria);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new ResolverChamadoRequest { Solucao = "", ComentarioInterno = false }));
        
        Assert.Empty(auditoria.Eventos);
    }

    [Fact]
    public async Task NaoDeveResolverChamadoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(Guid.NewGuid(), new ResolverChamadoRequest { Solucao = "Resolvido", ComentarioInterno = false }));
    }

    [Fact]
    public async Task NaoDeveResolverChamadoComAprovacaoPendenteBloqueante()
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
            "Aguarda aprovacao",
            bloqueiaAvancoAtendimento: true);
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new ResolverChamadoRequest { Solucao = "Tentativa", ComentarioInterno = false }));

        Assert.Equal("Este chamado possui aprovacao pendente e nao pode avancar enquanto a aprovacao bloqueante nao for decidida.", ex.Message);
        Assert.Null(context.Chamados.Single(x => x.Id == dados.Chamado.Id).ResolvidoEm);
        Assert.DoesNotContain(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.Resolvido);
    }

    [Fact]
    public async Task DeveManterFechamentoDefinitivoSeparadoDaResolucao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new ResolverChamadoRequest { Solucao = "Resolvido", ComentarioInterno = false });

        Assert.Equal("Resolvido", response.Status);
        var chamadoDb = context.Chamados.Single();
        Assert.NotNull(chamadoDb.ResolvidoEm);
        Assert.Null(chamadoDb.EncerradoEm); // Deve estar nulo, pois Resolvido != Encerrado
    }

    [Fact]
    public async Task DeveRegistrarHistoricoDeResolucao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        await useCase.ExecutarAsync(dados.Chamado.Id, new ResolverChamadoRequest { Solucao = "Resolvido", ComentarioInterno = false });

        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.Resolvido);
    }

    [Fact]
    public async Task DeveRegistrarAuditoriaDeResolucaoComStatusESolucaoTecnica()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var auditoria = new FakeAuditoriaService();

        var useCase = CriarUseCase(context, dados.AdminContexto, auditoria);
        await useCase.ExecutarAsync(dados.Chamado.Id, new ResolverChamadoRequest { Solucao = "Problema resolvido definitivamente", ComentarioInterno = true });

        var evento = Assert.Single(auditoria.Eventos);
        Assert.Equal("Chamado resolvido.", evento.Descricao);
        Assert.Equal(TipoAcaoAuditoria.ResolverChamado, evento.Acao);
        Assert.Equal(dados.AdminContexto.Id, evento.UsuarioId);
        Assert.Equal(dados.AdminContexto.Login, evento.UsuarioLogin);

        var dadosAntes = JsonNode.Parse(evento.DadosAntes!)!.AsObject();
        var dadosDepois = JsonNode.Parse(evento.DadosDepois!)!.AsObject();

        Assert.Equal(dados.Chamado.Id.ToString(), dadosAntes["ChamadoId"]!.ToString());
        Assert.Equal(dados.AdminContexto.Id.ToString(), dadosDepois["UsuarioExecutorId"]!.ToString());
        Assert.Equal("Em Atendimento", dadosAntes["StatusAnterior"]!.GetValue<string>());
        Assert.Equal("Resolvido", dadosDepois["StatusNovo"]!.GetValue<string>());
        Assert.Equal("Problema resolvido definitivamente", dadosDepois["SolucaoTecnica"]!.GetValue<string>());
        Assert.NotNull(dadosDepois["DataEventoUtc"]);
        Assert.NotNull(dadosDepois["ResolvidoEm"]);
    }

    private static ResolverChamadoUseCase CriarUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto,
        FakeAuditoriaService? auditoria = null)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            CriarRelacionamentosUseCase(context, contexto),
            CriarAprovacoesUseCase(context, contexto),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context),
            auditoria,
            new ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
                PortalUseCasesTestFactory.Repo<Chamado>(context),
                PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
                PortalUseCasesTestFactory.Repo<StatusChamado>(context)));

    private static RelacionamentosChamadoUseCases CriarRelacionamentosUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ChamadoRelacionamento>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static ChamadoAprovacoesUseCases CriarAprovacoesUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Chamado Chamado, Usuario Admin, UsuarioContextoAplicacao AdminContexto)> SeedAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "RES1");

        return (chamado, admin, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}
