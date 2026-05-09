using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

internal static class PortalUseCaseHelpers
{
    public static bool PodeVisaoAmpliada(UsuarioContextoAplicacao usuario)
        => usuario.PossuiQualquerPerfil("Administrador", "Atendente");

    public static bool PodeAcessarChamado(UsuarioContextoAplicacao usuario, Chamado chamado)
        => PodeVisaoAmpliada(usuario) || chamado.SolicitanteId == usuario.Id;

    public static ChamadoResumoResponse MapResumo(Chamado chamado)
    {
        return new ChamadoResumoResponse
        {
            Id = chamado.Id,
            Codigo = chamado.Codigo,
            Titulo = chamado.Titulo,
            Status = chamado.Status.Nome,
            Prioridade = chamado.Prioridade.Nome,
            Categoria = chamado.Categoria.Nome,
            Departamento = chamado.Departamento?.Nome,
            AbertoEm = chamado.AbertoEm,
            AtualizadoEm = chamado.AtualizadoEm,
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

    public static ChamadoDetalheResponse MapDetalhe(Chamado chamado, UsuarioContextoAplicacao? usuarioContexto = null)
    {
        var historico = chamado.Historicos.AsEnumerable();
        if (usuarioContexto is not null && !PodeVisaoAmpliada(usuarioContexto))
        {
            historico = historico.Where(x =>
                !(x.Tipo == TipoHistoricoChamado.ComentarioAdicionado &&
                  x.Descricao.Contains("interno", StringComparison.OrdinalIgnoreCase)));
        }

        return new ChamadoDetalheResponse
        {
            Id = chamado.Id,
            Codigo = chamado.Codigo,
            Titulo = chamado.Titulo,
            Descricao = chamado.Descricao,
            Status = chamado.Status.Nome,
            Prioridade = chamado.Prioridade.Nome,
            Categoria = chamado.Categoria.Nome,
            Departamento = chamado.Departamento?.Nome,
            Solicitante = chamado.Solicitante.Nome,
            Responsavel = chamado.Responsavel?.Nome,
            AbertoEm = chamado.AbertoEm,
            EncerradoEm = chamado.EncerradoEm,
            Comentarios = chamado.Comentarios
                .Where(x => x.Ativo && !x.Interno)
                .OrderBy(x => x.CriadoEm)
                .Select(x => new ComentarioChamadoResponse(x.Id, x.UsuarioId, x.Usuario.Nome, x.Mensagem, x.CriadoEm))
                .ToArray(),
            Anexos = chamado.Anexos
                .Where(x => x.Ativo)
                .OrderByDescending(x => x.CriadoEm)
                .Select(x => new AnexoChamadoResponse(x.Id, x.NomeArquivo, x.ContentType, x.TamanhoBytes, x.CriadoEm, x.UsuarioId, x.Usuario.Nome))
                .ToArray(),
            Historico = historico
                .OrderByDescending(x => x.CriadoEm)
                .Select(x => new HistoricoChamadoResponse(
                    x.Id,
                    (int)x.Tipo,
                    x.Tipo.ToString(),
                    x.Descricao,
                    x.CriadoEm,
                    x.UsuarioId,
                    x.Usuario?.Nome))
                .ToArray(),
            Sla = chamado.SlaControle is null
                ? null
                : new SlaResumoResponse(
                    chamado.SlaControle.PrazoPrimeiraRespostaEm,
                    chamado.SlaControle.PrimeiraRespostaEm,
                    chamado.SlaControle.PrazoResolucaoEm,
                    chamado.SlaControle.ResolvidoEm,
                    chamado.SlaControle.EstaVencido,
                    chamado.SlaControle.EstaPausado,
                    chamado.SlaControle.TotalMinutosPausado)
        };
    }

    public static string ObterDescricaoHistorico(TipoHistoricoChamado tipo)
        => tipo switch
        {
            TipoHistoricoChamado.Criado => "Chamado criado",
            TipoHistoricoChamado.ComentarioAdicionado => "Comentario adicionado",
            TipoHistoricoChamado.AnexoAdicionado => "Anexo adicionado",
            _ => tipo.ToString()
        };
}
