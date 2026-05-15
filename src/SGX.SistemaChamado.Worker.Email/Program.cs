using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Email;
using SGX.SistemaChamado.Infrastructure;
using SGX.SistemaChamado.Worker.Email;
using SGX.SistemaChamado.Worker.Email.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
    options.SingleLine = true;
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IUsuarioContextoAplicacaoService, WorkerUsuarioContextoAplicacaoService>();
builder.Services.AddScoped<IAuditoriaContextProvider, WorkerAuditoriaContextProvider>();
builder.Services.AddSingleton<IEmailImapClient, MailKitEmailImapClient>();
builder.Services.AddScoped<EmailIngestionService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
