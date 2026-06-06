using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence;

public sealed class SGXSistemaChamadoDbContext(DbContextOptions<SGXSistemaChamadoDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<PerfilAcesso> PerfisAcesso => Set<PerfilAcesso>();
    public DbSet<UsuarioPerfilAcesso> UsuariosPerfisAcesso => Set<UsuarioPerfilAcesso>();
    public DbSet<PermissaoSistema> PermissoesSistema => Set<PermissaoSistema>();
    public DbSet<PerfilAcessoPermissao> PerfisAcessoPermissoes => Set<PerfilAcessoPermissao>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<CategoriaChamado> CategoriasChamado => Set<CategoriaChamado>();
    public DbSet<SubcategoriaChamado> SubcategoriasChamado => Set<SubcategoriaChamado>();
    public DbSet<PrioridadeChamado> PrioridadesChamado => Set<PrioridadeChamado>();
    public DbSet<TipoSolicitacao> TiposSolicitacao => Set<TipoSolicitacao>();
    public DbSet<LocalUnidade> LocaisUnidade => Set<LocalUnidade>();
    public DbSet<GrupoTecnico> GruposTecnicos => Set<GrupoTecnico>();
    public DbSet<MembroGrupoTecnico> MembrosGruposTecnicos => Set<MembroGrupoTecnico>();
    public DbSet<FilaAtendimento> FilasAtendimento => Set<FilaAtendimento>();
    public DbSet<StatusChamado> StatusChamado => Set<StatusChamado>();
    public DbSet<Chamado> Chamados => Set<Chamado>();
    public DbSet<ChamadoRelacionamento> ChamadosRelacionamentos => Set<ChamadoRelacionamento>();
    public DbSet<ChamadoTarefa> ChamadosTarefas => Set<ChamadoTarefa>();
    public DbSet<AprovacaoChamado> AprovacoesChamado => Set<AprovacaoChamado>();
    public DbSet<HistoricoChamado> HistoricosChamado => Set<HistoricoChamado>();
    public DbSet<ComentarioChamado> ComentariosChamado => Set<ComentarioChamado>();
    public DbSet<AnexoChamado> AnexosChamado => Set<AnexoChamado>();
    public DbSet<BaseConhecimentoArtigo> BaseConhecimentoArtigos => Set<BaseConhecimentoArtigo>();
    public DbSet<ChamadoArtigoConhecimento> ChamadosArtigosConhecimento => Set<ChamadoArtigoConhecimento>();
    public DbSet<CatalogoServico> CatalogosServico => Set<CatalogoServico>();
    public DbSet<SlaConfiguracao> SlaConfiguracoes => Set<SlaConfiguracao>();
    public DbSet<SlaControle> SlaControles => Set<SlaControle>();
    public DbSet<ChamadoSla> ChamadosSla => Set<ChamadoSla>();
    public DbSet<PoliticaSla> SlaPoliticas => Set<PoliticaSla>();
    public DbSet<MetaSla> SlaMetas => Set<MetaSla>();
    public DbSet<ConfiguracaoAlertaSla> ConfiguracoesAlertaSla => Set<ConfiguracaoAlertaSla>();
    public DbSet<EventoSla> EventosSla => Set<EventoSla>();
    public DbSet<CalendarioCorporativo> CalendariosCorporativos => Set<CalendarioCorporativo>();
    public DbSet<HorarioAtendimentoCalendario> HorariosAtendimentoCalendario => Set<HorarioAtendimentoCalendario>();
    public DbSet<ExcecaoCalendarioCorporativo> ExcecoesCalendarioCorporativo => Set<ExcecaoCalendarioCorporativo>();
    public DbSet<ParametroSistema> ParametrosSistema => Set<ParametroSistema>();
    public DbSet<LogIntegracaoEmail> LogsIntegracaoEmail => Set<LogIntegracaoEmail>();
    public DbSet<TipoAtivoInventario> TiposAtivoInventario => Set<TipoAtivoInventario>();
    public DbSet<InventarioAtivo> InventarioAtivos => Set<InventarioAtivo>();
    public DbSet<HistoricoInventarioAtivo> HistoricosInventarioAtivo => Set<HistoricoInventarioAtivo>();
    public DbSet<RoadmapItsmItem> RoadmapItsmItens => Set<RoadmapItsmItem>();
    public DbSet<RoadmapCategoria> RoadmapCategorias => Set<RoadmapCategoria>();
    public DbSet<RoadmapChecklistItem> RoadmapChecklistItens => Set<RoadmapChecklistItem>();
    public DbSet<RoadmapImplementacaoFutura> RoadmapImplementacoesFuturas => Set<RoadmapImplementacaoFutura>();
    public DbSet<TokenRecuperacaoSenha> TokensRecuperacaoSenha => Set<TokenRecuperacaoSenha>();
    public DbSet<EventoAuditoria> EventosAuditoria => Set<EventoAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SGXSistemaChamadoDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
