using Serilog;
using SGX.SistemaChamado.Api.Extensions;
using SGX.SistemaChamado.Api.Middlewares;
using SGX.SistemaChamado.Api.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args)
    .AddStructuredLogging();

builder.Services.AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();
var startupLogger = app.Logger;

using (var scope = app.Services.CreateScope())
{
    try
    {
        startupLogger.LogInformation("Inicializando banco de dados do SGX Sistema de Chamados.");

        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        if (dbContext.Database.IsRelational())
        {
            startupLogger.LogInformation("Aplicando migrations pendentes (banco relacional).");
            await dbContext.Database.MigrateAsync();
        }
        else
        {
            startupLogger.LogInformation("Garantindo criacao do banco (provider nao relacional).");
            await dbContext.Database.EnsureCreatedAsync();
        }

        var administradorInicialService = scope.ServiceProvider.GetRequiredService<IAdministradorInicialService>();
        await administradorInicialService.SeedAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentSeedService>();
        await seeder.SeedAsync();

        startupLogger.LogInformation("Inicializacao de banco e seeds concluida com sucesso.");
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("PendingModelChangesWarning", StringComparison.Ordinal))
    {
        startupLogger.LogCritical(
            ex,
            "Falha ao aplicar migrations: EF Core detectou PendingModelChangesWarning. " +
            "Valide migrations pendentes com 'dotnet ef migrations has-pending-model-changes' e, " +
            "se nao houver diferenca de modelo, verifique assemblies travados (DLL/PDB) por debugger/API ativa.");
        throw;
    }
    catch (Exception ex)
    {
        startupLogger.LogCritical(ex, "Falha na inicializacao de banco/seeds durante startup da API.");
        throw;
    }
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionMiddleware>();

var swaggerOutsideDevelopmentHabilitado = builder.Configuration.GetValue<bool>("Swagger:EnableInNonDevelopment");
if (app.Environment.IsDevelopment() || swaggerOutsideDevelopmentHabilitado)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors(ServiceCollectionExtensions.AppCorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapControllers();

app.Run();

public partial class Program;
