using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

internal sealed class FakeAuditoriaService : IAuditoriaService
{
    public List<RegistrarEventoAuditoriaRequest> Eventos { get; } = [];

    public Task RegistrarAsync(RegistrarEventoAuditoriaRequest request, CancellationToken cancellationToken = default)
    {
        Eventos.Add(request);
        return Task.CompletedTask;
    }

    public Task RegistrarCriacaoAsync(string modulo, string entidade, string entidadeId, string descricao, string? dadosDepois = null, string? metadados = null, CancellationToken cancellationToken = default)
        => RegistrarAsync(new RegistrarEventoAuditoriaRequest
        {
            Modulo = modulo,
            Entidade = entidade,
            EntidadeId = entidadeId,
            Acao = TipoAcaoAuditoria.Criacao,
            Descricao = descricao,
            DadosDepois = dadosDepois,
            Metadados = metadados,
            Nivel = NivelAuditoria.Informacao,
            Sucesso = true
        }, cancellationToken);

    public Task RegistrarEdicaoAsync(string modulo, string entidade, string entidadeId, string descricao, string? dadosAntes = null, string? dadosDepois = null, string? metadados = null, CancellationToken cancellationToken = default)
        => RegistrarAsync(new RegistrarEventoAuditoriaRequest
        {
            Modulo = modulo,
            Entidade = entidade,
            EntidadeId = entidadeId,
            Acao = TipoAcaoAuditoria.Edicao,
            Descricao = descricao,
            DadosAntes = dadosAntes,
            DadosDepois = dadosDepois,
            Metadados = metadados,
            Nivel = NivelAuditoria.Informacao,
            Sucesso = true
        }, cancellationToken);

    public Task RegistrarExclusaoLogicaAsync(string modulo, string entidade, string entidadeId, string descricao, string? dadosAntes = null, string? metadados = null, CancellationToken cancellationToken = default)
        => RegistrarAsync(new RegistrarEventoAuditoriaRequest
        {
            Modulo = modulo,
            Entidade = entidade,
            EntidadeId = entidadeId,
            Acao = TipoAcaoAuditoria.ExclusaoLogica,
            Descricao = descricao,
            DadosAntes = dadosAntes,
            Metadados = metadados,
            Nivel = NivelAuditoria.Informacao,
            Sucesso = true
        }, cancellationToken);

    public Task RegistrarAtivacaoAsync(string modulo, string entidade, string entidadeId, string descricao, string? metadados = null, CancellationToken cancellationToken = default)
        => RegistrarAsync(new RegistrarEventoAuditoriaRequest
        {
            Modulo = modulo,
            Entidade = entidade,
            EntidadeId = entidadeId,
            Acao = TipoAcaoAuditoria.Ativacao,
            Descricao = descricao,
            Metadados = metadados,
            Nivel = NivelAuditoria.Informacao,
            Sucesso = true
        }, cancellationToken);

    public Task RegistrarInativacaoAsync(string modulo, string entidade, string entidadeId, string descricao, string? metadados = null, CancellationToken cancellationToken = default)
        => RegistrarAsync(new RegistrarEventoAuditoriaRequest
        {
            Modulo = modulo,
            Entidade = entidade,
            EntidadeId = entidadeId,
            Acao = TipoAcaoAuditoria.Inativacao,
            Descricao = descricao,
            Metadados = metadados,
            Nivel = NivelAuditoria.Informacao,
            Sucesso = true
        }, cancellationToken);

    public Task RegistrarLoginAsync(bool sucesso, string descricao, string? mensagemErro = null, Guid? usuarioId = null, string? usuarioNome = null, string? usuarioEmail = null, string? usuarioLogin = null, string? metadados = null, CancellationToken cancellationToken = default)
        => RegistrarAsync(new RegistrarEventoAuditoriaRequest
        {
            Modulo = "Autenticacao",
            Entidade = "Usuario",
            EntidadeId = usuarioId?.ToString(),
            Acao = TipoAcaoAuditoria.Login,
            Descricao = descricao,
            Metadados = metadados,
            Nivel = sucesso ? NivelAuditoria.Informacao : NivelAuditoria.Alerta,
            Sucesso = sucesso,
            MensagemErro = mensagemErro,
            UsuarioId = usuarioId,
            UsuarioNome = usuarioNome,
            UsuarioEmail = usuarioEmail,
            UsuarioLogin = usuarioLogin
        }, cancellationToken);

    public Task RegistrarLogoutAsync(string descricao, string? metadados = null, CancellationToken cancellationToken = default)
        => RegistrarAsync(new RegistrarEventoAuditoriaRequest
        {
            Modulo = "Autenticacao",
            Entidade = "Usuario",
            Acao = TipoAcaoAuditoria.Logout,
            Descricao = descricao,
            Metadados = metadados,
            Nivel = NivelAuditoria.Informacao,
            Sucesso = true
        }, cancellationToken);

    public Task RegistrarErroAsync(string modulo, string entidade, string descricao, string? entidadeId = null, Exception? exception = null, string? metadados = null, CancellationToken cancellationToken = default)
        => RegistrarAsync(new RegistrarEventoAuditoriaRequest
        {
            Modulo = modulo,
            Entidade = entidade,
            EntidadeId = entidadeId,
            Acao = TipoAcaoAuditoria.Erro,
            Descricao = descricao,
            Metadados = metadados,
            Nivel = NivelAuditoria.Critico,
            Sucesso = false,
            MensagemErro = exception?.Message
        }, cancellationToken);
}
