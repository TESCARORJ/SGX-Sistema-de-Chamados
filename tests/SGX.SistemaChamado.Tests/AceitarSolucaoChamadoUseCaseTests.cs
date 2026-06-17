using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using System.Reflection;
using System.Text.Json.Nodes;

namespace SGX.SistemaChamado.Tests;

public sealed class AceitarSolucaoChamadoUseCaseTests
{
    [Fact]
    public async Task DeveAceitarSolucaoEEncerrarChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedResolvidoAsync(context);

        var useCase = CriarUseCase(context, dados.SolicitanteContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new AceitarSolucaoChamadoRequest
        {
            ObservacaoAceite = "Pode encerrar."
        });

        Assert.Equal("Encerrado", response.Status);
        var chamadoDb = context.Chamados.Single(x => x.Id == dados.Chamado.Id);
        Assert.NotNull(chamadoDb.AceitoEm);
        Assert.Equal(dados.Solicitante.Id, chamadoDb.AceitoPorUsuarioId);
        Assert.NotNull(chamadoDb.EncerradoEm);
        Assert.NotNull(chamadoDb.ResolvidoEm);
        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.SolucaoAceita);
    }

    [Fact]
    public async Task NaoDeveAceitarSolucaoQuandoChamadoNaoEstaResolvido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin.aceite2@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol.aceite2@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "ACE2");
        
        var solicitanteContexto = AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante");
        var useCase = CriarUseCase(context, solicitanteContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(chamado.Id, new AceitarSolucaoChamadoRequest()));
        
        Assert.Equal("Apenas chamados com status Resolvido podem ter a solucao aceita.", ex.Message);

        var chamadoDb = context.Chamados.Single(x => x.Id == chamado.Id);
        Assert.Equal(StatusChamadoEnum.EmAtendimento, chamadoDb.Status.Codigo);
        Assert.Null(chamadoDb.EncerradoEm);
    }

    [Fact]
    public async Task NaoDeveAceitarSolucaoQuandoUsuarioNaoEhSolicitante()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedResolvidoAsync(context);
        var outroUsuario = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Outro", "outro@empresa.com", TipoPerfil.Solicitante);
        var outroContexto = AdminUseCasesTestFactory.Contexto(outroUsuario, "Solicitante");

        var useCase = CriarUseCase(context, outroContexto);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new AceitarSolucaoChamadoRequest()));
        
        Assert.Equal("Acesso negado. Apenas o solicitante pode aceitar a solucao do chamado.", ex.Message);
    }

    [Fact]
    public async Task DeveRegistrarAuditoriaDoAceiteComStatusUsuarioEDadosDeAceite()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedResolvidoAsync(context);
        var auditoria = new FakeAuditoriaService();

        var useCase = CriarUseCase(context, dados.SolicitanteContexto, auditoria);
        await useCase.ExecutarAsync(dados.Chamado.Id, new AceitarSolucaoChamadoRequest
        {
            ObservacaoAceite = "Aceite formal registrado."
        });

        var evento = Assert.Single(auditoria.Eventos);
        Assert.Equal("Solucao aceita pelo solicitante (fechamento definitivo).", evento.Descricao);
        Assert.Equal(TipoAcaoAuditoria.AceitarSolucaoChamado, evento.Acao);
        Assert.Equal(dados.Solicitante.Id, evento.UsuarioId);
        Assert.Equal(dados.SolicitanteContexto.Login, evento.UsuarioLogin);

        var dadosAntes = JsonNode.Parse(evento.DadosAntes!)!.AsObject();
        var dadosDepois = JsonNode.Parse(evento.DadosDepois!)!.AsObject();

        Assert.Equal(dados.Chamado.Id.ToString(), dadosAntes["ChamadoId"]!.ToString());
        Assert.Equal(dados.Solicitante.Id.ToString(), dadosDepois["UsuarioExecutorId"]!.ToString());
        Assert.Equal("Resolvido", dadosAntes["StatusAnterior"]!.GetValue<string>());
        Assert.Equal("Encerrado", dadosDepois["StatusNovo"]!.GetValue<string>());
        Assert.NotNull(dadosDepois["DataEventoUtc"]);
        Assert.NotNull(dadosDepois["AceitoEm"]);
        Assert.Equal(dados.Solicitante.Id.ToString(), dadosDepois["AceitoPorUsuarioId"]!.ToString());
        Assert.NotNull(dadosDepois["EncerradoEm"]);
        Assert.Equal("Aceite formal registrado.", dadosDepois["ObservacaoAceite"]!.GetValue<string>());
    }

    [Fact]
    public async Task BloqueiaAceiteManualQuandoHaAprovacaoPendenteBloqueante()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedResolvidoAsync(context);
        context.AprovacoesChamado.Add(CriarAprovacaoLegada(dados, bloqueiaAvancoAtendimento: true));
        await context.SaveChangesAsync();

        var auditoria = new FakeAuditoriaService();
        var useCase = CriarUseCase(context, dados.SolicitanteContexto, auditoria, validarBloqueio: null);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new AceitarSolucaoChamadoRequest
        {
            ObservacaoAceite = "Nao deveria aceitar."
        }));

        Assert.Equal("Este chamado possui aprovacao pendente e nao pode avancar enquanto a aprovacao bloqueante nao for decidida.", ex.Message);

        var chamadoDb = context.Chamados.Single(x => x.Id == dados.Chamado.Id);
        Assert.Equal(StatusChamadoEnum.Resolvido, chamadoDb.Status.Codigo);
        Assert.Null(chamadoDb.EncerradoEm);
        Assert.Null(chamadoDb.AceitoEm);
        Assert.DoesNotContain(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.SolucaoAceita);
        Assert.Empty(auditoria.Eventos);
    }

    [Fact]
    public async Task PermiteAceiteQuandoAprovacaoPendenteNaoEhBloqueante()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedResolvidoAsync(context);
        context.AprovacoesChamado.Add(CriarAprovacaoLegada(dados, bloqueiaAvancoAtendimento: false));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.SolicitanteContexto, validarBloqueio: null);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new AceitarSolucaoChamadoRequest
        {
            ObservacaoAceite = "Aceite permitido."
        });

        Assert.Equal("Encerrado", response.Status);
        Assert.NotNull(context.Chamados.Single(x => x.Id == dados.Chamado.Id).EncerradoEm);
    }

    private static AceitarSolucaoChamadoUseCase CriarUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto,
        FakeAuditoriaService? auditoria = null,
        IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase? validarBloqueio = null)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context),
            auditoria ?? new FakeAuditoriaService(),
            validarBloqueio ?? new ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
                PortalUseCasesTestFactory.Repo<Chamado>(context),
                PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
                PortalUseCasesTestFactory.Repo<StatusChamado>(context)));

    private static async Task<(Chamado Chamado, Usuario Solicitante, UsuarioContextoAplicacao SolicitanteContexto)> SeedResolvidoAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin.aceite@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol.aceite@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "ACE1");
        var resolvidoId = context.StatusChamado.Single(x => x.Codigo == StatusChamadoEnum.Resolvido).Id;
        chamado.Resolver(resolvidoId, "Solucao aplicada", admin.Login);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), DateTime.UtcNow.AddHours(-2));
        context.Chamados.Update(chamado);
        await context.SaveChangesAsync();

        return (chamado, solicitante, AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"));
    }

    private static AprovacaoChamado CriarAprovacaoLegada(
        (Chamado Chamado, Usuario Solicitante, UsuarioContextoAplicacao SolicitanteContexto) dados,
        bool bloqueiaAvancoAtendimento)
        => new(
            dados.Chamado.Id,
            TipoOrigemAprovacaoChamado.CatalogoServico,
            dados.Solicitante.Id,
            dados.SolicitanteContexto.Login,
            dados.Chamado.SolicitanteId,
            "Servico catalogo",
            "Aguarda aprovacao",
            bloqueiaAvancoAtendimento: bloqueiaAvancoAtendimento);

    private static void DefinirPropriedade(object alvo, string propriedade, object? valor)
    {
        var propertyInfo = alvo.GetType().GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        propertyInfo!.SetValue(alvo, valor);
    }
}
