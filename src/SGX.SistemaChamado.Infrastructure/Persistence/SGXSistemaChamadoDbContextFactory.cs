using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SGX.SistemaChamado.Infrastructure.Persistence;

public sealed class SGXSistemaChamadoDbContextFactory : IDesignTimeDbContextFactory<SGXSistemaChamadoDbContext>
{
    public SGXSistemaChamadoDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var apiPath = Path.GetFullPath(Path.Combine(currentDirectory, "..", "SGX.SistemaChamado.Api"));
        var basePath = Directory.Exists(apiPath) ? apiPath : currentDirectory;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=sgx_sistema_chamados;Username=user_sgxsc;Password=change_me";

        var optionsBuilder = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new SGXSistemaChamadoDbContext(optionsBuilder.Options);
    }
}
