using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class RelatoriosAvancadosSprint4AdminUseCasesTests
{
    [Fact]
    public async Task ObterResumoInventarioAtivosAsync_DeveRetornarTotaisAtivosEInativos()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoInventarioAtivosAsync(FiltroInventario(cenario));

        Assert.Equal(3, resumo.TotalAtivos);
        Assert.Equal(2, resumo.AtivosAtivos);
        Assert.Equal(1, resumo.AtivosInativos);
        Assert.Equal(1, resumo.TotalEmManutencao);
        Assert.Equal(1, resumo.TotalComDefeito);
    }

    [Fact]
    public async Task ObterInventarioAtivosPorStatusAsync_DeveRetornarDistribuicaoOperacionalEPatrimonial()
    {
        await using var cenario = await CriarCenarioAsync();

        var distribuicao = await cenario.UseCase.ObterInventarioAtivosPorStatusAsync(FiltroInventario(cenario));

        Assert.Contains(distribuicao.PorStatusOperacional, x => x.Nome == StatusOperacionalAtivo.Operacional.ToString());
        Assert.Contains(distribuicao.PorStatusPatrimonial, x => x.Nome == StatusPatrimonialAtivo.EmUso.ToString());
    }

    [Fact]
    public async Task ObterInventarioAtivosChamadosRecorrentesAsync_DeveRetornarRanking()
    {
        await using var cenario = await CriarCenarioAsync();

        var ranking = await cenario.UseCase.ObterInventarioAtivosChamadosRecorrentesAsync(FiltroInventario(cenario));

        var item = Assert.Single(ranking, x => x.InventarioAtivoId == cenario.AtivoNotebookId);
        Assert.Equal(2, item.TotalChamados);
        Assert.Equal(1, item.ChamadosAbertos);
        Assert.Equal(1, item.ChamadosEncerrados);
    }

    [Fact]
    public async Task ObterInventarioAtivosPorDepartamentoAsync_DeveAgruparPorDepartamento()
    {
        await using var cenario = await CriarCenarioAsync();

        var itens = await cenario.UseCase.ObterInventarioAtivosPorDepartamentoAsync(FiltroInventario(cenario));

        Assert.Contains(itens, x => x.DepartamentoId == cenario.DepartamentoInfraId && x.TotalAtivos >= 1);
        Assert.Contains(itens, x => x.DepartamentoId == cenario.DepartamentoAppsId && x.TotalAtivos >= 1);
    }

    [Fact]
    public async Task ObterResumoInventarioAtivosAsync_DeveFiltrarPorTipoStatusEDepartamento()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoInventarioAtivosAsync(new FiltroRelatorioInventarioAtivosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-30),
            DataFinal = cenario.BaseData,
            TipoAtivoInventarioId = cenario.TipoNotebookId,
            StatusOperacional = StatusOperacionalAtivo.Operacional.ToString(),
            DepartamentoId = cenario.DepartamentoInfraId
        });

        Assert.Equal(1, resumo.TotalAtivos);
        Assert.Equal(1, resumo.AtivosAtivos);
    }

    [Fact]
    public async Task ObterResumoBaseConhecimentoAsync_DeveRetornarTotaisPrincipais()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoBaseConhecimentoAsync(FiltroBase(cenario));

        Assert.Equal(3, resumo.TotalArtigos);
        Assert.Equal(1, resumo.ArtigosPublicados);
        Assert.Equal(1, resumo.ArtigosArquivados);
        Assert.Equal(2, resumo.ChamadosComArtigoVinculado);
    }

    [Fact]
    public async Task ObterBaseConhecimentoPorStatusAsync_DeveAgruparPorStatusEVisibilidade()
    {
        await using var cenario = await CriarCenarioAsync();

        var distribuicao = await cenario.UseCase.ObterBaseConhecimentoPorStatusAsync(FiltroBase(cenario));

        Assert.Contains(distribuicao.PorStatus, x => x.Nome == StatusArtigoConhecimento.Publicado.ToString());
        Assert.Contains(distribuicao.PorVisibilidade, x => x.Nome == VisibilidadeArtigoConhecimento.Atendente.ToString());
    }

    [Fact]
    public async Task ObterBaseConhecimentoVinculosChamadosAsync_DeveRetornarRankingDeVinculos()
    {
        await using var cenario = await CriarCenarioAsync();

        var ranking = await cenario.UseCase.ObterBaseConhecimentoVinculosChamadosAsync(FiltroBase(cenario));

        var item = Assert.Single(ranking, x => x.ArtigoId == cenario.ArtigoPublicadoId);
        Assert.Equal(2, item.TotalChamadosVinculados);
        Assert.NotNull(item.UltimoVinculoEm);
    }

    [Fact]
    public async Task ObterResumoBaseConhecimentoAsync_DeveFiltrarPorStatusCategoriaEVisibilidade()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoBaseConhecimentoAsync(new FiltroRelatorioBaseConhecimentoRequest
        {
            DataInicial = cenario.BaseData.AddDays(-30),
            DataFinal = cenario.BaseData,
            CategoriaId = cenario.CategoriaInfraId,
            StatusArtigo = StatusArtigoConhecimento.Publicado.ToString(),
            VisibilidadeArtigo = VisibilidadeArtigoConhecimento.Atendente.ToString()
        });

        Assert.Equal(1, resumo.TotalArtigos);
        Assert.Equal(1, resumo.ArtigosPublicados);
    }

    [Fact]
    public async Task ObterResumoAuditoriaAsync_DeveRetornarIndicadores()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoAuditoriaAsync(FiltroAuditoria(cenario));

        Assert.Equal(4, resumo.TotalAcoesAuditadas);
        Assert.Equal(2, resumo.UsuariosComAcoes);
        Assert.True(resumo.EntidadesAfetadas >= 2);
        Assert.NotEmpty(resumo.TotalPorTipoAcao);
    }

    [Fact]
    public async Task ObterAuditoriaPorUsuarioAsync_DeveAgruparPorUsuario()
    {
        await using var cenario = await CriarCenarioAsync();

        var itens = await cenario.UseCase.ObterAuditoriaPorUsuarioAsync(FiltroAuditoria(cenario));

        Assert.Contains(itens, x => x.UsuarioId == cenario.AdminId && x.TotalAcoes >= 1);
    }

    [Fact]
    public async Task ObterAuditoriaPorEntidadeAsync_DeveAgruparPorEntidade()
    {
        await using var cenario = await CriarCenarioAsync();

        var itens = await cenario.UseCase.ObterAuditoriaPorEntidadeAsync(FiltroAuditoria(cenario));

        Assert.Contains(itens, x => x.Entidade == "InventarioAtivo");
        Assert.Contains(itens, x => x.Entidade == "BaseConhecimentoArtigo");
    }

    [Fact]
    public async Task ObterResumoAuditoriaAsync_DeveFiltrarPorUsuarioEntidadeETipoAcao()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoAuditoriaAsync(new FiltroRelatorioAuditoriaRequest
        {
            DataInicial = cenario.BaseData.AddDays(-30),
            DataFinal = cenario.BaseData,
            UsuarioId = cenario.AdminId,
            Entidade = "InventarioAtivo",
            TipoAcao = TipoAcaoAuditoria.Criacao.ToString()
        });

        Assert.Equal(1, resumo.TotalAcoesAuditadas);
    }

    [Fact]
    public async Task ObterResumoInventarioAtivosAsync_ComDataInicialMaiorQueDataFinal_DeveFalhar()
    {
        await using var cenario = await CriarCenarioAsync();

        var erro = await Assert.ThrowsAsync<ArgumentException>(() => cenario.UseCase.ObterResumoInventarioAtivosAsync(new FiltroRelatorioInventarioAtivosRequest
        {
            DataInicial = cenario.BaseData,
            DataFinal = cenario.BaseData.AddDays(-1)
        }));

        Assert.Contains("DataInicial", erro.Message);
    }

    [Fact]
    public async Task ObterResumoAuditoriaAsync_ComPeriodoAcimaDoLimite_DeveFalhar()
    {
        await using var cenario = await CriarCenarioAsync();

        var erro = await Assert.ThrowsAsync<ArgumentException>(() => cenario.UseCase.ObterResumoAuditoriaAsync(new FiltroRelatorioAuditoriaRequest
        {
            DataInicial = cenario.BaseData.AddDays(-500),
            DataFinal = cenario.BaseData
        }));

        Assert.Contains("limite maximo", erro.Message);
    }

    [Fact]
    public async Task ObterResumoInventarioAtivosAsync_ComEnumInvalido_DeveFalhar()
    {
        await using var cenario = await CriarCenarioAsync();

        var erro = await Assert.ThrowsAsync<ArgumentException>(() => cenario.UseCase.ObterResumoInventarioAtivosAsync(new FiltroRelatorioInventarioAtivosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-30),
            DataFinal = cenario.BaseData,
            StatusOperacional = "NaoExiste"
        }));

        Assert.Contains("StatusOperacional", erro.Message);
    }

    private static FiltroRelatorioInventarioAtivosRequest FiltroInventario(Cenario cenario)
        => new()
        {
            DataInicial = cenario.BaseData.AddDays(-30),
            DataFinal = cenario.BaseData,
            LimiteRanking = 10
        };

    private static FiltroRelatorioBaseConhecimentoRequest FiltroBase(Cenario cenario)
        => new()
        {
            DataInicial = cenario.BaseData.AddDays(-30),
            DataFinal = cenario.BaseData,
            LimiteRanking = 10
        };

    private static FiltroRelatorioAuditoriaRequest FiltroAuditoria(Cenario cenario)
        => new()
        {
            DataInicial = cenario.BaseData.AddDays(-30),
            DataFinal = cenario.BaseData,
            LimiteRanking = 10
        };

    private static RelatoriosAvancadosAdminUseCases CriarUseCase(SGXSistemaChamadoDbContext context, Usuario usuario)
    {
        IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService = new FakeUsuarioContextoAplicacaoService(
            AdminUseCasesTestFactory.Contexto(usuario, "Administrador"));

        return new RelatoriosAvancadosAdminUseCases(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<CatalogoServico>(context),
            PortalUseCasesTestFactory.Repo<InventarioAtivo>(context),
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            PortalUseCasesTestFactory.Repo<ChamadoArtigoConhecimento>(context),
            PortalUseCasesTestFactory.Repo<EventoAuditoria>(context),
            usuarioContextoAplicacaoService);
    }

    private static async Task<Cenario> CriarCenarioAsync()
    {
        var context = AdminUseCasesTestFactory.CriarContexto();

        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Admin Relatorios Sprint4",
            $"admin.relatorios.sprint4.{Guid.NewGuid():N}@sgx.local",
            TipoPerfil.Administrador);

        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Atendente Sprint4",
            $"atendente.relatorios.sprint4.{Guid.NewGuid():N}@sgx.local",
            TipoPerfil.Atendente);

        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante Sprint4",
            $"solicitante.relatorios.sprint4.{Guid.NewGuid():N}@sgx.local",
            TipoPerfil.Solicitante);

        var departamentoInfra = new Departamento("Infraestrutura Sprint4", "INF4", "Infra", "teste");
        var departamentoApps = new Departamento("Aplicacoes Sprint4", "APP4", "Apps", "teste");
        context.Departamentos.AddRange(departamentoInfra, departamentoApps);

        var categoriaInfra = new CategoriaChamado("Categoria Infra Sprint4", "Categoria Infra", departamentoInfra.Id, "teste");
        var categoriaApps = new CategoriaChamado("Categoria Apps Sprint4", "Categoria Apps", departamentoApps.Id, "teste");
        context.CategoriasChamado.AddRange(categoriaInfra, categoriaApps);

        var tipoNotebook = new TipoAtivoInventario("Notebook", "Tipo notebook", "teste");
        var tipoServidor = new TipoAtivoInventario("Servidor", "Tipo servidor", "teste");
        context.TiposAtivoInventario.AddRange(tipoNotebook, tipoServidor);
        await context.SaveChangesAsync();

        var ativoNotebook = new InventarioAtivo("ATV-001", "Notebook Infra", tipoNotebook.Id, admin.Id, "teste");
        ativoNotebook.DefinirDepartamento(departamentoInfra.Id);
        ativoNotebook.DefinirUsuarioResponsavel(atendente.Id);
        ativoNotebook.DefinirStatusOperacional(StatusOperacionalAtivo.Operacional);
        ativoNotebook.DefinirStatusPatrimonial(StatusPatrimonialAtivo.EmUso);
        ativoNotebook.DefinirCriticidade(CriticidadeAtivo.Alta);

        var ativoServidor = new InventarioAtivo("ATV-002", "Servidor Apps", tipoServidor.Id, admin.Id, "teste");
        ativoServidor.DefinirDepartamento(departamentoApps.Id);
        ativoServidor.DefinirStatusOperacional(StatusOperacionalAtivo.EmManutencao);
        ativoServidor.DefinirStatusPatrimonial(StatusPatrimonialAtivo.EmUso);
        ativoServidor.DefinirCriticidade(CriticidadeAtivo.Critica);
        ativoServidor.Inativar(admin.Id, "teste");

        var ativoSwitch = new InventarioAtivo("ATV-003", "Switch Core", tipoServidor.Id, admin.Id, "teste");
        ativoSwitch.DefinirDepartamento(departamentoInfra.Id);
        ativoSwitch.DefinirStatusOperacional(StatusOperacionalAtivo.ComDefeito);
        ativoSwitch.DefinirStatusPatrimonial(StatusPatrimonialAtivo.EmEstoque);
        ativoSwitch.DefinirCriticidade(CriticidadeAtivo.Media);

        context.InventarioAtivos.AddRange(ativoNotebook, ativoServidor, ativoSwitch);
        await context.SaveChangesAsync();

        var statusAberto = await ObterOuCriarStatusAsync(context, StatusChamadoEnum.Aberto, "Aberto");
        var statusEncerrado = await ObterOuCriarStatusAsync(context, StatusChamadoEnum.Encerrado, "Encerrado");
        var prioridadeAlta = await context.PrioridadesChamado.FirstOrDefaultAsync(x => x.Nivel == PrioridadeChamadoEnum.Alta)
            ?? await context.PrioridadesChamado.FirstAsync();

        var chamado1 = new Chamado("CH-RAV4-001", "Chamado ativo notebook 1", "Descricao", solicitante.Id, categoriaInfra.Id, prioridadeAlta.Id, statusAberto.Id, OrigemChamado.Portal, "teste", categoriaInfra.DepartamentoId, inventarioAtivoId: ativoNotebook.Id);
        var chamado2 = new Chamado("CH-RAV4-002", "Chamado ativo notebook 2", "Descricao", solicitante.Id, categoriaInfra.Id, prioridadeAlta.Id, statusAberto.Id, OrigemChamado.Email, "teste", categoriaInfra.DepartamentoId, inventarioAtivoId: ativoNotebook.Id);
        chamado2.Encerrar(statusEncerrado.Id, "teste");

        context.Chamados.AddRange(chamado1, chamado2);
        await context.SaveChangesAsync();

        var baseData = DateTime.UtcNow.Date;
        DefinirDatasChamado(context, chamado1, baseData.AddDays(-5), null);
        DefinirDatasChamado(context, chamado2, baseData.AddDays(-4), baseData.AddDays(-3));
        context.Entry(ativoNotebook).Property(nameof(InventarioAtivo.CriadoEm)).CurrentValue = baseData.AddDays(-10);
        context.Entry(ativoServidor).Property(nameof(InventarioAtivo.CriadoEm)).CurrentValue = baseData.AddDays(-9);
        context.Entry(ativoSwitch).Property(nameof(InventarioAtivo.CriadoEm)).CurrentValue = baseData.AddDays(-8);

        var artigoPublicado = new BaseConhecimentoArtigo(
            "Artigo Publicado Sprint4",
            $"artigo-publicado-s4-{Guid.NewGuid():N}",
            "Resumo",
            "Conteudo",
            categoriaInfra.Id,
            StatusArtigoConhecimento.Publicado,
            VisibilidadeArtigoConhecimento.Atendente,
            "tag1",
            admin.Id,
            "teste");

        var artigoRascunho = new BaseConhecimentoArtigo(
            "Artigo Rascunho Sprint4",
            $"artigo-rascunho-s4-{Guid.NewGuid():N}",
            "Resumo",
            "Conteudo",
            categoriaApps.Id,
            StatusArtigoConhecimento.Rascunho,
            VisibilidadeArtigoConhecimento.Solicitante,
            "tag2",
            admin.Id,
            "teste");

        var artigoArquivado = new BaseConhecimentoArtigo(
            "Artigo Arquivado Sprint4",
            $"artigo-arquivado-s4-{Guid.NewGuid():N}",
            "Resumo",
            "Conteudo",
            categoriaApps.Id,
            StatusArtigoConhecimento.Arquivado,
            VisibilidadeArtigoConhecimento.Administrador,
            "tag3",
            admin.Id,
            "teste");

        context.BaseConhecimentoArtigos.AddRange(artigoPublicado, artigoRascunho, artigoArquivado);
        await context.SaveChangesAsync();

        context.Entry(artigoPublicado).Property(nameof(BaseConhecimentoArtigo.CriadoEm)).CurrentValue = baseData.AddDays(-7);
        context.Entry(artigoRascunho).Property(nameof(BaseConhecimentoArtigo.CriadoEm)).CurrentValue = baseData.AddDays(-6);
        context.Entry(artigoArquivado).Property(nameof(BaseConhecimentoArtigo.CriadoEm)).CurrentValue = baseData.AddDays(-5);

        var vinculo1 = new ChamadoArtigoConhecimento(chamado1.Id, artigoPublicado.Id, admin.Id, "Vinculo 1", "teste");
        var vinculo2 = new ChamadoArtigoConhecimento(chamado2.Id, artigoPublicado.Id, admin.Id, "Vinculo 2", "teste");
        context.ChamadosArtigosConhecimento.AddRange(vinculo1, vinculo2);
        await context.SaveChangesAsync();

        context.Entry(vinculo1).Property(nameof(ChamadoArtigoConhecimento.VinculadoEm)).CurrentValue = baseData.AddDays(-4).AddHours(10);
        context.Entry(vinculo2).Property(nameof(ChamadoArtigoConhecimento.VinculadoEm)).CurrentValue = baseData.AddDays(-3).AddHours(14);

        var evento1 = new EventoAuditoria(
            baseData.AddDays(-5).AddHours(8),
            admin.Id,
            admin.Nome,
            admin.Email,
            admin.Email,
            "127.0.0.1",
            "test-agent",
            "Inventario",
            "InventarioAtivo",
            ativoNotebook.Id.ToString(),
            TipoAcaoAuditoria.Criacao,
            "Criacao de ativo",
            null,
            null,
            null,
            NivelAuditoria.Informacao,
            true,
            null,
            "corr-1");

        var evento2 = new EventoAuditoria(
            baseData.AddDays(-4).AddHours(9),
            admin.Id,
            admin.Nome,
            admin.Email,
            admin.Email,
            "127.0.0.1",
            "test-agent",
            "BaseConhecimento",
            "BaseConhecimentoArtigo",
            artigoPublicado.Id.ToString(),
            TipoAcaoAuditoria.Edicao,
            "Edicao de artigo",
            null,
            null,
            null,
            NivelAuditoria.Informacao,
            true,
            null,
            "corr-2");

        var evento3 = new EventoAuditoria(
            baseData.AddDays(-3).AddHours(10),
            atendente.Id,
            atendente.Nome,
            atendente.Email,
            atendente.Email,
            "127.0.0.1",
            "test-agent",
            "Chamados",
            "Chamado",
            chamado1.Id.ToString(),
            TipoAcaoAuditoria.Visualizacao,
            "Visualizacao de chamado",
            null,
            null,
            null,
            NivelAuditoria.Informacao,
            true,
            null,
            "corr-3");

        var evento4 = new EventoAuditoria(
            baseData.AddDays(-2).AddHours(11),
            atendente.Id,
            atendente.Nome,
            atendente.Email,
            atendente.Email,
            "127.0.0.1",
            "test-agent",
            "Chamados",
            "Chamado",
            chamado2.Id.ToString(),
            TipoAcaoAuditoria.AlteracaoStatus,
            "Alteracao de status",
            null,
            null,
            null,
            NivelAuditoria.Alerta,
            true,
            null,
            "corr-4");

        context.EventosAuditoria.AddRange(evento1, evento2, evento3, evento4);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, admin);
        return new Cenario(
            context,
            useCase,
            baseData,
            admin.Id,
            departamentoInfra.Id,
            departamentoApps.Id,
            categoriaInfra.Id,
            tipoNotebook.Id,
            ativoNotebook.Id,
            artigoPublicado.Id);
    }

    private static void DefinirDatasChamado(SGXSistemaChamadoDbContext context, Chamado chamado, DateTime abertoEm, DateTime? encerradoEm)
    {
        context.Entry(chamado).Property(nameof(Chamado.AbertoEm)).CurrentValue = abertoEm;
        context.Entry(chamado).Property(nameof(Chamado.EncerradoEm)).CurrentValue = encerradoEm;
    }

    private static async Task<StatusChamado> ObterOuCriarStatusAsync(
        SGXSistemaChamadoDbContext context,
        StatusChamadoEnum codigo,
        string nome)
    {
        var existente = await context.StatusChamado.FirstOrDefaultAsync(x => x.Codigo == codigo);
        if (existente is not null)
        {
            return existente;
        }

        var status = new StatusChamado(nome, codigo, $"Status {nome} para testes.", false, false, "teste");
        context.StatusChamado.Add(status);
        await context.SaveChangesAsync();
        return status;
    }

    private sealed record Cenario(
        SGXSistemaChamadoDbContext Context,
        RelatoriosAvancadosAdminUseCases UseCase,
        DateTime BaseData,
        Guid AdminId,
        Guid DepartamentoInfraId,
        Guid DepartamentoAppsId,
        Guid CategoriaInfraId,
        Guid TipoNotebookId,
        Guid AtivoNotebookId,
        Guid ArtigoPublicadoId) : IAsyncDisposable
    {
        public ValueTask DisposeAsync()
            => Context.DisposeAsync();
    }
}
