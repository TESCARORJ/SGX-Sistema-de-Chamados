using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence;

public sealed class SGXSistemaChamadoDbContext(DbContextOptions<SGXSistemaChamadoDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<PerfilAcesso> PerfisAcesso => Set<PerfilAcesso>();
    public DbSet<UsuarioPerfilAcesso> UsuariosPerfisAcesso => Set<UsuarioPerfilAcesso>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<CategoriaChamado> CategoriasChamado => Set<CategoriaChamado>();
    public DbSet<PrioridadeChamado> PrioridadesChamado => Set<PrioridadeChamado>();
    public DbSet<StatusChamado> StatusChamado => Set<StatusChamado>();
    public DbSet<Chamado> Chamados => Set<Chamado>();
    public DbSet<HistoricoChamado> HistoricosChamado => Set<HistoricoChamado>();
    public DbSet<ComentarioChamado> ComentariosChamado => Set<ComentarioChamado>();
    public DbSet<AnexoChamado> AnexosChamado => Set<AnexoChamado>();
    public DbSet<SlaConfiguracao> SlaConfiguracoes => Set<SlaConfiguracao>();
    public DbSet<SlaControle> SlaControles => Set<SlaControle>();
    public DbSet<ParametroSistema> ParametrosSistema => Set<ParametroSistema>();
    public DbSet<LogIntegracaoEmail> LogsIntegracaoEmail => Set<LogIntegracaoEmail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SGXSistemaChamadoDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
