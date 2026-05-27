using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

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
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<CatalogoServico>(context),
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

    private static AbrirChamadoUseCase CriarUseCase(SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao usuario)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<CatalogoServico>(context),
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
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<CatalogoServico> CriarServicoCatalogoAsync(
        SGXSistemaChamadoDbContext context,
        Usuario criador,
        Guid departamentoId,
        Guid categoriaId,
        Guid subcategoriaId,
        Guid prioridadeId,
        StatusCatalogoServico status,
        VisibilidadeCatalogoServico visibilidade,
        bool ativo = true,
        bool permiteAberturaChamado = true,
        bool requerAprovacao = false)
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
            null,
            null,
            visibilidade,
            permiteAberturaChamado,
            requerAprovacao,
            1,
            criador.Id,
            criador.Login);

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
