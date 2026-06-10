using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisUseCaseTests
{
    [Fact]
    public async Task NaoDeveReavaliarQuandoNaoHaMudancaSensivel()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "sem-mudanca");
        var regra = await CriarRegraAsync(context, dados, "Regra base", dados.CatalogoOriginal.Id);
        var instancia = await CriarInstanciaAsync(context, dados, regra, StatusInstanciaAprovacaoChamado.Aprovada);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var statusChamadoAntes = dados.Chamado.StatusId;

        var response = await useCase.ExecutarAsync(new ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest
        {
            ChamadoId = dados.Chamado.Id,
            InstanciaAprovacaoChamadoId = instancia.Id,
            UsuarioId = dados.Aprovador.Id,
            NaturezaAnterior = NaturezaChamadoEnum.Requisicao,
            NaturezaNova = NaturezaChamadoEnum.Requisicao,
            TipoSolicitacaoAnteriorId = dados.TipoSolicitacaoOriginal.Id,
            TipoSolicitacaoNovoId = dados.TipoSolicitacaoOriginal.Id,
            CatalogoServicoAnteriorId = dados.CatalogoOriginal.Id,
            CatalogoServicoNovoId = dados.CatalogoOriginal.Id,
            CategoriaAnteriorId = dados.Categoria.Id,
            CategoriaNovaId = dados.Categoria.Id,
            SubcategoriaAnteriorId = dados.SubcategoriaOriginal.Id,
            SubcategoriaNovaId = dados.SubcategoriaOriginal.Id,
            ImpactoAnterior = ImpactoChamadoEnum.Medio,
            ImpactoNovo = ImpactoChamadoEnum.Medio,
            UrgenciaAnterior = UrgenciaChamadoEnum.Media,
            UrgenciaNova = UrgenciaChamadoEnum.Media,
            PrioridadeAnterior = PrioridadeChamadoEnum.Media,
            PrioridadeNova = PrioridadeChamadoEnum.Media,
            CustoAnterior = 1000m,
            CustoNovo = 1000m,
            NivelRiscoAnterior = 2,
            NivelRiscoNovo = 2,
            EscopoAnteriorSnapshot = "Escopo original",
            EscopoNovoSnapshot = "Escopo original",
            Motivo = "Nenhuma alteracao sensivel real."
        });

        var chamadoPersistido = await context.Chamados.AsNoTracking().SingleAsync(x => x.Id == dados.Chamado.Id);

        Assert.False(response.ReavaliacaoExecutada);
        Assert.False(response.ReavaliacaoNecessaria);
        Assert.Empty(response.MudancasSensiveisDetectadas);
        Assert.Equal(statusChamadoAntes, chamadoPersistido.StatusId);
        Assert.Empty(await context.DecisoesAprovacaoChamado.AsNoTracking().ToListAsync());
    }

    [Fact]
    public async Task DeveReavaliarQuandoCatalogoSensivelMuda()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "catalogo-sensivel");
        var regraOriginal = await CriarRegraAsync(context, dados, "Regra servico original", dados.CatalogoOriginal.Id);
        var regraSensivel = await CriarRegraAsync(context, dados, "Regra servico sensivel", dados.CatalogoNovoSensivel.Id);
        var instancia = await CriarInstanciaAsync(context, dados, regraOriginal, StatusInstanciaAprovacaoChamado.Aprovada);
        await CriarEtapaAsync(context, instancia, dados, StatusEtapaAprovacaoChamado.Aprovada);
        await CriarDecisaoAprovacaoAnteriorAsync(context, instancia, dados.Aprovador);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var response = await useCase.ExecutarAsync(new ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest
        {
            ChamadoId = dados.Chamado.Id,
            InstanciaAprovacaoChamadoId = instancia.Id,
            UsuarioId = dados.Aprovador.Id,
            NaturezaAnterior = NaturezaChamadoEnum.Requisicao,
            NaturezaNova = NaturezaChamadoEnum.Requisicao,
            TipoSolicitacaoAnteriorId = dados.TipoSolicitacaoOriginal.Id,
            TipoSolicitacaoNovoId = dados.TipoSolicitacaoOriginal.Id,
            CatalogoServicoAnteriorId = dados.CatalogoOriginal.Id,
            CatalogoServicoNovoId = dados.CatalogoNovoSensivel.Id,
            CategoriaAnteriorId = dados.Categoria.Id,
            CategoriaNovaId = dados.Categoria.Id,
            SubcategoriaAnteriorId = dados.SubcategoriaOriginal.Id,
            SubcategoriaNovaId = dados.SubcategoriaOriginal.Id,
            ImpactoAnterior = ImpactoChamadoEnum.Medio,
            ImpactoNovo = ImpactoChamadoEnum.Medio,
            UrgenciaAnterior = UrgenciaChamadoEnum.Media,
            UrgenciaNova = UrgenciaChamadoEnum.Media,
            PrioridadeAnterior = PrioridadeChamadoEnum.Media,
            PrioridadeNova = PrioridadeChamadoEnum.Media,
            CustoAnterior = 1000m,
            CustoNovo = 1000m,
            NivelRiscoAnterior = 2,
            NivelRiscoNovo = 2,
            EscopoAnteriorSnapshot = "Servico comum",
            EscopoNovoSnapshot = "Servico sensivel",
            Motivo = "Troca para servico sensivel fora do escopo aprovado."
        });

        var instanciaPersistida = await context.InstanciasAprovacaoChamado.AsNoTracking().SingleAsync(x => x.Id == instancia.Id);
        var etapaPersistida = await context.EtapasAprovacaoChamado.AsNoTracking().SingleAsync(x => x.InstanciaAprovacaoChamadoId == instancia.Id);
        var decisoes = await context.DecisoesAprovacaoChamado.AsNoTracking().OrderBy(x => x.CriadoEm).ToListAsync();
        var decisaoReavaliacao = Assert.Single(decisoes, x => x.TipoDecisao == TipoDecisaoAprovacaoChamado.Reavaliacao);

        Assert.NotNull(regraSensivel);
        Assert.True(response.ReavaliacaoExecutada);
        Assert.True(response.ReavaliacaoNecessaria);
        Assert.True(response.ExigeNovaAprovacao);
        Assert.False(response.PermiteContinuar);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Aprovada, response.StatusInstanciaAnterior);
        Assert.Equal(StatusInstanciaAprovacaoChamado.EmReavaliacao, response.StatusInstanciaNovo);
        Assert.Equal(StatusInstanciaAprovacaoChamado.EmReavaliacao, instanciaPersistida.Status);
        Assert.Equal(StatusEtapaAprovacaoChamado.EmReavaliacao, etapaPersistida.Status);
        Assert.Equal(ResultadoDecisaoAprovacaoChamado.RequerNovaAprovacao, decisaoReavaliacao.Resultado);
        Assert.Equal(2, decisoes.Count);
    }

    [Fact]
    public async Task DeveReavaliarQuandoImpactoAumentaAcimaDoAprovado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "impacto-aumenta");
        var regra = await CriarRegraAsync(context, dados, "Regra impacto", dados.CatalogoOriginal.Id);
        var instancia = await CriarInstanciaAsync(context, dados, regra, StatusInstanciaAprovacaoChamado.Aprovada);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var response = await useCase.ExecutarAsync(new ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest
        {
            ChamadoId = dados.Chamado.Id,
            InstanciaAprovacaoChamadoId = instancia.Id,
            UsuarioId = dados.Aprovador.Id,
            NaturezaAnterior = NaturezaChamadoEnum.Requisicao,
            NaturezaNova = NaturezaChamadoEnum.Requisicao,
            TipoSolicitacaoAnteriorId = dados.TipoSolicitacaoOriginal.Id,
            TipoSolicitacaoNovoId = dados.TipoSolicitacaoOriginal.Id,
            CatalogoServicoAnteriorId = dados.CatalogoOriginal.Id,
            CatalogoServicoNovoId = dados.CatalogoOriginal.Id,
            CategoriaAnteriorId = dados.Categoria.Id,
            CategoriaNovaId = dados.Categoria.Id,
            SubcategoriaAnteriorId = dados.SubcategoriaOriginal.Id,
            SubcategoriaNovaId = dados.SubcategoriaOriginal.Id,
            ImpactoAnterior = ImpactoChamadoEnum.Medio,
            ImpactoNovo = ImpactoChamadoEnum.Alto,
            UrgenciaAnterior = UrgenciaChamadoEnum.Media,
            UrgenciaNova = UrgenciaChamadoEnum.Media,
            PrioridadeAnterior = PrioridadeChamadoEnum.Media,
            PrioridadeNova = PrioridadeChamadoEnum.Media,
            CustoAnterior = 1000m,
            CustoNovo = 1000m,
            NivelRiscoAnterior = 2,
            NivelRiscoNovo = 2,
            Motivo = "Impacto operacional acima do escopo aprovado."
        });

        Assert.True(response.ReavaliacaoExecutada);
        Assert.Contains("ImpactoChamado", response.MudancasSensiveisDetectadas);
    }

    [Fact]
    public async Task DeveManterAprovacaoValidaQuandoNovoContextoContinuaCoberto()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "contexto-coberto");
        var regra = await CriarRegraAsync(context, dados, "Regra custo", dados.CatalogoOriginal.Id);
        var instancia = await CriarInstanciaAsync(context, dados, regra, StatusInstanciaAprovacaoChamado.Aprovada, custoAvaliado: 1500m, nivelRisco: 3);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var response = await useCase.ExecutarAsync(new ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest
        {
            ChamadoId = dados.Chamado.Id,
            InstanciaAprovacaoChamadoId = instancia.Id,
            UsuarioId = dados.Aprovador.Id,
            NaturezaAnterior = NaturezaChamadoEnum.Requisicao,
            NaturezaNova = NaturezaChamadoEnum.Requisicao,
            TipoSolicitacaoAnteriorId = dados.TipoSolicitacaoOriginal.Id,
            TipoSolicitacaoNovoId = dados.TipoSolicitacaoOriginal.Id,
            CatalogoServicoAnteriorId = dados.CatalogoOriginal.Id,
            CatalogoServicoNovoId = dados.CatalogoOriginal.Id,
            CategoriaAnteriorId = dados.Categoria.Id,
            CategoriaNovaId = dados.Categoria.Id,
            SubcategoriaAnteriorId = dados.SubcategoriaOriginal.Id,
            SubcategoriaNovaId = dados.SubcategoriaOriginal.Id,
            ImpactoAnterior = ImpactoChamadoEnum.Medio,
            ImpactoNovo = ImpactoChamadoEnum.Medio,
            UrgenciaAnterior = UrgenciaChamadoEnum.Media,
            UrgenciaNova = UrgenciaChamadoEnum.Media,
            PrioridadeAnterior = PrioridadeChamadoEnum.Media,
            PrioridadeNova = PrioridadeChamadoEnum.Media,
            CustoAnterior = 1500m,
            CustoNovo = 1200m,
            NivelRiscoAnterior = 3,
            NivelRiscoNovo = 2,
            Motivo = "Reducao de sensibilidade ainda coberta pelo escopo aprovado."
        });

        var instanciaPersistida = await context.InstanciasAprovacaoChamado.AsNoTracking().SingleAsync(x => x.Id == instancia.Id);

        Assert.False(response.ReavaliacaoExecutada);
        Assert.False(response.ReavaliacaoNecessaria);
        Assert.True(response.PermiteContinuar);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Aprovada, instanciaPersistida.Status);
        Assert.Empty(await context.DecisoesAprovacaoChamado.AsNoTracking().ToListAsync());
    }

    [Fact]
    public async Task NaoDeveReavaliarInstanciaReprovadaMasPodeSinalizarNovaAprovacaoFutura()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "instancia-reprovada");
        var regraOriginal = await CriarRegraAsync(context, dados, "Regra original", dados.CatalogoOriginal.Id);
        await CriarRegraAsync(context, dados, "Regra nova", dados.CatalogoNovoSensivel.Id);
        var instancia = await CriarInstanciaAsync(context, dados, regraOriginal, StatusInstanciaAprovacaoChamado.Reprovada);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var response = await useCase.ExecutarAsync(new ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest
        {
            ChamadoId = dados.Chamado.Id,
            InstanciaAprovacaoChamadoId = instancia.Id,
            UsuarioId = dados.Aprovador.Id,
            NaturezaAnterior = NaturezaChamadoEnum.Requisicao,
            NaturezaNova = NaturezaChamadoEnum.Requisicao,
            TipoSolicitacaoAnteriorId = dados.TipoSolicitacaoOriginal.Id,
            TipoSolicitacaoNovoId = dados.TipoSolicitacaoOriginal.Id,
            CatalogoServicoAnteriorId = dados.CatalogoOriginal.Id,
            CatalogoServicoNovoId = dados.CatalogoNovoSensivel.Id,
            CategoriaAnteriorId = dados.Categoria.Id,
            CategoriaNovaId = dados.Categoria.Id,
            SubcategoriaAnteriorId = dados.SubcategoriaOriginal.Id,
            SubcategoriaNovaId = dados.SubcategoriaOriginal.Id,
            ImpactoAnterior = ImpactoChamadoEnum.Medio,
            ImpactoNovo = ImpactoChamadoEnum.Medio,
            UrgenciaAnterior = UrgenciaChamadoEnum.Media,
            UrgenciaNova = UrgenciaChamadoEnum.Media,
            PrioridadeAnterior = PrioridadeChamadoEnum.Media,
            PrioridadeNova = PrioridadeChamadoEnum.Media,
            CustoAnterior = 1000m,
            CustoNovo = 1000m,
            NivelRiscoAnterior = 2,
            NivelRiscoNovo = 2,
            Motivo = "Instancia antiga reprovada nao deve ser reaberta."
        });

        Assert.False(response.ReavaliacaoExecutada);
        Assert.True(response.ExigeNovaAprovacao);
        Assert.False(response.PermiteContinuar);
    }

    [Fact]
    public async Task DeveRetornarConsultivoQuandoNaoExisteInstanciaRelacionada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "sem-instancia");
        await CriarRegraAsync(context, dados, "Regra nova", dados.CatalogoNovoSensivel.Id);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var response = await useCase.ExecutarAsync(new ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest
        {
            ChamadoId = dados.Chamado.Id,
            UsuarioId = dados.Aprovador.Id,
            NaturezaAnterior = NaturezaChamadoEnum.Requisicao,
            NaturezaNova = NaturezaChamadoEnum.Requisicao,
            TipoSolicitacaoAnteriorId = dados.TipoSolicitacaoOriginal.Id,
            TipoSolicitacaoNovoId = dados.TipoSolicitacaoOriginal.Id,
            CatalogoServicoAnteriorId = dados.CatalogoOriginal.Id,
            CatalogoServicoNovoId = dados.CatalogoNovoSensivel.Id,
            CategoriaAnteriorId = dados.Categoria.Id,
            CategoriaNovaId = dados.Categoria.Id,
            SubcategoriaAnteriorId = dados.SubcategoriaOriginal.Id,
            SubcategoriaNovaId = dados.SubcategoriaOriginal.Id,
            ImpactoAnterior = ImpactoChamadoEnum.Medio,
            ImpactoNovo = ImpactoChamadoEnum.Medio,
            UrgenciaAnterior = UrgenciaChamadoEnum.Media,
            UrgenciaNova = UrgenciaChamadoEnum.Media,
            PrioridadeAnterior = PrioridadeChamadoEnum.Media,
            PrioridadeNova = PrioridadeChamadoEnum.Media,
            CustoAnterior = 1000m,
            CustoNovo = 1000m,
            NivelRiscoAnterior = 2,
            NivelRiscoNovo = 2,
            Motivo = "Ainda nao ha instancia para reavaliar."
        });

        Assert.False(response.ReavaliacaoExecutada);
        Assert.True(response.ExigeNovaAprovacao);
        Assert.Null(response.InstanciaAprovacaoChamadoId);
    }

    private static ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisUseCase CriarUseCase(SGXSistemaChamadoDbContext context, Usuario usuarioAtual)
    {
        var usuarioContexto = new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            usuarioAtual.Id,
            usuarioAtual.Nome,
            usuarioAtual.Email,
            usuarioAtual.Login,
            ["Administrador"]));

        var configuracaoUseCases = new ConfiguracaoRegraAprovacaoAdminUseCases(
            PortalUseCasesTestFactory.Repo<ConfiguracaoRegraAprovacao>(context),
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            PortalUseCasesTestFactory.Repo<CatalogoServico>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            usuarioContexto,
            PortalUseCasesTestFactory.Uow(context));

        return new ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisUseCase(
            PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<EtapaAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<DecisaoAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            configuracaoUseCases,
            usuarioContexto,
            PortalUseCasesTestFactory.Uow(context));
    }

    private static async Task<(Usuario Administrador, Usuario Solicitante, Usuario Aprovador, CategoriaChamado Categoria, SubcategoriaChamado SubcategoriaOriginal, TipoSolicitacao TipoSolicitacaoOriginal, CatalogoServico CatalogoOriginal, CatalogoServico CatalogoNovoSensivel, Chamado Chamado)> CriarCenarioAsync(
        SGXSistemaChamadoDbContext context,
        string sufixo)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Admin {sufixo}", $"admin.reaval.{sufixo}@sgx.local", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Solicitante {sufixo}", $"sol.reaval.{sufixo}@sgx.local", TipoPerfil.Solicitante);
        var aprovador = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Aprovador {sufixo}", $"aprov.reaval.{sufixo}@sgx.local", TipoPerfil.Administrador);
        var departamento = context.Departamentos.FirstOrDefault();
        if (departamento is null)
        {
            departamento = new Departamento($"Departamento {sufixo}", $"DP{sufixo[..Math.Min(2, sufixo.Length)].ToUpperInvariant()}", null, "teste");
            context.Departamentos.Add(departamento);
            await context.SaveChangesAsync();
        }
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria reavaliacao {sufixo}", departamento.Id);
        var subcategoria = new SubcategoriaChamado(categoria.Id, $"Subcategoria {sufixo}", null, "teste");
        context.SubcategoriasChamado.Add(subcategoria);
        var tipoSolicitacao = new TipoSolicitacao($"Tipo {sufixo}", null, "teste");
        context.TiposSolicitacao.Add(tipoSolicitacao);
        await context.SaveChangesAsync();

        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Nivel == PrioridadeChamadoEnum.Media);
        var catalogoOriginal = new CatalogoServico(
            $"Servico original {sufixo}",
            $"servico-original-{sufixo}",
            null,
            null,
            departamento.Id,
            categoria.Id,
            subcategoria.Id,
            prioridade.Id,
            null,
            null,
            VisibilidadeCatalogoServico.Interno,
            true,
            false,
            1,
            admin.Id,
            "teste");
        catalogoOriginal.Publicar(admin.Id, "teste");

        var catalogoNovoSensivel = new CatalogoServico(
            $"Servico sensivel {sufixo}",
            $"servico-sensivel-{sufixo}",
            null,
            null,
            departamento.Id,
            categoria.Id,
            subcategoria.Id,
            prioridade.Id,
            null,
            null,
            VisibilidadeCatalogoServico.Interno,
            true,
            true,
            2,
            admin.Id,
            "teste");
        catalogoNovoSensivel.Publicar(admin.Id, "teste");

        context.CatalogosServico.AddRange(catalogoOriginal, catalogoNovoSensivel);
        await context.SaveChangesAsync();

        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            solicitante,
            categoria,
            StatusChamadoEnum.EmAtendimento,
            prioridade.Id,
            sufixo,
            "teste",
            subcategoria.Id,
            tipoSolicitacao.Id,
            null,
            NaturezaChamadoEnum.Requisicao);

        return (admin, solicitante, aprovador, categoria, subcategoria, tipoSolicitacao, catalogoOriginal, catalogoNovoSensivel, chamado);
    }

    private static async Task<ConfiguracaoRegraAprovacao> CriarRegraAsync(
        SGXSistemaChamadoDbContext context,
        (Usuario Administrador, Usuario Solicitante, Usuario Aprovador, CategoriaChamado Categoria, SubcategoriaChamado SubcategoriaOriginal, TipoSolicitacao TipoSolicitacaoOriginal, CatalogoServico CatalogoOriginal, CatalogoServico CatalogoNovoSensivel, Chamado Chamado) dados,
        string nome,
        Guid catalogoServicoId)
    {
        var regra = new ConfiguracaoRegraAprovacao(
            nome,
            TipoRegraAprovacao.Combinada,
            EscopoRegraAprovacao.AtendimentoChamado,
            EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            TipoFluxoAprovacao.Simples,
            TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico,
            1,
            catalogoServicoId == dados.CatalogoNovoSensivel.Id ? 20 : 10,
            1,
            dados.Administrador.Id,
            "teste",
            "Regra de aprovacao para reavaliacao",
            NaturezaChamadoEnum.Requisicao,
            dados.TipoSolicitacaoOriginal.Id,
            catalogoServicoId,
            dados.Categoria.Id,
            dados.SubcategoriaOriginal.Id,
            ImpactoChamadoEnum.Medio,
            UrgenciaChamadoEnum.Media,
            PrioridadeChamadoEnum.Media,
            null,
            null,
            true,
            true,
            false,
            false,
            dados.Aprovador.Id,
            null,
            24,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(30));

        context.ConfiguracoesRegrasAprovacao.Add(regra);
        await context.SaveChangesAsync();
        return regra;
    }

    private static async Task<InstanciaAprovacaoChamado> CriarInstanciaAsync(
        SGXSistemaChamadoDbContext context,
        (Usuario Administrador, Usuario Solicitante, Usuario Aprovador, CategoriaChamado Categoria, SubcategoriaChamado SubcategoriaOriginal, TipoSolicitacao TipoSolicitacaoOriginal, CatalogoServico CatalogoOriginal, CatalogoServico CatalogoNovoSensivel, Chamado Chamado) dados,
        ConfiguracaoRegraAprovacao regra,
        StatusInstanciaAprovacaoChamado status,
        decimal? custoAvaliado = 1000m,
        int? nivelRisco = 2)
    {
        var instancia = new InstanciaAprovacaoChamado(
            chamadoId: dados.Chamado.Id,
            solicitanteId: dados.Solicitante.Id,
            origem: OrigemInstanciaAprovacaoChamado.RegraMotor,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            tipoRegra: TipoRegraAprovacao.Combinada,
            exigeAprovacao: true,
            bloqueante: true,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico,
            criadoPorUsuarioId: dados.Administrador.Id,
            criadoPor: "teste",
            configuracaoRegraAprovacaoId: regra.Id,
            naturezaChamado: NaturezaChamadoEnum.Requisicao,
            tipoSolicitacaoId: dados.TipoSolicitacaoOriginal.Id,
            catalogoServicoId: dados.CatalogoOriginal.Id,
            categoriaId: dados.Categoria.Id,
            subcategoriaId: dados.SubcategoriaOriginal.Id,
            impactoAvaliado: ImpactoChamadoEnum.Medio,
            urgenciaAvaliada: UrgenciaChamadoEnum.Media,
            prioridadeAvaliada: PrioridadeChamadoEnum.Media,
            custoAvaliado: custoAvaliado,
            nivelRiscoAvaliado: nivelRisco,
            aprovadorEspecificoUsuarioId: dados.Aprovador.Id,
            aprovadorResolvidoUsuarioId: dados.Aprovador.Id,
            regraNomeSnapshot: regra.Nome,
            regraVersaoSnapshot: regra.Versao,
            regraCriterioSnapshot: regra.Descricao);

        context.InstanciasAprovacaoChamado.Add(instancia);
        await context.SaveChangesAsync();

        if (status == StatusInstanciaAprovacaoChamado.Aprovada)
        {
            instancia.RegistrarDecisaoResumo(StatusInstanciaAprovacaoChamado.Aprovada, dados.Aprovador.Id, dados.Administrador.Id, "teste");
        }
        else if (status == StatusInstanciaAprovacaoChamado.Reprovada)
        {
            instancia.RegistrarDecisaoResumo(StatusInstanciaAprovacaoChamado.Reprovada, dados.Aprovador.Id, dados.Administrador.Id, "teste");
        }

        await context.SaveChangesAsync();
        return instancia;
    }

    private static async Task CriarEtapaAsync(
        SGXSistemaChamadoDbContext context,
        InstanciaAprovacaoChamado instancia,
        (Usuario Administrador, Usuario Solicitante, Usuario Aprovador, CategoriaChamado Categoria, SubcategoriaChamado SubcategoriaOriginal, TipoSolicitacao TipoSolicitacaoOriginal, CatalogoServico CatalogoOriginal, CatalogoServico CatalogoNovoSensivel, Chamado Chamado) dados,
        StatusEtapaAprovacaoChamado status)
    {
        var etapa = new EtapaAprovacaoChamado(
            instanciaAprovacaoChamadoId: instancia.Id,
            solicitanteId: dados.Solicitante.Id,
            tipoEtapa: TipoEtapaAprovacaoChamado.Simples,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico,
            ordem: 1,
            nivel: 1,
            criadoPorUsuarioId: dados.Administrador.Id,
            criadoPor: "teste",
            obrigatoria: true,
            criticaParaConsolidacao: true,
            aprovadorEspecificoUsuarioId: dados.Aprovador.Id,
            aprovadorResolvidoUsuarioId: dados.Aprovador.Id,
            escopoResumoSnapshot: "Etapa aprovada",
            regraNomeSnapshot: instancia.RegraNomeSnapshot,
            regraVersaoSnapshot: instancia.RegraVersaoSnapshot,
            regraCriterioSnapshot: instancia.RegraCriterioSnapshot);

        context.EtapasAprovacaoChamado.Add(etapa);
        await context.SaveChangesAsync();

        if (status == StatusEtapaAprovacaoChamado.Aprovada)
        {
            etapa.RegistrarDecisaoResumo(StatusEtapaAprovacaoChamado.Aprovada, dados.Aprovador.Id, dados.Administrador.Id, "teste");
            await context.SaveChangesAsync();
        }
    }

    private static async Task CriarDecisaoAprovacaoAnteriorAsync(
        SGXSistemaChamadoDbContext context,
        InstanciaAprovacaoChamado instancia,
        Usuario aprovador)
    {
        var decisao = new DecisaoAprovacaoChamado(
            instanciaAprovacaoChamadoId: instancia.Id,
            etapaAprovacaoChamadoId: null,
            tipoDecisao: TipoDecisaoAprovacaoChamado.Aprovacao,
            resultado: ResultadoDecisaoAprovacaoChamado.Aprovada,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            statusInstanciaAnterior: StatusInstanciaAprovacaoChamado.Pendente,
            statusInstanciaNovo: StatusInstanciaAprovacaoChamado.Aprovada,
            criadoPorUsuarioId: aprovador.Id,
            criadoPor: aprovador.Login,
            decisorUsuarioId: aprovador.Id,
            justificativa: "Aprovacao anterior registrada.",
            escopoDecididoSnapshot: "Escopo original",
            decisaoParcial: false,
            decisaoFinal: true,
            liberaAvanco: false,
            mantemBloqueio: true,
            exigeReavaliacao: false,
            permiteNovaSolicitacao: false,
            cancelaFluxo: false,
            regraNomeSnapshot: instancia.RegraNomeSnapshot,
            regraVersaoSnapshot: instancia.RegraVersaoSnapshot,
            regraCriterioSnapshot: instancia.RegraCriterioSnapshot);

        context.DecisoesAprovacaoChamado.Add(decisao);
        await context.SaveChangesAsync();
    }
}
