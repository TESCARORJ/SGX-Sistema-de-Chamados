using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class IntegracaoNotificacaoEncerramentoChamadoTests
{
    [Fact]
    public async Task DevePersistirNotificacaoAoEncerrarChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Encerramento", "admin.encerrar@sgx.local", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Encerramento", "sol.encerrar@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Categoria Encerramento");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, criadoPor: "test.encerrar");

        await NotificacoesItsmTestFactory.CriarTemplatesPadraoChamadoAsync(context, admin.Id);

        var fluxoStatus = new FluxoStatusChamadoService();
        var contextoAdmin = AdminUseCasesTestFactory.Contexto(admin, "Administrador");
        var useCase = new EncerrarChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            fluxoStatus,
            new AcoesChamadoService(fluxoStatus),
            new FakeAdminRelacionamentosChamadoUseCases(),
            new FakeAdminChamadoAprovacoesUseCases(),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contextoAdmin),
            PortalUseCasesTestFactory.Uow(context),
            processarEventoCandidatoNotificacaoUseCase: NotificacoesItsmTestFactory.CriarOrquestrador(context, contextoAdmin),
            logger: NullLogger<EncerrarChamadoUseCase>.Instance);

        await useCase.ExecutarAsync(chamado.Id, new EncerrarChamadoRequest
        {
            Solucao = "Aplicacao ajustada e validada.",
            ComentarioInterno = false
        });

        var notificacoes = await context.Notificacoes
            .Where(x => x.ChamadoId == chamado.Id)
            .ToListAsync();

        Assert.Equal(2, notificacoes.Count);
        Assert.All(notificacoes, x => Assert.Equal(solicitante.Id, x.DestinatarioUsuarioId));
    }
}
