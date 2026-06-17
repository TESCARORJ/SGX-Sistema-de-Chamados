using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;
using System.Reflection;
using System.Text.Json.Nodes;

namespace SGX.SistemaChamado.Tests;

public sealed class AuditoriaCicloFechamentoChamadoTests
{
    [Fact]
    public async Task AuditoriaCicloFechamentoChamado_DeveRegistrarUmUnicoEventoPorFluxoCritico()
    {
        using (var context = AdminUseCasesTestFactory.CriarContexto())
        {
            var dados = await CriarChamadoEmAtendimentoAdminAsync(context, "AUD-RES");
            var auditoria = new FakeAuditoriaService();
            var useCase = CriarResolverUseCase(context, dados.Contexto, auditoria);
            await useCase.ExecutarAsync(dados.Chamado.Id, new ResolverChamadoRequest { Solucao = "Resolvido", ComentarioInterno = false });
            Assert.Single(auditoria.Eventos);
        }

        using (var context = PortalUseCasesTestFactory.CriarContexto())
        {
            var dados = await CriarChamadoResolvidoPortalAsync(context, "AUD-ACE");
            var auditoria = new FakeAuditoriaService();
            var useCase = CriarAceiteUseCase(context, dados.Contexto, auditoria);
            await useCase.ExecutarAsync(dados.Chamado.Id, new AceitarSolucaoChamadoRequest { ObservacaoAceite = "Aceito" });
            Assert.Single(auditoria.Eventos);
        }

        using (var context = PortalUseCasesTestFactory.CriarContexto())
        {
            var dados = await CriarChamadoResolvidoPortalAsync(context, "AUD-REJ");
            var auditoria = new FakeAuditoriaService();
            var useCase = CriarRejeicaoUseCase(context, dados.Contexto, auditoria);
            await useCase.ExecutarAsync(dados.Chamado.Id, new RejeitarSolucaoChamadoRequest { MotivoRejeicao = "Nao funcionou" });
            Assert.Single(auditoria.Eventos);
        }

        using (var context = PortalUseCasesTestFactory.CriarContexto())
        {
            var chamado = await CriarChamadoStatusAsync(context, "AUD-FEC", StatusChamadoEnum.Resolvido);
            DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), DateTime.UtcNow.AddHours(-100));
            await context.SaveChangesAsync();
            var auditoria = new FakeAuditoriaService();
            var useCase = CriarFechamentoAutomaticoUseCase(context, auditoria);
            await useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest { DataReferencia = DateTime.UtcNow, PrazoAceiteHoras = 72 });
            Assert.Single(auditoria.Eventos);
        }

        using (var context = AdminUseCasesTestFactory.CriarContexto())
        {
            var dados = await CriarChamadoEncerradoAdminAsync(context, "AUD-REA");
            var auditoria = new FakeAuditoriaService();
            var useCase = CriarReabrirUseCase(context, dados.Contexto, auditoria);
            await useCase.ExecutarAsync(dados.Chamado.Id, new ReabrirChamadoRequest { Mensagem = "Reabrir por auditoria" });
            Assert.Single(auditoria.Eventos);
        }
    }

    [Fact]
    public async Task AuditoriaCicloFechamentoChamado_DevePadronizarCamposMinimosNosPayloads()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarChamadoResolvidoPortalAsync(context, "AUD-CAM");
        var auditoria = new FakeAuditoriaService();
        var useCase = CriarAceiteUseCase(context, dados.Contexto, auditoria);

        await useCase.ExecutarAsync(dados.Chamado.Id, new AceitarSolucaoChamadoRequest { ObservacaoAceite = "Aceito com rastreabilidade" });

        var evento = Assert.Single(auditoria.Eventos);
        var dadosAntes = JsonNode.Parse(evento.DadosAntes!)!.AsObject();
        var dadosDepois = JsonNode.Parse(evento.DadosDepois!)!.AsObject();

        Assert.NotNull(dadosAntes["StatusAnterior"]);
        Assert.NotNull(dadosDepois["StatusNovo"]);
        Assert.NotNull(dadosDepois["AceitoEm"]);
        Assert.NotNull(evento.Metadados);
        Assert.Equal(TipoAcaoAuditoria.AceitarSolucaoChamado, evento.Acao);
        Assert.NotNull(dadosDepois["DataEventoUtc"]);
    }

    private static ResolverChamadoUseCase CriarResolverUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto,
        FakeAuditoriaService auditoria)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            new RelacionamentosChamadoUseCases(
                PortalUseCasesTestFactory.Repo<Chamado>(context),
                PortalUseCasesTestFactory.Repo<ChamadoRelacionamento>(context),
                PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
                new FakeUsuarioContextoAplicacaoService(contexto),
                PortalUseCasesTestFactory.Uow(context)),
            new ChamadoAprovacoesUseCases(
                PortalUseCasesTestFactory.Repo<Chamado>(context),
                PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
                PortalUseCasesTestFactory.Repo<Usuario>(context),
                PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
                new FakeUsuarioContextoAplicacaoService(contexto),
                PortalUseCasesTestFactory.Uow(context)),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context),
            auditoria,
            new ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
                PortalUseCasesTestFactory.Repo<Chamado>(context),
                PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
                PortalUseCasesTestFactory.Repo<StatusChamado>(context)));

    private static AceitarSolucaoChamadoUseCase CriarAceiteUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto,
        FakeAuditoriaService auditoria)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context),
            auditoria,
            new ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
                PortalUseCasesTestFactory.Repo<Chamado>(context),
                PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
                PortalUseCasesTestFactory.Repo<StatusChamado>(context)));

    private static RejeitarSolucaoChamadoUseCase CriarRejeicaoUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto,
        FakeAuditoriaService auditoria)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Uow(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            auditoria,
            new ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
                PortalUseCasesTestFactory.Repo<Chamado>(context),
                PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
                PortalUseCasesTestFactory.Repo<StatusChamado>(context)));

    private static FecharChamadosAutomaticamentePorPrazoAceiteUseCase CriarFechamentoAutomaticoUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        FakeAuditoriaService auditoria)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<ParametroSistema>(context),
            new ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
                PortalUseCasesTestFactory.Repo<Chamado>(context),
                PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
                PortalUseCasesTestFactory.Repo<StatusChamado>(context)),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);

    private static ReabrirChamadoUseCase CriarReabrirUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto,
        FakeAuditoriaService auditoria)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context),
            auditoria,
            new ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
                PortalUseCasesTestFactory.Repo<Chamado>(context),
                PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
                PortalUseCasesTestFactory.Repo<StatusChamado>(context)));

    private static async Task<(Chamado Chamado, UsuarioContextoAplicacao Contexto)> CriarChamadoEmAtendimentoAdminAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        string codigo)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", $"{codigo.ToLowerInvariant()}@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", $"sol.{codigo.ToLowerInvariant()}@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, null, codigo);
        return (chamado, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }

    private static async Task<(Chamado Chamado, Usuario Solicitante, UsuarioContextoAplicacao Contexto)> CriarChamadoResolvidoPortalAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        string codigo)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", $"admin.{codigo.ToLowerInvariant()}@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", $"sol.{codigo.ToLowerInvariant()}@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, null, codigo);
        chamado.Resolver(context.StatusChamado.Single(x => x.Codigo == StatusChamadoEnum.Resolvido).Id, "Solucao aplicada", admin.Login);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), DateTime.UtcNow.AddHours(-5));
        context.Chamados.Update(chamado);
        await context.SaveChangesAsync();
        return (chamado, solicitante, AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"));
    }

    private static async Task<(Chamado Chamado, UsuarioContextoAplicacao Contexto)> CriarChamadoEncerradoAdminAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        string codigo)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", $"adm.{codigo.ToLowerInvariant()}@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", $"solic.{codigo.ToLowerInvariant()}@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Encerrado, null, codigo);
        chamado.Encerrar(context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Encerrado).Id, "teste");
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), DateTime.UtcNow.AddHours(-12));
        DefinirPropriedade(chamado, nameof(Chamado.EncerradoEm), DateTime.UtcNow.AddHours(-6));
        context.Chamados.Update(chamado);
        await context.SaveChangesAsync();
        return (chamado, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }

    private static async Task<Chamado> CriarChamadoStatusAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        string codigo,
        StatusChamadoEnum status)
    {
        var chamado = new Chamado(
            codigo,
            "Titulo teste",
            "Descricao teste",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            status switch
            {
                StatusChamadoEnum.Resolvido => SeedData.StatusResolvidoId,
                StatusChamadoEnum.Encerrado => SeedData.StatusEncerradoId,
                StatusChamadoEnum.Cancelado => SeedData.StatusCanceladoId,
                StatusChamadoEnum.EmAtendimento => SeedData.StatusEmAtendimentoId,
                _ => throw new InvalidOperationException("Status nao suportado para teste.")
            },
            OrigemChamado.Admin,
            "teste");

        await context.Chamados.AddAsync(chamado);
        await context.SaveChangesAsync();
        return chamado;
    }

    private static void DefinirPropriedade(object alvo, string propriedade, object? valor)
    {
        var propertyInfo = alvo.GetType().GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        propertyInfo!.SetValue(alvo, valor);
    }
}
