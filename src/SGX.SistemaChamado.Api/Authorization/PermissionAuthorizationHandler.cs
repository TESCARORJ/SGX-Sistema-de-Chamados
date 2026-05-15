using Microsoft.AspNetCore.Authorization;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Api.Authorization;

public sealed class PermissionAuthorizationHandler(
    IUsuarioAtualService usuarioAtualService,
    IHttpContextAccessor httpContextAccessor,
    IAuditoriaService? auditoriaService = null)
    : AuthorizationHandler<PermissionRequirement>
{
    private const string CacheKeyPermissoesAuditadas = "sgx.audit.permissoes.negadas";

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(requirement.CodigoPermissao))
        {
            return;
        }

        UsuarioAutenticadoContexto usuarioAtual;
        try
        {
            usuarioAtual = await usuarioAtualService.ObterAsync();
        }
        catch
        {
            return;
        }

        if (usuarioAtual.PossuiPerfil(PerfisInternos.Administrador))
        {
            context.Succeed(requirement);
            return;
        }

        if (usuarioAtual.Permissoes.Contains(requirement.CodigoPermissao, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
            return;
        }

        if (auditoriaService is null)
        {
            return;
        }

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return;
        }

        var permissoesAuditadas = ObterPermissoesAuditadas(httpContext);
        if (!permissoesAuditadas.Add(requirement.CodigoPermissao))
        {
            return;
        }

        await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
        {
            Modulo = "Autenticacao Corporativa",
            Entidade = "Autorizacao",
            EntidadeId = usuarioAtual.Id.ToString(),
            Acao = TipoAcaoAuditoria.Erro,
            Descricao = "Acesso negado por falta de permissao.",
            Nivel = NivelAuditoria.Alerta,
            Sucesso = false,
            MensagemErro = $"Permissao requerida: {requirement.CodigoPermissao}",
            UsuarioId = usuarioAtual.Id,
            UsuarioNome = usuarioAtual.Nome,
            UsuarioEmail = usuarioAtual.Email,
            UsuarioLogin = usuarioAtual.Login,
            Metadados = AuditoriaDiffHelper.CriarMetadadosPadrao(
                origem: "api",
                modulo: "Autenticacao Corporativa",
                entidade: "Autorizacao",
                entidadeId: usuarioAtual.Id.ToString(),
                codigo: requirement.CodigoPermissao,
                nome: httpContext.Request.Path,
                operacao: "Autorizacao",
                resultado: "Negado",
                observacao: "Permissao insuficiente")
        });
    }

    private static HashSet<string> ObterPermissoesAuditadas(HttpContext httpContext)
    {
        if (httpContext.Items.TryGetValue(CacheKeyPermissoesAuditadas, out var valorCache)
            && valorCache is HashSet<string> cache)
        {
            return cache;
        }

        var novoCache = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        httpContext.Items[CacheKeyPermissoesAuditadas] = novoCache;
        return novoCache;
    }
}
