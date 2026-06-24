using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class RegressaoNotificacoesFluxosItsmTests
{
    [Fact]
    public async Task ReabrirChamadoNaoDeveGerarNotificacaoPorqueNaoEstaNoEscopoPriorizado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Reabertura", "admin.reabertura.notif@sgx.local", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Reabertura", "sol.reabertura.notif@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Categoria Reabertura");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Encerrado, criadoPor: "test.reabertura");

        var fluxoStatus = new FluxoStatusChamadoService();
        var contextoAdmin = AdminUseCasesTestFactory.Contexto(admin, "Administrador");
        var useCase = new ReabrirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            fluxoStatus,
            new AcoesChamadoService(fluxoStatus),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contextoAdmin),
            PortalUseCasesTestFactory.Uow(context));

        await useCase.ExecutarAsync(chamado.Id, new ReabrirChamadoRequest { Mensagem = "Reabertura tecnica." });

        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }

    [Fact]
    public async Task FalhaDeTemplateNaoDeveDesfazerAlteracaoDeStatus()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Status Sem Template", "admin.status.sem.template@sgx.local", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Status Sem Template", "sol.status.sem.template@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Categoria Status Sem Template");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, criadoPor: "test.status.sem.template");

        var fluxoStatus = new FluxoStatusChamadoService();
        var contextoAdmin = AdminUseCasesTestFactory.Contexto(admin, "Administrador");
        var useCase = new AlterarStatusChamadoUseCase(
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
            processarEventoCandidatoNotificacaoUseCase: NotificacoesItsmTestFactory.CriarOrquestrador(context, contextoAdmin));

        var status = await context.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.EmAtendimento);
        await useCase.ExecutarAsync(chamado.Id, new AlterarStatusChamadoRequest { StatusId = status.Id });

        var atualizado = await context.Chamados.Include(x => x.Status).SingleAsync(x => x.Id == chamado.Id);
        Assert.Equal(StatusChamadoEnum.EmAtendimento, atualizado.Status.Codigo);
        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }
}
