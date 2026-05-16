using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

[Collection("EnvironmentVariables")]
public sealed class AdministradorInicialServiceTests
{
    private const string VariavelEmail = "SGX_ADMIN_INICIAL_EMAIL";
    private const string VariavelSenha = "SGX_ADMIN_INICIAL_SENHA";
    private const string VariavelNome = "SGX_ADMIN_INICIAL_NOME";
    private static readonly SemaphoreSlim MutexAmbiente = new(1, 1);

    [Fact]
    public async Task CriaAdministradorInicialQuandoNaoExisteAdministradorAtivoEVariaveisExistem()
    {
        await MutexAmbiente.WaitAsync();
        try
        {
            using var contexto = PortalUseCasesTestFactory.CriarContexto();
            var logger = new TestLogger<AdministradorInicialService>();
            var service = CriarService(contexto, logger);

            using var _ = new EscopoVariaveisAmbiente(new Dictionary<string, string?>
            {
                [VariavelEmail] = "admin.inicial@empresa.com",
                [VariavelSenha] = "Senha@Inicial123",
                [VariavelNome] = "Administrador Inicial"
            });

            await service.SeedAsync();

            var usuario = await contexto.Usuarios
                .Include(x => x.UsuarioPerfis)
                .ThenInclude(x => x.PerfilAcesso)
                .SingleAsync(x => x.Email == "admin.inicial@empresa.com");

            Assert.True(usuario.Ativo);
            Assert.Equal(SituacaoUsuario.Ativo, usuario.Situacao);
            Assert.True(usuario.DeveAlterarSenha);
            Assert.Contains(usuario.UsuarioPerfis, x => x.PerfilAcesso.TipoPerfil == TipoPerfil.Administrador);
            Assert.Contains(logger.Messages, x => x.Contains("Administrador inicial criado a partir de variáveis de ambiente.", StringComparison.Ordinal));
        }
        finally
        {
            MutexAmbiente.Release();
        }
    }

    [Fact]
    public async Task NaoCriaAdministradorQuandoVariaveisNaoExistem()
    {
        await MutexAmbiente.WaitAsync();
        try
        {
            using var contexto = PortalUseCasesTestFactory.CriarContexto();
            var logger = new TestLogger<AdministradorInicialService>();
            var service = CriarService(contexto, logger);

            using var _ = new EscopoVariaveisAmbiente(new Dictionary<string, string?>
            {
                [VariavelEmail] = null,
                [VariavelSenha] = null,
                [VariavelNome] = null
            });

            await service.SeedAsync();

            Assert.Equal(0, await contexto.Usuarios.CountAsync());
        }
        finally
        {
            MutexAmbiente.Release();
        }
    }

    [Fact]
    public async Task NaoCriaSegundoAdministradorQuandoJaExisteAdministradorAtivo()
    {
        await MutexAmbiente.WaitAsync();
        try
        {
            using var contexto = PortalUseCasesTestFactory.CriarContexto();
            var logger = new TestLogger<AdministradorInicialService>();
            var service = CriarService(contexto, logger);

            await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
                contexto,
                "Admin Existente",
                "admin.existente@empresa.com",
                TipoPerfil.Administrador);

            using var _ = new EscopoVariaveisAmbiente(new Dictionary<string, string?>
            {
                [VariavelEmail] = "admin.novo@empresa.com",
                [VariavelSenha] = "Senha@Inicial123",
                [VariavelNome] = "Novo Admin"
            });

            await service.SeedAsync();

            Assert.Null(await contexto.Usuarios.FirstOrDefaultAsync(x => x.Email == "admin.novo@empresa.com"));
        }
        finally
        {
            MutexAmbiente.Release();
        }
    }

    [Fact]
    public async Task SenhaDoAdministradorInicialEhHasheadaENaoEhTextoPuro()
    {
        await MutexAmbiente.WaitAsync();
        try
        {
            using var contexto = PortalUseCasesTestFactory.CriarContexto();
            var logger = new TestLogger<AdministradorInicialService>();
            var service = CriarService(contexto, logger);
            const string senha = "Senha@Inicial123";

            using var _ = new EscopoVariaveisAmbiente(new Dictionary<string, string?>
            {
                [VariavelEmail] = "admin.hash@empresa.com",
                [VariavelSenha] = senha,
                [VariavelNome] = "Admin Hash"
            });

            await service.SeedAsync();

            var usuario = await contexto.Usuarios.SingleAsync(x => x.Email == "admin.hash@empresa.com");
            Assert.False(string.IsNullOrWhiteSpace(usuario.SenhaHashLocal));
            Assert.NotEqual(senha, usuario.SenhaHashLocal);

            var hasher = new PasswordHasher<Usuario>();
            var verificacao = hasher.VerifyHashedPassword(usuario, usuario.SenhaHashLocal!, senha);
            Assert.NotEqual(PasswordVerificationResult.Failed, verificacao);
        }
        finally
        {
            MutexAmbiente.Release();
        }
    }

    [Fact]
    public async Task NaoCriaAdministradorComSenhaFraca()
    {
        await MutexAmbiente.WaitAsync();
        try
        {
            using var contexto = PortalUseCasesTestFactory.CriarContexto();
            var logger = new TestLogger<AdministradorInicialService>();
            var service = CriarService(contexto, logger);

            using var _ = new EscopoVariaveisAmbiente(new Dictionary<string, string?>
            {
                [VariavelEmail] = "admin.fraca@empresa.com",
                [VariavelSenha] = "123456",
                [VariavelNome] = "Admin Fraco"
            });

            await service.SeedAsync();

            Assert.Null(await contexto.Usuarios.FirstOrDefaultAsync(x => x.Email == "admin.fraca@empresa.com"));
            Assert.Contains(logger.Messages, x => x.Contains("Senha do Administrador inicial rejeitada", StringComparison.Ordinal));
        }
        finally
        {
            MutexAmbiente.Release();
        }
    }

    [Fact]
    public async Task NaoCriaAdministradorComEmailInvalido()
    {
        await MutexAmbiente.WaitAsync();
        try
        {
            using var contexto = PortalUseCasesTestFactory.CriarContexto();
            var logger = new TestLogger<AdministradorInicialService>();
            var service = CriarService(contexto, logger);

            using var _ = new EscopoVariaveisAmbiente(new Dictionary<string, string?>
            {
                [VariavelEmail] = "email-invalido",
                [VariavelSenha] = "Senha@Inicial123",
                [VariavelNome] = "Admin Invalido"
            });

            await service.SeedAsync();

            Assert.Equal(0, await contexto.Usuarios.CountAsync());
        }
        finally
        {
            MutexAmbiente.Release();
        }
    }

    [Fact]
    public async Task NaoCriaAdministradorComNomeVazio()
    {
        await MutexAmbiente.WaitAsync();
        try
        {
            using var contexto = PortalUseCasesTestFactory.CriarContexto();
            var logger = new TestLogger<AdministradorInicialService>();
            var service = CriarService(contexto, logger);

            using var _ = new EscopoVariaveisAmbiente(new Dictionary<string, string?>
            {
                [VariavelEmail] = "admin.semnome@empresa.com",
                [VariavelSenha] = "Senha@Inicial123",
                [VariavelNome] = "   "
            });

            await service.SeedAsync();

            Assert.Equal(0, await contexto.Usuarios.CountAsync());
        }
        finally
        {
            MutexAmbiente.Release();
        }
    }

    [Fact]
    public async Task PerfilAdministradorEhAssociadoCorretamente()
    {
        await MutexAmbiente.WaitAsync();
        try
        {
            using var contexto = PortalUseCasesTestFactory.CriarContexto();
            var logger = new TestLogger<AdministradorInicialService>();
            var service = CriarService(contexto, logger);

            using var _ = new EscopoVariaveisAmbiente(new Dictionary<string, string?>
            {
                [VariavelEmail] = "admin.perfil@empresa.com",
                [VariavelSenha] = "Senha@Inicial123",
                [VariavelNome] = "Admin Perfil"
            });

            await service.SeedAsync();

            var vinculo = await contexto.UsuariosPerfisAcesso
                .Include(x => x.PerfilAcesso)
                .Include(x => x.Usuario)
                .SingleAsync(x => x.Usuario.Email == "admin.perfil@empresa.com");

            Assert.Equal(TipoPerfil.Administrador, vinculo.PerfilAcesso.TipoPerfil);
        }
        finally
        {
            MutexAmbiente.Release();
        }
    }

    [Fact]
    public async Task GarantePerfilAdministradorAtivoAntesDaCriacao()
    {
        await MutexAmbiente.WaitAsync();
        try
        {
            using var contexto = PortalUseCasesTestFactory.CriarContexto();
            var perfilAdmin = await contexto.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Administrador);
            perfilAdmin.Desativar("teste");
            contexto.PerfisAcesso.Update(perfilAdmin);
            await contexto.SaveChangesAsync();

            var logger = new TestLogger<AdministradorInicialService>();
            var service = CriarService(contexto, logger);

            using var _ = new EscopoVariaveisAmbiente(new Dictionary<string, string?>
            {
                [VariavelEmail] = "admin.reativa@empresa.com",
                [VariavelSenha] = "Senha@Inicial123",
                [VariavelNome] = "Admin Reativa"
            });

            await service.SeedAsync();

            var perfilAtualizado = await contexto.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Administrador);
            Assert.True(perfilAtualizado.Ativo);
        }
        finally
        {
            MutexAmbiente.Release();
        }
    }

    [Fact]
    public async Task NaoAceitaSenhaAdmin123456ComoAdministradorInicial()
    {
        await MutexAmbiente.WaitAsync();
        try
        {
            using var contexto = PortalUseCasesTestFactory.CriarContexto();
            var logger = new TestLogger<AdministradorInicialService>();
            var service = CriarService(contexto, logger);

            using var _ = new EscopoVariaveisAmbiente(new Dictionary<string, string?>
            {
                [VariavelEmail] = "admin.fragil@empresa.com",
                [VariavelSenha] = "Admin@123456",
                [VariavelNome] = "Admin Fragil"
            });

            await service.SeedAsync();

            Assert.Equal(0, await contexto.Usuarios.CountAsync());
        }
        finally
        {
            MutexAmbiente.Release();
        }
    }

    [Fact]
    public async Task LogNaoContemSenha()
    {
        await MutexAmbiente.WaitAsync();
        try
        {
            using var contexto = PortalUseCasesTestFactory.CriarContexto();
            var logger = new TestLogger<AdministradorInicialService>();
            var service = CriarService(contexto, logger);
            const string senha = "Senha@MuitoSegura123";

            using var _ = new EscopoVariaveisAmbiente(new Dictionary<string, string?>
            {
                [VariavelEmail] = "admin.log@empresa.com",
                [VariavelSenha] = senha,
                [VariavelNome] = "Admin Log"
            });

            await service.SeedAsync();

            Assert.DoesNotContain(logger.Messages, mensagem => mensagem.Contains(senha, StringComparison.Ordinal));
        }
        finally
        {
            MutexAmbiente.Release();
        }
    }

    private static AdministradorInicialService CriarService(
        SGXSistemaChamadoDbContext contexto,
        TestLogger<AdministradorInicialService> logger)
    {
        return new AdministradorInicialService(
            contexto,
            new PasswordHasher<Usuario>(),
            new PoliticaSenhaService(
                Options.Create(new AuthOptions()),
                new PasswordHasher<Usuario>()),
            logger);
    }

    private sealed class EscopoVariaveisAmbiente : IDisposable
    {
        private readonly Dictionary<string, string?> _anteriores = [];

        public EscopoVariaveisAmbiente(IDictionary<string, string?> valores)
        {
            foreach (var item in valores)
            {
                _anteriores[item.Key] = Environment.GetEnvironmentVariable(item.Key);
                Environment.SetEnvironmentVariable(item.Key, item.Value);
            }
        }

        public void Dispose()
        {
            foreach (var item in _anteriores)
            {
                Environment.SetEnvironmentVariable(item.Key, item.Value);
            }
        }
    }

    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
}
