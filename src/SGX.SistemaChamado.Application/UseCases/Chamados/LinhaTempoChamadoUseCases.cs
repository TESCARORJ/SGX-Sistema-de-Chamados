using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Chamados;

public sealed class ListarLinhaTempoChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<ComentarioChamado> comentarioRepository,
    IRepository<AnexoChamado> anexoRepository,
    IRepository<HistoricoChamado> historicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarLinhaTempoChamadoUseCase
{
    public async Task<LinhaTempoChamadoResponse> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        ComentariosChamadoUseCaseHelpers.ValidarPerfilPermitido(usuario);

        var chamado = await chamadoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Solicitante)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        if (!ComentariosChamadoUseCaseHelpers.PodeAcessarChamado(usuario, chamado))
        {
            throw new KeyNotFoundException("Chamado nao encontrado.");
        }

        var ehSolicitante = ComentariosChamadoUseCaseHelpers.EhSolicitante(usuario);
        var items = new List<LinhaTempoChamadoItemResponse>
        {
            CriarEventoAbertura(chamado)
        };

        var comentarios = await comentarioRepository.Query()
            .AsNoTracking()
            .Include(x => x.Usuario)
            .Where(x => x.Ativo && x.ChamadoId == chamadoId)
            .OrderBy(x => x.CriadoEm)
            .ToListAsync(cancellationToken);

        foreach (var comentario in comentarios)
        {
            if (ehSolicitante && comentario.Interno)
            {
                continue;
            }

            items.Add(CriarEventoComentario(comentario));
        }

        var anexos = await anexoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Usuario)
            .Where(x => x.Ativo && x.ChamadoId == chamadoId)
            .OrderBy(x => x.CriadoEm)
            .ToListAsync(cancellationToken);

        items.AddRange(anexos.Select(CriarEventoAnexo));

        var historicos = await historicoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Usuario)
            .Where(x => x.ChamadoId == chamadoId)
            .OrderBy(x => x.CriadoEm)
            .ToListAsync(cancellationToken);

        foreach (var historico in historicos)
        {
            if (historico.Tipo is TipoHistoricoChamado.Criado
                or TipoHistoricoChamado.ComentarioAdicionado
                or TipoHistoricoChamado.AnexoAdicionado)
            {
                continue;
            }

            if (ehSolicitante && !EhHistoricoVisivelParaSolicitante(historico))
            {
                continue;
            }

            items.Add(CriarEventoHistorico(historico));
        }

        var itemsOrdenados = items
            .OrderBy(x => x.DataHora)
            .ThenBy(x => x.Tipo, StringComparer.Ordinal)
            .ThenBy(x => x.Id)
            .ToArray();

        return new LinhaTempoChamadoResponse
        {
            ChamadoId = chamado.Id,
            Codigo = chamado.Codigo,
            Items = itemsOrdenados
        };
    }

    private static LinhaTempoChamadoItemResponse CriarEventoAbertura(Chamado chamado)
        => new()
        {
            Id = chamado.Id,
            Tipo = "abertura",
            TipoDescricao = "Abertura",
            DataHora = chamado.AbertoEm,
            UsuarioId = chamado.SolicitanteId,
            Usuario = chamado.Solicitante.Nome,
            Titulo = "Chamado aberto",
            Descricao = $"Chamado {chamado.Codigo} aberto.",
            Interno = false,
            ReferenciaId = chamado.Id,
            ReferenciaTipo = "chamado"
        };

    private static LinhaTempoChamadoItemResponse CriarEventoComentario(ComentarioChamado comentario)
        => new()
        {
            Id = comentario.Id,
            Tipo = "comentario",
            TipoDescricao = "Comentario",
            DataHora = comentario.CriadoEm,
            UsuarioId = comentario.UsuarioId,
            Usuario = comentario.Usuario.Nome,
            Titulo = comentario.Interno ? "Comentario interno" : "Comentario adicionado",
            Descricao = comentario.Mensagem,
            Interno = comentario.Interno,
            ReferenciaId = comentario.Id,
            ReferenciaTipo = "comentario"
        };

    private static LinhaTempoChamadoItemResponse CriarEventoAnexo(AnexoChamado anexo)
        => new()
        {
            Id = anexo.Id,
            Tipo = "anexo",
            TipoDescricao = "Anexo",
            DataHora = anexo.CriadoEm,
            UsuarioId = anexo.UsuarioId,
            Usuario = anexo.Usuario.Nome,
            Titulo = "Anexo adicionado",
            Descricao = anexo.NomeArquivo,
            Interno = false,
            ReferenciaId = anexo.Id,
            ReferenciaTipo = "anexo",
            NomeArquivo = anexo.NomeArquivo,
            ContentType = anexo.ContentType,
            TamanhoBytes = anexo.TamanhoBytes
        };

    private static LinhaTempoChamadoItemResponse CriarEventoHistorico(HistoricoChamado historico)
    {
        var (tipo, tipoDescricao, titulo, interno) = MapearHistorico(historico);

        return new LinhaTempoChamadoItemResponse
        {
            Id = historico.Id,
            Tipo = tipo,
            TipoDescricao = tipoDescricao,
            DataHora = historico.CriadoEm,
            UsuarioId = historico.UsuarioId,
            Usuario = historico.Usuario?.Nome,
            Titulo = titulo,
            Descricao = historico.Descricao,
            Interno = interno,
            ReferenciaId = historico.Id,
            ReferenciaTipo = "historico"
        };
    }

    private static (string Tipo, string TipoDescricao, string Titulo, bool Interno) MapearHistorico(HistoricoChamado historico)
        => historico.Tipo switch
        {
            TipoHistoricoChamado.StatusAlterado => ("status", "Status", "Status alterado", false),
            TipoHistoricoChamado.ResponsavelAlterado => ("responsavel", "Responsavel", "Responsavel alterado", true),
            TipoHistoricoChamado.PrioridadeAlterada => ("prioridade", "Prioridade", "Prioridade alterada", true),
            TipoHistoricoChamado.CategoriaAlterada => ("categoria", "Categoria", "Categoria alterada", true),
            TipoHistoricoChamado.AtivoVinculado => ("ativo", "Ativo", "Ativo vinculado", false),
            TipoHistoricoChamado.AtivoRemovido => ("ativo", "Ativo", "Vinculo de ativo removido", false),
            TipoHistoricoChamado.Encerrado => ("encerramento", "Encerramento", "Chamado encerrado", false),
            TipoHistoricoChamado.Reaberto => ("reabertura", "Reabertura", "Chamado reaberto", false),
            TipoHistoricoChamado.IntegracaoEmail => ("historico", "Historico", "Atualizacao interna", true),
            _ => ("historico", "Historico", "Atualizacao do chamado", true)
        };

    private static bool EhHistoricoVisivelParaSolicitante(HistoricoChamado historico)
    {
        if (historico.Tipo is TipoHistoricoChamado.StatusAlterado
            or TipoHistoricoChamado.Encerrado
            or TipoHistoricoChamado.Reaberto
            or TipoHistoricoChamado.AtivoVinculado
            or TipoHistoricoChamado.AtivoRemovido)
        {
            return true;
        }

        return false;
    }
}
