using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AprovarAprovacaoChamadoUseCaseTests
{
    [Fact]
    public async Task DeveAprovarInstanciaSimplesPendente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "simples");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Simples);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var statusChamadoAntes = dados.Chamado.StatusId;

        var response = await useCase.ExecutarAsync(new AprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Escopo aprovado.",
            LiberaAvanco = true
        });

        var instanciaPersistida = await context.InstanciasAprovacaoChamado.AsNoTracking().SingleAsync(x => x.Id == instancia.Id);
        var decisao = await context.DecisoesAprovacaoChamado.AsNoTracking().SingleAsync();
        var chamadoPersistido = await context.Chamados.AsNoTracking().SingleAsync(x => x.Id == dados.Chamado.Id);

        Assert.True(response.Aprovada);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Pendente, response.StatusInstanciaAnterior);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Aprovada, response.StatusInstanciaNovo);
        Assert.True(response.DecisaoFinal);
        Assert.True(response.LiberaAvanco);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Aprovada, instanciaPersistida.Status);
        Assert.NotNull(instanciaPersistida.DecididaEm);
        Assert.Equal(TipoDecisaoAprovacaoChamado.Aprovacao, decisao.TipoDecisao);
        Assert.Equal(ResultadoDecisaoAprovacaoChamado.Aprovada, decisao.Resultado);
        Assert.Equal(dados.Aprovador.Id, decisao.DecisorUsuarioId);
        Assert.Equal(statusChamadoAntes, chamadoPersistido.StatusId);
    }

    [Fact]
    public async Task DeveAprovarEtapaSemConsolidarInstanciaQuandoHaOutraObrigatoriaPendente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "etapa-pendente");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Sequencial);
        var etapa1 = await CriarEtapaAsync(context, instancia, dados, ordem: 1, nivel: 1);
        await CriarEtapaAsync(context, instancia, dados, ordem: 2, nivel: 1);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var response = await useCase.ExecutarAsync(new AprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            EtapaAprovacaoChamadoId = etapa1.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Primeira etapa aprovada.",
            DecisaoParcial = true
        });

        var instanciaPersistida = await context.InstanciasAprovacaoChamado.AsNoTracking().SingleAsync(x => x.Id == instancia.Id);
        var etapaPersistida = await context.EtapasAprovacaoChamado.AsNoTracking().SingleAsync(x => x.Id == etapa1.Id);
        var decisao = await context.DecisoesAprovacaoChamado.AsNoTracking().SingleAsync();

        Assert.Equal(StatusEtapaAprovacaoChamado.Pendente, response.StatusEtapaAnterior);
        Assert.Equal(StatusEtapaAprovacaoChamado.Aprovada, response.StatusEtapaNovo);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Pendente, response.StatusInstanciaNovo);
        Assert.False(response.DecisaoFinal);
        Assert.Equal(StatusEtapaAprovacaoChamado.Aprovada, etapaPersistida.Status);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Pendente, instanciaPersistida.Status);
        Assert.True(decisao.DecisaoParcial);
        Assert.False(decisao.DecisaoFinal);
    }

    [Fact]
    public async Task DeveAprovarUltimaEtapaObrigatoriaComDecisaoFinal()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "ultima-etapa");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Sequencial);
        var etapa = await CriarEtapaAsync(context, instancia, dados, ordem: 1, nivel: 1);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var response = await useCase.ExecutarAsync(new AprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            EtapaAprovacaoChamadoId = etapa.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Etapa final aprovada.",
            DecisaoFinal = true,
            LiberaAvanco = true
        });

        var instanciaPersistida = await context.InstanciasAprovacaoChamado.AsNoTracking().SingleAsync(x => x.Id == instancia.Id);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Aprovada, instanciaPersistida.Status);
        Assert.True(response.DecisaoFinal);
        Assert.True(response.LiberaAvanco);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Aprovada, response.StatusInstanciaNovo);
    }

    [Fact]
    public async Task NaoDeveAprovarInstanciaInexistente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "inexistente");
        var useCase = CriarUseCase(context, dados.Aprovador);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(new AprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = Guid.NewGuid(),
            DecisorUsuarioId = dados.Aprovador.Id
        }));

        Assert.Contains("Instancia de aprovacao nao encontrada", ex.Message);
    }

    [Fact]
    public async Task NaoDeveAprovarInstanciaCancelada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "cancelada");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Simples);
        instancia.MarcarCancelada(dados.Administrador.Id, dados.Administrador.Id, "teste", "Cancelamento administrativo");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.Aprovador);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new AprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisorUsuarioId = dados.Aprovador.Id
        }));

        Assert.Contains("cancelada", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task NaoDeveAprovarEtapaDeOutraInstancia()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "outra-etapa");
        var instancia1 = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Sequencial);
        var instancia2 = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Sequencial);
        var etapaDaOutra = await CriarEtapaAsync(context, instancia2, dados, ordem: 1, nivel: 1);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new AprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia1.Id,
            EtapaAprovacaoChamadoId = etapaDaOutra.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            DecisaoFinal = true
        }));

        Assert.Contains("nao pertence", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task NaoDeveCriarDecisaoFinalDuplicada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "duplicada");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Simples);
        var useCase = CriarUseCase(context, dados.Aprovador);

        await useCase.ExecutarAsync(new AprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            DecisaoFinal = true,
            LiberaAvanco = true
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new AprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            DecisaoFinal = true
        }));

        Assert.Contains("decisao final positiva", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(1, await context.DecisoesAprovacaoChamado.CountAsync());
    }

    [Fact]
    public async Task NaoDeveAprovarDiretamenteQuandoHaEtapaObrigatoriaPendente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "direta-bloqueada");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Sequencial);
        await CriarEtapaAsync(context, instancia, dados, ordem: 1, nivel: 1);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new AprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            DecisaoFinal = true
        }));

        Assert.Contains("etapa obrigatoria pendente", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AprovarInstanciaDeveLiberarBloqueioDoChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "integracao-liberacao");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Simples);
        
        var useCaseBloqueio = new ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context));

        // 1. Antes da aprovação: bloqueia
        var requestBloqueio = new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
        {
            ChamadoId = dados.Chamado.Id,
            TipoAcao = TipoAcaoMovimentacaoChamado.Encerrar
        };
        var bloqueioAntes = await useCaseBloqueio.ExecutarAsync(requestBloqueio);
        Assert.True(bloqueioAntes.Bloqueado);

        // 2. Executa aprovação
        var useCaseAprovacao = CriarUseCase(context, dados.Aprovador);
        await useCaseAprovacao.ExecutarAsync(new AprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Escopo aprovado e liberado.",
            LiberaAvanco = true
        });

        // 3. Depois da aprovação: não bloqueia mais
        var bloqueioDepois = await useCaseBloqueio.ExecutarAsync(requestBloqueio);
        Assert.False(bloqueioDepois.Bloqueado);
        Assert.True(bloqueioDepois.Permitido);
    }

    [Fact]
    public async Task DeveAceitarAprovacaoParaInstanciaDeGrupoAprovadorFuturo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "grupo-futuro");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Simples);

        // Ajusta a instância para GrupoAprovadorFuturo
        var prop = typeof(InstanciaAprovacaoChamado).GetProperty(nameof(InstanciaAprovacaoChamado.TipoResolucaoAprovador));
        prop?.SetValue(instancia, TipoResolucaoAprovadorRegraAprovacao.GrupoAprovadorFuturo);
        await context.SaveChangesAsync();

        var useCaseAprovacao = CriarUseCase(context, dados.Aprovador);
        var response = await useCaseAprovacao.ExecutarAsync(new AprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            DecisaoFinal = true,
            LiberaAvanco = true
        });

        Assert.True(response.Aprovada);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Aprovada, response.StatusInstanciaNovo);
        
        var decisao = await context.DecisoesAprovacaoChamado.SingleAsync();
        Assert.True(decisao.DecisorEhMembroGrupo);
        Assert.Null(decisao.GrupoAprovadorSnapshot); // O request não enviou snapshot pq não foi implementado grupo real ainda.
    }

    [Fact]
    public async Task DeveAprovarEtapaMultiNivelPreservandoNivelEOrdemERamo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "multinivel");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Sequencial);
        
        // Nivel 1, Ordem 1, Ramo "Tecnico"
        var etapa1 = await CriarEtapaAsync(context, instancia, dados, ordem: 1, nivel: 1, ramo: "Tecnico");
        
        // Nivel 2, Ordem 1, Ramo "Gerencial"
        var etapa2 = await CriarEtapaAsync(context, instancia, dados, ordem: 1, nivel: 2, ramo: "Gerencial");
        
        var useCase = CriarUseCase(context, dados.Aprovador);

        // Aprovar primeira etapa (Nível 1)
        var response = await useCase.ExecutarAsync(new AprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            EtapaAprovacaoChamadoId = etapa1.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Aprovado nivel 1",
            DecisaoParcial = true
        });

        Assert.Equal(StatusEtapaAprovacaoChamado.Aprovada, response.StatusEtapaNovo);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Pendente, response.StatusInstanciaNovo); // Tem nivel 2 pendente
        
        var decisao = await context.DecisoesAprovacaoChamado.SingleAsync();
        Assert.Equal(1, decisao.NivelEtapaSnapshot);
        Assert.Equal(1, decisao.OrdemEtapaSnapshot);
        Assert.Equal("Tecnico", decisao.RamoEtapaSnapshot);
        
        // Aprovar segunda etapa (Nível 2)
        var response2 = await useCase.ExecutarAsync(new AprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            EtapaAprovacaoChamadoId = etapa2.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Aprovado nivel 2",
            DecisaoFinal = true
        });

        Assert.Equal(StatusEtapaAprovacaoChamado.Aprovada, response2.StatusEtapaNovo);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Aprovada, response2.StatusInstanciaNovo); // Todas as obrigatórias fechadas

        var decisoes = await context.DecisoesAprovacaoChamado.OrderBy(x => x.CriadoEm).ToListAsync();
        Assert.Equal(2, decisoes.Count);
        
        var decisaoFinal = decisoes.Last();
        Assert.Equal(2, decisaoFinal.NivelEtapaSnapshot);
        Assert.Equal(1, decisaoFinal.OrdemEtapaSnapshot);
        Assert.Equal("Gerencial", decisaoFinal.RamoEtapaSnapshot);
    }

    private static AprovarAprovacaoChamadoUseCase CriarUseCase(SGXSistemaChamadoDbContext context, Usuario usuarioAtual)
        => new(
            PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<EtapaAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<DecisaoAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
                usuarioAtual.Id,
                usuarioAtual.Nome,
                usuarioAtual.Email,
                usuarioAtual.Login,
                ["Administrador"])),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Usuario Administrador, Usuario Solicitante, Usuario Aprovador, CategoriaChamado Categoria, Chamado Chamado)> CriarCenarioAsync(
        SGXSistemaChamadoDbContext context,
        string sufixo)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Admin {sufixo}", $"admin.apr.{sufixo}@sgx.local", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Solicitante {sufixo}", $"sol.apr.{sufixo}@sgx.local", TipoPerfil.Solicitante);
        var aprovador = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Aprovador {sufixo}", $"aprov.apr.{sufixo}@sgx.local", TipoPerfil.Administrador);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria aprovacao {sufixo}");
        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Nivel == PrioridadeChamadoEnum.Media);
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, prioridade.Id, sufixo);
        return (admin, solicitante, aprovador, categoria, chamado);
    }

    private static async Task<InstanciaAprovacaoChamado> CriarInstanciaAsync(
        SGXSistemaChamadoDbContext context,
        (Usuario Administrador, Usuario Solicitante, Usuario Aprovador, CategoriaChamado Categoria, Chamado Chamado) dados,
        TipoFluxoAprovacao fluxo)
    {
        var instancia = new InstanciaAprovacaoChamado(
            chamadoId: dados.Chamado.Id,
            solicitanteId: dados.Solicitante.Id,
            origem: OrigemInstanciaAprovacaoChamado.RegraMotor,
            tipoFluxoAprovacao: fluxo,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            tipoRegra: TipoRegraAprovacao.Combinada,
            exigeAprovacao: true,
            bloqueante: true,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico,
            criadoPorUsuarioId: dados.Administrador.Id,
            criadoPor: "teste",
            categoriaId: dados.Categoria.Id,
            impactoAvaliado: dados.Chamado.ImpactoChamado,
            urgenciaAvaliada: dados.Chamado.UrgenciaChamado,
            prioridadeAvaliada: PrioridadeChamadoEnum.Media,
            aprovadorEspecificoUsuarioId: dados.Aprovador.Id,
            regraNomeSnapshot: "Regra de aprovacao",
            regraVersaoSnapshot: 1,
            regraCriterioSnapshot: "Categoria e prioridade");

        context.InstanciasAprovacaoChamado.Add(instancia);
        await context.SaveChangesAsync();
        return instancia;
    }

    private static async Task<EtapaAprovacaoChamado> CriarEtapaAsync(
        SGXSistemaChamadoDbContext context,
        InstanciaAprovacaoChamado instancia,
        (Usuario Administrador, Usuario Solicitante, Usuario Aprovador, CategoriaChamado Categoria, Chamado Chamado) dados,
        int ordem,
        int nivel,
        string? ramo = null)
    {
        var etapa = new EtapaAprovacaoChamado(
            instanciaAprovacaoChamadoId: instancia.Id,
            solicitanteId: dados.Solicitante.Id,
            tipoEtapa: TipoEtapaAprovacaoChamado.Simples,
            tipoFluxoAprovacao: instancia.TipoFluxoAprovacao,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico,
            ordem: ordem,
            nivel: nivel,
            ramo: ramo,
            criadoPorUsuarioId: dados.Aprovador.Id,
            criadoPor: "teste",
            obrigatoria: true,
            criticaParaConsolidacao: true,
            aprovadorEspecificoUsuarioId: dados.Aprovador.Id,
            escopoResumoSnapshot: $"Etapa {ordem}",
            regraNomeSnapshot: instancia.RegraNomeSnapshot,
            regraVersaoSnapshot: instancia.RegraVersaoSnapshot,
            regraCriterioSnapshot: instancia.RegraCriterioSnapshot);

        context.EtapasAprovacaoChamado.Add(etapa);
        await context.SaveChangesAsync();
        return etapa;
    }
}
