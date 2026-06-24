using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class IntegracaoNotificacaoStatusChamadoTests
{
    [Fact]
    public async Task DeveGerarNotificacaoQuandoStatusForRelevante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Status", "admin.status@sgx.local", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Status", "sol.status@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Categoria Status");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, criadoPor: "test.status");

        await NotificacoesItsmTestFactory.CriarTemplatesPadraoChamadoAsync(context, admin.Id);

        var useCase = CriarUseCase(context, admin);
        var status = await context.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.EmAtendimento);

        await useCase.ExecutarAsync(chamado.Id, new AlterarStatusChamadoRequest { StatusId = status.Id });

        var notificacoes = await context.Notificacoes
            .Where(x => x.ChamadoId == chamado.Id)
            .ToListAsync();

        Assert.Equal(2, notificacoes.Count);
        Assert.All(notificacoes, x => Assert.Equal(solicitante.Id, x.DestinatarioUsuarioId));
    }

    [Fact]
    public async Task DeveNaoGerarNotificacaoQuandoStatusNaoForPriorizado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Cancelamento", "admin.cancelamento@sgx.local", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Cancelamento", "sol.cancelamento@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Categoria Cancelamento");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, criadoPor: "test.cancelamento");

        await NotificacoesItsmTestFactory.CriarTemplatesPadraoChamadoAsync(context, admin.Id);

        var useCase = CriarUseCase(context, admin);
        var status = await context.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Cancelado);

        await useCase.ExecutarAsync(chamado.Id, new AlterarStatusChamadoRequest { StatusId = status.Id });

        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }

    private static AlterarStatusChamadoUseCase CriarUseCase(
        Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        Usuario admin)
    {
        var contextoAdmin = AdminUseCasesTestFactory.Contexto(admin, "Administrador");
        var fluxoStatus = new FluxoStatusChamadoService();

        return new AlterarStatusChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            fluxoStatus,
            new AcoesChamadoService(fluxoStatus),
            new FakeAdminRelacionamentosChamadoUseCases(),
            new FakeAdminChamadoAprovacoesUseCases(),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contextoAdmin),
            PortalUseCasesTestFactory.Uow(context),
            processarEventoCandidatoNotificacaoUseCase: NotificacoesItsmTestFactory.CriarOrquestrador(context, contextoAdmin),
            logger: NullLogger<AlterarStatusChamadoUseCase>.Instance);
    }
}
