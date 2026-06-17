using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;
using System.Text.Json.Nodes;

namespace SGX.SistemaChamado.Tests;

public sealed class FecharChamadosAutomaticamentePorPrazoAceiteUseCaseTests
{
    [Fact]
    public async Task Fecha_Automaticamente_Chamado_Resolvido_Com_Prazo_Expirado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var chamado = await CriarChamadoAsync(context, "AUTO-001", StatusChamadoEnum.Resolvido);
        var dataReferencia = new DateTime(2026, 6, 12, 12, 0, 0, DateTimeKind.Utc);
        var resolvidoEm = dataReferencia.AddHours(-73);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), resolvidoEm);
        await context.SaveChangesAsync();

        var auditoria = new FakeAuditoriaService();
        var useCase = CriarUseCase(context, new FakeValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(), auditoria);

        var response = await useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest
        {
            DataReferencia = dataReferencia,
            PrazoAceiteHoras = 72,
            UsuarioSistemaId = Guid.NewGuid()
        });

        var atualizado = await context.Chamados.Include(x => x.Historicos).SingleAsync(x => x.Id == chamado.Id);

        Assert.Equal(1, response.TotalAnalisados);
        Assert.Equal(1, response.TotalFechados);
        Assert.Equal(0, response.TotalIgnorados);
        Assert.Equal(0, response.TotalBloqueadosPorAprovacao);
        Assert.Equal(SeedData.StatusEncerradoId, atualizado.StatusId);
        Assert.Equal(dataReferencia, atualizado.EncerradoEm);
        Assert.Equal(resolvidoEm, atualizado.ResolvidoEm);
        Assert.Null(atualizado.AceitoEm);
        Assert.Null(atualizado.AceitoPorUsuarioId);
        Assert.Single(atualizado.Historicos);
        Assert.Equal(TipoHistoricoChamado.FechamentoAutomatico, atualizado.Historicos.Single().Tipo);
        Assert.Contains("ausencia de manifestacao do solicitante", atualizado.Historicos.Single().Descricao);
        Assert.Single(response.ChamadosFechados);
        Assert.Equal(atualizado.Id, response.ChamadosFechados.Single().ChamadoId);
        Assert.Equal("Encerrado", response.ChamadosFechados.Single().StatusNovo);
        Assert.Single(auditoria.Eventos);
        var evento = auditoria.Eventos.Single();
        Assert.Equal("Chamado fechado automaticamente por prazo de aceite.", evento.Descricao);
        Assert.Equal(TipoAcaoAuditoria.FecharChamadoAutomaticamentePorPrazoAceite, evento.Acao);

        var dadosAntes = JsonNode.Parse(evento.DadosAntes!)!.AsObject();
        var dadosDepois = JsonNode.Parse(evento.DadosDepois!)!.AsObject();

        Assert.Equal(chamado.Id.ToString(), dadosAntes["ChamadoId"]!.ToString());
        Assert.Equal("Resolvido", dadosAntes["StatusAnterior"]!.GetValue<string>());
        Assert.Equal("Encerrado", dadosDepois["StatusNovo"]!.GetValue<string>());
        Assert.Equal("Automatica", dadosDepois["OrigemFechamento"]!.GetValue<string>());
        Assert.Equal(72, dadosDepois["PrazoAceiteHoras"]!.GetValue<int>());
        Assert.NotNull(dadosDepois["DataEventoUtc"]);
        Assert.Null(dadosDepois["AceitoEm"]);
        Assert.Null(dadosDepois["AceitoPorUsuarioId"]);
    }

    [Fact]
    public async Task Nao_Fecha_Chamado_Resolvido_Dentro_Do_Prazo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var chamado = await CriarChamadoAsync(context, "AUTO-002", StatusChamadoEnum.Resolvido);
        var dataReferencia = new DateTime(2026, 6, 12, 12, 0, 0, DateTimeKind.Utc);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), dataReferencia.AddHours(-24));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, new FakeValidarBloqueioMovimentacaoAprovacaoPendenteUseCase());

        var response = await useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest
        {
            DataReferencia = dataReferencia,
            PrazoAceiteHoras = 72
        });

        var atualizado = await context.Chamados.SingleAsync(x => x.Id == chamado.Id);

        Assert.Equal(0, response.TotalAnalisados);
        Assert.Equal(0, response.TotalFechados);
        Assert.Equal(SeedData.StatusResolvidoId, atualizado.StatusId);
        Assert.Null(atualizado.EncerradoEm);
    }

    [Theory]
    [InlineData(StatusChamadoEnum.Encerrado)]
    [InlineData(StatusChamadoEnum.Cancelado)]
    [InlineData(StatusChamadoEnum.EmAtendimento)]
    public async Task Nao_Fecha_Chamado_Que_Nao_Esta_Resolvido(StatusChamadoEnum statusAtual)
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var chamado = await CriarChamadoAsync(context, $"AUTO-{(int)statusAtual:000}", statusAtual);
        var dataReferencia = new DateTime(2026, 6, 12, 12, 0, 0, DateTimeKind.Utc);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), dataReferencia.AddHours(-120));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, new FakeValidarBloqueioMovimentacaoAprovacaoPendenteUseCase());

        var response = await useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest
        {
            DataReferencia = dataReferencia,
            PrazoAceiteHoras = 72
        });

        var atualizado = await context.Chamados.SingleAsync(x => x.Id == chamado.Id);

        Assert.Equal(0, response.TotalAnalisados);
        Assert.Equal(0, response.TotalFechados);
        Assert.Equal(chamado.StatusId, atualizado.StatusId);
    }

    [Fact]
    public async Task Nao_Fecha_Chamado_Que_Retornou_Ao_Atendimento_Apos_Rejeicao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var chamado = await CriarChamadoAsync(context, "AUTO-RET", StatusChamadoEnum.Resolvido);
        var dataReferencia = new DateTime(2026, 6, 12, 12, 0, 0, DateTimeKind.Utc);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), dataReferencia.AddHours(-96));
        await context.SaveChangesAsync();
        chamado.RejeitarSolucao(SeedData.StatusEmAtendimentoId, Guid.NewGuid(), "Precisa revisar a solucao.", "teste");
        await context.SaveChangesAsync();

        var resolvidoEmOriginal = chamado.ResolvidoEm;
        var motivoRejeicao = chamado.MotivoRejeicaoSolucao;
        var rejeitadoEm = chamado.SolucaoRejeitadaEm;

        var useCase = CriarUseCase(context, new FakeValidarBloqueioMovimentacaoAprovacaoPendenteUseCase());

        var response = await useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest
        {
            DataReferencia = dataReferencia,
            PrazoAceiteHoras = 72
        });

        var atualizado = await context.Chamados.SingleAsync(x => x.Id == chamado.Id);

        Assert.Equal(0, response.TotalAnalisados);
        Assert.Equal(0, response.TotalFechados);
        Assert.Equal(SeedData.StatusEmAtendimentoId, atualizado.StatusId);
        Assert.Equal(resolvidoEmOriginal, atualizado.ResolvidoEm);
        Assert.Equal(motivoRejeicao, atualizado.MotivoRejeicaoSolucao);
        Assert.Equal(rejeitadoEm, atualizado.SolucaoRejeitadaEm);
    }

    [Fact]
    public async Task Bloqueia_Fechamento_Quando_Ha_Aprovacao_Pendente_Bloqueante()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var chamado = await CriarChamadoAsync(context, "AUTO-003", StatusChamadoEnum.Resolvido);
        var dataReferencia = new DateTime(2026, 6, 12, 12, 0, 0, DateTimeKind.Utc);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), dataReferencia.AddHours(-96));
        await context.SaveChangesAsync();

        var auditoria = new FakeAuditoriaService();
        var useCase = CriarUseCase(context, new FakeValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
            new ValidarBloqueioMovimentacaoAprovacaoPendenteResponse
            {
                Bloqueado = true,
                MensagemUsuario = "Aprovacao pendente bloqueante."
            }), auditoria);

        var response = await useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest
        {
            DataReferencia = dataReferencia,
            PrazoAceiteHoras = 72
        });

        var atualizado = await context.Chamados.Include(x => x.Historicos).SingleAsync(x => x.Id == chamado.Id);

        Assert.Equal(1, response.TotalAnalisados);
        Assert.Equal(0, response.TotalFechados);
        Assert.Equal(1, response.TotalIgnorados);
        Assert.Equal(1, response.TotalBloqueadosPorAprovacao);
        Assert.Single(response.ChamadosIgnorados);
        Assert.True(response.ChamadosIgnorados.Single().BloqueadoPorAprovacao);
        Assert.Equal(SeedData.StatusResolvidoId, atualizado.StatusId);
        Assert.Null(atualizado.EncerradoEm);
        Assert.Empty(atualizado.Historicos);
        Assert.Empty(auditoria.Eventos);
    }

    [Fact]
    public async Task Retorna_Resumo_Correto_Com_Fechados_E_Bloqueados()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var chamadoFechado = await CriarChamadoAsync(context, "AUTO-004", StatusChamadoEnum.Resolvido);
        var chamadoBloqueado = await CriarChamadoAsync(context, "AUTO-005", StatusChamadoEnum.Resolvido);
        var dataReferencia = new DateTime(2026, 6, 12, 12, 0, 0, DateTimeKind.Utc);
        DefinirPropriedade(chamadoFechado, nameof(Chamado.ResolvidoEm), dataReferencia.AddHours(-100));
        DefinirPropriedade(chamadoBloqueado, nameof(Chamado.ResolvidoEm), dataReferencia.AddHours(-101));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, new FakeValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
            new ValidarBloqueioMovimentacaoAprovacaoPendenteResponse { Bloqueado = false },
            new ValidarBloqueioMovimentacaoAprovacaoPendenteResponse { Bloqueado = true, MensagemUsuario = "Bloqueado." }));

        var response = await useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest
        {
            DataReferencia = dataReferencia,
            PrazoAceiteHoras = 72
        });

        Assert.Equal(2, response.TotalAnalisados);
        Assert.Equal(1, response.TotalFechados);
        Assert.Equal(1, response.TotalIgnorados);
        Assert.Equal(1, response.TotalBloqueadosPorAprovacao);
        Assert.Single(response.ChamadosFechados);
        Assert.Single(response.ChamadosIgnorados);
    }

    [Fact]
    public async Task NaoRegistraAuditoriaDeFechamentoAutomaticoQuandoBloqueadoPorAprovacaoPendente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var chamado = await CriarChamadoAsync(context, "AUTO-APR-BLQ", StatusChamadoEnum.Resolvido);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), DateTime.UtcNow.AddHours(-96));
        await context.SaveChangesAsync();

        var auditoria = new FakeAuditoriaService();
        var useCase = CriarUseCase(
            context,
            new FakeValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
                new ValidarBloqueioMovimentacaoAprovacaoPendenteResponse
                {
                    Bloqueado = true,
                    MensagemUsuario = "Aprovacao pendente bloqueante."
                }),
            auditoria);

        await useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest
        {
            DataReferencia = DateTime.UtcNow,
            PrazoAceiteHoras = 72
        });

        Assert.Empty(auditoria.Eventos);
    }

    [Fact]
    public async Task FechaAutomaticamenteQuandoInstanciaBloqueanteJaFoiAprovada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var chamado = await CriarChamadoAsync(context, "AUTO-APR-OK", StatusChamadoEnum.Resolvido);
        var dataReferencia = new DateTime(2026, 6, 14, 12, 0, 0, DateTimeKind.Utc);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), dataReferencia.AddHours(-96));

        var instancia = new InstanciaAprovacaoChamado(
            chamado.Id,
            chamado.SolicitanteId,
            OrigemInstanciaAprovacaoChamado.RegraMotor,
            TipoFluxoAprovacao.Simples,
            EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            EscopoRegraAprovacao.AtendimentoChamado,
            TipoRegraAprovacao.TipoSolicitacao,
            exigeAprovacao: true,
            bloqueante: true,
            TipoResolucaoAprovadorRegraAprovacao.NaoDefinido,
            Guid.NewGuid(),
            "teste",
            titulo: "Aprovacao resolvida",
            descricao: "Instancia aprovada antes do fechamento automatico.",
            naturezaChamado: chamado.NaturezaChamado,
            categoriaId: chamado.CategoriaId,
            prazoDecisaoHoras: 4);
        instancia.RegistrarDecisaoResumo(StatusInstanciaAprovacaoChamado.Aprovada, Guid.NewGuid(), Guid.NewGuid(), "teste");
        context.InstanciasAprovacaoChamado.Add(instancia);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(
            context,
            new ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
                PortalUseCasesTestFactory.Repo<Chamado>(context),
                PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
                PortalUseCasesTestFactory.Repo<StatusChamado>(context)));

        var response = await useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest
        {
            DataReferencia = dataReferencia,
            PrazoAceiteHoras = 72
        });

        var atualizado = await context.Chamados.SingleAsync(x => x.Id == chamado.Id);
        Assert.Equal(1, response.TotalFechados);
        Assert.Equal(0, response.TotalBloqueadosPorAprovacao);
        Assert.Equal(SeedData.StatusEncerradoId, atualizado.StatusId);
        Assert.Equal(dataReferencia, atualizado.EncerradoEm);
    }

    [Fact]
    public async Task Fecha_Automaticamente_Respeitando_Prazo_Da_Configuracao_Administrativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        
        var parametroId = SeedData.ParametroPrazoAutoFechamentoChamadoId;
        var parametro = await context.ParametrosSistema.SingleOrDefaultAsync(x => x.Id == parametroId);
        if (parametro == null)
        {
            parametro = new ParametroSistema(
                ConfiguracaoAutoFechamentoChamadoConstantes.ChaveParametroPrazoAceiteHoras,
                "120",
                "Prazo aceite horas",
                false,
                "teste"
            );
            typeof(ParametroSistema).GetProperty("Id")?.SetValue(parametro, parametroId);
            context.ParametrosSistema.Add(parametro);
        }
        else
        {
            parametro.AtualizarValor("120", "teste");
        }
        
        var chamado = await CriarChamadoAsync(context, "AUTO-CONF", StatusChamadoEnum.Resolvido);
        var dataReferencia = new DateTime(2026, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), dataReferencia.AddHours(-121));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, new FakeValidarBloqueioMovimentacaoAprovacaoPendenteUseCase());

        var response = await useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest
        {
            DataReferencia = dataReferencia,
            PrazoAceiteHoras = null // Nao informa prazo explicito
        });

        var atualizado = await context.Chamados.SingleAsync(x => x.Id == chamado.Id);

        Assert.Equal(1, response.TotalFechados);
        Assert.Equal(SeedData.StatusEncerradoId, atualizado.StatusId);
        Assert.Equal(dataReferencia, atualizado.EncerradoEm);
    }

    [Fact]
    public async Task Valida_Prazo_Positivo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = CriarUseCase(context, new FakeValidarBloqueioMovimentacaoAprovacaoPendenteUseCase());

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest
        {
            PrazoAceiteHoras = 0
        }));
    }

    private static FecharChamadosAutomaticamentePorPrazoAceiteUseCase CriarUseCase(
        Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase validarBloqueioUseCase,
        FakeAuditoriaService? auditoriaService = null)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<ParametroSistema>(context),
            validarBloqueioUseCase,
            PortalUseCasesTestFactory.Uow(context),
            auditoriaService ?? new FakeAuditoriaService());

    private static async Task<Chamado> CriarChamadoAsync(
        Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
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
            ObterStatusId(status),
            OrigemChamado.Admin,
            "teste");

        await context.Chamados.AddAsync(chamado);
        await context.SaveChangesAsync();
        return chamado;
    }

    private static Guid ObterStatusId(StatusChamadoEnum status)
        => status switch
        {
            StatusChamadoEnum.Resolvido => SeedData.StatusResolvidoId,
            StatusChamadoEnum.Encerrado => SeedData.StatusEncerradoId,
            StatusChamadoEnum.Cancelado => SeedData.StatusCanceladoId,
            StatusChamadoEnum.EmAtendimento => SeedData.StatusEmAtendimentoId,
            _ => throw new InvalidOperationException($"Status nao mapeado para teste: {status}.")
        };

    private static void DefinirPropriedade<T>(Chamado chamado, string propriedade, T valor)
    {
        var propertyInfo = typeof(Chamado).GetProperty(
            propriedade,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        propertyInfo!.SetValue(chamado, valor);
    }

    private sealed class FakeValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
        params ValidarBloqueioMovimentacaoAprovacaoPendenteResponse[] respostas) : IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase
    {
        private readonly Queue<ValidarBloqueioMovimentacaoAprovacaoPendenteResponse> _respostas =
            new(respostas.Length == 0
                ? [new ValidarBloqueioMovimentacaoAprovacaoPendenteResponse { Bloqueado = false }]
                : respostas);

        public Task<ValidarBloqueioMovimentacaoAprovacaoPendenteResponse> ExecutarAsync(
            ValidarBloqueioMovimentacaoAprovacaoPendenteRequest request,
            CancellationToken cancellationToken = default)
        {
            var resposta = _respostas.Count > 1 ? _respostas.Dequeue() : _respostas.Peek();
            return Task.FromResult(resposta);
        }
    }
}
