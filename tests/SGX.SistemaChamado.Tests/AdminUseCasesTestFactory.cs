using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

internal static class AdminUseCasesTestFactory
{
    public static SGXSistemaChamadoDbContext CriarContexto() => PortalUseCasesTestFactory.CriarContexto();

    public static async Task<Usuario> CriarUsuarioComPerfilAsync(
        SGXSistemaChamadoDbContext context,
        string nome,
        string email,
        TipoPerfil perfil,
        string criadoPor = "teste")
    {
        var usuario = new Usuario(nome, email, email, criadoPor);
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var perfilAcesso = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == perfil);
        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilAcesso.Id, criadoPor));
        await context.SaveChangesAsync();
        return usuario;
    }

    public static async Task<CategoriaChamado> CriarCategoriaAsync(SGXSistemaChamadoDbContext context, string nome, Guid? departamentoId = null, string criadoPor = "teste")
    {
        var categoria = new CategoriaChamado(nome, null, departamentoId, criadoPor);
        context.CategoriasChamado.Add(categoria);
        await context.SaveChangesAsync();
        return categoria;
    }

    public static async Task<Chamado> CriarChamadoAsync(
        SGXSistemaChamadoDbContext context,
        Usuario solicitante,
        CategoriaChamado categoria,
        StatusChamadoEnum status,
        Guid? prioridadeId = null,
        string sufixoCodigo = "001",
        string criadoPor = "teste",
        Guid? subcategoriaId = null,
        Guid? tipoSolicitacaoId = null,
        Guid? localUnidadeId = null)
    {
        var prioridade = prioridadeId.HasValue
            ? await context.PrioridadesChamado.FirstAsync(x => x.Id == prioridadeId.Value)
            : await context.PrioridadesChamado.FirstAsync();

        var statusEntidade = await context.StatusChamado.FirstAsync(x => x.Codigo == status);

        var chamado = new Chamado(
            $"CH-ADMIN-{sufixoCodigo}",
            $"Chamado {sufixoCodigo}",
            "Descricao de teste",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            statusEntidade.Id,
            OrigemChamado.Portal,
            criadoPor,
            categoria.DepartamentoId,
            subcategoriaId,
            tipoSolicitacaoId,
            localUnidadeId);

        context.Chamados.Add(chamado);
        await context.SaveChangesAsync();
        return chamado;
    }

    public static UsuarioContextoAplicacao Contexto(Usuario usuario, params string[] perfis)
        => new(usuario.Id, usuario.Nome, usuario.Email, usuario.Login, perfis);
}
