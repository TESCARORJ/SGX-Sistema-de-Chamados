using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using System.Text.Json.Nodes;

namespace SGX.SistemaChamado.Tests;

public sealed class RejeitarSolucaoChamadoUseCaseTests
{
    [Fact]
    public async Task DeveRejeitarSolucaoEVoltarParaEmAtendimento()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var chamado = context.Chamados.Single();
        var resolvidoId = context.StatusChamado.Single(s => s.Codigo == StatusChamadoEnum.Resolvido).Id;
        chamado.Resolver(resolvidoId, "Solucao de teste", "Admin");
        context.SaveChanges();

        var useCase = CriarUseCase(context, dados.SolicitanteContexto);

        var request = new RejeitarSolucaoChamadoRequest { MotivoRejeicao = "A solucao nao funcionou." };
        var response = await useCase.ExecutarAsync(chamado.Id, request);

        Assert.Equal("Em Atendimento", response.Status);

        var chamadoDb = context.Chamados.Single();
        Assert.Equal(StatusChamadoEnum.EmAtendimento, chamadoDb.Status.Codigo);
        Assert.NotNull(chamadoDb.SolucaoRejeitadaEm);
        Assert.Equal("A solucao nao funcionou.", chamadoDb.MotivoRejeicaoSolucao);
        Assert.Equal(dados.SolicitanteContexto.Id, chamadoDb.SolucaoRejeitadaPorUsuarioId);
        Assert.Contains(context.HistoricosChamado, h => h.Tipo == TipoHistoricoChamado.SolucaoRejeitada);
        Assert.Contains(context.ComentariosChamado, c => c.Mensagem != null && c.Mensagem.Contains("SOLUCAO REJEITADA"));
    }

    [Fact]
    public async Task NaoDeveRejeitarSeNaoForResolvido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var chamado = context.Chamados.Single();
        var useCase = CriarUseCase(context, dados.SolicitanteContexto);

        var request = new RejeitarSolucaoChamadoRequest { MotivoRejeicao = "A solucao nao funcionou." };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(chamado.Id, request));
        Assert.Contains("O chamado precisa estar resolvido para ter a solucao rejeitada.", ex.Message);
    }

    [Fact]
    public async Task NaoDeveRejeitarSeMotivoEstiverVazio()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var chamado = context.Chamados.Single();
        var resolvidoId = context.StatusChamado.Single(s => s.Codigo == StatusChamadoEnum.Resolvido).Id;
        chamado.Resolver(resolvidoId, "Solucao de teste", "Admin");
        context.SaveChanges();

        var useCase = CriarUseCase(context, dados.SolicitanteContexto);
        var request = new RejeitarSolucaoChamadoRequest { MotivoRejeicao = "" };

        var ex = await Assert.ThrowsAsync<ValidationException>(() => useCase.ExecutarAsync(chamado.Id, request));
        Assert.Contains("MotivoRejeicao", ex.Message);
    }

    [Fact]
    public async Task DeveRegistrarAuditoriaDaRejeicaoComStatusMotivoEUsuario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var chamado = context.Chamados.Single();
        var resolvidoId = context.StatusChamado.Single(s => s.Codigo == StatusChamadoEnum.Resolvido).Id;
        chamado.Resolver(resolvidoId, "Solucao de teste", "Admin");
        context.SaveChanges();

        var auditoria = new FakeAuditoriaService();
        var useCase = CriarUseCase(context, dados.SolicitanteContexto, auditoria);

        await useCase.ExecutarAsync(chamado.Id, new RejeitarSolucaoChamadoRequest { MotivoRejeicao = "Nao funcionou na pratica." });

        var evento = Assert.Single(auditoria.Eventos);
        Assert.Equal("Solucao do chamado rejeitada pelo solicitante.", evento.Descricao);
        Assert.Equal(TipoAcaoAuditoria.RejeitarSolucaoChamado, evento.Acao);
        Assert.Equal(dados.SolicitanteContexto.Id, evento.UsuarioId);
        Assert.Equal(dados.SolicitanteContexto.Login, evento.UsuarioLogin);

        var dadosAntes = JsonNode.Parse(evento.DadosAntes!)!.AsObject();
        var dadosDepois = JsonNode.Parse(evento.DadosDepois!)!.AsObject();

        Assert.Equal(chamado.Id.ToString(), dadosAntes["ChamadoId"]!.ToString());
        Assert.Equal(dados.SolicitanteContexto.Id.ToString(), dadosDepois["UsuarioExecutorId"]!.ToString());
        Assert.Equal("Resolvido", dadosAntes["StatusAnterior"]!.GetValue<string>());
        Assert.Equal("Em Atendimento", dadosDepois["StatusNovo"]!.GetValue<string>());
        Assert.Equal("Nao funcionou na pratica.", dadosDepois["MotivoRejeicaoSolucao"]!.GetValue<string>());
        Assert.NotNull(dadosDepois["DataEventoUtc"]);
        Assert.NotNull(dadosDepois["SolucaoRejeitadaEm"]);
    }

    [Fact]
    public async Task DevePreservarResolucaoENaoConfigurarCamposDeReaberturaOuFechamento()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var chamado = context.Chamados.Single();
        var resolvidoId = context.StatusChamado.Single(s => s.Codigo == StatusChamadoEnum.Resolvido).Id;
        chamado.Resolver(resolvidoId, "Solucao de teste", "Admin");
        var resolvidoEm = chamado.ResolvidoEm;
        context.SaveChanges();

        var useCase = CriarUseCase(context, dados.SolicitanteContexto);

        var request = new RejeitarSolucaoChamadoRequest { MotivoRejeicao = "A solucao nao funcionou." };
        await useCase.ExecutarAsync(chamado.Id, request);

        var chamadoDb = context.Chamados.Single();
        
        Assert.Equal(resolvidoEm, chamadoDb.ResolvidoEm);
        Assert.NotNull(chamadoDb.SolucaoRejeitadaEm);
        Assert.Equal(dados.SolicitanteContexto.Id, chamadoDb.SolucaoRejeitadaPorUsuarioId);
        Assert.Equal("A solucao nao funcionou.", chamadoDb.MotivoRejeicaoSolucao);
        Assert.Null(chamadoDb.EncerradoEm);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task NaoDeveRejeitarSeMotivoNuloOuApenasEspacos(string? motivoInvalido)
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var chamado = context.Chamados.Single();
        var resolvidoId = context.StatusChamado.Single(s => s.Codigo == StatusChamadoEnum.Resolvido).Id;
        chamado.Resolver(resolvidoId, "Solucao de teste", "Admin");
        context.SaveChanges();

        var auditoria = new FakeAuditoriaService();
        var useCase = CriarUseCase(context, dados.SolicitanteContexto, auditoria);
        // Utilizando reflexao ou instanciando para ignorar warning nullable, ja que queremos testar o validator
        var request = new RejeitarSolucaoChamadoRequest { MotivoRejeicao = motivoInvalido! };

        await Assert.ThrowsAsync<ValidationException>(() => useCase.ExecutarAsync(chamado.Id, request));

        var chamadoDb = context.Chamados.Single();
        Assert.Equal(StatusChamadoEnum.Resolvido, chamadoDb.Status.Codigo);
        Assert.Empty(auditoria.Eventos);
        Assert.DoesNotContain(context.HistoricosChamado, h => h.Tipo == TipoHistoricoChamado.SolucaoRejeitada);
    }

    [Fact]
    public async Task NaoDeveRejeitarSeUsuarioNaoForSolicitante()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var chamado = context.Chamados.Single();
        var resolvidoId = context.StatusChamado.Single(s => s.Codigo == StatusChamadoEnum.Resolvido).Id;
        chamado.Resolver(resolvidoId, "Solucao de teste", "Admin");
        context.SaveChanges();

        var usuarioDiferente = new UsuarioContextoAplicacao(Guid.NewGuid(), "Outro", "outro@teste.com", "outro", ["Solicitante"]);
        var useCase = CriarUseCase(context, usuarioDiferente);

        var request = new RejeitarSolucaoChamadoRequest { MotivoRejeicao = "Nao funcionou" };

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(chamado.Id, request));
        Assert.Contains("Apenas o solicitante pode rejeitar a solucao", ex.Message);
    }

    private static RejeitarSolucaoChamadoUseCase CriarUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto,
        FakeAuditoriaService? auditoria = null)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Uow(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            auditoria ?? new FakeAuditoriaService(),
            new ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
                PortalUseCasesTestFactory.Repo<Chamado>(context),
                PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
                PortalUseCasesTestFactory.Repo<StatusChamado>(context))
        );

    private static async Task<(Chamado Chamado, Usuario Solicitante, UsuarioContextoAplicacao SolicitanteContexto)> SeedAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "REJ1");

        return (chamado, solicitante, AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"));
    }
}
