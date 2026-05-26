using System.Text.Json;
using Microsoft.Extensions.Logging;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Api.Services;

internal static class AuditoriaAutenticacaoHelper
{
    public const string ModuloAutenticacao = "Autenticacao";
    public const string EntidadeAutenticacao = "EventoAutenticacao";
    public const string EntidadeMetodosLogin = "MetodosLogin";

    public static async Task RegistrarEventoAsync(
        IAuditoriaService? auditoriaService,
        ILogger logger,
        TipoEventoAutenticacao tipoEvento,
        ResultadoEventoAutenticacao resultado,
        string descricao,
        string provedor,
        string? mensagemTecnica = null,
        Guid? usuarioId = null,
        string? usuarioNome = null,
        string? usuarioEmail = null,
        string? usuarioLogin = null,
        Guid? usuarioAlvoId = null,
        string? usuarioAlvoEmail = null,
        string? dadosAntes = null,
        string? dadosDepois = null,
        string? observacao = null,
        CancellationToken cancellationToken = default)
    {
        if (auditoriaService is null)
        {
            return;
        }

        try
        {
            await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
            {
                Modulo = ModuloAutenticacao,
                Entidade = EntidadeAutenticacao,
                EntidadeId = usuarioAlvoId?.ToString() ?? usuarioId?.ToString(),
                Acao = TipoAcaoAuditoria.Login,
                Descricao = descricao,
                DadosAntes = dadosAntes,
                DadosDepois = dadosDepois,
                Nivel = MapearNivel(resultado),
                Sucesso = resultado == ResultadoEventoAutenticacao.Sucesso,
                MensagemErro = string.IsNullOrWhiteSpace(mensagemTecnica) ? null : mensagemTecnica.Trim(),
                UsuarioId = usuarioId,
                UsuarioNome = usuarioNome,
                UsuarioEmail = usuarioEmail,
                UsuarioLogin = usuarioLogin,
                Metadados = JsonSerializer.Serialize(new
                {
                    tipoEventoAutenticacao = tipoEvento.ToString(),
                    resultadoAutenticacao = resultado.ToString(),
                    provedor = string.IsNullOrWhiteSpace(provedor) ? "NaoInformado" : provedor.Trim(),
                    usuarioAlvoId = usuarioAlvoId?.ToString(),
                    usuarioAlvoEmail = string.IsNullOrWhiteSpace(usuarioAlvoEmail) ? null : usuarioAlvoEmail.Trim().ToLowerInvariant(),
                    observacao = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim()
                })
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Falha ao registrar auditoria de autenticacao. TipoEvento={TipoEvento}, Resultado={Resultado}, Provedor={Provedor}",
                tipoEvento,
                resultado,
                provedor);
        }
    }

    public static async Task RegistrarEventoAdministrativoAsync(
        IAuditoriaService? auditoriaService,
        ILogger logger,
        TipoEventoAutenticacao tipoEvento,
        ResultadoEventoAutenticacao resultado,
        string descricao,
        string provedor,
        string entidadeId,
        string? mensagemTecnica = null,
        string? dadosAntes = null,
        string? dadosDepois = null,
        string? observacao = null,
        CancellationToken cancellationToken = default)
    {
        if (auditoriaService is null)
        {
            return;
        }

        try
        {
            await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
            {
                Modulo = ModuloAutenticacao,
                Entidade = EntidadeMetodosLogin,
                EntidadeId = entidadeId,
                Acao = TipoAcaoAuditoria.Configuracao,
                Descricao = descricao,
                DadosAntes = dadosAntes,
                DadosDepois = dadosDepois,
                Nivel = MapearNivel(resultado),
                Sucesso = resultado == ResultadoEventoAutenticacao.Sucesso,
                MensagemErro = string.IsNullOrWhiteSpace(mensagemTecnica) ? null : mensagemTecnica.Trim(),
                Metadados = JsonSerializer.Serialize(new
                {
                    tipoEventoAutenticacao = tipoEvento.ToString(),
                    resultadoAutenticacao = resultado.ToString(),
                    provedor = string.IsNullOrWhiteSpace(provedor) ? "NaoInformado" : provedor.Trim(),
                    observacao = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim()
                })
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Falha ao registrar auditoria administrativa de autenticacao. TipoEvento={TipoEvento}, Resultado={Resultado}, Provedor={Provedor}",
                tipoEvento,
                resultado,
                provedor);
        }
    }

    private static NivelAuditoria MapearNivel(ResultadoEventoAutenticacao resultado)
    {
        return resultado switch
        {
            ResultadoEventoAutenticacao.Sucesso => NivelAuditoria.Informacao,
            ResultadoEventoAutenticacao.Bloqueado => NivelAuditoria.Alerta,
            ResultadoEventoAutenticacao.Negado => NivelAuditoria.Alerta,
            _ => NivelAuditoria.Alerta
        };
    }
}
