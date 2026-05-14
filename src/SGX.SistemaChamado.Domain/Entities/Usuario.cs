using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class Usuario : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Login { get; private set; } = string.Empty;
    public string? SenhaHashLocal { get; private set; }
    public bool DeveAlterarSenha { get; private set; }
    public DateTime? UltimaAlteracaoSenhaEm { get; private set; }
    public int TentativasInvalidas { get; private set; }
    public DateTime? BloqueadoAte { get; private set; }
    public DateTime? UltimoLoginEm { get; private set; }
    public SituacaoUsuario Situacao { get; private set; } = SituacaoUsuario.Ativo;
    public DateTime? UltimoAcessoEm { get; private set; }
    public Guid? DepartamentoId { get; private set; }

    public Departamento? Departamento { get; private set; }
    public ICollection<UsuarioPerfilAcesso> UsuarioPerfis { get; private set; } = [];

    [NotMapped]
    public IReadOnlyCollection<PerfilAcesso> Perfis => UsuarioPerfis.Select(x => x.PerfilAcesso).ToArray();

    private Usuario()
    {
    }

    public Usuario(string nome, string email, string login, string criadoPor, Guid? departamentoId = null)
    {
        DefinirNome(nome);
        DefinirEmail(email);
        DefinirLogin(login);
        DepartamentoId = departamentoId;
        Situacao = SituacaoUsuario.Ativo;
        DefinirCriacao(criadoPor);
    }

    public void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do usuario e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    public void DefinirEmail(string email)
    {
        Email = EmailCorporativo.Criar(email).Valor;
    }

    public void DefinirLogin(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            throw new ArgumentException("O login do usuario e obrigatorio.", nameof(login));
        }

        Login = login.Trim().ToLowerInvariant();
    }

    public void DefinirSenhaHashLocal(string senhaHashLocal, string atualizadoPor)
    {
        if (string.IsNullOrWhiteSpace(senhaHashLocal))
        {
            throw new ArgumentException("A senha hash local e obrigatoria.", nameof(senhaHashLocal));
        }

        SenhaHashLocal = senhaHashLocal.Trim();
        UltimaAlteracaoSenhaEm = DateTime.UtcNow;
        AtualizarAuditoria(atualizadoPor);
    }

    public void RemoverSenhaLocal(string atualizadoPor)
    {
        SenhaHashLocal = null;
        DeveAlterarSenha = false;
        AtualizarAuditoria(atualizadoPor);
    }

    public void DefinirDeveAlterarSenha(bool deveAlterarSenha, string atualizadoPor)
    {
        DeveAlterarSenha = deveAlterarSenha;
        AtualizarAuditoria(atualizadoPor);
    }

    public void AtualizarUltimoAcesso(DateTime acessoEmUtc, string atualizadoPor)
    {
        UltimoAcessoEm = acessoEmUtc;
        AtualizarAuditoria(atualizadoPor);
    }

    public void RegistrarLoginLocalBemSucedido(DateTime loginEmUtc, string atualizadoPor)
    {
        UltimoLoginEm = loginEmUtc;
        TentativasInvalidas = 0;
        BloqueadoAte = null;
        AtualizarAuditoria(atualizadoPor);
    }

    public void RegistrarFalhaLoginLocal(int tentativasMaximas, TimeSpan janelaBloqueio, DateTime agoraUtc, string atualizadoPor)
    {
        TentativasInvalidas++;

        if (TentativasInvalidas >= tentativasMaximas)
        {
            BloqueadoAte = agoraUtc.Add(janelaBloqueio);
            TentativasInvalidas = 0;
        }

        AtualizarAuditoria(atualizadoPor);
    }

    public void LimparLockout(string atualizadoPor)
    {
        TentativasInvalidas = 0;
        BloqueadoAte = null;
        AtualizarAuditoria(atualizadoPor);
    }

    public void AlterarSituacao(SituacaoUsuario situacao, string atualizadoPor)
    {
        Situacao = situacao;
        AtualizarAuditoria(atualizadoPor);
    }

    public void DefinirDepartamento(Guid? departamentoId, string atualizadoPor)
    {
        DepartamentoId = departamentoId;
        AtualizarAuditoria(atualizadoPor);
    }
}
