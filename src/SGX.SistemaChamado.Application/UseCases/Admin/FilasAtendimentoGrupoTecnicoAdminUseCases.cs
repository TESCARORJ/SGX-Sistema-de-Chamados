using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarFilasAtendimentoGrupoTecnicoAdminUseCase(
    IRepository<GrupoTecnico> grupoTecnicoRepository,
    IRepository<FilaAtendimento> filaAtendimentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarFilasAtendimentoGrupoTecnicoAdminUseCase
{
    public async Task<IReadOnlyCollection<FilaAtendimentoResumoResponse>> ExecutarAsync(
        Guid grupoTecnicoId,
        ListarFilasAtendimentoGrupoTecnicoRequest request,
        CancellationToken cancellationToken = default)
    {
        ObterGrupoTecnicoAdminUseCase.GarantirIdValido(grupoTecnicoId);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var grupoExiste = await grupoTecnicoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == grupoTecnicoId, cancellationToken);

        if (!grupoExiste)
        {
            throw new KeyNotFoundException("Grupo tecnico nao encontrado.");
        }

        var query = filaAtendimentoRepository.Query()
            .AsNoTracking()
            .Where(x => x.GrupoTecnicoId == grupoTecnicoId);

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Busca))
        {
            var busca = request.Busca.Trim();
            query = query.Where(x => x.Nome.Contains(busca) || (x.Descricao ?? string.Empty).Contains(busca));
        }

        var filas = await query
            .OrderBy(x => x.Nome)
            .ToListAsync(cancellationToken);

        return filas.Select(MapResumo).ToArray();
    }

    private static FilaAtendimentoResumoResponse MapResumo(FilaAtendimento fila)
        => new(fila.Id, fila.GrupoTecnicoId, fila.Nome, fila.Descricao, fila.Ativo);
}
