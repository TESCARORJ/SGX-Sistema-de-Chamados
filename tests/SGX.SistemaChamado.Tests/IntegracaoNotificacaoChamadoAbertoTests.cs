using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class IntegracaoNotificacaoChamadoAbertoTests
{
    [Fact]
    public async Task DevePersistirNotificacaoAoAbrirChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Abertura", "sol.abertura@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Categoria Abertura");
        var prioridade = await context.PrioridadesChamado.FirstAsync();

        await NotificacoesItsmTestFactory.CriarTemplatesPadraoChamadoAsync(context, solicitante.Id);

        var useCase = CriarUseCase(
            context,
            AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"));

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro de login",
            Descricao = "Usuario sem acesso ao portal.",
            CategoriaId = categoria.Id,
            PrioridadeId = prioridade.Id
        });

        var notificacoes = await context.Notificacoes
            .Where(x => x.ChamadoId == response.Id)
            .ToListAsync();

        Assert.Equal(2, notificacoes.Count);
        Assert.Contains(notificacoes, x => x.Canal == CanalNotificacao.Sistema);
        Assert.Contains(notificacoes, x => x.Canal == CanalNotificacao.Email);
    }

    [Fact]
    public async Task DevePreservarAberturaQuandoTemplateEstiverAusente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Sem Template", "sol.abertura.sem.template@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Categoria Sem Template");
        var prioridade = await context.PrioridadesChamado.FirstAsync();

        var useCase = CriarUseCase(
            context,
            AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"));

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Chamado sem template",
            Descricao = "A abertura deve persistir mesmo sem notificacao.",
            CategoriaId = categoria.Id,
            PrioridadeId = prioridade.Id
        });

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(1, await context.Chamados.CountAsync());
        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }

    private static AbrirChamadoUseCase CriarUseCase(
        Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao usuarioContexto)
    {
        return new AbrirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<CatalogoServico>(context),
            PortalUseCasesTestFactory.Repo<InventarioAtivo>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoInventarioAtivo>(context),
            SlaTestFactory.CriarService(context),
            new FakeCodigoChamadoService(),
            new PrioridadeChamadoMatrizService(PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context)),
            new CamposObrigatoriosChamadoService(),
            new FakeUsuarioContextoAplicacaoService(usuarioContexto),
            PortalUseCasesTestFactory.Uow(context),
            processarEventoCandidatoNotificacaoUseCase: NotificacoesItsmTestFactory.CriarOrquestrador(context, usuarioContexto),
            logger: NullLogger<AbrirChamadoUseCase>.Instance);
    }
}
