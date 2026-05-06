using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

internal static class AdminUseCaseHelpers
{
    public static bool PodeOperarAdmin(UsuarioContextoAplicacao usuario)
        => usuario.PossuiQualquerPerfil("Administrador", "Atendente");

    public static bool EhAdministrador(UsuarioContextoAplicacao usuario)
        => usuario.PossuiPerfil("Administrador");

    public static string ObterDescricaoHistorico(TipoHistoricoChamado tipo, string? detalhe = null)
        => tipo switch
        {
            TipoHistoricoChamado.ResponsavelAlterado => string.IsNullOrWhiteSpace(detalhe)
                ? "Responsavel alterado"
                : detalhe,
            TipoHistoricoChamado.StatusAlterado => string.IsNullOrWhiteSpace(detalhe)
                ? "Status alterado"
                : detalhe,
            TipoHistoricoChamado.PrioridadeAlterada => string.IsNullOrWhiteSpace(detalhe)
                ? "Prioridade alterada"
                : detalhe,
            TipoHistoricoChamado.CategoriaAlterada => string.IsNullOrWhiteSpace(detalhe)
                ? "Categoria alterada"
                : detalhe,
            TipoHistoricoChamado.ComentarioAdicionado => string.IsNullOrWhiteSpace(detalhe)
                ? "Comentario administrativo adicionado"
                : detalhe,
            TipoHistoricoChamado.Encerrado => string.IsNullOrWhiteSpace(detalhe)
                ? "Chamado encerrado"
                : detalhe,
            TipoHistoricoChamado.Reaberto => string.IsNullOrWhiteSpace(detalhe)
                ? "Chamado reaberto"
                : detalhe,
            _ => detalhe ?? tipo.ToString()
        };

    public static ChamadoAdminResumoResponse MapResumo(Chamado chamado)
    {
        return new ChamadoAdminResumoResponse
        {
            Id = chamado.Id,
            Codigo = chamado.Codigo,
            Titulo = chamado.Titulo,
            SolicitanteNome = chamado.Solicitante.Nome,
            SolicitanteEmail = chamado.Solicitante.Email,
            ResponsavelNome = chamado.Responsavel?.Nome,
            Status = chamado.Status.Nome,
            Prioridade = chamado.Prioridade.Nome,
            Categoria = chamado.Categoria.Nome,
            Departamento = chamado.Departamento?.Nome,
            AbertoEm = chamado.AbertoEm,
            AtualizadoEm = chamado.AtualizadoEm,
            EncerradoEm = chamado.EncerradoEm,
            SlaVencido = chamado.SlaControle?.EstaVencido ?? false,
            SlaProximoVencimento = SlaRules.EstaProximoDoVencimento(chamado.SlaControle, DateTime.UtcNow),
            PrazoPrimeiraRespostaEm = chamado.SlaControle?.PrazoPrimeiraRespostaEm,
            PrimeiraRespostaEm = chamado.SlaControle?.PrimeiraRespostaEm,
            PrazoResolucaoEm = chamado.SlaControle?.PrazoResolucaoEm,
            ResolvidoEm = chamado.SlaControle?.ResolvidoEm,
            EstaPausado = chamado.SlaControle?.EstaPausado ?? false,
            TotalMinutosPausado = chamado.SlaControle?.TotalMinutosPausado ?? 0
        };
    }

    public static ChamadoAdminDetalheResponse MapDetalhe(Chamado chamado)
    {
        return new ChamadoAdminDetalheResponse
        {
            Id = chamado.Id,
            Codigo = chamado.Codigo,
            Titulo = chamado.Titulo,
            Descricao = chamado.Descricao,
            Solicitante = new SolicitanteAdminResponse(
                chamado.SolicitanteId,
                chamado.Solicitante.Nome,
                chamado.Solicitante.Email),
            Responsavel = chamado.Responsavel is null
                ? null
                : new ResponsavelAdminResponse(chamado.Responsavel.Id, chamado.Responsavel.Nome, chamado.Responsavel.Email),
            Status = chamado.Status.Nome,
            Prioridade = chamado.Prioridade.Nome,
            Categoria = chamado.Categoria.Nome,
            Departamento = chamado.Departamento?.Nome,
            Origem = chamado.Origem.ToString(),
            AbertoEm = chamado.AbertoEm,
            EncerradoEm = chamado.EncerradoEm,
            Comentarios = chamado.Comentarios
                .Where(x => x.Ativo)
                .OrderBy(x => x.CriadoEm)
                .Select(x => new ComentarioAdminResponse(x.Id, x.UsuarioId, x.Usuario.Nome, x.Mensagem, x.Interno, x.CriadoEm))
                .ToArray(),
            Anexos = chamado.Anexos
                .Where(x => x.Ativo)
                .OrderByDescending(x => x.CriadoEm)
                .Select(x => new AnexoAdminResponse(x.Id, x.NomeArquivo, x.ContentType, x.TamanhoBytes, x.CriadoEm, x.UsuarioId, x.Usuario.Nome))
                .ToArray(),
            Historico = chamado.Historicos
                .OrderByDescending(x => x.CriadoEm)
                .Select(x => new HistoricoAdminResponse(x.Id, (int)x.Tipo, x.Tipo.ToString(), x.Descricao, x.CriadoEm, x.UsuarioId, x.Usuario?.Nome))
                .ToArray(),
            Sla = chamado.SlaControle is null
                ? null
                : new SlaAdminResponse(
                    chamado.SlaControle.PrazoPrimeiraRespostaEm,
                    chamado.SlaControle.PrimeiraRespostaEm,
                    chamado.SlaControle.PrazoResolucaoEm,
                    chamado.SlaControle.ResolvidoEm,
                    chamado.SlaControle.EstaVencido,
                    chamado.SlaControle.EstaPausado,
                    chamado.SlaControle.TotalMinutosPausado)
        };
    }
}
