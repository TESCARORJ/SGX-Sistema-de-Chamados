using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Application.UseCases.Chamados;

namespace SGX.SistemaChamado.Tests;

public sealed class CancelarChamadoUseCaseTests
{
    [Fact]
    public async Task DeveCancelarChamadoComMotivoValido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new CancelarChamadoRequest { Motivo = "Cliente desistiu" });

        Assert.Equal("Cancelado", response.Status);
        Assert.NotNull(context.Chamados.Single().EncerradoEm);
    }

    [Fact]
    public async Task NaoDeveCancelarChamadoComMotivoVazio()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new CancelarChamadoRequest { Motivo = "" }));
    }

    [Fact]
    public async Task NaoDeveCancelarChamadoComMotivoNulo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new CancelarChamadoRequest { Motivo = null! }));
    }

    [Fact]
    public async Task NaoDeveCancelarChamadoComMotivoSomenteEspacos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new CancelarChamadoRequest { Motivo = "   " }));
    }

    [Fact]
    public async Task NaoDeveAlterarStatusQuandoMotivoCancelamentoInvalido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var statusOriginal = dados.Chamado.Status.Codigo;

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new CancelarChamadoRequest { Motivo = "" }));
        
        var chamadoDb = context.Chamados.Single();
        Assert.Equal(statusOriginal, chamadoDb.Status.Codigo);
    }

    [Fact]
    public async Task NaoDeveRegistrarHistoricoQuandoMotivoCancelamentoInvalido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var totalHistoricoAntes = context.HistoricosChamado.Count();

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new CancelarChamadoRequest { Motivo = "" }));
        
        var totalHistoricoDepois = context.HistoricosChamado.Count();
        Assert.Equal(totalHistoricoAntes, totalHistoricoDepois);
    }

    [Fact]
    public async Task NaoDeveRegistrarAuditoriaQuandoMotivoCancelamentoInvalido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var totalAuditoriaAntes = context.EventosAuditoria.Count();

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new CancelarChamadoRequest { Motivo = "" }));
        
        var totalAuditoriaDepois = context.EventosAuditoria.Count();
        Assert.Equal(totalAuditoriaAntes, totalAuditoriaDepois);
    }

    [Fact]
    public async Task NaoDevePreencherResolvidoEmAoCancelarChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        await useCase.ExecutarAsync(dados.Chamado.Id, new CancelarChamadoRequest { Motivo = "Motivo válido" });
        
        var chamadoDb = context.Chamados.Single();
        Assert.Null(chamadoDb.ResolvidoEm);
        Assert.NotNull(chamadoDb.EncerradoEm);
    }

    [Fact]
    public async Task NaoDeveCancelarChamadoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(Guid.NewGuid(), new CancelarChamadoRequest { Motivo = "Cancelado" }));
    }

    [Fact]
    public async Task NaoDeveCancelarChamadoComAprovacaoPendenteBloqueante()
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
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new CancelarChamadoRequest { Motivo = "Tentativa de cancelar aprovacao bloqueante" }));

        Assert.Equal("Este chamado possui aprovacao pendente e nao pode avancar enquanto a aprovacao bloqueante nao for decidida.", ex.Message);
        Assert.Null(context.Chamados.Single(x => x.Id == dados.Chamado.Id).EncerradoEm);
        Assert.DoesNotContain(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.Cancelado);
    }

    [Fact]
    public async Task DeveRegistrarHistoricoComMotivoAoCancelarChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        await useCase.ExecutarAsync(dados.Chamado.Id, new CancelarChamadoRequest { Motivo = "Cancelado pelo cliente" });

        var historico = context.HistoricosChamado.SingleOrDefault(x => x.Tipo == TipoHistoricoChamado.Cancelado);
        Assert.NotNull(historico);
        Assert.Contains("Cancelado pelo cliente", historico.Descricao);

        var comentario = context.ComentariosChamado.FirstOrDefault(x => x.Mensagem.Contains("Cancelado pelo cliente"));
        Assert.NotNull(comentario);
    }

    private static CancelarChamadoUseCase CriarUseCase(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao contexto)
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
            null,
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
