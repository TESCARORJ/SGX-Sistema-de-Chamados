using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Api.Services;

public interface IAdministradorInicialService
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}

public sealed class AdministradorInicialService(
    SGXSistemaChamadoDbContext dbContext,
    IPasswordHasher<Usuario> passwordHasher,
    IPoliticaSenhaService politicaSenhaService,
    ILogger<AdministradorInicialService> logger) : IAdministradorInicialService
{
    private const string UsuarioTecnico = "seed.admin-inicial";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var email = ObterVariavelObrigatoria("SGX_ADMIN_INICIAL_EMAIL");
        var senha = ObterVariavelObrigatoria("SGX_ADMIN_INICIAL_SENHA");
        var nome = ObterVariavelObrigatoria("SGX_ADMIN_INICIAL_NOME");

        if (email is null && senha is null && nome is null)
        {
            return;
        }

        if (email is null || senha is null || nome is null)
        {
            logger.LogWarning("Variáveis do Administrador inicial incompletas. Nenhum usuário foi criado.");
            return;
        }

        if (!EmailValido(email))
        {
            logger.LogWarning("E-mail do Administrador inicial inválido. Nenhum usuário foi criado.");
            return;
        }

        if (string.IsNullOrWhiteSpace(nome))
        {
            logger.LogWarning("Nome do Administrador inicial inválido. Nenhum usuário foi criado.");
            return;
        }

        var validacaoSenha = politicaSenhaService.ValidarSenha(senha);
        if (!validacaoSenha.Valida)
        {
            logger.LogWarning("Senha do Administrador inicial rejeitada por política de segurança: {Motivo}", validacaoSenha.Motivo);
            return;
        }

        var perfilAdministrador = await dbContext.PerfisAcesso
            .FirstOrDefaultAsync(x => x.TipoPerfil == TipoPerfil.Administrador, cancellationToken);

        if (perfilAdministrador is null)
        {
            logger.LogError("Perfil Administrador não encontrado. Administrador inicial não foi criado.");
            return;
        }

        if (!perfilAdministrador.Ativo)
        {
            perfilAdministrador.Ativar(UsuarioTecnico);
            dbContext.PerfisAcesso.Update(perfilAdministrador);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var jaExisteAdministradorAtivo = await dbContext.UsuariosPerfisAcesso
            .AsNoTracking()
            .Include(x => x.Usuario)
            .AnyAsync(
                vinculo =>
                    vinculo.PerfilAcessoId == perfilAdministrador.Id &&
                    vinculo.Usuario.Ativo &&
                    vinculo.Usuario.Situacao == SituacaoUsuario.Ativo,
                cancellationToken);

        if (jaExisteAdministradorAtivo)
        {
            return;
        }

        var emailNormalizado = email.Trim().ToLowerInvariant();
        var loginNormalizado = emailNormalizado;
        var nomeNormalizado = nome.Trim();

        var usuario = await dbContext.Usuarios
            .FirstOrDefaultAsync(
                x => x.Email == emailNormalizado || x.Login == loginNormalizado,
                cancellationToken);

        if (usuario is null)
        {
            usuario = new Usuario(nomeNormalizado, emailNormalizado, loginNormalizado, UsuarioTecnico);
            await dbContext.Usuarios.AddAsync(usuario, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            usuario.DefinirNome(nomeNormalizado);
            usuario.DefinirEmail(emailNormalizado);
            usuario.DefinirLogin(loginNormalizado);
            usuario.Ativar(UsuarioTecnico);
            usuario.AlterarSituacao(SituacaoUsuario.Ativo, UsuarioTecnico);
            dbContext.Usuarios.Update(usuario);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var senhaHash = passwordHasher.HashPassword(usuario, senha);
        usuario.DefinirSenhaHashLocal(senhaHash, UsuarioTecnico);
        usuario.DefinirDeveAlterarSenha(true, UsuarioTecnico);
        dbContext.Usuarios.Update(usuario);

        var possuiPerfil = await dbContext.UsuariosPerfisAcesso
            .AnyAsync(x => x.UsuarioId == usuario.Id && x.PerfilAcessoId == perfilAdministrador.Id, cancellationToken);

        if (!possuiPerfil)
        {
            await dbContext.UsuariosPerfisAcesso.AddAsync(
                new UsuarioPerfilAcesso(usuario.Id, perfilAdministrador.Id, UsuarioTecnico),
                cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Administrador inicial criado a partir de variáveis de ambiente.");
    }

    private static string? ObterVariavelObrigatoria(string nomeVariavel)
    {
        var valor = Environment.GetEnvironmentVariable(nomeVariavel);
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }

    private static bool EmailValido(string email)
    {
        try
        {
            var parsed = new MailAddress(email);
            return string.Equals(parsed.Address, email, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
