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

    public void AtualizarUltimoAcesso(DateTime acessoEmUtc, string atualizadoPor)
    {
        UltimoAcessoEm = acessoEmUtc;
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
