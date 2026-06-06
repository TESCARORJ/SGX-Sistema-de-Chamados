using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

internal static class AdminCadastrosHelpers
{
    private static readonly string[] Direcoes = ["asc", "desc"];

    public static void GarantirAdminOuAtendente(UsuarioContextoAplicacao usuario)
    {
        if (!usuario.PossuiQualquerPerfil("Administrador", "Atendente"))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }
    }

    public static void GarantirAdministrador(UsuarioContextoAplicacao usuario)
    {
        if (!usuario.PossuiPerfil("Administrador"))
        {
            throw new UnauthorizedAccessException("Acao permitida somente para Administrador.");
        }
    }

    public static (int pagina, int tamanho) NormalizarPaginacao(FiltroCadastroRequest request)
        => NormalizarPaginacao(request.Pagina, request.TamanhoPagina);

    public static (int pagina, int tamanho) NormalizarPaginacao(int paginaRequest, int tamanhoPaginaRequest)
    {
        var pagina = paginaRequest <= 0 ? 1 : paginaRequest;
        var tamanho = tamanhoPaginaRequest <= 0 ? 20 : Math.Min(tamanhoPaginaRequest, 100);
        return (pagina, tamanho);
    }

    public static bool DirecaoDesc(string? direcao)
        => string.Equals(direcao, "desc", StringComparison.OrdinalIgnoreCase);

    public static bool DirecaoValida(string? direcao)
        => !string.IsNullOrWhiteSpace(direcao) &&
           Direcoes.Contains(direcao.Trim(), StringComparer.OrdinalIgnoreCase);

    public static string MascararValorSensivel(string valor)
    {
        if (string.IsNullOrEmpty(valor))
        {
            return string.Empty;
        }

        return "********";
    }

    public static PerfilAcessoResumoResponse MapPerfilResumo(PerfilAcesso perfil)
        => new(
            perfil.Id,
            perfil.Nome,
            (int)perfil.TipoPerfil,
            perfil.TipoPerfil.ToString(),
            perfil.Ativo);

    public static async Task<int> ContarAdministradoresAtivosAsync(IQueryable<Usuario> usuarios)
    {
        return await usuarios
            .Where(u => u.Ativo && u.Situacao == SituacaoUsuario.Ativo)
            .Where(u => u.UsuarioPerfis.Any(up => up.PerfilAcesso.Ativo && up.PerfilAcesso.TipoPerfil == TipoPerfil.Administrador))
            .CountAsync();
    }
}
