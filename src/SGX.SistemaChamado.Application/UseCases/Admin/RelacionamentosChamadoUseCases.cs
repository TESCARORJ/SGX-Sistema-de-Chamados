using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class RelacionamentosChamadoUseCases(
    IRepository<Chamado> chamadoRepository,
    IRepository<ChamadoRelacionamento> chamadoRelacionamentoRepository,
    IRepository<HistoricoChamado> historicoChamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAdminRelacionamentosChamadoUseCases
{
    internal const string MensagemRelacionamentoDuplicado = "Ja existe um relacionamento ativo entre estes chamados com este tipo de vinculo.";
    internal const string MensagemCicloRelacionamento = "Este relacionamento criaria um ciclo indevido entre chamados.";

    public async Task<ChamadoRelacionamentoAdminResponse> CriarAsync(
        CriarChamadoRelacionamentoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var usuario = await GarantirPermissaoOperacionalAsync(cancellationToken);

        var chamadoOrigem = await chamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Id == request.ChamadoOrigemId && x.Ativo)
            .Select(x => new { x.Id, x.Codigo })
            .FirstOrDefaultAsync(cancellationToken);
        if (chamadoOrigem is null)
        {
            throw new KeyNotFoundException("Chamado de origem nao encontrado.");
        }

        var chamadoDestino = await chamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Id == request.ChamadoDestinoId && x.Ativo)
            .Select(x => new { x.Id, x.Codigo })
            .FirstOrDefaultAsync(cancellationToken);
        if (chamadoDestino is null)
        {
            throw new KeyNotFoundException("Chamado de destino nao encontrado.");
        }

        return await CriarInternoAsync(
            request,
            usuario,
            chamadoOrigem.Codigo,
            chamadoDestino.Codigo,
            salvarAlteracoes: true,
            cancellationToken);
    }

    public async Task<ChamadoRelacionamentoAdminResponse> CriarNaUnidadeDeTrabalhoAsync(
        CriarChamadoRelacionamentoRequest request,
        string chamadoOrigemCodigo,
        string chamadoDestinoCodigo,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(chamadoOrigemCodigo))
        {
            throw new ArgumentException("Codigo do chamado de origem e obrigatorio.", nameof(chamadoOrigemCodigo));
        }

        if (string.IsNullOrWhiteSpace(chamadoDestinoCodigo))
        {
            throw new ArgumentException("Codigo do chamado de destino e obrigatorio.", nameof(chamadoDestinoCodigo));
        }

        var usuario = await GarantirPermissaoOperacionalAsync(cancellationToken);

        return await CriarInternoAsync(
            request,
            usuario,
            chamadoOrigemCodigo,
            chamadoDestinoCodigo,
            salvarAlteracoes: false,
            cancellationToken);
    }

    private async Task<ChamadoRelacionamentoAdminResponse> CriarInternoAsync(
        CriarChamadoRelacionamentoRequest request,
        UsuarioContextoAplicacao usuario,
        string chamadoOrigemCodigo,
        string chamadoDestinoCodigo,
        bool salvarAlteracoes,
        CancellationToken cancellationToken)
    {
        var jaExisteRelacionamentoAtivo = await chamadoRelacionamentoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x =>
                x.Ativo &&
                x.ChamadoOrigemId == request.ChamadoOrigemId &&
                x.ChamadoDestinoId == request.ChamadoDestinoId &&
                x.TipoRelacionamento == request.TipoRelacionamento,
                cancellationToken);

        if (jaExisteRelacionamentoAtivo)
        {
            throw new InvalidOperationException(MensagemRelacionamentoDuplicado);
        }

        await ValidarCicloIndevidoAsync(request, cancellationToken);

        var relacionamento = new ChamadoRelacionamento(
            request.ChamadoOrigemId,
            request.ChamadoDestinoId,
            request.TipoRelacionamento,
            usuario.Id,
            usuario.Login,
            request.Justificativa);

        await chamadoRelacionamentoRepository.AddAsync(relacionamento, cancellationToken);

        var descricaoOrigem = CriarDescricaoHistoricoVinculoCriado(relacionamento, chamadoDestinoCodigo);
        var descricaoDestino = CriarDescricaoHistoricoVinculoRecebido(relacionamento, chamadoOrigemCodigo);

        var historicoOrigem = new HistoricoChamado(
            relacionamento.ChamadoOrigemId,
            TipoHistoricoChamado.RelacionamentoCriado,
            descricaoOrigem,
            usuario.Id,
            usuario.Login);

        var historicoDestino = new HistoricoChamado(
            relacionamento.ChamadoDestinoId,
            TipoHistoricoChamado.RelacionamentoRecebido,
            descricaoDestino,
            usuario.Id,
            usuario.Login);

        await historicoChamadoRepository.AddAsync(historicoOrigem, cancellationToken);
        await historicoChamadoRepository.AddAsync(historicoDestino, cancellationToken);

        if (salvarAlteracoes)
        {
            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (EhViolacaoRelacionamentoDuplicadoAtivo(ex))
            {
                throw new InvalidOperationException(MensagemRelacionamentoDuplicado);
            }
        }

        return MapResponse(relacionamento, chamadoOrigemCodigo, chamadoDestinoCodigo);
    }

    public async Task RemoverAsync(
        RemoverChamadoRelacionamentoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var usuario = await GarantirPermissaoOperacionalAsync(cancellationToken);

        var relacionamento = await chamadoRelacionamentoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == request.RelacionamentoId, cancellationToken);

        if (relacionamento is null)
        {
            throw new KeyNotFoundException("Relacionamento nao encontrado.");
        }

        if (request.ChamadoId.HasValue &&
            relacionamento.ChamadoOrigemId != request.ChamadoId.Value &&
            relacionamento.ChamadoDestinoId != request.ChamadoId.Value)
        {
            throw new InvalidOperationException("Relacionamento nao pertence ao chamado informado.");
        }

        if (!relacionamento.Ativo)
        {
            throw new InvalidOperationException("Relacionamento ja esta inativo.");
        }

        var chamadoOrigem = await chamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Id == relacionamento.ChamadoOrigemId)
            .Select(x => new { x.Id, x.Codigo })
            .FirstOrDefaultAsync(cancellationToken);

        var chamadoDestino = await chamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Id == relacionamento.ChamadoDestinoId)
            .Select(x => new { x.Id, x.Codigo })
            .FirstOrDefaultAsync(cancellationToken);

        var codigoOrigem = chamadoOrigem?.Codigo ?? string.Empty;
        var codigoDestino = chamadoDestino?.Codigo ?? string.Empty;

        relacionamento.Inativar(usuario.Id, usuario.Login, request.Motivo);

        var descricaoOrigem = CriarDescricaoHistoricoVinculoRemovido(relacionamento, codigoDestino);
        var descricaoDestino = CriarDescricaoHistoricoVinculoRemovidoRecebido(relacionamento, codigoOrigem);

        var historicoOrigem = new HistoricoChamado(
            relacionamento.ChamadoOrigemId,
            TipoHistoricoChamado.RelacionamentoRemovido,
            descricaoOrigem,
            usuario.Id,
            usuario.Login);

        var historicoDestino = new HistoricoChamado(
            relacionamento.ChamadoDestinoId,
            TipoHistoricoChamado.RelacionamentoRemovidoRecebido,
            descricaoDestino,
            usuario.Id,
            usuario.Login);

        await historicoChamadoRepository.AddAsync(historicoOrigem, cancellationToken);
        await historicoChamadoRepository.AddAsync(historicoDestino, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChamadoRelacionamentoAdminResponse>> ListarPorChamadoAsync(
        Guid chamadoId,
        bool incluirInativos = false,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado informado e invalido.", nameof(chamadoId));
        }

        await GarantirPermissaoOperacionalAsync(cancellationToken);

        var chamadoExiste = await chamadoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == chamadoId, cancellationToken);

        if (!chamadoExiste)
        {
            throw new KeyNotFoundException("Chamado nao encontrado.");
        }

        var query = chamadoRelacionamentoRepository.Query()
            .AsNoTracking()
            .Include(x => x.ChamadoOrigem)
            .Include(x => x.ChamadoDestino)
            .Where(x => x.ChamadoOrigemId == chamadoId || x.ChamadoDestinoId == chamadoId);

        if (!incluirInativos)
        {
            query = query.Where(x => x.Ativo);
        }

        var relacionamentos = await query
            .OrderByDescending(x => x.CriadoEm)
            .ToListAsync(cancellationToken);

        return relacionamentos
            .Select(x => MapResponse(x, x.ChamadoOrigem.Codigo, x.ChamadoDestino.Codigo))
            .ToList();
    }

    public async Task<IReadOnlyList<DependenciaChamadoAdminResponse>> ListarDependenciasPorChamadoAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado informado e invalido.", nameof(chamadoId));
        }

        await GarantirPermissaoOperacionalAsync(cancellationToken);
        await GarantirChamadoExisteAsync(chamadoId, cancellationToken);

        var relacionamentos = await chamadoRelacionamentoRepository.Query()
            .AsNoTracking()
            .Include(x => x.ChamadoOrigem)
            .Include(x => x.ChamadoDestino)
            .Where(x =>
                x.Ativo &&
                (x.TipoRelacionamento == TipoRelacionamentoChamadoEnum.Bloqueia ||
                 x.TipoRelacionamento == TipoRelacionamentoChamadoEnum.BloqueadoPor) &&
                (x.ChamadoOrigemId == chamadoId || x.ChamadoDestinoId == chamadoId))
            .OrderByDescending(x => x.CriadoEm)
            .ToListAsync(cancellationToken);

        return relacionamentos
            .Select(x => MapDependenciaResponse(x, chamadoId))
            .ToList();
    }

    public async Task<bool> PossuiDependenciasAtivasAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default)
        => await EstaBloqueadoPorDependenciaAsync(chamadoId, cancellationToken);

    public async Task<bool> EstaBloqueadoPorDependenciaAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado informado e invalido.", nameof(chamadoId));
        }

        await GarantirPermissaoOperacionalAsync(cancellationToken);
        await GarantirChamadoExisteAsync(chamadoId, cancellationToken);

        return await chamadoRelacionamentoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x =>
                x.Ativo &&
                ((x.TipoRelacionamento == TipoRelacionamentoChamadoEnum.BloqueadoPor && x.ChamadoOrigemId == chamadoId) ||
                (x.TipoRelacionamento == TipoRelacionamentoChamadoEnum.Bloqueia && x.ChamadoDestinoId == chamadoId)),
                cancellationToken);
    }

    public async Task<BloqueioChamadoAdminResponse> ObterBloqueioPorChamadoAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default)
    {
        var dependencias = await ListarDependenciasPorChamadoAsync(chamadoId, cancellationToken);
        var bloqueadores = dependencias
            .Where(x => x.ChamadoConsultadoEhDependente)
            .ToList();
        var chamadosBloqueados = dependencias
            .Where(x => x.ChamadoConsultadoEhBloqueador)
            .ToList();

        return new BloqueioChamadoAdminResponse
        {
            ChamadoId = chamadoId,
            EstaBloqueado = bloqueadores.Count > 0,
            BloqueiaOutrosChamados = chamadosBloqueados.Count > 0,
            Bloqueadores = bloqueadores,
            ChamadosBloqueados = chamadosBloqueados
        };
    }

    public async Task<ChamadoRelacionamentoAdminResponse> ObterPorIdAsync(
        Guid relacionamentoId,
        CancellationToken cancellationToken = default)
    {
        if (relacionamentoId == Guid.Empty)
        {
            throw new ArgumentException("O relacionamento informado e invalido.", nameof(relacionamentoId));
        }

        await GarantirPermissaoOperacionalAsync(cancellationToken);

        var relacionamento = await chamadoRelacionamentoRepository.Query()
            .AsNoTracking()
            .Include(x => x.ChamadoOrigem)
            .Include(x => x.ChamadoDestino)
            .FirstOrDefaultAsync(x => x.Id == relacionamentoId, cancellationToken);

        if (relacionamento is null)
        {
            throw new KeyNotFoundException("Relacionamento nao encontrado.");
        }

        return MapResponse(relacionamento, relacionamento.ChamadoOrigem.Codigo, relacionamento.ChamadoDestino.Codigo);
    }

    private async Task GarantirChamadoExisteAsync(Guid chamadoId, CancellationToken cancellationToken)
    {
        var chamadoExiste = await chamadoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == chamadoId, cancellationToken);

        if (!chamadoExiste)
        {
            throw new KeyNotFoundException("Chamado nao encontrado.");
        }
    }

    private async Task<UsuarioContextoAplicacao> GarantirPermissaoOperacionalAsync(CancellationToken cancellationToken)
    {
        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        return usuario;
    }

    private static DependenciaChamadoAdminResponse MapDependenciaResponse(
        ChamadoRelacionamento relacionamento,
        Guid chamadoConsultadoId)
    {
        var normalizada = NormalizarDependencia(relacionamento);

        return new DependenciaChamadoAdminResponse
        {
            RelacionamentoId = relacionamento.Id,
            ChamadoDependenteId = normalizada.Dependente.Id,
            ChamadoDependenteCodigo = normalizada.Dependente.Codigo,
            ChamadoBloqueadorId = normalizada.Bloqueador.Id,
            ChamadoBloqueadorCodigo = normalizada.Bloqueador.Codigo,
            TipoRelacionamentoOriginal = relacionamento.TipoRelacionamento,
            TipoRelacionamentoDescricao = relacionamento.TipoRelacionamento.ToString(),
            Justificativa = relacionamento.Justificativa,
            CriadoEm = relacionamento.CriadoEm,
            ChamadoConsultadoEhDependente = normalizada.Dependente.Id == chamadoConsultadoId,
            ChamadoConsultadoEhBloqueador = normalizada.Bloqueador.Id == chamadoConsultadoId
        };
    }

    private static (Chamado Dependente, Chamado Bloqueador) NormalizarDependencia(ChamadoRelacionamento relacionamento)
    {
        return relacionamento.TipoRelacionamento switch
        {
            TipoRelacionamentoChamadoEnum.BloqueadoPor => (relacionamento.ChamadoOrigem, relacionamento.ChamadoDestino),
            TipoRelacionamentoChamadoEnum.Bloqueia => (relacionamento.ChamadoDestino, relacionamento.ChamadoOrigem),
            _ => throw new InvalidOperationException("O relacionamento informado nao representa dependencia entre chamados.")
        };
    }

    private static ChamadoRelacionamentoAdminResponse MapResponse(
        ChamadoRelacionamento relacionamento,
        string codigoOrigem,
        string codigoDestino)
    {
        return new ChamadoRelacionamentoAdminResponse
        {
            Id = relacionamento.Id,
            ChamadoOrigemId = relacionamento.ChamadoOrigemId,
            ChamadoOrigemCodigo = codigoOrigem,
            ChamadoDestinoId = relacionamento.ChamadoDestinoId,
            ChamadoDestinoCodigo = codigoDestino,
            TipoRelacionamento = relacionamento.TipoRelacionamento,
            TipoRelacionamentoDescricao = relacionamento.TipoRelacionamento.ToString(),
            Justificativa = relacionamento.Justificativa,
            Ativo = relacionamento.Ativo,
            CriadoEm = relacionamento.CriadoEm,
            CriadoPor = relacionamento.CriadoPor,
            RemovidoEm = relacionamento.RemovidoEm,
            MotivoRemocao = relacionamento.MotivoRemocao
        };
    }

    private static bool EhViolacaoRelacionamentoDuplicadoAtivo(DbUpdateException ex)
        => ex.InnerException?.Message.Contains("ux_chamados_relacionamentos_origem_destino_tipo_ativo", StringComparison.OrdinalIgnoreCase) == true
           || ex.Message.Contains("ux_chamados_relacionamentos_origem_destino_tipo_ativo", StringComparison.OrdinalIgnoreCase);

    private static string CriarDescricaoHistoricoVinculoCriado(ChamadoRelacionamento relacionamento, string codigoChamadoDestino)
    {
        var descricao = $"Vinculo criado com o chamado {codigoChamadoDestino} ({relacionamento.ChamadoDestinoId}) do tipo {relacionamento.TipoRelacionamento}. RelacionamentoId: {relacionamento.Id}.";
        if (!string.IsNullOrWhiteSpace(relacionamento.Justificativa))
        {
            descricao += $" Justificativa: {relacionamento.Justificativa}.";
        }

        return descricao;
    }

    private static string CriarDescricaoHistoricoVinculoRecebido(ChamadoRelacionamento relacionamento, string codigoChamadoOrigem)
    {
        var descricao = $"Vinculo recebido do chamado {codigoChamadoOrigem} ({relacionamento.ChamadoOrigemId}) do tipo {relacionamento.TipoRelacionamento}. RelacionamentoId: {relacionamento.Id}.";
        if (!string.IsNullOrWhiteSpace(relacionamento.Justificativa))
        {
            descricao += $" Justificativa: {relacionamento.Justificativa}.";
        }

        return descricao;
    }

    private static string CriarDescricaoHistoricoVinculoRemovido(ChamadoRelacionamento relacionamento, string codigoChamadoDestino)
    {
        var descricao = $"Vinculo removido com o chamado {codigoChamadoDestino} ({relacionamento.ChamadoDestinoId}) do tipo {relacionamento.TipoRelacionamento}. RelacionamentoId: {relacionamento.Id}.";
        if (!string.IsNullOrWhiteSpace(relacionamento.MotivoRemocao))
        {
            descricao += $" Motivo: {relacionamento.MotivoRemocao}.";
        }

        return descricao;
    }

    private static string CriarDescricaoHistoricoVinculoRemovidoRecebido(ChamadoRelacionamento relacionamento, string codigoChamadoOrigem)
    {
        var descricao = $"Vinculo removido recebido do chamado {codigoChamadoOrigem} ({relacionamento.ChamadoOrigemId}) do tipo {relacionamento.TipoRelacionamento}. RelacionamentoId: {relacionamento.Id}.";
        if (!string.IsNullOrWhiteSpace(relacionamento.MotivoRemocao))
        {
            descricao += $" Motivo: {relacionamento.MotivoRemocao}.";
        }

        return descricao;
    }

    private async Task ValidarCicloIndevidoAsync(
        CriarChamadoRelacionamentoRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryNormalizarDirecaoLogica(
            request.ChamadoOrigemId,
            request.ChamadoDestinoId,
            request.TipoRelacionamento,
            out var familia,
            out var origemNormalizada,
            out var destinoNormalizado))
        {
            return;
        }

        var tiposFamilia = ObterTiposDaFamilia(familia);

        var relacionamentosAtivosMesmaFamilia = await chamadoRelacionamentoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo && tiposFamilia.Contains(x.TipoRelacionamento))
            .ToListAsync(cancellationToken);

        var grafo = new Dictionary<Guid, List<Guid>>();
        foreach (var relacionamento in relacionamentosAtivosMesmaFamilia)
        {
            _ = TryNormalizarDirecaoLogica(
                relacionamento.ChamadoOrigemId,
                relacionamento.ChamadoDestinoId,
                relacionamento.TipoRelacionamento,
                out _,
                out var origem,
                out var destino);

            if (!grafo.TryGetValue(origem, out var adjacentes))
            {
                adjacentes = [];
                grafo[origem] = adjacentes;
            }

            adjacentes.Add(destino);
        }

        if (ExisteCaminho(grafo, destinoNormalizado, origemNormalizada))
        {
            throw new InvalidOperationException(MensagemCicloRelacionamento);
        }
    }

    private static bool ExisteCaminho(
        IReadOnlyDictionary<Guid, List<Guid>> grafo,
        Guid origem,
        Guid destino)
    {
        if (origem == destino)
        {
            return true;
        }

        var visitados = new HashSet<Guid>();
        var fila = new Queue<Guid>();
        fila.Enqueue(origem);
        visitados.Add(origem);

        while (fila.Count > 0)
        {
            var atual = fila.Dequeue();
            if (!grafo.TryGetValue(atual, out var adjacentes))
            {
                continue;
            }

            foreach (var proximo in adjacentes)
            {
                if (proximo == destino)
                {
                    return true;
                }

                if (visitados.Add(proximo))
                {
                    fila.Enqueue(proximo);
                }
            }
        }

        return false;
    }

    private static bool TryNormalizarDirecaoLogica(
        Guid chamadoOrigemId,
        Guid chamadoDestinoId,
        TipoRelacionamentoChamadoEnum tipoRelacionamento,
        out FamiliaRelacionamento familia,
        out Guid origemNormalizada,
        out Guid destinoNormalizado)
    {
        switch (tipoRelacionamento)
        {
            case TipoRelacionamentoChamadoEnum.Pai:
                familia = FamiliaRelacionamento.Hierarquia;
                origemNormalizada = chamadoOrigemId;
                destinoNormalizado = chamadoDestinoId;
                return true;
            case TipoRelacionamentoChamadoEnum.Filho:
                familia = FamiliaRelacionamento.Hierarquia;
                origemNormalizada = chamadoDestinoId;
                destinoNormalizado = chamadoOrigemId;
                return true;
            case TipoRelacionamentoChamadoEnum.Bloqueia:
                familia = FamiliaRelacionamento.Bloqueio;
                origemNormalizada = chamadoOrigemId;
                destinoNormalizado = chamadoDestinoId;
                return true;
            case TipoRelacionamentoChamadoEnum.BloqueadoPor:
                familia = FamiliaRelacionamento.Bloqueio;
                origemNormalizada = chamadoDestinoId;
                destinoNormalizado = chamadoOrigemId;
                return true;
            case TipoRelacionamentoChamadoEnum.Origina:
                familia = FamiliaRelacionamento.Derivacao;
                origemNormalizada = chamadoOrigemId;
                destinoNormalizado = chamadoDestinoId;
                return true;
            case TipoRelacionamentoChamadoEnum.DerivadoDe:
                familia = FamiliaRelacionamento.Derivacao;
                origemNormalizada = chamadoDestinoId;
                destinoNormalizado = chamadoOrigemId;
                return true;
            default:
                familia = default;
                origemNormalizada = Guid.Empty;
                destinoNormalizado = Guid.Empty;
                return false;
        }
    }

    private static TipoRelacionamentoChamadoEnum[] ObterTiposDaFamilia(FamiliaRelacionamento familia)
        => familia switch
        {
            FamiliaRelacionamento.Hierarquia =>
            [
                TipoRelacionamentoChamadoEnum.Pai,
                TipoRelacionamentoChamadoEnum.Filho
            ],
            FamiliaRelacionamento.Bloqueio =>
            [
                TipoRelacionamentoChamadoEnum.Bloqueia,
                TipoRelacionamentoChamadoEnum.BloqueadoPor
            ],
            FamiliaRelacionamento.Derivacao =>
            [
                TipoRelacionamentoChamadoEnum.Origina,
                TipoRelacionamentoChamadoEnum.DerivadoDe
            ],
            _ => []
        };

    private enum FamiliaRelacionamento
    {
        Hierarquia = 1,
        Bloqueio = 2,
        Derivacao = 3
    }
}
