using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Email;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Application.Options;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.Services.Email;
using SGX.SistemaChamado.Application.Services.Sla;
using SGX.SistemaChamado.Application.UseCases;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Infrastructure.Persistence;
using SGX.SistemaChamado.Infrastructure.Repositories;
using SGX.SistemaChamado.Infrastructure.Storage;

namespace SGX.SistemaChamado.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("A connection string 'DefaultConnection' nao foi configurada.");

        var arquivosOptions = new ArquivosOptions();
        configuration.GetSection(ArquivosOptions.SectionName).Bind(arquivosOptions);
        var emailWorkerOptions = new EmailWorkerOptions();
        configuration.GetSection(EmailWorkerOptions.SectionName).Bind(emailWorkerOptions);

        services.AddSingleton(arquivosOptions);
        services.AddSingleton<IOptions<ArquivosOptions>>(Options.Create(arquivosOptions));
        services.AddSingleton(emailWorkerOptions);
        services.AddSingleton<IOptions<EmailWorkerOptions>>(Options.Create(emailWorkerOptions));
        services.AddDbContext<SGXSistemaChamadoDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IArquivoStorageService, LocalArquivoStorageService>();
        services.AddScoped<ICodigoChamadoService, CodigoChamadoService>();
        services.AddScoped<IEmailCorrelationService, EmailCorrelationService>();
        services.AddScoped<IEmailMessageProcessor, EmailMessageProcessor>();
        services.AddScoped<ISlaCalculator, SlaCalculator>();
        services.AddScoped<ISlaService, SlaService>();

        services.AddScoped<IApiInfoUseCase, ApiInfoUseCase>();
        services.AddScoped<IAbrirChamadoUseCase, AbrirChamadoUseCase>();
        services.AddScoped<IListarMeusChamadosUseCase, ListarMeusChamadosUseCase>();
        services.AddScoped<IDetalharMeuChamadoUseCase, DetalharMeuChamadoUseCase>();
        services.AddScoped<IComentarChamadoUseCase, ComentarChamadoUseCase>();
        services.AddScoped<IAnexarArquivoChamadoUseCase, AnexarArquivoChamadoUseCase>();
        services.AddScoped<IObterPortalContextoUseCase, ObterPortalContextoUseCase>();
        services.AddScoped<IObterAdminContextoUseCase, ObterAdminContextoUseCase>();
        services.AddScoped<IListarChamadosAdminUseCase, ListarChamadosAdminUseCase>();
        services.AddScoped<IDetalharChamadoAdminUseCase, DetalharChamadoAdminUseCase>();
        services.AddScoped<IAssumirChamadoUseCase, AssumirChamadoUseCase>();
        services.AddScoped<IAtribuirChamadoUseCase, AtribuirChamadoUseCase>();
        services.AddScoped<IAlterarStatusChamadoUseCase, AlterarStatusChamadoUseCase>();
        services.AddScoped<IAlterarPrioridadeChamadoUseCase, AlterarPrioridadeChamadoUseCase>();
        services.AddScoped<IAlterarCategoriaChamadoUseCase, AlterarCategoriaChamadoUseCase>();
        services.AddScoped<IComentarChamadoAdminUseCase, ComentarChamadoAdminUseCase>();
        services.AddScoped<IEncerrarChamadoUseCase, EncerrarChamadoUseCase>();
        services.AddScoped<IReabrirChamadoUseCase, ReabrirChamadoUseCase>();
        services.AddScoped<IObterDashboardAdminUseCase, AdminIndicadoresUseCases>();
        services.AddScoped<IObterIndicadoresChamadosPorStatusUseCase, AdminIndicadoresUseCases>();
        services.AddScoped<IObterIndicadoresChamadosPorPrioridadeUseCase, AdminIndicadoresUseCases>();
        services.AddScoped<IObterIndicadoresChamadosPorCategoriaUseCase, AdminIndicadoresUseCases>();
        services.AddScoped<IObterIndicadoresSlaUseCase, AdminIndicadoresUseCases>();
        services.AddScoped<IObterIndicadoresProdutividadeUseCase, AdminIndicadoresUseCases>();
        services.AddScoped<IListarLogsIntegracaoEmailUseCase, ListarLogsIntegracaoEmailUseCase>();
        services.AddScoped<IObterLogIntegracaoEmailUseCase, ObterLogIntegracaoEmailUseCase>();
        services.AddScoped<IListarUsuariosAdminUseCase, ListarUsuariosAdminUseCase>();
        services.AddScoped<IObterUsuarioAdminUseCase, ObterUsuarioAdminUseCase>();
        services.AddScoped<ICriarUsuarioAdminUseCase, CriarUsuarioAdminUseCase>();
        services.AddScoped<IAtualizarUsuarioAdminUseCase, AtualizarUsuarioAdminUseCase>();
        services.AddScoped<IInativarUsuarioAdminUseCase, InativarUsuarioAdminUseCase>();
        services.AddScoped<IReativarUsuarioAdminUseCase, ReativarUsuarioAdminUseCase>();
        services.AddScoped<IAlterarPerfisUsuarioUseCase, AlterarPerfisUsuarioUseCase>();
        services.AddScoped<IListarPerfisAcessoUseCase, ListarPerfisAcessoUseCase>();
        services.AddScoped<IObterPerfilAcessoUseCase, ObterPerfilAcessoUseCase>();
        services.AddScoped<ICriarPerfilAcessoUseCase, CriarPerfilAcessoUseCase>();
        services.AddScoped<IAtualizarPerfilAcessoUseCase, AtualizarPerfilAcessoUseCase>();
        services.AddScoped<IInativarPerfilAcessoUseCase, InativarPerfilAcessoUseCase>();
        services.AddScoped<IReativarPerfilAcessoUseCase, ReativarPerfilAcessoUseCase>();
        services.AddScoped<IListarDepartamentosAdminUseCase, ListarDepartamentosAdminUseCase>();
        services.AddScoped<IObterDepartamentoAdminUseCase, ObterDepartamentoAdminUseCase>();
        services.AddScoped<ICriarDepartamentoUseCase, CriarDepartamentoUseCase>();
        services.AddScoped<IAtualizarDepartamentoUseCase, AtualizarDepartamentoUseCase>();
        services.AddScoped<IInativarDepartamentoUseCase, InativarDepartamentoUseCase>();
        services.AddScoped<IReativarDepartamentoUseCase, ReativarDepartamentoUseCase>();
        services.AddScoped<IListarCategoriasAdminUseCase, ListarCategoriasAdminUseCase>();
        services.AddScoped<IObterCategoriaAdminUseCase, ObterCategoriaAdminUseCase>();
        services.AddScoped<ICriarCategoriaUseCase, CriarCategoriaUseCase>();
        services.AddScoped<IAtualizarCategoriaUseCase, AtualizarCategoriaUseCase>();
        services.AddScoped<IInativarCategoriaUseCase, InativarCategoriaUseCase>();
        services.AddScoped<IReativarCategoriaUseCase, ReativarCategoriaUseCase>();
        services.AddScoped<IListarPrioridadesAdminUseCase, ListarPrioridadesAdminUseCase>();
        services.AddScoped<IObterPrioridadeAdminUseCase, ObterPrioridadeAdminUseCase>();
        services.AddScoped<ICriarPrioridadeUseCase, CriarPrioridadeUseCase>();
        services.AddScoped<IAtualizarPrioridadeUseCase, AtualizarPrioridadeUseCase>();
        services.AddScoped<IInativarPrioridadeUseCase, InativarPrioridadeUseCase>();
        services.AddScoped<IReativarPrioridadeUseCase, ReativarPrioridadeUseCase>();
        services.AddScoped<IListarStatusAdminUseCase, ListarStatusAdminUseCase>();
        services.AddScoped<IObterStatusAdminUseCase, ObterStatusAdminUseCase>();
        services.AddScoped<ICriarStatusUseCase, CriarStatusUseCase>();
        services.AddScoped<IAtualizarStatusUseCase, AtualizarStatusUseCase>();
        services.AddScoped<IInativarStatusUseCase, InativarStatusUseCase>();
        services.AddScoped<IReativarStatusUseCase, ReativarStatusUseCase>();
        services.AddScoped<IListarParametrosSistemaUseCase, ListarParametrosSistemaUseCase>();
        services.AddScoped<IObterParametroSistemaUseCase, ObterParametroSistemaUseCase>();
        services.AddScoped<ICriarParametroSistemaUseCase, CriarParametroSistemaUseCase>();
        services.AddScoped<IAtualizarParametroSistemaUseCase, AtualizarParametroSistemaUseCase>();
        services.AddScoped<IInativarParametroSistemaUseCase, InativarParametroSistemaUseCase>();
        services.AddScoped<IReativarParametroSistemaUseCase, ReativarParametroSistemaUseCase>();

        return services;
    }
}
