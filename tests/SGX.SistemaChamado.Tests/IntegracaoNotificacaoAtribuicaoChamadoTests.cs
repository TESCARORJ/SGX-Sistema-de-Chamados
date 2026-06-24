using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class IntegracaoNotificacaoAtribuicaoChamadoTests
{
    [Fact]
    public async Task DeveGerarNotificacaoParaNovoResponsavelAoAtribuir()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Atribuicao", "admin.atr@sgx.local", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Atribuicao", "sol.atr@sgx.local", TipoPerfil.Solicitante);
        var responsavel = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente Destino", "atendente.destino@sgx.local", TipoPerfil.Atendente);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Categoria Atribuicao");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, criadoPor: "test.atr");

        await NotificacoesItsmTestFactory.CriarTemplatesPadraoChamadoAsync(context, admin.Id);

        var useCase = new AtribuirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<MembroGrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context),
            processarEventoCandidatoNotificacaoUseCase: NotificacoesItsmTestFactory.CriarOrquestrador(context, AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            logger: NullLogger<AtribuirChamadoUseCase>.Instance);

        await useCase.ExecutarAsync(chamado.Id, new AtribuirChamadoRequest { ResponsavelId = responsavel.Id });

        var notificacoes = await context.Notificacoes
            .Where(x => x.ChamadoId == chamado.Id)
            .ToListAsync();

        Assert.Equal(2, notificacoes.Count);
        Assert.All(notificacoes, x => Assert.Equal(responsavel.Id, x.DestinatarioUsuarioId));
    }

    [Fact]
    public async Task DeveNaoDuplicarNotificacaoParaAutoAssuncao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente Assumir", "atendente.assumir@sgx.local", TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Assumir", "sol.assumir@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Categoria Assumir");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, criadoPor: "test.assumir");

        await NotificacoesItsmTestFactory.CriarTemplatesPadraoChamadoAsync(context, atendente.Id);

        var useCase = new AssumirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(atendente, "Atendente")),
            PortalUseCasesTestFactory.Uow(context),
            processarEventoCandidatoNotificacaoUseCase: NotificacoesItsmTestFactory.CriarOrquestrador(context, AdminUseCasesTestFactory.Contexto(atendente, "Atendente")),
            logger: NullLogger<AssumirChamadoUseCase>.Instance);

        await useCase.ExecutarAsync(chamado.Id);

        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }
}
