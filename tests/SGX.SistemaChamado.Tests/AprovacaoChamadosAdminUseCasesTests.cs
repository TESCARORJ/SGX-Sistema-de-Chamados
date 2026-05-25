using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AprovacaoChamadosAdminUseCasesTests
{
    [Fact]
    public async Task SolicitarAprovacaoValida()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var auditoria = new FakeAuditoriaService();

        var useCase = CriarUseCase(context, dados.ContextoAdmin, auditoria);
        var response = await useCase.SolicitarAsync(dados.Chamado.Id, new SolicitarAprovacaoChamadoRequest
        {
            TipoOrigem = TipoOrigemAprovacaoChamado.Manual,
            OrigemDescricao = "Aprovacao manual administrativa",
            JustificativaSolicitacao = "Necessario validar antes da execucao"
        });

        Assert.Equal(StatusAprovacaoChamado.Pendente, response.Status);
        Assert.Equal(dados.Chamado.Id, response.ChamadoId);
        Assert.Single(context.AprovacoesChamado);
        Assert.Contains(auditoria.Eventos, x => x.Modulo == "Aprovacao de Chamados" && x.Entidade == "AprovacaoChamado");
    }

    [Fact]
    public async Task ImpedeAprovacaoPendenteDuplicadaParaMesmoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        _ = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.SolicitarAsync(dados.Chamado.Id, new SolicitarAprovacaoChamadoRequest
        {
            TipoOrigem = TipoOrigemAprovacaoChamado.Manual
        }));
    }

    [Fact]
    public async Task AprovaAprovacaoPendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.AprovarAsync(aprovacao.Id, new DecidirAprovacaoChamadoRequest
        {
            JustificativaDecisao = "Aprovado para continuidade"
        });

        Assert.Equal(StatusAprovacaoChamado.Aprovado, response.Status);
        Assert.NotNull(response.DecididaEm);
    }

    [Fact]
    public async Task ImpedeAprovarAprovacaoJaAprovada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        _ = await useCase.AprovarAsync(aprovacao.Id, new DecidirAprovacaoChamadoRequest { JustificativaDecisao = "Aprovado" });

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.AprovarAsync(aprovacao.Id, new DecidirAprovacaoChamadoRequest
        {
            JustificativaDecisao = "Tentativa duplicada"
        }));
    }

    [Fact]
    public async Task ReprovaAprovacaoPendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.ReprovarAsync(aprovacao.Id, new DecidirAprovacaoChamadoRequest
        {
            JustificativaDecisao = "Reprovado por falta de documentacao"
        });

        Assert.Equal(StatusAprovacaoChamado.Reprovado, response.Status);
        Assert.Equal("Reprovado por falta de documentacao", response.JustificativaDecisao);
    }

    [Fact]
    public async Task ExigeJustificativaParaReprovacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ReprovarAsync(aprovacao.Id, new DecidirAprovacaoChamadoRequest
        {
            JustificativaDecisao = ""
        }));
    }

    [Fact]
    public async Task CancelaAprovacaoPendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.CancelarAsync(aprovacao.Id, new CancelarAprovacaoChamadoRequest
        {
            JustificativaDecisao = "Cancelada por mudanca de escopo"
        });

        Assert.Equal(StatusAprovacaoChamado.Cancelado, response.Status);
        Assert.NotNull(response.DecididaEm);
    }

    [Fact]
    public async Task ExigeJustificativaParaCancelamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.CancelarAsync(aprovacao.Id, new CancelarAprovacaoChamadoRequest
        {
            JustificativaDecisao = ""
        }));
    }

    [Fact]
    public async Task ImpedeCancelarAprovacaoJaDecidida()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        _ = await useCase.AprovarAsync(aprovacao.Id, new DecidirAprovacaoChamadoRequest { JustificativaDecisao = "Aprovado" });

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CancelarAsync(aprovacao.Id, new CancelarAprovacaoChamadoRequest
        {
            JustificativaDecisao = "Tentativa posterior"
        }));
    }

    [Fact]
    public async Task ListaAprovacoesPorStatus()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var pendente = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);
        var aprovada = await CriarAprovacaoPendenteAsync(context, dados.ChamadoSecundario, dados.AdminUsuario);
        aprovada.Aprovar(dados.AdminUsuario.Id, dados.AdminUsuario.Id, dados.AdminUsuario.Login, "Aprovada");
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.ListarAsync(new FiltroAprovacaoChamadoRequest
        {
            Status = StatusAprovacaoChamado.Aprovado
        });

        Assert.Single(response.Items);
        Assert.DoesNotContain(response.Items, x => x.Id == pendente.Id);
        Assert.Contains(response.Items, x => x.Id == aprovada.Id);
    }

    [Fact]
    public async Task ListaAprovacoesPorChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacaoPrimeiroChamado = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);
        _ = await CriarAprovacaoPendenteAsync(context, dados.ChamadoSecundario, dados.AdminUsuario, "Outra");
        aprovacaoPrimeiroChamado.Reprovar(dados.AdminUsuario.Id, dados.AdminUsuario.Id, dados.AdminUsuario.Login, "Reprovada");
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.ListarAsync(new FiltroAprovacaoChamadoRequest
        {
            ChamadoId = dados.Chamado.Id
        });

        Assert.Single(response.Items);
        Assert.Equal(dados.Chamado.Id, response.Items.Single().ChamadoId);
    }

    [Fact]
    public async Task ObterDetalheAprovacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.ObterPorIdAsync(aprovacao.Id);

        Assert.Equal(aprovacao.Id, response.Id);
        Assert.Equal(dados.Chamado.Codigo, response.NumeroProtocoloChamado);
        Assert.Equal(dados.Chamado.Titulo, response.TituloChamado);
    }

    [Fact]
    public async Task RegistraHistoricoAprovacaoSolicitada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        _ = await useCase.SolicitarAsync(dados.Chamado.Id, new SolicitarAprovacaoChamadoRequest
        {
            TipoOrigem = TipoOrigemAprovacaoChamado.Manual
        });

        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.AprovacaoSolicitada);
    }

    [Fact]
    public async Task RegistraHistoricoChamadoAprovado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        _ = await useCase.AprovarAsync(aprovacao.Id, new DecidirAprovacaoChamadoRequest { JustificativaDecisao = "Aprovado" });

        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.ChamadoAprovado);
    }

    [Fact]
    public async Task RegistraHistoricoChamadoReprovado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        _ = await useCase.ReprovarAsync(aprovacao.Id, new DecidirAprovacaoChamadoRequest { JustificativaDecisao = "Reprovado" });

        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.ChamadoReprovado);
    }

    [Fact]
    public async Task RegistraHistoricoAprovacaoCancelada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = await CriarAprovacaoPendenteAsync(context, dados.Chamado, dados.AdminUsuario);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        _ = await useCase.CancelarAsync(aprovacao.Id, new CancelarAprovacaoChamadoRequest { JustificativaDecisao = "Cancelado" });

        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.AprovacaoCancelada);
    }

    private static AprovacaoChamadosAdminUseCases CriarUseCase(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto,
        FakeAuditoriaService? auditoriaService = null)
        => new(
            PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context),
            auditoriaService);

    private static async Task<AprovacaoChamado> CriarAprovacaoPendenteAsync(
        SGXSistemaChamadoDbContext context,
        Chamado chamado,
        Usuario admin,
        string sufixoOrigem = "Manual")
    {
        var aprovacao = new AprovacaoChamado(
            chamado.Id,
            TipoOrigemAprovacaoChamado.Manual,
            admin.Id,
            admin.Login,
            chamado.SolicitanteId,
            $"Origem {sufixoOrigem}",
            "Solicitacao de aprovacao");

        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();
        return aprovacao;
    }

    private static async Task<(Chamado Chamado, Chamado ChamadoSecundario, Usuario AdminUsuario, UsuarioContextoAplicacao ContextoAdmin)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", $"admin.{Guid.NewGuid():N}@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", $"sol.{Guid.NewGuid():N}@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Operacoes");

        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "APR1");
        var chamadoSecundario = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "APR2");

        return (chamado, chamadoSecundario, admin, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}
