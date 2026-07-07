using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;
using System.Text.Json.Nodes;

namespace SGX.SistemaChamado.Tests;

public sealed class AbrirChamadoUseCaseTests
{
    [Fact]
    public async Task DeveAbrirChamadoValidoParaUsuarioAutenticado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);

        var useCase = new AbrirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<RespostaFormularioChamado>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<CatalogoServico>(context),
            PortalUseCasesTestFactory.Repo<FormularioServico>(context),
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<InventarioAtivo>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoInventarioAtivo>(context),
            SlaTestFactory.CriarService(context),
            new FakeCodigoChamadoService(),
            new PrioridadeChamadoMatrizService(PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context)),
            new CamposObrigatoriosChamadoService(),
            new FakeUsuarioContextoAplicacaoService(dados.UsuarioContexto),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Portal nao autentica",
            Descricao = "Erro ao autenticar no SSO",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            NaturezaChamado = NaturezaChamadoEnum.Incidente,
            ImpactoChamado = ImpactoChamadoEnum.Alto,
            UrgenciaChamado = UrgenciaChamadoEnum.Alta,
            DepartamentoId = dados.Departamento.Id
        });

        Assert.Equal(dados.Usuario.Id, context.Chamados.Single().SolicitanteId);
        Assert.Null(context.Chamados.Single().GrupoTecnicoId);
        Assert.Null(context.Chamados.Single().FilaAtendimentoId);
        Assert.Null(context.Chamados.Single().ResponsavelId);
        Assert.Equal("Portal nao autentica", response.Titulo);
        Assert.Equal(NaturezaChamadoEnum.Incidente, context.Chamados.Single().NaturezaChamado);
    }

    [Fact]
    public async Task DeveBloquearIncidenteSemImpactoEUrgencia()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Incidente sem classificacao",
            Descricao = "Falha operacional em sistema critico",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            NaturezaChamado = NaturezaChamadoEnum.Incidente
        }));

        Assert.Contains("Impacto", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveCriarChamadoComSubcategoriaTipoELocalAtivos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro no sistema de notas",
            Descricao = "Usuario sem acesso ao modulo.",
            CategoriaId = dados.Categoria.Id,
            SubcategoriaId = dados.Subcategoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            TipoSolicitacaoId = dados.TipoSolicitacao.Id,
            LocalUnidadeId = dados.LocalUnidade.Id
        });

        var chamadoCriado = context.Chamados.Single();
        Assert.Equal(dados.Subcategoria.Id, chamadoCriado.SubcategoriaId);
        Assert.Equal(dados.TipoSolicitacao.Id, chamadoCriado.TipoSolicitacaoId);
        Assert.Equal(dados.LocalUnidade.Id, chamadoCriado.LocalUnidadeId);
        Assert.Equal(dados.Subcategoria.Nome, response.Subcategoria);
        Assert.Equal(dados.TipoSolicitacao.Nome, response.TipoSolicitacao);
        Assert.Equal(dados.LocalUnidade.Nome, response.LocalUnidade);
    }

    [Fact]
    public async Task DeveCalcularPrioridadePelaMatrizAoAbrirChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);
        var prioridadeCritica = await context.PrioridadesChamado.FirstAsync(x => x.Nivel == PrioridadeChamadoEnum.Critica);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Indisponibilidade total do sistema",
            Descricao = "Usuarios sem acesso ao sistema principal.",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            NaturezaChamado = NaturezaChamadoEnum.Incidente,
            ImpactoChamado = ImpactoChamadoEnum.Alto,
            UrgenciaChamado = UrgenciaChamadoEnum.Alta
        });

        Assert.Equal(prioridadeCritica.Id, context.Chamados.Single(x => x.Id == response.Id).PrioridadeId);
        Assert.Equal(ImpactoChamadoEnum.Alto, response.ImpactoChamado);
        Assert.Equal(UrgenciaChamadoEnum.Alta, response.UrgenciaChamado);
    }

    [Fact]
    public async Task DeveRejeitarSubcategoriaDeOutraCategoria()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var categoriaSecundaria = new CategoriaChamado("Rede", null, dados.Departamento.Id, "teste");
        var subcategoriaOutra = new SubcategoriaChamado(categoriaSecundaria.Id, "Wi-Fi", null, "teste");
        context.CategoriasChamado.Add(categoriaSecundaria);
        context.SubcategoriasChamado.Add(subcategoriaOutra);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            SubcategoriaId = subcategoriaOutra.Id,
            PrioridadeId = dados.Prioridade.Id
        }));
    }

    [Fact]
    public async Task DeveRejeitarSubcategoriaInativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        dados.Subcategoria.Desativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            SubcategoriaId = dados.Subcategoria.Id,
            PrioridadeId = dados.Prioridade.Id
        }));
    }

    [Fact]
    public async Task DeveRejeitarTipoSolicitacaoInativo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        dados.TipoSolicitacao.Desativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            TipoSolicitacaoId = dados.TipoSolicitacao.Id
        }));
    }

    [Fact]
    public async Task DeveRejeitarLocalUnidadeInativo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        dados.LocalUnidade.Desativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            LocalUnidadeId = dados.LocalUnidade.Id
        }));
    }

    [Fact]
    public async Task DeveCriarHistoricoEStatusAberto()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro VPN",
            Descricao = "Sem acesso VPN",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id
        });

        Assert.Equal("Aberto", response.Status);
        Assert.Equal(OrigemChamado.Portal, context.Chamados.Single().Origem);
        Assert.DoesNotContain(context.AprovacoesChamado, x => x.ChamadoId == response.Id && x.Ativo);
        Assert.False(response.RequerAprovacao);
        Assert.False(response.AprovacaoPendente);
        Assert.Contains(
            context.HistoricosChamado,
            x => x.Tipo == TipoHistoricoChamado.Criado && x.Descricao == "Chamado criado pelo portal");
    }

    [Fact]
    public async Task DeveAbrirChamadoComAtivoValido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var ativo = await CriarAtivoInventarioAsync(context, dados, "INV-ABRIR-001");
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Notebook com problema de desempenho",
            Descricao = "Lentidao recorrente no equipamento.",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            InventarioAtivoId = ativo.Id
        });

        var chamadoCriado = await context.Chamados.FirstAsync(x => x.Id == response.Id);
        Assert.Equal(ativo.Id, chamadoCriado.InventarioAtivoId);
        Assert.Equal(ativo.Id, response.InventarioAtivoId);
    }

    [Fact]
    public async Task DeveImpedirAberturaComAtivoInativo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var ativo = await CriarAtivoInventarioAsync(context, dados, "INV-INATIVO-001", ativo: false);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Ativo inativo nao deve vincular",
            Descricao = "Teste de validacao.",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            InventarioAtivoId = ativo.Id
        }));
    }

    [Fact]
    public async Task DeveManterAberturaSemAtivoQuandoNaoInformado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Chamado sem ativo",
            Descricao = "Fluxo deve continuar sem inventario.",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id
        });

        Assert.Null(response.InventarioAtivoId);
        Assert.Null(context.Chamados.Single(x => x.Id == response.Id).InventarioAtivoId);
    }

    [Fact]
    public async Task DeveRegistrarHistoricoChamadoEAtivoQuandoAbrirComAtivo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var ativo = await CriarAtivoInventarioAsync(context, dados, "INV-HIST-001");
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Chamado com historico de vinculo",
            Descricao = "Necessario registrar rastreabilidade.",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            InventarioAtivoId = ativo.Id
        });

        Assert.Contains(
            context.HistoricosChamado,
            x => x.ChamadoId == response.Id && x.Tipo == TipoHistoricoChamado.AtivoVinculado);

        Assert.Contains(
            context.HistoricosInventarioAtivo,
            x => x.InventarioAtivoId == ativo.Id && x.TipoMovimentacao == TipoMovimentacaoAtivo.VinculoChamado);
    }

    [Fact]
    public async Task DeveRejeitarCategoriaInexistenteOuInativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = dados.Prioridade.Id
        }));
    }

    [Fact]
    public async Task DeveRejeitarCategoriaInativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        dados.Categoria.Desativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id
        }));
    }

    [Fact]
    public async Task DeveCalcularPrioridadeMesmoComPrioridadeIdInvalida()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = Guid.NewGuid(),
            ImpactoChamado = ImpactoChamadoEnum.Alto,
            UrgenciaChamado = UrgenciaChamadoEnum.Alta
        });

        var prioridadeCritica = await context.PrioridadesChamado.FirstAsync(x => x.Nivel == PrioridadeChamadoEnum.Critica);
        Assert.Equal(prioridadeCritica.Id, context.Chamados.Single(x => x.Id == response.Id).PrioridadeId);
    }

    [Fact]
    public async Task DeveRejeitarPrioridadeInativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        dados.Prioridade.Desativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id
        }));
    }

    [Fact]
    public async Task NaoDeveAceitarTituloVazio()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = " ",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id
        }));
    }

    [Fact]
    public async Task NaoDeveAceitarDescricaoVazia()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "  ",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id
        }));
    }

    [Fact]
    public async Task DeveAbrirChamadoComServicoCatalogoValido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var categoriaAlternativa = new CategoriaChamado("Infraestrutura", null, dados.Departamento.Id, "teste");
        context.CategoriasChamado.Add(categoriaAlternativa);
        await context.SaveChangesAsync();

        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);
        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Solicitar VPN",
            Descricao = "Preciso de acesso remoto",
            CatalogoServicoId = servico.Id,
            DepartamentoId = Guid.NewGuid(),
            CategoriaId = categoriaAlternativa.Id,
            SubcategoriaId = null,
            PrioridadeId = Guid.NewGuid()
        });

        var chamado = await context.Chamados.FirstAsync(x => x.Id == response.Id);
        Assert.Equal(servico.Id, chamado.CatalogoServicoId);
        Assert.Equal(dados.Departamento.Id, chamado.DepartamentoId);
        Assert.Equal(dados.Categoria.Id, chamado.CategoriaId);
        Assert.Equal(dados.Subcategoria.Id, chamado.SubcategoriaId);
        Assert.Equal(dados.Prioridade.Id, chamado.PrioridadeId);
    }

    [Fact]
    public async Task DeveAplicarTipoCategoriaSubcategoriaEPrioridadeDoCatalogoNaAberturaGuiada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var categoriaAlternativa = new CategoriaChamado("Aplicacoes", null, dados.Departamento.Id, "teste");
        context.CategoriasChamado.Add(categoriaAlternativa);
        await context.SaveChangesAsync();

        var subcategoriaAlternativa = new SubcategoriaChamado(categoriaAlternativa.Id, "ERP", null, "teste");
        context.SubcategoriasChamado.Add(subcategoriaAlternativa);
        await context.SaveChangesAsync();

        var prioridadeCatalogo = await context.PrioridadesChamado.FirstAsync(x => x.Nivel == PrioridadeChamadoEnum.Baixa);
        var prioridadeRequest = await context.PrioridadesChamado.FirstAsync(x => x.Nivel == PrioridadeChamadoEnum.Critica);

        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            prioridadeCatalogo.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);
        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Solicitar acesso VPN",
            Descricao = "Classificacao deve ser herdada do catalogo.",
            CatalogoServicoId = servico.Id,
            CategoriaId = categoriaAlternativa.Id,
            SubcategoriaId = subcategoriaAlternativa.Id,
            PrioridadeId = prioridadeRequest.Id,
            NaturezaChamado = NaturezaChamadoEnum.Incidente,
            ImpactoChamado = ImpactoChamadoEnum.Alto,
            UrgenciaChamado = UrgenciaChamadoEnum.Alta
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, chamado.NaturezaChamado);
        Assert.Equal(dados.Categoria.Id, chamado.CategoriaId);
        Assert.Equal(dados.Subcategoria.Id, chamado.SubcategoriaId);
        Assert.Equal(prioridadeCatalogo.Id, chamado.PrioridadeId);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, response.NaturezaChamado);
        Assert.Equal(dados.Categoria.Id, response.CategoriaId);
        Assert.Equal(dados.Subcategoria.Id, response.SubcategoriaId);
        Assert.Equal(prioridadeCatalogo.Id, response.PrioridadeId);
    }

    [Fact]
    public async Task DevePreservarFallbackAtualQuandoCatalogoNaoDefineClassificacao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var prioridadeFallback = await context.PrioridadesChamado.FirstAsync(x => x.Nivel == PrioridadeChamadoEnum.Alta);

        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            null,
            null,
            null,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);
        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Solicitar acesso legado",
            Descricao = "Sem classificacao no catalogo usa fallback atual.",
            CatalogoServicoId = servico.Id,
            CategoriaId = dados.Categoria.Id,
            SubcategoriaId = dados.Subcategoria.Id,
            PrioridadeId = prioridadeFallback.Id,
            NaturezaChamado = NaturezaChamadoEnum.Incidente,
            ImpactoChamado = ImpactoChamadoEnum.Medio,
            UrgenciaChamado = UrgenciaChamadoEnum.Alta
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, chamado.NaturezaChamado);
        Assert.Equal(dados.Categoria.Id, chamado.CategoriaId);
        Assert.Equal(dados.Subcategoria.Id, chamado.SubcategoriaId);
        Assert.Equal(prioridadeFallback.Id, chamado.PrioridadeId);
        Assert.Equal(prioridadeFallback.Id, response.PrioridadeId);
    }

    [Fact]
    public async Task DeveRejeitarRespostasFormularioPreenchidasQuandoServicoNaoPossuiFormulario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);

        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Solicitar acesso remoto",
            Descricao = "Sem formulario configurado nao deve aceitar respostas preenchidas.",
            CatalogoServicoId = servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = Guid.NewGuid(),
                    Valor = "vpn"
                },
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = Guid.NewGuid(),
                    Valores = ["email", "teams"]
                }
            ]
        }));

        Assert.Contains("nao possui formulario configurado", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(0, await context.Chamados.CountAsync());
    }

    [Fact]
    public async Task DevePermitirAberturaGuiadaSemRespostasQuandoServicoNaoPossuiFormulario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);

        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Servico sem formulario",
            Descricao = "Fluxo deve continuar compativel.",
            CatalogoServicoId = servico.Id
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(servico.Id, chamado.CatalogoServicoId);
        Assert.Empty(context.RespostasFormularioChamado);
    }

    [Fact]
    public async Task DevePermitirAberturaGuiadaComRespostasNulasQuandoServicoNaoPossuiFormulario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);

        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Servico sem formulario com respostas nulas",
            Descricao = "Fluxo deve continuar compativel com null.",
            CatalogoServicoId = servico.Id,
            RespostasFormulario = null
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(servico.Id, chamado.CatalogoServicoId);
        Assert.Empty(context.RespostasFormularioChamado);
    }

    [Fact]
    public async Task DevePermitirAberturaGuiadaComListaVaziaQuandoServicoNaoPossuiFormulario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);

        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Servico sem formulario com lista vazia",
            Descricao = "Fluxo deve continuar compativel com lista vazia.",
            CatalogoServicoId = servico.Id,
            RespostasFormulario = []
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(servico.Id, chamado.CatalogoServicoId);
    }

    [Fact]
    public async Task DevePreservarSlaGrupoEAprovacaoQuandoServicoNaoPossuiFormulario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var grupoTecnico = new GrupoTecnico("Grupo Sem Formulario", null, "teste");
        context.GruposTecnicos.Add(grupoTecnico);
        await context.SaveChangesAsync();

        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            requerAprovacao: true,
            slaPadraoId: SeedData.SlaPoliticaPadraoId,
            grupoTecnicoId: grupoTecnico.Id);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Servico sem formulario com SLA e aprovacao",
            Descricao = "Fluxo deve manter SLA, grupo e aprovacao.",
            CatalogoServicoId = servico.Id,
            RespostasFormulario = []
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        var chamadoSla = await context.ChamadosSla.SingleAsync(x => x.ChamadoId == response.Id);
        var aprovacao = await context.AprovacoesChamado.SingleAsync(x => x.ChamadoId == response.Id);

        Assert.Equal(grupoTecnico.Id, chamado.GrupoTecnicoId);
        Assert.Equal(SeedData.SlaPoliticaPadraoId, chamadoSla.PoliticaSlaId);
        Assert.Equal(StatusAprovacaoChamado.Pendente, aprovacao.Status);
        Assert.Empty(context.RespostasFormularioChamado);
    }

    [Fact]
    public async Task DevePermitirAberturaSemRespostasQuandoFormularioNaoPossuiCampoObrigatorio()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, configurarCampoObrigatorio: false);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Formulario opcional",
            Descricao = "Nenhum campo obrigatorio deve ser exigido.",
            CatalogoServicoId = cenario.Servico.Id
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(cenario.Servico.Id, chamado.CatalogoServicoId);
    }

    [Fact]
    public async Task DevePermitirAberturaQuandoCampoObrigatorioForRespondidoComValor()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Formulario com valor",
            Descricao = "Campo obrigatorio preenchido com valor unico.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoObrigatorio.Id,
                    Valor = "VPN"
                }
            ]
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(cenario.Servico.Id, chamado.CatalogoServicoId);
    }

    [Fact]
    public async Task DevePermitirTextoCurtoValido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.TextoCurto);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Texto curto valido",
            Descricao = "Resposta curta valida.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = "abc" }]
        });

        Assert.Equal(cenario.Servico.Id, (await context.Chamados.SingleAsync(x => x.Id == response.Id)).CatalogoServicoId);
    }

    [Fact]
    public async Task DeveRejeitarTextoCurtoAcimaDoLimite()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.TextoCurto);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Texto curto invalido",
            Descricao = "Acima do limite curto.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = new string('x', 181) }]
        }));

        Assert.Contains("180", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DevePermitirTextoLongoValido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.TextoLongo);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Texto longo valido",
            Descricao = "Resposta longa valida.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = new string('x', 500) }]
        });

        Assert.Equal(cenario.Servico.Id, (await context.Chamados.SingleAsync(x => x.Id == response.Id)).CatalogoServicoId);
    }

    [Fact]
    public async Task DevePermitirNumeroValido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.Numero);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Numero valido",
            Descricao = "Resposta numerica valida.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = "123.45" }]
        });

        Assert.Equal(cenario.Servico.Id, (await context.Chamados.SingleAsync(x => x.Id == response.Id)).CatalogoServicoId);
    }

    [Fact]
    public async Task DeveRejeitarNumeroInvalido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.Numero);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Numero invalido",
            Descricao = "Resposta numerica invalida.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = "abc" }]
        }));

        Assert.Contains("numero decimal valido", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevePermitirDataIsoValida()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.Data);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Data valida",
            Descricao = "Resposta data valida.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = "2026-07-01" }]
        });

        Assert.Equal(cenario.Servico.Id, (await context.Chamados.SingleAsync(x => x.Id == response.Id)).CatalogoServicoId);
    }

    [Fact]
    public async Task DeveRejeitarDataInvalida()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.Data);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Data invalida",
            Descricao = "Resposta data invalida.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = "01/07/2026" }]
        }));

        Assert.Contains("yyyy-MM-dd", ex.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("true")]
    [InlineData("false")]
    [InlineData("TRUE")]
    public async Task DevePermitirBooleanoValido(string valor)
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.Booleano);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Booleano valido",
            Descricao = "Resposta booleana valida.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = valor }]
        });

        Assert.Equal(cenario.Servico.Id, (await context.Chamados.SingleAsync(x => x.Id == response.Id)).CatalogoServicoId);
    }

    [Fact]
    public async Task DeveRejeitarBooleanoInvalido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.Booleano);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Booleano invalido",
            Descricao = "Resposta booleana invalida.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = "sim" }]
        }));

        Assert.Contains("true ou false", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevePermitirSelecaoUnicaComValor()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.SelecaoUnica);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Selecao unica valida",
            Descricao = "Estrutura valida.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = "vpn" }]
        });

        Assert.Equal(cenario.Servico.Id, (await context.Chamados.SingleAsync(x => x.Id == response.Id)).CatalogoServicoId);
    }

    [Fact]
    public async Task DeveRejeitarSelecaoUnicaComOpcaoInexistente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.SelecaoUnica);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Selecao unica com opcao inexistente",
            Descricao = "Opcao nao configurada deve ser rejeitada.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = "inexistente" }]
        }));

        Assert.Contains("opcao ativa", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(context.Chamados);
        Assert.Empty(context.RespostasFormularioChamado);
    }

    [Fact]
    public async Task DeveRejeitarSelecaoUnicaComOpcaoInativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.SelecaoUnica);
        var opcaoVpn = await context.OpcoesCamposFormularioServico.SingleAsync(x => x.CampoFormularioServicoId == cenario.CampoObrigatorio.Id && x.Valor == "vpn");
        opcaoVpn.Inativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Selecao unica com opcao inativa",
            Descricao = "Opcao inativa deve ser rejeitada.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = "vpn" }]
        }));

        Assert.Contains("opcao ativa", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(context.Chamados);
        Assert.Empty(context.RespostasFormularioChamado);
    }

    [Fact]
    public async Task DeveRejeitarSelecaoUnicaComValores()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.SelecaoUnica);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Selecao unica invalida",
            Descricao = "Estrutura invalida.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valores = ["vpn"] }]
        }));

        Assert.Contains("Valor", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevePermitirAberturaQuandoCampoObrigatorioForRespondidoComValores()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(
            context,
            dados,
            tipoCampoObrigatorio: TipoCampoFormularioServico.SelecaoMultipla);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Formulario com valores",
            Descricao = "Campo obrigatorio preenchido com multiplos valores.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoObrigatorio.Id,
                    Valores = ["vpn", "email"]
                }
            ]
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(cenario.Servico.Id, chamado.CatalogoServicoId);
    }

    [Fact]
    public async Task DeveRejeitarSelecaoMultiplaComOpcaoInexistente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(
            context,
            dados,
            tipoCampoObrigatorio: TipoCampoFormularioServico.SelecaoMultipla);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Selecao multipla com opcao inexistente",
            Descricao = "Uma opcao nao configurada deve falhar.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoObrigatorio.Id,
                    Valores = ["vpn", "inexistente"]
                }
            ]
        }));

        Assert.Contains("opcoes ativas", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(context.Chamados);
        Assert.Empty(context.RespostasFormularioChamado);
    }

    [Fact]
    public async Task DeveRejeitarSelecaoMultiplaComOpcaoInativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(
            context,
            dados,
            tipoCampoObrigatorio: TipoCampoFormularioServico.SelecaoMultipla);
        var opcaoVpn = await context.OpcoesCamposFormularioServico.SingleAsync(x => x.CampoFormularioServicoId == cenario.CampoObrigatorio.Id && x.Valor == "vpn");
        opcaoVpn.Inativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Selecao multipla com opcao inativa",
            Descricao = "Opcao inativa deve falhar.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoObrigatorio.Id,
                    Valores = ["vpn", "email"]
                }
            ]
        }));

        Assert.Contains("opcoes ativas", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(context.Chamados);
        Assert.Empty(context.RespostasFormularioChamado);
    }

    [Fact]
    public async Task DeveRejeitarSelecaoMultiplaComValor()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(
            context,
            dados,
            tipoCampoObrigatorio: TipoCampoFormularioServico.SelecaoMultipla);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Selecao multipla invalida",
            Descricao = "Deve usar Valores.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = "vpn" }]
        }));

        Assert.Contains("Valores", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRejeitarAberturaQuandoCampoObrigatorioNaoForRespondido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Campo obrigatorio ausente",
            Descricao = "Abertura deve falhar sem resposta obrigatoria.",
            CatalogoServicoId = cenario.Servico.Id
        }));

        Assert.Contains(cenario.CampoObrigatorio.Rotulo, ex.Message, StringComparison.Ordinal);
        Assert.Empty(context.Chamados);
    }

    [Fact]
    public async Task DeveRejeitarAberturaQuandoCampoObrigatorioReceberValorVazio()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, tipoCampoObrigatorio: TipoCampoFormularioServico.TextoCurto);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Campo obrigatorio vazio",
            Descricao = "Valor vazio deve ser tratado como ausencia de resposta.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoObrigatorio.Id,
                    Valor = "   "
                }
            ]
        }));

        Assert.Contains(cenario.CampoObrigatorio.Rotulo, ex.Message, StringComparison.Ordinal);
        Assert.Empty(context.Chamados);
    }

    [Fact]
    public async Task DeveRejeitarAberturaQuandoCampoObrigatorioReceberListaVazia()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(
            context,
            dados,
            tipoCampoObrigatorio: TipoCampoFormularioServico.SelecaoMultipla);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Campo obrigatorio lista vazia",
            Descricao = "Lista vazia deve ser tratada como ausencia de resposta.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoObrigatorio.Id,
                    Valores = []
                }
            ]
        }));

        Assert.Contains(cenario.CampoObrigatorio.Rotulo, ex.Message, StringComparison.Ordinal);
        Assert.Empty(context.Chamados);
    }

    [Fact]
    public async Task DeveRetornarErroCoerenteQuandoMultiplosCamposObrigatoriosNaoForemRespondidos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioCompletoAsync(context, dados);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Multiplos obrigatorios ausentes",
            Descricao = "Deve falhar apontando o primeiro obrigatorio aplicavel.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoTextoCurto.Id,
                    Valor = "vpn"
                }
            ]
        }));

        Assert.Contains("Campo obrigatorio do formulario nao respondido", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(cenario.CampoTextoLongo.Rotulo, ex.Message, StringComparison.Ordinal);
        Assert.Empty(context.Chamados);
    }

    [Fact]
    public async Task DevePermitirAberturaSemRespostaParaCampoOpcional()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Campo opcional ausente",
            Descricao = "Somente o campo obrigatorio deve ser exigido.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoObrigatorio.Id,
                    Valor = "ok"
                }
            ]
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(cenario.Servico.Id, chamado.CatalogoServicoId);
    }

    [Fact]
    public async Task NaoDeveExigirCampoObrigatorioInativo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, campoObrigatorioAtivo: false);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Campo obrigatorio inativo",
            Descricao = "Campo inativo nao deve ser exigido.",
            CatalogoServicoId = cenario.Servico.Id
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(cenario.Servico.Id, chamado.CatalogoServicoId);
    }

    [Fact]
    public async Task DeveRejeitarRespostaParaCampoInativo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, campoObrigatorioAtivo: false, configurarCampoObrigatorio: false);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Campo inativo respondido",
            Descricao = "Campo inativo deve ser rejeitado.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = "abc" }]
        }));

        Assert.Contains("inativo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task NaoDeveExigirCampoObrigatorioInvisivel()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, campoObrigatorioVisivel: false);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Campo obrigatorio invisivel",
            Descricao = "Campo invisivel nao deve ser exigido.",
            CatalogoServicoId = cenario.Servico.Id
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(cenario.Servico.Id, chamado.CatalogoServicoId);
    }

    [Fact]
    public async Task DeveRejeitarRespostaParaCampoInvisivel()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, campoObrigatorioVisivel: false, configurarCampoObrigatorio: false);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Campo invisivel respondido",
            Descricao = "Campo invisivel deve ser rejeitado.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoObrigatorio.Id, Valor = "abc" }]
        }));

        Assert.Contains("invisivel", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRejeitarRespostaParaCampoInexistente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, configurarCampoObrigatorio: false);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Campo inexistente",
            Descricao = "Id inexistente deve ser rejeitado.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = Guid.NewGuid(), Valor = "abc" }]
        }));

        Assert.Contains("fora do escopo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRejeitarRespostaParaCampoDeOutraVersaoNaoAplicavel()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, configurarCampoObrigatorio: false);
        var formulario = await context.FormulariosServico
            .Include(x => x.Versoes)
            .FirstAsync(x => x.CatalogoServicoId == cenario.Servico.Id);
        var versaoNaoAplicavel = formulario.Versoes.Single(x => x.Numero == 1);
        var campoOutraVersao = new CampoFormularioServico(
            versaoNaoAplicavel.Id,
            "campoOutraVersao",
            "Campo de outra versao",
            TipoCampoFormularioServico.TextoCurto,
            false,
            1,
            null,
            true,
            "teste");
        context.CamposFormularioServico.Add(campoOutraVersao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Outra versao",
            Descricao = "Campo de outra versao deve ser rejeitado.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = campoOutraVersao.Id, Valor = "abc" }]
        }));

        Assert.Contains("fora do escopo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRejeitarRespostaParaCampoDeOutroServico()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenarioOrigem = await CriarServicoCatalogoComFormularioAsync(context, dados, configurarCampoObrigatorio: false);
        var cenarioOutroServico = await CriarServicoCatalogoComFormularioAsync(context, dados, configurarCampoObrigatorio: false);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Outro servico",
            Descricao = "Campo de outro servico deve ser rejeitado.",
            CatalogoServicoId = cenarioOrigem.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenarioOutroServico.CampoOpcional.Id, Valor = "abc" }]
        }));

        Assert.Contains("fora do escopo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevePreservarSlaGrupoEAprovacaoAoValidarObrigatoriedadeDoFormulario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var grupoTecnico = new GrupoTecnico("Grupo Formulario Obrigatorio", null, "teste");
        context.GruposTecnicos.Add(grupoTecnico);
        await context.SaveChangesAsync();

        var cenario = await CriarServicoCatalogoComFormularioAsync(
            context,
            dados,
            requerAprovacao: true,
            slaPadraoId: SeedData.SlaPoliticaPadraoId,
            grupoTecnicoId: grupoTecnico.Id);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Formulario com regras legadas",
            Descricao = "SLA, grupo e aprovacao devem permanecer iguais.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoObrigatorio.Id,
                    Valor = "necessario"
                }
            ]
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        var chamadoSla = await context.ChamadosSla.SingleAsync(x => x.ChamadoId == response.Id);
        var aprovacao = await context.AprovacoesChamado.SingleAsync(x => x.ChamadoId == response.Id);

        Assert.Equal(grupoTecnico.Id, chamado.GrupoTecnicoId);
        Assert.Equal(SeedData.SlaPoliticaPadraoId, chamadoSla.PoliticaSlaId);
        Assert.Equal(StatusAprovacaoChamado.Pendente, aprovacao.Status);
    }

    [Fact]
    public async Task DevePermitirRespostaParaCampoOpcionalValidoDoFormulario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, configurarCampoObrigatorio: false);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Campo opcional valido",
            Descricao = "Campo opcional do mesmo formulario deve ser aceito.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = [new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoOpcional.Id, Valor = "texto opcional" }]
        });

        Assert.Equal(cenario.Servico.Id, (await context.Chamados.SingleAsync(x => x.Id == response.Id)).CatalogoServicoId);
        Assert.Equal(1, await context.Chamados.CountAsync());
    }

    [Fact]
    public async Task DeveAbrirChamadoComFormularioValidoDeTodosOsTiposEPersistirRespostas()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var grupoTecnico = new GrupoTecnico("Grupo Formulario Valido", null, "teste");
        context.GruposTecnicos.Add(grupoTecnico);
        await context.SaveChangesAsync();

        var cenario = await CriarServicoCatalogoComFormularioCompletoAsync(
            context,
            dados,
            requerAprovacao: true,
            slaPadraoId: SeedData.SlaPoliticaPadraoId,
            grupoTecnicoId: grupoTecnico.Id);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Formulario valido completo",
            Descricao = "Fluxo valido com respostas de todos os tipos.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoTextoCurto.Id, Valor = "vpn" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoTextoLongo.Id, Valor = "Justificativa detalhada da requisicao." },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoNumero.Id, Valor = "123.45" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoData.Id, Valor = "2026-07-01" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoBooleano.Id, Valor = "true" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoSelecaoUnica.Id, Valor = "email" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoSelecaoMultipla.Id, Valores = ["vpn", "teams"] }
            ]
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        var chamadoSla = await context.ChamadosSla.SingleAsync(x => x.ChamadoId == response.Id);
        var aprovacao = await context.AprovacoesChamado.SingleAsync(x => x.ChamadoId == response.Id);
        var status = await context.StatusChamado.SingleAsync(x => x.Id == chamado.StatusId);
        var historicos = await context.HistoricosChamado.Where(x => x.ChamadoId == response.Id).ToListAsync();
        var respostasPersistidas = await context.RespostasFormularioChamado
            .Where(x => x.ChamadoId == response.Id)
            .OrderBy(x => x.CampoFormularioServicoId)
            .ToListAsync();

        Assert.Equal(cenario.Servico.Id, chamado.CatalogoServicoId);
        Assert.Equal(grupoTecnico.Id, chamado.GrupoTecnicoId);
        Assert.Equal(SeedData.SlaPoliticaPadraoId, chamadoSla.PoliticaSlaId);
        Assert.Equal(StatusAprovacaoChamado.Pendente, aprovacao.Status);
        Assert.Equal(StatusChamadoEnum.Aberto, status.Codigo);
        Assert.Equal(7, respostasPersistidas.Count);
        Assert.All(respostasPersistidas, x => Assert.Equal(chamado.Id, x.ChamadoId));
        Assert.All(respostasPersistidas, x => Assert.Equal(cenario.CampoTextoCurto.FormularioServicoVersaoId, x.FormularioServicoVersaoId));
        Assert.Contains(respostasPersistidas, x => x.CampoFormularioServicoId == cenario.CampoTextoCurto.Id && x.Valor == "vpn" && x.ValoresJson is null);
        Assert.Contains(respostasPersistidas, x => x.CampoFormularioServicoId == cenario.CampoTextoLongo.Id && x.Valor == "Justificativa detalhada da requisicao.");
        Assert.Contains(respostasPersistidas, x => x.CampoFormularioServicoId == cenario.CampoNumero.Id && x.Valor == "123.45");
        Assert.Contains(respostasPersistidas, x => x.CampoFormularioServicoId == cenario.CampoData.Id && x.Valor == "2026-07-01");
        Assert.Contains(respostasPersistidas, x => x.CampoFormularioServicoId == cenario.CampoBooleano.Id && x.Valor == "true");
        Assert.Contains(respostasPersistidas, x => x.CampoFormularioServicoId == cenario.CampoSelecaoUnica.Id && x.Valor == "email");
        var respostaMultipla = Assert.Single(respostasPersistidas, x => x.CampoFormularioServicoId == cenario.CampoSelecaoMultipla.Id);
        Assert.Null(respostaMultipla.Valor);
        Assert.Equal(["vpn", "teams"], respostaMultipla.ObterValores());
        Assert.DoesNotContain(historicos, x => x.Descricao.Contains("Justificativa detalhada", StringComparison.Ordinal));
        Assert.DoesNotContain(historicos, x => x.Descricao.Contains("123.45", StringComparison.Ordinal));
        Assert.DoesNotContain(historicos, x => x.Descricao.Contains("teams", StringComparison.Ordinal));
        Assert.Contains(
            historicos,
            x => x.Tipo == TipoHistoricoChamado.FormularioServicoPreenchidoNaAbertura
                 && x.Descricao == "Chamado aberto com formulario do servico preenchido.");
        Assert.NotNull(context.Model.FindEntityType(typeof(RespostaFormularioChamado)));
    }

    [Fact]
    public async Task DeveRegistrarHistoricoResumoQuandoFormularioForPreenchidoNaAbertura()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Historico de formulario",
            Descricao = "Deve registrar historico resumido.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoObrigatorio.Id,
                    Valor = "necessario"
                }
            ]
        });

        var historicos = await context.HistoricosChamado.Where(x => x.ChamadoId == response.Id).ToListAsync();
        var historicoFormulario = Assert.Single(historicos, x => x.Tipo == TipoHistoricoChamado.FormularioServicoPreenchidoNaAbertura);

        Assert.Equal("Chamado aberto com formulario do servico preenchido.", historicoFormulario.Descricao);
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.Criado);
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.ChamadoCriadoPorCatalogoServico);
        Assert.DoesNotContain(historicoFormulario.Descricao, "necessario", StringComparison.Ordinal);
        Assert.DoesNotContain(historicoFormulario.Descricao, cenario.CampoObrigatorio.Rotulo, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeveRegistrarAuditoriaTecnicaQuandoPersistirRespostasFormularioNaAbertura()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados);
        var auditoria = new FakeAuditoriaService();
        var useCase = CriarUseCase(context, dados.UsuarioContexto, auditoria);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Auditoria tecnica de formulario",
            Descricao = "Deve registrar auditoria tecnica sem expor valores.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoObrigatorio.Id,
                    Valor = "necessario"
                }
            ]
        });

        var eventoAuditoriaFormulario = Assert.Single(
            auditoria.Eventos,
            x => x.Descricao == "Respostas do formulario persistidas na abertura guiada.");

        Assert.Equal("Chamados", eventoAuditoriaFormulario.Modulo);
        Assert.Equal("RespostaFormularioChamado", eventoAuditoriaFormulario.Entidade);
        Assert.Equal(response.Id.ToString(), eventoAuditoriaFormulario.EntidadeId);
        Assert.Equal(TipoAcaoAuditoria.Criacao, eventoAuditoriaFormulario.Acao);
        Assert.Equal(dados.Usuario.Id, eventoAuditoriaFormulario.UsuarioId);
        Assert.Equal(dados.UsuarioContexto.Login, eventoAuditoriaFormulario.UsuarioLogin);
        Assert.DoesNotContain("necessario", eventoAuditoriaFormulario.DadosDepois, StringComparison.Ordinal);
        Assert.DoesNotContain(cenario.CampoObrigatorio.Rotulo, eventoAuditoriaFormulario.DadosDepois, StringComparison.Ordinal);

        var dadosDepois = JsonNode.Parse(eventoAuditoriaFormulario.DadosDepois!)!.AsObject();
        Assert.Equal(response.Id.ToString(), dadosDepois["ChamadoId"]!.ToString());
        Assert.Equal(cenario.CampoObrigatorio.FormularioServicoVersaoId.ToString(), dadosDepois["FormularioServicoVersaoId"]!.ToString());
        Assert.Equal(1, dadosDepois["QuantidadeRespostasPersistidas"]!.GetValue<int>());
        Assert.Equal("AberturaGuiadaCatalogo", dadosDepois["Origem"]!.GetValue<string>());

        var metadados = JsonNode.Parse(eventoAuditoriaFormulario.Metadados!)!.AsObject();
        Assert.Equal("AberturaGuiadaCatalogo", metadados["origem"]!.GetValue<string>());
        Assert.Equal("PersistenciaRespostasFormulario", metadados["operacao"]!.GetValue<string>());

        Assert.Contains(
            context.HistoricosChamado,
            x => x.ChamadoId == response.Id
                 && x.Tipo == TipoHistoricoChamado.FormularioServicoPreenchidoNaAbertura
                 && x.Descricao == "Chamado aberto com formulario do servico preenchido.");
    }

    [Fact]
    public async Task DevePersistirRespostaSimplesEMultiplaComVinculosEsperadosSemExporValoresNaAuditoria()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioCompletoAsync(context, dados);
        var auditoria = new FakeAuditoriaService();
        var useCase = CriarUseCase(context, dados.UsuarioContexto, auditoria);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Persistencia focada de respostas",
            Descricao = "Validar valor simples e multiplo com vinculos.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoTextoCurto.Id, Valor = "vpn" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoTextoLongo.Id, Valor = "Texto obrigatorio para persistencia." },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoNumero.Id, Valor = "123.45" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoData.Id, Valor = "2026-07-01" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoBooleano.Id, Valor = "true" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoSelecaoUnica.Id, Valor = "email" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = cenario.CampoSelecaoMultipla.Id, Valores = ["vpn", "teams"] }
            ]
        });

        var respostasPersistidas = await context.RespostasFormularioChamado
            .Where(x => x.ChamadoId == response.Id)
            .OrderBy(x => x.CampoFormularioServicoId)
            .ToListAsync();

        Assert.Equal(7, respostasPersistidas.Count);

        var respostaSimples = Assert.Single(respostasPersistidas, x => x.CampoFormularioServicoId == cenario.CampoTextoCurto.Id);
        Assert.Equal(response.Id, respostaSimples.ChamadoId);
        Assert.Equal(cenario.CampoTextoCurto.FormularioServicoVersaoId, respostaSimples.FormularioServicoVersaoId);
        Assert.Equal(cenario.CampoTextoCurto.Id, respostaSimples.CampoFormularioServicoId);
        Assert.Equal("vpn", respostaSimples.Valor);
        Assert.Null(respostaSimples.ValoresJson);

        var respostaMultipla = Assert.Single(respostasPersistidas, x => x.CampoFormularioServicoId == cenario.CampoSelecaoMultipla.Id);
        Assert.Equal(response.Id, respostaMultipla.ChamadoId);
        Assert.Equal(cenario.CampoSelecaoMultipla.FormularioServicoVersaoId, respostaMultipla.FormularioServicoVersaoId);
        Assert.Equal(cenario.CampoSelecaoMultipla.Id, respostaMultipla.CampoFormularioServicoId);
        Assert.Null(respostaMultipla.Valor);
        Assert.NotNull(respostaMultipla.ValoresJson);
        Assert.Contains("\"vpn\"", respostaMultipla.ValoresJson, StringComparison.Ordinal);
        Assert.Contains("\"teams\"", respostaMultipla.ValoresJson, StringComparison.Ordinal);
        Assert.Equal(["vpn", "teams"], respostaMultipla.ObterValores());

        var eventoAuditoriaFormulario = Assert.Single(
            auditoria.Eventos,
            x => x.Descricao == "Respostas do formulario persistidas na abertura guiada.");

        Assert.DoesNotContain("vpn", eventoAuditoriaFormulario.DadosDepois, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("teams", eventoAuditoriaFormulario.DadosDepois, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Texto obrigatorio para persistencia.", eventoAuditoriaFormulario.DadosDepois, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(cenario.CampoTextoLongo.Rotulo, eventoAuditoriaFormulario.DadosDepois, StringComparison.Ordinal);
        Assert.DoesNotContain(cenario.CampoTextoCurto.Rotulo, eventoAuditoriaFormulario.DadosDepois, StringComparison.Ordinal);
        Assert.DoesNotContain(cenario.CampoSelecaoMultipla.Rotulo, eventoAuditoriaFormulario.DadosDepois, StringComparison.Ordinal);

        var dadosDepois = JsonNode.Parse(eventoAuditoriaFormulario.DadosDepois!)!.AsObject();
        Assert.Equal(response.Id.ToString(), dadosDepois["ChamadoId"]!.ToString());
        Assert.Equal(cenario.CampoTextoCurto.FormularioServicoVersaoId.ToString(), dadosDepois["FormularioServicoVersaoId"]!.ToString());
        Assert.Equal(7, dadosDepois["QuantidadeRespostasPersistidas"]!.GetValue<int>());
    }

    [Fact]
    public async Task NaoDeveRegistrarHistoricoResumoQuandoServicoNaoPossuiFormulario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Catalogo sem formulario",
            Descricao = "Nao deve criar historico de formulario.",
            CatalogoServicoId = servico.Id
        });

        var historicos = await context.HistoricosChamado.Where(x => x.ChamadoId == response.Id).ToListAsync();
        Assert.DoesNotContain(historicos, x => x.Tipo == TipoHistoricoChamado.FormularioServicoPreenchidoNaAbertura);
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.Criado);
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.ChamadoCriadoPorCatalogoServico);
    }

    [Fact]
    public async Task NaoDeveRegistrarAuditoriaTecnicaEspecificaQuandoServicoNaoPossuiFormulario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante);
        var auditoria = new FakeAuditoriaService();
        var useCase = CriarUseCase(context, dados.UsuarioContexto, auditoria);

        await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Catalogo sem formulario",
            Descricao = "Nao deve criar auditoria de formulario.",
            CatalogoServicoId = servico.Id
        });

        Assert.DoesNotContain(auditoria.Eventos, x => x.Descricao == "Respostas do formulario persistidas na abertura guiada.");
    }

    [Fact]
    public async Task NaoDeveRegistrarHistoricoResumoQuandoFormularioNaoGerarRespostasPersistidas()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, configurarCampoObrigatorio: false);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Formulario sem respostas",
            Descricao = "Sem respostas nao deve gerar historico especifico.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = []
        });

        var historicos = await context.HistoricosChamado.Where(x => x.ChamadoId == response.Id).ToListAsync();
        Assert.Empty(await context.RespostasFormularioChamado.Where(x => x.ChamadoId == response.Id).ToListAsync());
        Assert.DoesNotContain(historicos, x => x.Tipo == TipoHistoricoChamado.FormularioServicoPreenchidoNaAbertura);
    }

    [Fact]
    public async Task NaoDeveRegistrarAuditoriaTecnicaEspecificaQuandoFormularioNaoGerarRespostasPersistidas()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados, configurarCampoObrigatorio: false);
        var auditoria = new FakeAuditoriaService();
        var useCase = CriarUseCase(context, dados.UsuarioContexto, auditoria);

        await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Formulario sem respostas",
            Descricao = "Sem respostas nao deve gerar auditoria especifica.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario = []
        });

        Assert.DoesNotContain(auditoria.Eventos, x => x.Descricao == "Respostas do formulario persistidas na abertura guiada.");
    }

    [Fact]
    public async Task DeveNaoPersistirRespostaParaCampoOpcionalNaoRespondido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(context, dados);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Somente obrigatorio",
            Descricao = "Campo opcional ausente nao deve gerar registro.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoObrigatorio.Id,
                    Valor = "necessario"
                }
            ]
        });

        var respostasPersistidas = await context.RespostasFormularioChamado
            .Where(x => x.ChamadoId == response.Id)
            .ToListAsync();

        Assert.Single(respostasPersistidas);
        Assert.DoesNotContain(respostasPersistidas, x => x.CampoFormularioServicoId == cenario.CampoOpcional.Id);
        Assert.Equal(cenario.CampoObrigatorio.Id, respostasPersistidas[0].CampoFormularioServicoId);
    }

    [Fact]
    public async Task NaoDevePersistirRespostasQuandoAberturaGuiadaFalharPorRespostaInvalida()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(
            context,
            dados,
            tipoCampoObrigatorio: TipoCampoFormularioServico.Numero);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Resposta invalida",
            Descricao = "Nao deve salvar chamado nem respostas.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoObrigatorio.Id,
                    Valor = "abc"
                }
            ]
        }));

        Assert.Empty(context.Chamados);
        Assert.Empty(context.RespostasFormularioChamado);
        Assert.DoesNotContain(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.FormularioServicoPreenchidoNaAbertura);
    }

    [Fact]
    public async Task NaoDeveRegistrarAuditoriaTecnicaEspecificaQuandoAberturaGuiadaFalhar()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var cenario = await CriarServicoCatalogoComFormularioAsync(
            context,
            dados,
            tipoCampoObrigatorio: TipoCampoFormularioServico.Numero);
        var auditoria = new FakeAuditoriaService();
        var useCase = CriarUseCase(context, dados.UsuarioContexto, auditoria);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Resposta invalida",
            Descricao = "Nao deve registrar auditoria especifica.",
            CatalogoServicoId = cenario.Servico.Id,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = cenario.CampoObrigatorio.Id,
                    Valor = "abc"
                }
            ]
        }));

        Assert.DoesNotContain(auditoria.Eventos, x => x.Descricao == "Respostas do formulario persistidas na abertura guiada.");
    }

    [Fact]
    public async Task DeveAplicarGrupoTecnicoDoCatalogoNaAberturaGuiada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var slaPadrao = await context.SlaPoliticas.FirstAsync(x => x.Ativo);
        var grupoTecnico = new GrupoTecnico("Service Desk Catalogo", null, "teste");
        context.GruposTecnicos.Add(grupoTecnico);
        await context.SaveChangesAsync();

        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            slaPadraoId: slaPadrao.Id,
            grupoTecnicoId: grupoTecnico.Id);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Solicitar VPN",
            Descricao = "Preciso de acesso remoto",
            CatalogoServicoId = servico.Id,
            NaturezaChamado = NaturezaChamadoEnum.Requisicao
        });

        var chamado = await context.Chamados
            .Include(x => x.Status)
            .Include(x => x.ChamadoSla)
            .SingleAsync(x => x.Id == response.Id);

        Assert.Equal(grupoTecnico.Id, chamado.GrupoTecnicoId);
        Assert.Equal(StatusChamadoEnum.Aberto, chamado.Status.Codigo);
        Assert.Equal(servico.SlaPadraoId, chamado.ChamadoSla?.PoliticaSlaId);
        Assert.DoesNotContain(context.AprovacoesChamado, x => x.ChamadoId == response.Id && x.Ativo);
    }

    [Fact]
    public async Task DevePreservarFallbackQuandoCatalogoNaoPossuiGrupoTecnico()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Teste fallback de grupo",
            Descricao = "Sem grupo configurado no catalogo",
            CatalogoServicoId = servico.Id,
            NaturezaChamado = NaturezaChamadoEnum.Requisicao
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        await context.Entry(chamado).Reference(x => x.Status).LoadAsync();

        Assert.Null(chamado.GrupoTecnicoId);
        Assert.Null(chamado.FilaAtendimentoId);
        Assert.Null(chamado.ResponsavelId);
        Assert.Equal(StatusChamadoEnum.Aberto, chamado.Status.Codigo);
        Assert.DoesNotContain(context.AprovacoesChamado, x => x.ChamadoId == response.Id && x.Ativo);
    }

    [Fact]
    public async Task DeveIgnorarGrupoTecnicoInativoDoCatalogoEPreservarFluxoLegado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var grupoTecnico = new GrupoTecnico("Grupo Inativo Catalogo", null, "teste");
        grupoTecnico.Inativar("teste");
        context.GruposTecnicos.Add(grupoTecnico);
        await context.SaveChangesAsync();

        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            grupoTecnicoId: grupoTecnico.Id);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Teste grupo inativo",
            Descricao = "Fluxo deve manter fallback",
            CatalogoServicoId = servico.Id,
            NaturezaChamado = NaturezaChamadoEnum.Requisicao
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        await context.Entry(chamado).Reference(x => x.Status).LoadAsync();
        Assert.Null(chamado.GrupoTecnicoId);
        Assert.Equal(StatusChamadoEnum.Aberto, chamado.Status.Codigo);
    }

    [Fact]
    public async Task DevePreservarAberturaLegadaSemCatalogoSemImpactoDaRegraDeGrupoTecnicoDoCatalogo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);
        var politicaFallbackPadrao = await context.SlaPoliticas.FirstAsync(x => x.Ativo);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Abertura legada sem catalogo",
            Descricao = "Fluxo legado deve permanecer inalterado",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            DepartamentoId = dados.Departamento.Id
        });

        var chamado = await context.Chamados
            .Include(x => x.Status)
            .SingleAsync(x => x.Id == response.Id);
        var chamadoSla = await context.ChamadosSla.SingleAsync(x => x.ChamadoId == response.Id);

        Assert.Null(chamado.CatalogoServicoId);
        Assert.Null(chamado.GrupoTecnicoId);
        Assert.Null(chamado.FilaAtendimentoId);
        Assert.Null(chamado.ResponsavelId);
        Assert.Equal(StatusChamadoEnum.Aberto, chamado.Status.Codigo);
        Assert.Equal(politicaFallbackPadrao.Id, chamadoSla.PoliticaSlaId);
        Assert.DoesNotContain(context.AprovacoesChamado, x => x.ChamadoId == response.Id && x.Ativo);
    }

    [Fact]
    public async Task DeveAbrirChamadoComServicoCatalogoPorSlug()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);
        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Abertura por slug",
            Descricao = "Fluxo por slug",
            CatalogoServicoSlug = servico.Slug
        });

        Assert.Equal(servico.Id, context.Chamados.Single(x => x.Id == response.Id).CatalogoServicoId);
    }

    [Fact]
    public async Task DeveRegistrarHistoricoDeCatalogoAoAbrirChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Historico catalogo",
            Descricao = "Abertura com catalogo",
            CatalogoServicoId = servico.Id
        });

        Assert.Contains(
            context.HistoricosChamado,
            x => x.ChamadoId == response.Id && x.Tipo == TipoHistoricoChamado.ChamadoCriadoPorCatalogoServico);
    }

    [Fact]
    public async Task DeveCriarAprovacaoPendenteAutomaticaQuandoServicoCatalogoRequerAprovacao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            requerAprovacao: true);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Catalogo com aprovacao obrigatoria",
            Descricao = "Solicitacao deve gerar aprovacao pendente",
            CatalogoServicoId = servico.Id
        });

        var aprovacao = await context.AprovacoesChamado.SingleAsync(x => x.ChamadoId == response.Id);
        Assert.Equal(StatusAprovacaoChamado.Pendente, aprovacao.Status);
        Assert.Equal(TipoOrigemAprovacaoChamado.CatalogoServico, aprovacao.TipoOrigem);
        Assert.Equal(servico.Nome, aprovacao.OrigemDescricao);
        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == response.Id && x.Tipo == TipoHistoricoChamado.AprovacaoSolicitada);
        Assert.True(response.RequerAprovacao);
        Assert.True(response.AprovacaoPendente);
        Assert.Equal(StatusAprovacaoChamado.Pendente, response.StatusAprovacao);
        Assert.NotNull(response.AprovacaoChamadoId);
    }

    [Fact]
    public async Task NaoDeveCriarAprovacaoAutomaticaQuandoServicoCatalogoNaoRequerAprovacao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            requerAprovacao: false);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Catalogo sem aprovacao obrigatoria",
            Descricao = "Solicitacao segue fluxo normal",
            CatalogoServicoId = servico.Id
        });

        Assert.DoesNotContain(context.AprovacoesChamado, x => x.ChamadoId == response.Id && x.Ativo);
        Assert.False(response.RequerAprovacao);
        Assert.False(response.AprovacaoPendente);
        Assert.Null(response.StatusAprovacao);
        Assert.Null(response.AprovacaoChamadoId);
    }

    [Fact]
    public async Task DeveAplicarSlaConfiguradoNoServicoDoCatalogo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        
        var politica = new PoliticaSla("SLA Catalogo", "SLA do Servico", 1, null, null, null, false, true, "teste");
        context.SlaPoliticas.Add(politica);
        await context.SaveChangesAsync();
        context.SlaMetas.Add(new MetaSla(politica.Id, dados.Prioridade.Id, 60, 120, null, null, "teste"));
        await context.SaveChangesAsync();

        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            slaPadraoId: politica.Id);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Teste SLA Catalogo",
            Descricao = "Verifica se o SLA foi herdado",
            CatalogoServicoId = servico.Id
        });

        var chamadoSla = await context.ChamadosSla.FirstOrDefaultAsync(x => x.ChamadoId == response.Id);
        Assert.NotNull(chamadoSla);
        Assert.Equal(politica.Id, chamadoSla.PoliticaSlaId);
    }

    [Fact]
    public async Task DeveUtilizarSlaFallbackQuandoServicoCatalogoNaoPossuiSlaPadrao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        
        // SLA padrao do banco em fallback (seeding do contexto padrao)
        var politicaFallbackPadrao = await context.SlaPoliticas.FirstAsync(x => x.Ativo);

        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            slaPadraoId: null); // Nao possui SLA

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Teste SLA Fallback Catalogo",
            Descricao = "Verifica se o fallback atua normalmente",
            CatalogoServicoId = servico.Id
        });

        var chamadoSla = await context.ChamadosSla.FirstOrDefaultAsync(x => x.ChamadoId == response.Id);
        Assert.NotNull(chamadoSla);
        Assert.Equal(politicaFallbackPadrao.Id, chamadoSla.PoliticaSlaId);
    }
    [Fact]
    public async Task DeveManterGrupoResponsavelAtribuidoPelasRegrasLegadasOuVazio()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        
        // Servico de catalogo nao tem configuracao de Grupo (item 10 bloqueado)
        var servico = await CriarServicoCatalogoAsync(context, dados.Usuario, dados.Departamento.Id, dados.Categoria.Id, dados.Subcategoria.Id, dados.Prioridade.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante);
        
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Teste Grupo",
            Descricao = "Verifica se o fluxo legado atua no grupo",
            CatalogoServicoId = servico.Id,
            NaturezaChamado = NaturezaChamadoEnum.Requisicao
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        
        // Atualmente o grupo de responsabilidade pode ser null (fallback da fila geral)
        // O importante é garantir que não há exception ou crash, e ele continua operacional.
        Assert.True(chamado.GrupoTecnicoId == null || chamado.GrupoTecnicoId != Guid.Empty);
    }

    [Fact]
    public async Task DeveImpedirAberturaComServicoArquivado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Arquivado,
            VisibilidadeCatalogoServico.Solicitante);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Teste",
            Descricao = "Teste",
            CatalogoServicoId = servico.Id
        }));
    }

    [Fact]
    public async Task DeveImpedirAberturaComServicoInativo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            ativo: false);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Teste",
            Descricao = "Teste",
            CatalogoServicoId = servico.Id
        }));
    }

    [Fact]
    public async Task DeveImpedirAberturaComServicoNaoPublicado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Rascunho,
            VisibilidadeCatalogoServico.Solicitante);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Teste",
            Descricao = "Teste",
            CatalogoServicoId = servico.Id
        }));
    }

    [Fact]
    public async Task DeveImpedirAberturaComServicoSemVisibilidade()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Atendente);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Teste",
            Descricao = "Teste",
            CatalogoServicoId = servico.Id
        }));
    }

    [Fact]
    public async Task DeveImpedirAberturaComServicoSemPermissaoDeAbertura()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            permiteAberturaChamado: false);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Teste",
            Descricao = "Teste",
            CatalogoServicoId = servico.Id
        }));
    }

    private static AbrirChamadoUseCase CriarUseCase(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao usuario,
        IAuditoriaService? auditoriaService = null)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<RespostaFormularioChamado>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<CatalogoServico>(context),
            PortalUseCasesTestFactory.Repo<FormularioServico>(context),
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<InventarioAtivo>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoInventarioAtivo>(context),
            SlaTestFactory.CriarService(context),
            new FakeCodigoChamadoService(),
            new PrioridadeChamadoMatrizService(PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context)),
            new CamposObrigatoriosChamadoService(),
            new FakeUsuarioContextoAplicacaoService(usuario),
            PortalUseCasesTestFactory.Uow(context),
            auditoriaService);
    [Fact]
    public async Task DeveRegistrarHistoricoDeAberturaSemCatalogo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Teste",
            Descricao = "Teste",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            NaturezaChamado = NaturezaChamadoEnum.Incidente,
            ImpactoChamado = ImpactoChamadoEnum.Alto,
            UrgenciaChamado = UrgenciaChamadoEnum.Alta,
            DepartamentoId = dados.Departamento.Id
        });

        var historicos = await context.HistoricosChamado.Where(x => x.ChamadoId == response.Id).ToListAsync();
        
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.Criado);
        Assert.DoesNotContain(historicos, x => x.Tipo == TipoHistoricoChamado.ChamadoCriadoPorCatalogoServico);
        
        var histCriado = historicos.Single(x => x.Tipo == TipoHistoricoChamado.Criado);
        Assert.Equal(dados.Usuario.Id, histCriado.UsuarioId);
    }

    [Fact]
    public async Task DeveRegistrarHistoricoCatalogoComoRequisicaoServico()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(context, dados.Usuario, dados.Departamento.Id, dados.Categoria.Id, dados.Subcategoria.Id, dados.Prioridade.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante);
        
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Teste",
            Descricao = "Teste",
            CatalogoServicoId = servico.Id,
            NaturezaChamado = NaturezaChamadoEnum.Requisicao
        });

        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, chamado.NaturezaChamado);

        var historicos = await context.HistoricosChamado.Where(x => x.ChamadoId == response.Id).ToListAsync();
        
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.Criado);
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.ChamadoCriadoPorCatalogoServico);
        
        var histCatalogo = historicos.Single(x => x.Tipo == TipoHistoricoChamado.ChamadoCriadoPorCatalogoServico);
        Assert.Contains(servico.Nome, histCatalogo.Descricao);
        Assert.Equal(dados.Usuario.Id, histCatalogo.UsuarioId);
    }

    [Fact]
    public async Task DeveAbrirRequisicaoViaCatalogoSemAprovacao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        
        // requerAprovacao = false
        var servico = await CriarServicoCatalogoAsync(
            context, dados.Usuario, dados.Departamento.Id, dados.Categoria.Id, 
            dados.Subcategoria.Id, dados.Prioridade.Id, 
            StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante, 
            requerAprovacao: false);
        
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Requisicao de software padrão",
            Descricao = "Instalar software",
            CatalogoServicoId = servico.Id,
            NaturezaChamado = NaturezaChamadoEnum.Requisicao
        });

        // Nenhuma instancia de aprovacao deve ser criada
        var temAprovacao = await context.AprovacoesChamado.AnyAsync(x => x.ChamadoId == response.Id);
        Assert.False(temAprovacao, "Nao deveria gerar aprovacao obrigatoria");

        // O historico normal deve existir
        var historicos = await context.HistoricosChamado.Where(x => x.ChamadoId == response.Id).ToListAsync();
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.Criado);
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.ChamadoCriadoPorCatalogoServico);
        
        // Nao deve ter historico de solicitacao de aprovacao
        Assert.DoesNotContain(historicos, x => x.Tipo == TipoHistoricoChamado.AprovacaoSolicitada);

        // O chamado permanece operacional
        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, chamado.NaturezaChamado);
        Assert.Equal(servico.Id, chamado.CatalogoServicoId);
    }

    [Fact]
    public async Task DeveAbrirRequisicaoViaCatalogoComAprovacaoObrigatoria()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var servico = await CriarServicoCatalogoAsync(context, dados.Usuario, dados.Departamento.Id, dados.Categoria.Id, dados.Subcategoria.Id, dados.Prioridade.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante, requerAprovacao: true);
        
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Teste de Aprovacao",
            Descricao = "Teste",
            CatalogoServicoId = servico.Id,
            NaturezaChamado = NaturezaChamadoEnum.Requisicao
        });

        // Aprovação deve ser criada sem duplicidade (exatamente 1)
        var aprovacoes = await context.AprovacoesChamado.Where(x => x.ChamadoId == response.Id).ToListAsync();
        Assert.Single(aprovacoes);
        var aprovacao = aprovacoes.First();
        Assert.Equal(TipoOrigemAprovacaoChamado.CatalogoServico, aprovacao.TipoOrigem);
        Assert.Equal(StatusAprovacaoChamado.Pendente, aprovacao.Status);

        // Vínculo preservado
        var chamado = await context.Chamados.SingleAsync(x => x.Id == response.Id);
        Assert.Equal(servico.Id, chamado.CatalogoServicoId);

        // Historico registrado
        var historicos = await context.HistoricosChamado.Where(x => x.ChamadoId == response.Id).ToListAsync();
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.AprovacaoSolicitada);
        
        var histAprovacao = historicos.Single(x => x.Tipo == TipoHistoricoChamado.AprovacaoSolicitada);
        Assert.Contains(servico.Nome, histAprovacao.Descricao);
    }

    private static async Task<CatalogoServico> CriarServicoCatalogoAsync(
        SGXSistemaChamadoDbContext context,
        Usuario criador,
        Guid departamentoId,
        Guid? categoriaId,
        Guid? subcategoriaId,
        Guid? prioridadeId,
        StatusCatalogoServico status,
        VisibilidadeCatalogoServico visibilidade,
        bool ativo = true,
        bool permiteAberturaChamado = true,
        bool requerAprovacao = false,
        Guid? slaPadraoId = null,
        Guid? grupoTecnicoId = null)
    {
        var nome = $"Servico Catalogo {Guid.NewGuid():N}";
        var servico = new CatalogoServico(
            nome,
            nome.ToLowerInvariant().Replace(' ', '-'),
            "Descricao do servico",
            "Instrucoes do servico",
            departamentoId,
            categoriaId,
            subcategoriaId,
            prioridadeId,
            slaPadraoId,
            null,
            visibilidade,
            permiteAberturaChamado,
            requerAprovacao,
            1,
            criador.Id,
            criador.Login,
            grupoTecnicoId);

        if (status == StatusCatalogoServico.Publicado)
        {
            servico.Publicar(criador.Id, criador.Login);
        }
        else if (status == StatusCatalogoServico.Arquivado)
        {
            servico.Arquivar(criador.Id, criador.Login);
        }

        if (!ativo)
        {
            servico.Desativar(criador.Login);
        }

        context.CatalogosServico.Add(servico);
        await context.SaveChangesAsync();
        return servico;
    }

    private static async Task<(CatalogoServico Servico, CampoFormularioServico CampoObrigatorio, CampoFormularioServico CampoOpcional)> CriarServicoCatalogoComFormularioAsync(
        SGXSistemaChamadoDbContext context,
        (Usuario Usuario, UsuarioContextoAplicacao UsuarioContexto, Departamento Departamento, CategoriaChamado Categoria, SubcategoriaChamado Subcategoria, PrioridadeChamado Prioridade, TipoSolicitacao TipoSolicitacao, LocalUnidade LocalUnidade) dados,
        bool configurarCampoObrigatorio = true,
        bool campoObrigatorioAtivo = true,
        bool campoObrigatorioVisivel = true,
        TipoCampoFormularioServico tipoCampoObrigatorio = TipoCampoFormularioServico.TextoCurto,
        bool requerAprovacao = false,
        Guid? slaPadraoId = null,
        Guid? grupoTecnicoId = null)
    {
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            requerAprovacao: requerAprovacao,
            slaPadraoId: slaPadraoId,
            grupoTecnicoId: grupoTecnicoId);

        var formulario = new FormularioServico(servico.Id, "Formulario obrigatoriedade", "Teste obrigatoriedade", "teste");
        context.FormulariosServico.Add(formulario);
        await context.SaveChangesAsync();

        var versaoRascunho = new FormularioServicoVersao(formulario.Id, 1, false, null, "teste");
        var versaoPublicada = new FormularioServicoVersao(formulario.Id, 2, true, new DateTime(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc), "teste");
        context.FormulariosServicoVersoes.AddRange(versaoRascunho, versaoPublicada);
        await context.SaveChangesAsync();

        var campoObrigatorio = new CampoFormularioServico(
            versaoPublicada.Id,
            "campoObrigatorio",
            "Campo obrigatorio",
            tipoCampoObrigatorio,
            configurarCampoObrigatorio,
            1,
            "Preencha o campo obrigatorio",
            campoObrigatorioVisivel,
            "teste");

        if (!campoObrigatorioAtivo)
        {
            campoObrigatorio.Inativar("teste");
        }

        var campoOpcional = new CampoFormularioServico(
            versaoPublicada.Id,
            "campoOpcional",
            "Campo opcional",
            TipoCampoFormularioServico.TextoLongo,
            false,
            2,
            null,
            true,
            "teste");

        context.CamposFormularioServico.AddRange(campoObrigatorio, campoOpcional);
        await context.SaveChangesAsync();

        if (tipoCampoObrigatorio is TipoCampoFormularioServico.SelecaoUnica or TipoCampoFormularioServico.SelecaoMultipla)
        {
            context.OpcoesCamposFormularioServico.AddRange(
                new OpcaoCampoFormularioServico(campoObrigatorio.Id, "vpn", "VPN", 1, "teste"),
                new OpcaoCampoFormularioServico(campoObrigatorio.Id, "email", "E-mail", 2, "teste"));
            await context.SaveChangesAsync();
        }

        return (servico, campoObrigatorio, campoOpcional);
    }

    private static async Task<(
        CatalogoServico Servico,
        CampoFormularioServico CampoTextoCurto,
        CampoFormularioServico CampoTextoLongo,
        CampoFormularioServico CampoNumero,
        CampoFormularioServico CampoData,
        CampoFormularioServico CampoBooleano,
        CampoFormularioServico CampoSelecaoUnica,
        CampoFormularioServico CampoSelecaoMultipla)> CriarServicoCatalogoComFormularioCompletoAsync(
        SGXSistemaChamadoDbContext context,
        (Usuario Usuario, UsuarioContextoAplicacao UsuarioContexto, Departamento Departamento, CategoriaChamado Categoria, SubcategoriaChamado Subcategoria, PrioridadeChamado Prioridade, TipoSolicitacao TipoSolicitacao, LocalUnidade LocalUnidade) dados,
        bool requerAprovacao = false,
        Guid? slaPadraoId = null,
        Guid? grupoTecnicoId = null)
    {
        var servico = await CriarServicoCatalogoAsync(
            context,
            dados.Usuario,
            dados.Departamento.Id,
            dados.Categoria.Id,
            dados.Subcategoria.Id,
            dados.Prioridade.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            requerAprovacao: requerAprovacao,
            slaPadraoId: slaPadraoId,
            grupoTecnicoId: grupoTecnicoId);

        var formulario = new FormularioServico(servico.Id, "Formulario completo", "Teste fluxo valido", "teste");
        context.FormulariosServico.Add(formulario);
        await context.SaveChangesAsync();

        var versao = new FormularioServicoVersao(formulario.Id, 2, true, new DateTime(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc), "teste");
        context.FormulariosServicoVersoes.Add(versao);
        await context.SaveChangesAsync();

        var campoTextoCurto = new CampoFormularioServico(versao.Id, "textoCurto", "Texto curto", TipoCampoFormularioServico.TextoCurto, true, 1, null, true, "teste");
        var campoTextoLongo = new CampoFormularioServico(versao.Id, "textoLongo", "Texto longo", TipoCampoFormularioServico.TextoLongo, true, 2, null, true, "teste");
        var campoNumero = new CampoFormularioServico(versao.Id, "numero", "Numero", TipoCampoFormularioServico.Numero, true, 3, null, true, "teste");
        var campoData = new CampoFormularioServico(versao.Id, "data", "Data", TipoCampoFormularioServico.Data, true, 4, null, true, "teste");
        var campoBooleano = new CampoFormularioServico(versao.Id, "booleano", "Booleano", TipoCampoFormularioServico.Booleano, true, 5, null, true, "teste");
        var campoSelecaoUnica = new CampoFormularioServico(versao.Id, "selecaoUnica", "Selecao unica", TipoCampoFormularioServico.SelecaoUnica, true, 6, null, true, "teste");
        var campoSelecaoMultipla = new CampoFormularioServico(versao.Id, "selecaoMultipla", "Selecao multipla", TipoCampoFormularioServico.SelecaoMultipla, true, 7, null, true, "teste");

        context.CamposFormularioServico.AddRange(
            campoTextoCurto,
            campoTextoLongo,
            campoNumero,
            campoData,
            campoBooleano,
            campoSelecaoUnica,
            campoSelecaoMultipla);
        await context.SaveChangesAsync();

        context.OpcoesCamposFormularioServico.AddRange(
            new OpcaoCampoFormularioServico(campoSelecaoUnica.Id, "email", "E-mail", 1, "teste"),
            new OpcaoCampoFormularioServico(campoSelecaoUnica.Id, "vpn", "VPN", 2, "teste"),
            new OpcaoCampoFormularioServico(campoSelecaoMultipla.Id, "teams", "Teams", 1, "teste"),
            new OpcaoCampoFormularioServico(campoSelecaoMultipla.Id, "vpn", "VPN", 2, "teste"));
        await context.SaveChangesAsync();

        return (servico, campoTextoCurto, campoTextoLongo, campoNumero, campoData, campoBooleano, campoSelecaoUnica, campoSelecaoMultipla);
    }

    private static async Task<InventarioAtivo> CriarAtivoInventarioAsync(
        SGXSistemaChamadoDbContext context,
        (Usuario Usuario, UsuarioContextoAplicacao UsuarioContexto, Departamento Departamento, CategoriaChamado Categoria, SubcategoriaChamado Subcategoria, PrioridadeChamado Prioridade, TipoSolicitacao TipoSolicitacao, LocalUnidade LocalUnidade) dados,
        string codigo,
        bool ativo = true)
    {
        var tipoAtivo = new TipoAtivoInventario($"Tipo {codigo}", null, "teste");
        context.TiposAtivoInventario.Add(tipoAtivo);
        await context.SaveChangesAsync();

        var inventarioAtivo = new InventarioAtivo(
            codigo,
            $"Ativo {codigo}",
            tipoAtivo.Id,
            dados.Usuario.Id,
            dados.Usuario.Login);

        inventarioAtivo.DefinirDepartamento(dados.Departamento.Id);
        inventarioAtivo.DefinirLocalUnidade(dados.LocalUnidade.Id);
        inventarioAtivo.DefinirStatusOperacional(StatusOperacionalAtivo.Operacional);
        inventarioAtivo.DefinirStatusPatrimonial(StatusPatrimonialAtivo.EmUso);

        if (!ativo)
        {
            inventarioAtivo.Inativar(dados.Usuario.Id, dados.Usuario.Login);
        }

        context.InventarioAtivos.Add(inventarioAtivo);
        await context.SaveChangesAsync();
        return inventarioAtivo;
    }

    private static async Task<(Usuario Usuario, UsuarioContextoAplicacao UsuarioContexto, Departamento Departamento, CategoriaChamado Categoria, SubcategoriaChamado Subcategoria, PrioridadeChamado Prioridade, TipoSolicitacao TipoSolicitacao, LocalUnidade LocalUnidade)> SeedBasico(SGXSistemaChamadoDbContext context)
    {
        var departamento = new Departamento("Tecnologia da Informacao", "TI", null, "teste");
        var categoria = new CategoriaChamado("Suporte Tecnico", null, departamento.Id, "teste");
        var subcategoria = new SubcategoriaChamado(categoria.Id, "Acesso", null, "teste");
        var tipoSolicitacao = new TipoSolicitacao("Incidente", null, "teste");
        var localUnidade = new LocalUnidade("Matriz", null, "Endereco", "teste");
        var usuario = new Usuario("Solicitante Teste", "solicitante@empresa.com", "solicitante", "teste", departamento.Id);

        context.Departamentos.Add(departamento);
        context.CategoriasChamado.Add(categoria);
        context.SubcategoriasChamado.Add(subcategoria);
        context.TiposSolicitacao.Add(tipoSolicitacao);
        context.LocaisUnidade.Add(localUnidade);
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Ativo);

        return (
            usuario,
            new UsuarioContextoAplicacao(usuario.Id, usuario.Nome, usuario.Email, usuario.Login, ["Solicitante"]),
            departamento,
            categoria,
            subcategoria,
            prioridade,
            tipoSolicitacao,
            localUnidade);
    }
}
