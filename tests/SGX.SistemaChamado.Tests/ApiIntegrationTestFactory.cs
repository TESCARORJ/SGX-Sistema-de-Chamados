using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class ApiIntegrationTestFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"sgx-integration-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Port=5432;Database=sgx_dummy;Username=sgx;Password=sgx",
                ["Authentication:ModoLocalHabilitado"] = "true",
                ["Authentication:AdminLocalEmail"] = "admin.local@sgx.local",
                ["Authentication:AdminLocalNome"] = "Administrador Local",
                ["AzureAd:TenantId"] = "",
                ["AzureAd:ClientId"] = "",
                ["AzureAd:Audience"] = "",
                ["AzureAd:Issuer"] = "",
                ["Swagger:EnableInNonDevelopment"] = "false"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<SGXSistemaChamadoDbContext>));
            services.RemoveAll(typeof(SGXSistemaChamadoDbContext));
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<SGXSistemaChamadoDbContext>));

            services.AddDbContext<SGXSistemaChamadoDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
            dbContext.Database.EnsureCreated();
        });
    }

    public async Task SeedPortalChamadosAsync(string emailSolicitanteA, string emailSolicitanteB, CancellationToken cancellationToken = default)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var usuarioA = await dbContext.Usuarios.FirstAsync(x => x.Email == emailSolicitanteA, cancellationToken);
        var usuarioB = await dbContext.Usuarios.FirstAsync(x => x.Email == emailSolicitanteB, cancellationToken);

        var departamento = await dbContext.Departamentos.FirstOrDefaultAsync(cancellationToken);
        if (departamento is null)
        {
            departamento = new Departamento("Tecnologia da Informacao", "TI", "Departamento tecnico de testes.", "integration-test");
            dbContext.Departamentos.Add(departamento);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(cancellationToken);
        if (categoria is null)
        {
            categoria = new CategoriaChamado("Suporte Tecnico", "Categoria para testes de integracao.", departamento.Id, "integration-test");
            dbContext.CategoriasChamado.Add(categoria);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var prioridade = await dbContext.PrioridadesChamado
            .FirstOrDefaultAsync(x => x.Id == SeedData.PrioridadeMediaId || x.Nivel == PrioridadeChamadoEnum.Media, cancellationToken);
        if (prioridade is null)
        {
            prioridade = new PrioridadeChamado("Media", PrioridadeChamadoEnum.Media, "Prioridade de teste.", 4, 24, "integration-test");
            dbContext.PrioridadesChamado.Add(prioridade);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var status = await dbContext.StatusChamado
            .FirstOrDefaultAsync(x => x.Id == SeedData.StatusAbertoId || x.Codigo == StatusChamadoEnum.Aberto, cancellationToken);
        if (status is null)
        {
            status = new StatusChamado("Aberto", StatusChamadoEnum.Aberto, "Status inicial de teste.", false, false, "integration-test");
            dbContext.StatusChamado.Add(status);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (!await dbContext.Chamados.AnyAsync(x => x.SolicitanteId == usuarioA.Id, cancellationToken))
        {
            dbContext.Chamados.Add(new Chamado(
                "SGX-2026-900001",
                "Chamado do solicitante A",
                "Descricao A",
                usuarioA.Id,
                categoria.Id,
                prioridade.Id,
                status.Id,
                OrigemChamado.Portal,
                "integration-test"));
        }

        if (!await dbContext.Chamados.AnyAsync(x => x.SolicitanteId == usuarioB.Id, cancellationToken))
        {
            dbContext.Chamados.Add(new Chamado(
                "SGX-2026-900002",
                "Chamado do solicitante B",
                "Descricao B",
                usuarioB.Id,
                categoria.Id,
                prioridade.Id,
                status.Id,
                OrigemChamado.Portal,
                "integration-test"));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
