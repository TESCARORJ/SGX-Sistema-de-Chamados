using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class InventarioAtivosAdminUseCases(
    IRepository<InventarioAtivo> inventarioAtivoRepository,
    IRepository<Chamado> chamadoRepository,
    IRepository<HistoricoInventarioAtivo> historicoInventarioAtivoRepository,
    IRepository<TipoAtivoInventario> tipoAtivoInventarioRepository,
    IRepository<Departamento> departamentoRepository,
    IRepository<LocalUnidade> localUnidadeRepository,
    IRepository<Usuario> usuarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAdminInventarioAtivosUseCases
{
    public async Task<PagedResultResponse<InventarioAtivoListagemDto>> ListarAsync(
        FiltroInventarioAtivoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = inventarioAtivoRepository.Query()
            .AsNoTracking()
            .Include(x => x.TipoAtivoInventario)
            .Include(x => x.Departamento)
            .Include(x => x.LocalUnidade)
            .Include(x => x.UsuarioResponsavel)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Termo))
        {
            var termo = request.Termo.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.Codigo.ToLower().Contains(termo) ||
                x.Nome.ToLower().Contains(termo) ||
                ((x.Descricao ?? string.Empty).ToLower().Contains(termo)) ||
                ((x.NumeroPatrimonio ?? string.Empty).ToLower().Contains(termo)) ||
                ((x.NumeroSerie ?? string.Empty).ToLower().Contains(termo)) ||
                ((x.Fabricante ?? string.Empty).ToLower().Contains(termo)) ||
                ((x.Modelo ?? string.Empty).ToLower().Contains(termo)) ||
                ((x.Fornecedor ?? string.Empty).ToLower().Contains(termo)));
        }

        if (request.TipoAtivoInventarioId.HasValue)
        {
            query = query.Where(x => x.TipoAtivoInventarioId == request.TipoAtivoInventarioId.Value);
        }

        if (request.DepartamentoId.HasValue)
        {
            query = query.Where(x => x.DepartamentoId == request.DepartamentoId.Value);
        }

        if (request.LocalUnidadeId.HasValue)
        {
            query = query.Where(x => x.LocalUnidadeId == request.LocalUnidadeId.Value);
        }

        if (request.UsuarioResponsavelId.HasValue)
        {
            query = query.Where(x => x.UsuarioResponsavelId == request.UsuarioResponsavelId.Value);
        }

        if (request.StatusOperacional.HasValue)
        {
            query = query.Where(x => x.StatusOperacional == request.StatusOperacional.Value);
        }

        if (request.StatusPatrimonial.HasValue)
        {
            query = query.Where(x => x.StatusPatrimonial == request.StatusPatrimonial.Value);
        }

        if (request.Criticidade.HasValue)
        {
            query = query.Where(x => x.Criticidade == request.Criticidade.Value);
        }

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (request.DataAquisicaoInicial.HasValue)
        {
            var dataInicial = request.DataAquisicaoInicial.Value.Date;
            query = query.Where(x => x.DataAquisicao.HasValue && x.DataAquisicao.Value >= dataInicial);
        }

        if (request.DataAquisicaoFinal.HasValue)
        {
            var dataFinal = request.DataAquisicaoFinal.Value.Date;
            query = query.Where(x => x.DataAquisicao.HasValue && x.DataAquisicao.Value <= dataFinal);
        }

        if (request.DataFimGarantiaInicial.HasValue)
        {
            var dataInicial = request.DataFimGarantiaInicial.Value.Date;
            query = query.Where(x => x.DataFimGarantia.HasValue && x.DataFimGarantia.Value >= dataInicial);
        }

        if (request.DataFimGarantiaFinal.HasValue)
        {
            var dataFinal = request.DataFimGarantiaFinal.Value.Date;
            query = query.Where(x => x.DataFimGarantia.HasValue && x.DataFimGarantia.Value <= dataFinal);
        }

        var desc = AdminCadastrosHelpers.DirecaoDesc(request.DirecaoOrdenacao);
        query = (request.OrdenarPor ?? "atualizadoEm").Trim().ToLowerInvariant() switch
        {
            "codigo" => desc ? query.OrderByDescending(x => x.Codigo) : query.OrderBy(x => x.Codigo),
            "nome" => desc ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome),
            "dataaquisicao" => desc ? query.OrderByDescending(x => x.DataAquisicao) : query.OrderBy(x => x.DataAquisicao),
            "datafimgarantia" => desc ? query.OrderByDescending(x => x.DataFimGarantia) : query.OrderBy(x => x.DataFimGarantia),
            "criadoem" => desc ? query.OrderByDescending(x => x.CriadoEm) : query.OrderBy(x => x.CriadoEm),
            _ => desc
                ? query.OrderByDescending(x => x.AtualizadoEm ?? x.CriadoEm)
                : query.OrderBy(x => x.AtualizadoEm ?? x.CriadoEm)
        };

        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 100);

        var total = await query.CountAsync(cancellationToken);
        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new PagedResultResponse<InventarioAtivoListagemDto>
        {
            Items = itens.Select(InventarioAtivoMapeamentos.MapListagem).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }

    public async Task<InventarioAtivoDetalheDto> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var ativo = await ObterInventarioCompletoPorIdAsync(id, asNoTracking: true, cancellationToken)
            ?? throw new KeyNotFoundException("Ativo de inventario nao encontrado.");

        return InventarioAtivoMapeamentos.MapDetalhe(ativo);
    }

    public async Task<InventarioAtivoDetalheDto> CriarAsync(
        CriarInventarioAtivoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var codigo = request.Codigo.Trim().ToUpperInvariant();
        var nome = request.Nome.Trim();
        var numeroPatrimonio = NormalizarTextoOpcional(request.NumeroPatrimonio);
        var numeroSerie = NormalizarTextoOpcional(request.NumeroSerie);

        await InventarioAtivoValidacoes.ValidarRelacionamentosAsync(
            tipoAtivoInventarioRepository,
            departamentoRepository,
            localUnidadeRepository,
            usuarioRepository,
            request.TipoAtivoInventarioId,
            request.DepartamentoId,
            request.LocalUnidadeId,
            request.UsuarioResponsavelId,
            cancellationToken);

        await InventarioAtivoValidacoes.ValidarUnicidadeAsync(
            inventarioAtivoRepository,
            codigo,
            numeroPatrimonio,
            numeroSerie,
            null,
            cancellationToken);

        var ativo = new InventarioAtivo(
            codigo,
            nome,
            request.TipoAtivoInventarioId,
            usuarioAtual.Id,
            usuarioAtual.Login);

        ativo.DefinirDescricao(request.Descricao);
        ativo.DefinirNumeroPatrimonio(numeroPatrimonio);
        ativo.DefinirNumeroSerie(numeroSerie);
        ativo.DefinirFabricante(request.Fabricante);
        ativo.DefinirModelo(request.Modelo);
        ativo.DefinirDepartamento(request.DepartamentoId);
        ativo.DefinirLocalUnidade(request.LocalUnidadeId);
        ativo.DefinirUsuarioResponsavel(request.UsuarioResponsavelId);
        ativo.DefinirStatusOperacional(request.StatusOperacional ?? StatusOperacionalAtivo.Operacional);
        ativo.DefinirStatusPatrimonial(request.StatusPatrimonial ?? StatusPatrimonialAtivo.EmUso);
        ativo.DefinirCriticidade(request.Criticidade ?? CriticidadeAtivo.Media);
        ativo.DefinirDataAquisicao(request.DataAquisicao);
        ativo.DefinirDataFimGarantia(request.DataFimGarantia);
        ativo.DefinirValorAquisicao(request.ValorAquisicao);
        ativo.DefinirFornecedor(request.Fornecedor);
        ativo.DefinirObservacoes(request.Observacoes);

        await inventarioAtivoRepository.AddAsync(ativo, cancellationToken);
        await RegistrarHistoricoCriacaoAsync(ativo, usuarioAtual.Id, usuarioAtual.Login, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await ObterInventarioCompletoPorIdAsync(ativo.Id, asNoTracking: true, cancellationToken)
            ?? throw new KeyNotFoundException("Ativo de inventario nao encontrado apos criacao.");

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarCriacaoAsync(
                "Inventario/Ativos",
                "InventarioAtivo",
                ativo.Id.ToString(),
                "Ativo de inventario criado.",
                dadosDepois: InventarioAtivoMapeamentos.SerializarAuditoria(completo),
                metadados: InventarioAtivoAuditoriaHelper.CriarMetadados(completo, "CriacaoInventarioAtivo"),
                cancellationToken: cancellationToken);
        }

        return InventarioAtivoMapeamentos.MapDetalhe(completo);
    }

    public async Task<InventarioAtivoDetalheDto> AtualizarAsync(
        Guid id,
        AtualizarInventarioAtivoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var ativo = await ObterInventarioCompletoPorIdAsync(id, asNoTracking: false, cancellationToken)
            ?? throw new KeyNotFoundException("Ativo de inventario nao encontrado.");

        if (!ativo.Ativo)
        {
            throw new InvalidOperationException("Ativo inativo nao pode ser editado. Reative-o antes da edicao.");
        }

        var codigo = request.Codigo.Trim().ToUpperInvariant();
        var nome = request.Nome.Trim();
        var numeroPatrimonio = NormalizarTextoOpcional(request.NumeroPatrimonio);
        var numeroSerie = NormalizarTextoOpcional(request.NumeroSerie);

        await InventarioAtivoValidacoes.ValidarRelacionamentosAsync(
            tipoAtivoInventarioRepository,
            departamentoRepository,
            localUnidadeRepository,
            usuarioRepository,
            request.TipoAtivoInventarioId,
            request.DepartamentoId,
            request.LocalUnidadeId,
            request.UsuarioResponsavelId,
            cancellationToken);

        await InventarioAtivoValidacoes.ValidarUnicidadeAsync(
            inventarioAtivoRepository,
            codigo,
            numeroPatrimonio,
            numeroSerie,
            ativo.Id,
            cancellationToken);

        var estadoAnterior = InventarioAtivoHistoricoHelper.CapturarEstado(ativo);
        var dadosAntes = InventarioAtivoMapeamentos.SerializarAuditoria(ativo);

        ativo.DefinirCodigo(codigo);
        ativo.DefinirNome(nome);
        ativo.DefinirDescricao(request.Descricao);
        ativo.DefinirNumeroPatrimonio(numeroPatrimonio);
        ativo.DefinirNumeroSerie(numeroSerie);
        ativo.DefinirTipoAtivoInventario(request.TipoAtivoInventarioId);
        ativo.DefinirFabricante(request.Fabricante);
        ativo.DefinirModelo(request.Modelo);
        ativo.DefinirDepartamento(request.DepartamentoId);
        ativo.DefinirLocalUnidade(request.LocalUnidadeId);
        ativo.DefinirUsuarioResponsavel(request.UsuarioResponsavelId);
        ativo.DefinirStatusOperacional(request.StatusOperacional);
        ativo.DefinirStatusPatrimonial(request.StatusPatrimonial);
        ativo.DefinirCriticidade(request.Criticidade);
        ativo.DefinirDataAquisicao(request.DataAquisicao);
        ativo.DefinirDataFimGarantia(request.DataFimGarantia);
        ativo.DefinirValorAquisicao(request.ValorAquisicao);
        ativo.DefinirFornecedor(request.Fornecedor);
        ativo.DefinirObservacoes(request.Observacoes);
        ativo.AtualizarAuditoriaUsuario(usuarioAtual.Id, usuarioAtual.Login);

        var mudancasRelevantes = InventarioAtivoHistoricoHelper.DetectarMudancasRelevantes(
            estadoAnterior,
            InventarioAtivoHistoricoHelper.CapturarEstado(ativo));

        if (mudancasRelevantes.PossuiMudanca)
        {
            await RegistrarHistoricoMudancaRelevanteAsync(
                ativo,
                mudancasRelevantes,
                usuarioAtual.Id,
                usuarioAtual.Login,
                "Alteracao relevante no cadastro de ativo.",
                origemMovimentacao: false,
                cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await ObterInventarioCompletoPorIdAsync(ativo.Id, asNoTracking: true, cancellationToken)
            ?? throw new KeyNotFoundException("Ativo de inventario nao encontrado apos atualizacao.");

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Inventario/Ativos",
                "InventarioAtivo",
                ativo.Id.ToString(),
                "Ativo de inventario atualizado.",
                dadosAntes: dadosAntes,
                dadosDepois: InventarioAtivoMapeamentos.SerializarAuditoria(completo),
                metadados: InventarioAtivoAuditoriaHelper.CriarMetadados(completo, "AtualizacaoInventarioAtivo"),
                cancellationToken: cancellationToken);
        }

        return InventarioAtivoMapeamentos.MapDetalhe(completo);
    }

    public async Task<InventarioAtivoDetalheDto> MovimentarAsync(
        Guid id,
        MovimentarInventarioAtivoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var ativo = await ObterInventarioCompletoPorIdAsync(id, asNoTracking: false, cancellationToken)
            ?? throw new KeyNotFoundException("Ativo de inventario nao encontrado.");

        if (!ativo.Ativo)
        {
            throw new InvalidOperationException("Ativo inativo nao pode ser movimentado.");
        }

        await InventarioAtivoValidacoes.ValidarRelacionamentosMovimentacaoAsync(
            departamentoRepository,
            localUnidadeRepository,
            usuarioRepository,
            request.DepartamentoId,
            request.LocalUnidadeId,
            request.UsuarioResponsavelId,
            cancellationToken);

        var estadoAnterior = InventarioAtivoHistoricoHelper.CapturarEstado(ativo);
        var dadosAntes = InventarioAtivoMapeamentos.SerializarAuditoria(ativo);

        if (request.DepartamentoId.HasValue && request.DepartamentoId.Value != ativo.DepartamentoId)
        {
            ativo.DefinirDepartamento(request.DepartamentoId.Value);
        }

        if (request.LocalUnidadeId.HasValue && request.LocalUnidadeId.Value != ativo.LocalUnidadeId)
        {
            ativo.DefinirLocalUnidade(request.LocalUnidadeId.Value);
        }

        if (request.UsuarioResponsavelId.HasValue && request.UsuarioResponsavelId.Value != ativo.UsuarioResponsavelId)
        {
            ativo.DefinirUsuarioResponsavel(request.UsuarioResponsavelId.Value);
        }

        if (request.StatusOperacional.HasValue && request.StatusOperacional.Value != ativo.StatusOperacional)
        {
            ativo.DefinirStatusOperacional(request.StatusOperacional.Value);
        }

        if (request.StatusPatrimonial.HasValue && request.StatusPatrimonial.Value != ativo.StatusPatrimonial)
        {
            ativo.DefinirStatusPatrimonial(request.StatusPatrimonial.Value);
        }

        var mudancasRelevantes = InventarioAtivoHistoricoHelper.DetectarMudancasRelevantes(
            estadoAnterior,
            InventarioAtivoHistoricoHelper.CapturarEstado(ativo));

        if (!mudancasRelevantes.PossuiMudanca)
        {
            throw new InvalidOperationException("Nenhuma alteracao relevante foi informada para movimentacao.");
        }

        ativo.AtualizarAuditoriaUsuario(usuarioAtual.Id, usuarioAtual.Login);

        await RegistrarHistoricoMudancaRelevanteAsync(
            ativo,
            mudancasRelevantes,
            usuarioAtual.Id,
            usuarioAtual.Login,
            request.Observacao,
            origemMovimentacao: true,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await ObterInventarioCompletoPorIdAsync(ativo.Id, asNoTracking: true, cancellationToken)
            ?? throw new KeyNotFoundException("Ativo de inventario nao encontrado apos movimentacao.");

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Inventario/Ativos",
                "InventarioAtivo",
                ativo.Id.ToString(),
                "Ativo de inventario movimentado.",
                dadosAntes: dadosAntes,
                dadosDepois: InventarioAtivoMapeamentos.SerializarAuditoria(completo),
                metadados: InventarioAtivoAuditoriaHelper.CriarMetadados(completo, "MovimentacaoInventarioAtivo"),
                cancellationToken: cancellationToken);
        }

        return InventarioAtivoMapeamentos.MapDetalhe(completo);
    }

    public async Task<PagedResultResponse<HistoricoInventarioAtivoDto>> ListarHistoricoAsync(
        Guid inventarioAtivoId,
        FiltroHistoricoInventarioAtivoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (inventarioAtivoId == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(inventarioAtivoId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var ativoExiste = await inventarioAtivoRepository.Query()
            .AnyAsync(x => x.Id == inventarioAtivoId, cancellationToken);

        if (!ativoExiste)
        {
            throw new KeyNotFoundException("Ativo de inventario nao encontrado.");
        }

        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 100);

        var query = historicoInventarioAtivoRepository.Query()
            .AsNoTracking()
            .Include(x => x.DepartamentoOrigem)
            .Include(x => x.DepartamentoDestino)
            .Include(x => x.LocalUnidadeOrigem)
            .Include(x => x.LocalUnidadeDestino)
            .Include(x => x.UsuarioResponsavelOrigem)
            .Include(x => x.UsuarioResponsavelDestino)
            .Include(x => x.CriadoPorUsuario)
            .Where(x => x.InventarioAtivoId == inventarioAtivoId)
            .OrderByDescending(x => x.CriadoEm)
            .AsQueryable();

        var total = await query.CountAsync(cancellationToken);
        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new PagedResultResponse<HistoricoInventarioAtivoDto>
        {
            Items = itens.Select(InventarioAtivoMapeamentos.MapHistorico).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }

    public async Task<PagedResultResponse<ChamadoRelacionadoInventarioAtivoDto>> ListarChamadosAsync(
        Guid inventarioAtivoId,
        FiltroChamadosRelacionadosInventarioAtivoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (inventarioAtivoId == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(inventarioAtivoId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var ativoExiste = await inventarioAtivoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == inventarioAtivoId, cancellationToken);

        if (!ativoExiste)
        {
            throw new KeyNotFoundException("Ativo de inventario nao encontrado.");
        }

        var query = chamadoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Status)
            .Include(x => x.Prioridade)
            .Include(x => x.Solicitante)
            .Where(x => x.InventarioAtivoId == inventarioAtivoId)
            .OrderByDescending(x => x.AtualizadoEm ?? x.CriadoEm)
            .ThenByDescending(x => x.AbertoEm)
            .AsQueryable();

        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 100);
        var total = await query.CountAsync(cancellationToken);
        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new PagedResultResponse<ChamadoRelacionadoInventarioAtivoDto>
        {
            Items = itens.Select(InventarioAtivoMapeamentos.MapChamadoRelacionado).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }

    public async Task<AlterarSituacaoCadastroResponse> InativarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var ativo = await inventarioAtivoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Ativo de inventario nao encontrado.");

        if (!ativo.Ativo)
        {
            return new AlterarSituacaoCadastroResponse(ativo.Id, false, "Ativo ja estava inativo.");
        }

        ativo.Inativar(usuarioAtual.Id, usuarioAtual.Login);

        var historico = new HistoricoInventarioAtivo(
            ativo.Id,
            TipoMovimentacaoAtivo.Inativacao,
            usuarioAtual.Id,
            usuarioAtual.Login,
            "Ativo inativado.");
        await historicoInventarioAtivoRepository.AddAsync(historico, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarInativacaoAsync(
                "Inventario/Ativos",
                "InventarioAtivo",
                ativo.Id.ToString(),
                "Ativo de inventario inativado.",
                InventarioAtivoAuditoriaHelper.CriarMetadados(ativo, "InativacaoInventarioAtivo"),
                cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(ativo.Id, false, "Ativo inativado com sucesso.");
    }

    public async Task<AlterarSituacaoCadastroResponse> ReativarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var ativo = await inventarioAtivoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Ativo de inventario nao encontrado.");

        if (ativo.Ativo)
        {
            throw new InvalidOperationException("Somente ativos inativos podem ser reativados.");
        }

        ativo.Reativar(usuarioAtual.Id, usuarioAtual.Login);

        var historico = new HistoricoInventarioAtivo(
            ativo.Id,
            TipoMovimentacaoAtivo.Reativacao,
            usuarioAtual.Id,
            usuarioAtual.Login,
            "Ativo reativado.");
        await historicoInventarioAtivoRepository.AddAsync(historico, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAtivacaoAsync(
                "Inventario/Ativos",
                "InventarioAtivo",
                ativo.Id.ToString(),
                "Ativo de inventario reativado.",
                InventarioAtivoAuditoriaHelper.CriarMetadados(ativo, "ReativacaoInventarioAtivo"),
                cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(ativo.Id, true, "Ativo reativado com sucesso.");
    }

    public async Task<IReadOnlyCollection<TipoAtivoInventarioDto>> ListarTiposAtivoAsync(CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var tipos = await tipoAtivoInventarioRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.Nome)
            .ToListAsync(cancellationToken);

        return tipos
            .Select(x => new TipoAtivoInventarioDto(x.Id, x.Nome, x.Descricao, x.Ativo))
            .ToArray();
    }

    private async Task RegistrarHistoricoCriacaoAsync(
        InventarioAtivo ativo,
        Guid usuarioId,
        string usuarioLogin,
        CancellationToken cancellationToken)
    {
        var historico = new HistoricoInventarioAtivo(
            ativo.Id,
            TipoMovimentacaoAtivo.Criacao,
            usuarioId,
            usuarioLogin,
            "Ativo criado.");

        historico.DefinirDepartamentos(null, ativo.DepartamentoId);
        historico.DefinirLocaisUnidade(null, ativo.LocalUnidadeId);
        historico.DefinirUsuariosResponsaveis(null, ativo.UsuarioResponsavelId);
        historico.DefinirStatusOperacional(null, ativo.StatusOperacional);
        historico.DefinirStatusPatrimonial(null, ativo.StatusPatrimonial);

        await historicoInventarioAtivoRepository.AddAsync(historico, cancellationToken);
    }

    private async Task RegistrarHistoricoMudancaRelevanteAsync(
        InventarioAtivo ativo,
        InventarioAtivoMudancasRelevantes mudancas,
        Guid usuarioId,
        string usuarioLogin,
        string? observacao,
        bool origemMovimentacao,
        CancellationToken cancellationToken)
    {
        var tipoMovimentacao = InventarioAtivoHistoricoHelper.DeterminarTipoMovimentacao(
            mudancas,
            origemMovimentacao,
            mudancas.StatusOperacionalNovo);

        var historico = new HistoricoInventarioAtivo(
            ativo.Id,
            tipoMovimentacao,
            usuarioId,
            usuarioLogin,
            observacao);

        if (mudancas.DepartamentoAlterado)
        {
            historico.DefinirDepartamentos(mudancas.DepartamentoOrigemId, mudancas.DepartamentoDestinoId);
        }

        if (mudancas.LocalAlterado)
        {
            historico.DefinirLocaisUnidade(mudancas.LocalOrigemId, mudancas.LocalDestinoId);
        }

        if (mudancas.UsuarioResponsavelAlterado)
        {
            historico.DefinirUsuariosResponsaveis(mudancas.UsuarioResponsavelOrigemId, mudancas.UsuarioResponsavelDestinoId);
        }

        if (mudancas.StatusOperacionalAlterado)
        {
            historico.DefinirStatusOperacional(mudancas.StatusOperacionalAnterior, mudancas.StatusOperacionalNovo);
        }

        if (mudancas.StatusPatrimonialAlterado)
        {
            historico.DefinirStatusPatrimonial(mudancas.StatusPatrimonialAnterior, mudancas.StatusPatrimonialNovo);
        }

        await historicoInventarioAtivoRepository.AddAsync(historico, cancellationToken);
    }

    private async Task<InventarioAtivo?> ObterInventarioCompletoPorIdAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken)
    {
        var query = inventarioAtivoRepository.Query()
            .Include(x => x.TipoAtivoInventario)
            .Include(x => x.Departamento)
            .Include(x => x.LocalUnidade)
            .Include(x => x.UsuarioResponsavel)
            .Where(x => x.Id == id);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    private static string? NormalizarTextoOpcional(string? valor)
        => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}

internal static class InventarioAtivoValidacoes
{
    public static async Task ValidarRelacionamentosAsync(
        IRepository<TipoAtivoInventario> tipoAtivoInventarioRepository,
        IRepository<Departamento> departamentoRepository,
        IRepository<LocalUnidade> localUnidadeRepository,
        IRepository<Usuario> usuarioRepository,
        Guid tipoAtivoInventarioId,
        Guid? departamentoId,
        Guid? localUnidadeId,
        Guid? usuarioResponsavelId,
        CancellationToken cancellationToken)
    {
        var tipoValido = await tipoAtivoInventarioRepository.Query()
            .AnyAsync(x => x.Id == tipoAtivoInventarioId && x.Ativo, cancellationToken);
        if (!tipoValido)
        {
            throw new InvalidOperationException("Tipo de ativo informado nao encontrado ou inativo.");
        }

        await ValidarRelacionamentosMovimentacaoAsync(
            departamentoRepository,
            localUnidadeRepository,
            usuarioRepository,
            departamentoId,
            localUnidadeId,
            usuarioResponsavelId,
            cancellationToken);
    }

    public static async Task ValidarRelacionamentosMovimentacaoAsync(
        IRepository<Departamento> departamentoRepository,
        IRepository<LocalUnidade> localUnidadeRepository,
        IRepository<Usuario> usuarioRepository,
        Guid? departamentoId,
        Guid? localUnidadeId,
        Guid? usuarioResponsavelId,
        CancellationToken cancellationToken)
    {
        if (departamentoId.HasValue)
        {
            var departamentoValido = await departamentoRepository.Query()
                .AnyAsync(x => x.Id == departamentoId.Value && x.Ativo, cancellationToken);
            if (!departamentoValido)
            {
                throw new InvalidOperationException("Departamento informado nao encontrado ou inativo.");
            }
        }

        if (localUnidadeId.HasValue)
        {
            var localValido = await localUnidadeRepository.Query()
                .AnyAsync(x => x.Id == localUnidadeId.Value && x.Ativo, cancellationToken);
            if (!localValido)
            {
                throw new InvalidOperationException("Local/unidade informado nao encontrado ou inativo.");
            }
        }

        if (usuarioResponsavelId.HasValue)
        {
            var usuarioValido = await usuarioRepository.Query()
                .AnyAsync(x => x.Id == usuarioResponsavelId.Value &&
                               x.Ativo &&
                               x.Situacao == SituacaoUsuario.Ativo,
                    cancellationToken);
            if (!usuarioValido)
            {
                throw new InvalidOperationException("Usuario responsavel informado nao encontrado ou inativo.");
            }
        }
    }

    public static async Task ValidarUnicidadeAsync(
        IRepository<InventarioAtivo> inventarioAtivoRepository,
        string codigo,
        string? numeroPatrimonio,
        string? numeroSerie,
        Guid? idIgnorado,
        CancellationToken cancellationToken)
    {
        var query = inventarioAtivoRepository.Query();
        if (idIgnorado.HasValue)
        {
            query = query.Where(x => x.Id != idIgnorado.Value);
        }

        var codigoDuplicado = await query.AnyAsync(x => x.Codigo == codigo, cancellationToken);
        if (codigoDuplicado)
        {
            throw new InvalidOperationException("Ja existe ativo com este codigo.");
        }

        if (!string.IsNullOrWhiteSpace(numeroPatrimonio))
        {
            var patrimonioDuplicado = await query.AnyAsync(x => x.NumeroPatrimonio == numeroPatrimonio, cancellationToken);
            if (patrimonioDuplicado)
            {
                throw new InvalidOperationException("Ja existe ativo com este numero de patrimonio.");
            }
        }

        if (!string.IsNullOrWhiteSpace(numeroSerie))
        {
            var serieDuplicada = await query.AnyAsync(x => x.NumeroSerie == numeroSerie, cancellationToken);
            if (serieDuplicada)
            {
                throw new InvalidOperationException("Ja existe ativo com este numero de serie.");
            }
        }
    }
}

internal static class InventarioAtivoMapeamentos
{
    public static InventarioAtivoListagemDto MapListagem(InventarioAtivo ativo)
        => new(
            ativo.Id,
            ativo.Codigo,
            ativo.Nome,
            ativo.NumeroPatrimonio,
            ativo.NumeroSerie,
            ativo.TipoAtivoInventarioId,
            ativo.TipoAtivoInventario?.Nome ?? string.Empty,
            ativo.DepartamentoId,
            ativo.Departamento?.Nome,
            ativo.LocalUnidadeId,
            ativo.LocalUnidade?.Nome,
            ativo.UsuarioResponsavelId,
            ativo.UsuarioResponsavel?.Nome,
            ativo.StatusOperacional,
            DescricaoStatusOperacional(ativo.StatusOperacional),
            ativo.StatusPatrimonial,
            DescricaoStatusPatrimonial(ativo.StatusPatrimonial),
            ativo.Criticidade,
            DescricaoCriticidade(ativo.Criticidade),
            ativo.DataAquisicao,
            ativo.DataFimGarantia,
            ativo.Ativo,
            ativo.CriadoEm,
            ativo.AtualizadoEm);

    public static InventarioAtivoDetalheDto MapDetalhe(InventarioAtivo ativo)
        => new(
            ativo.Id,
            ativo.Codigo,
            ativo.Nome,
            ativo.Descricao,
            ativo.NumeroPatrimonio,
            ativo.NumeroSerie,
            ativo.TipoAtivoInventarioId,
            ativo.TipoAtivoInventario?.Nome ?? string.Empty,
            ativo.Fabricante,
            ativo.Modelo,
            ativo.DepartamentoId,
            ativo.Departamento?.Nome,
            ativo.LocalUnidadeId,
            ativo.LocalUnidade?.Nome,
            ativo.UsuarioResponsavelId,
            ativo.UsuarioResponsavel?.Nome,
            ativo.StatusOperacional,
            DescricaoStatusOperacional(ativo.StatusOperacional),
            ativo.StatusPatrimonial,
            DescricaoStatusPatrimonial(ativo.StatusPatrimonial),
            ativo.Criticidade,
            DescricaoCriticidade(ativo.Criticidade),
            ativo.DataAquisicao,
            ativo.DataFimGarantia,
            ativo.ValorAquisicao,
            ativo.Fornecedor,
            ativo.Observacoes,
            ativo.Ativo,
            ativo.CriadoEm,
            ativo.CriadoPorUsuarioId,
            ativo.AtualizadoEm,
            ativo.AtualizadoPorUsuarioId,
            ativo.InativadoEm,
            ativo.InativadoPorUsuarioId);

    public static HistoricoInventarioAtivoDto MapHistorico(HistoricoInventarioAtivo historico)
        => new(
            historico.Id,
            historico.InventarioAtivoId,
            historico.TipoMovimentacao,
            DescricaoTipoMovimentacao(historico.TipoMovimentacao),
            historico.DepartamentoOrigem?.Nome,
            historico.DepartamentoDestino?.Nome,
            historico.LocalUnidadeOrigem?.Nome,
            historico.LocalUnidadeDestino?.Nome,
            historico.UsuarioResponsavelOrigem?.Nome,
            historico.UsuarioResponsavelDestino?.Nome,
            historico.StatusOperacionalAnterior,
            historico.StatusOperacionalNovo,
            historico.StatusPatrimonialAnterior,
            historico.StatusPatrimonialNovo,
            historico.Observacao,
            historico.CriadoEm,
            historico.CriadoPorUsuario?.Nome ?? historico.CriadoPor);

    public static ChamadoRelacionadoInventarioAtivoDto MapChamadoRelacionado(Chamado chamado)
        => new(
            chamado.Id,
            chamado.Codigo,
            chamado.Titulo,
            chamado.Status.Nome,
            chamado.Prioridade.Nome,
            chamado.Solicitante.Nome,
            chamado.CriadoEm,
            chamado.AtualizadoEm,
            chamado.EncerradoEm);

    public static string? SerializarAuditoria(InventarioAtivo ativo)
        => AuditoriaDiffHelper.SerializarSeguro(new
        {
            ativo.Codigo,
            ativo.Nome,
            ativo.Descricao,
            ativo.NumeroPatrimonio,
            ativo.NumeroSerie,
            ativo.TipoAtivoInventarioId,
            ativo.Fabricante,
            ativo.Modelo,
            ativo.DepartamentoId,
            ativo.LocalUnidadeId,
            ativo.UsuarioResponsavelId,
            ativo.StatusOperacional,
            ativo.StatusPatrimonial,
            ativo.Criticidade,
            ativo.DataAquisicao,
            ativo.DataFimGarantia,
            ativo.ValorAquisicao,
            ativo.Fornecedor,
            ativo.Observacoes,
            ativo.Ativo,
            ativo.InativadoEm,
            ativo.InativadoPorUsuarioId
        });

    private static string DescricaoStatusOperacional(StatusOperacionalAtivo value) => value switch
    {
        StatusOperacionalAtivo.Operacional => "Operacional",
        StatusOperacionalAtivo.EmManutencao => "Em manutencao",
        StatusOperacionalAtivo.ComDefeito => "Com defeito",
        StatusOperacionalAtivo.Reservado => "Reservado",
        StatusOperacionalAtivo.Baixado => "Baixado",
        _ => value.ToString()
    };

    private static string DescricaoStatusPatrimonial(StatusPatrimonialAtivo value) => value switch
    {
        StatusPatrimonialAtivo.EmUso => "Em uso",
        StatusPatrimonialAtivo.EmEstoque => "Em estoque",
        StatusPatrimonialAtivo.Emprestado => "Emprestado",
        StatusPatrimonialAtivo.EmTransferencia => "Em transferencia",
        StatusPatrimonialAtivo.Descartado => "Descartado",
        StatusPatrimonialAtivo.Extraviado => "Extraviado",
        _ => value.ToString()
    };

    private static string DescricaoCriticidade(CriticidadeAtivo value) => value switch
    {
        CriticidadeAtivo.Baixa => "Baixa",
        CriticidadeAtivo.Media => "Media",
        CriticidadeAtivo.Alta => "Alta",
        CriticidadeAtivo.Critica => "Critica",
        _ => value.ToString()
    };

    private static string DescricaoTipoMovimentacao(TipoMovimentacaoAtivo value) => value switch
    {
        TipoMovimentacaoAtivo.Criacao => "Criacao",
        TipoMovimentacaoAtivo.Edicao => "Edicao",
        TipoMovimentacaoAtivo.TransferenciaDepartamento => "Transferencia de departamento",
        TipoMovimentacaoAtivo.TransferenciaLocal => "Transferencia de local",
        TipoMovimentacaoAtivo.AlteracaoResponsavel => "Alteracao de responsavel",
        TipoMovimentacaoAtivo.AlteracaoStatusOperacional => "Alteracao de status operacional",
        TipoMovimentacaoAtivo.AlteracaoStatusPatrimonial => "Alteracao de status patrimonial",
        TipoMovimentacaoAtivo.Manutencao => "Manutencao",
        TipoMovimentacaoAtivo.Inativacao => "Inativacao",
        TipoMovimentacaoAtivo.Reativacao => "Reativacao",
        TipoMovimentacaoAtivo.VinculoChamado => "Vinculo com chamado",
        TipoMovimentacaoAtivo.RemocaoVinculoChamado => "Remocao de vinculo com chamado",
        _ => value.ToString()
    };
}

internal static class InventarioAtivoAuditoriaHelper
{
    public static string CriarMetadados(InventarioAtivo ativo, string operacao, string? observacao = null)
        => AuditoriaDiffHelper.CriarMetadadosPadrao(
            origem: "api",
            modulo: "Inventario/Ativos",
            entidade: "InventarioAtivo",
            entidadeId: ativo.Id.ToString(),
            codigo: ativo.Codigo,
            nome: ativo.Nome,
            operacao: operacao,
            resultado: "Sucesso",
            observacao: observacao);
}

internal sealed record InventarioAtivoEstado(
    Guid? DepartamentoId,
    Guid? LocalUnidadeId,
    Guid? UsuarioResponsavelId,
    StatusOperacionalAtivo StatusOperacional,
    StatusPatrimonialAtivo StatusPatrimonial);

internal sealed record InventarioAtivoMudancasRelevantes(
    bool DepartamentoAlterado,
    Guid? DepartamentoOrigemId,
    Guid? DepartamentoDestinoId,
    bool LocalAlterado,
    Guid? LocalOrigemId,
    Guid? LocalDestinoId,
    bool UsuarioResponsavelAlterado,
    Guid? UsuarioResponsavelOrigemId,
    Guid? UsuarioResponsavelDestinoId,
    bool StatusOperacionalAlterado,
    StatusOperacionalAtivo? StatusOperacionalAnterior,
    StatusOperacionalAtivo? StatusOperacionalNovo,
    bool StatusPatrimonialAlterado,
    StatusPatrimonialAtivo? StatusPatrimonialAnterior,
    StatusPatrimonialAtivo? StatusPatrimonialNovo)
{
    public bool PossuiMudanca =>
        DepartamentoAlterado ||
        LocalAlterado ||
        UsuarioResponsavelAlterado ||
        StatusOperacionalAlterado ||
        StatusPatrimonialAlterado;

    public int QuantidadeMudancas =>
        (DepartamentoAlterado ? 1 : 0) +
        (LocalAlterado ? 1 : 0) +
        (UsuarioResponsavelAlterado ? 1 : 0) +
        (StatusOperacionalAlterado ? 1 : 0) +
        (StatusPatrimonialAlterado ? 1 : 0);
}

internal static class InventarioAtivoHistoricoHelper
{
    public static InventarioAtivoEstado CapturarEstado(InventarioAtivo ativo)
        => new(
            ativo.DepartamentoId,
            ativo.LocalUnidadeId,
            ativo.UsuarioResponsavelId,
            ativo.StatusOperacional,
            ativo.StatusPatrimonial);

    public static InventarioAtivoMudancasRelevantes DetectarMudancasRelevantes(InventarioAtivoEstado anterior, InventarioAtivoEstado novo)
    {
        var departamentoAlterado = anterior.DepartamentoId != novo.DepartamentoId;
        var localAlterado = anterior.LocalUnidadeId != novo.LocalUnidadeId;
        var usuarioAlterado = anterior.UsuarioResponsavelId != novo.UsuarioResponsavelId;
        var statusOperacionalAlterado = anterior.StatusOperacional != novo.StatusOperacional;
        var statusPatrimonialAlterado = anterior.StatusPatrimonial != novo.StatusPatrimonial;

        return new InventarioAtivoMudancasRelevantes(
            departamentoAlterado,
            departamentoAlterado ? anterior.DepartamentoId : null,
            departamentoAlterado ? novo.DepartamentoId : null,
            localAlterado,
            localAlterado ? anterior.LocalUnidadeId : null,
            localAlterado ? novo.LocalUnidadeId : null,
            usuarioAlterado,
            usuarioAlterado ? anterior.UsuarioResponsavelId : null,
            usuarioAlterado ? novo.UsuarioResponsavelId : null,
            statusOperacionalAlterado,
            statusOperacionalAlterado ? anterior.StatusOperacional : null,
            statusOperacionalAlterado ? novo.StatusOperacional : null,
            statusPatrimonialAlterado,
            statusPatrimonialAlterado ? anterior.StatusPatrimonial : null,
            statusPatrimonialAlterado ? novo.StatusPatrimonial : null);
    }

    public static TipoMovimentacaoAtivo DeterminarTipoMovimentacao(
        InventarioAtivoMudancasRelevantes mudancas,
        bool origemMovimentacao,
        StatusOperacionalAtivo? statusOperacionalNovo)
    {
        if (!mudancas.PossuiMudanca)
        {
            return TipoMovimentacaoAtivo.Edicao;
        }

        if (mudancas.QuantidadeMudancas > 1)
        {
            return TipoMovimentacaoAtivo.Edicao;
        }

        if (mudancas.DepartamentoAlterado)
        {
            return TipoMovimentacaoAtivo.TransferenciaDepartamento;
        }

        if (mudancas.LocalAlterado)
        {
            return TipoMovimentacaoAtivo.TransferenciaLocal;
        }

        if (mudancas.UsuarioResponsavelAlterado)
        {
            return TipoMovimentacaoAtivo.AlteracaoResponsavel;
        }

        if (mudancas.StatusOperacionalAlterado)
        {
            if (origemMovimentacao &&
                statusOperacionalNovo is StatusOperacionalAtivo.EmManutencao or StatusOperacionalAtivo.ComDefeito)
            {
                return TipoMovimentacaoAtivo.Manutencao;
            }

            return TipoMovimentacaoAtivo.AlteracaoStatusOperacional;
        }

        if (mudancas.StatusPatrimonialAlterado)
        {
            return TipoMovimentacaoAtivo.AlteracaoStatusPatrimonial;
        }

        return TipoMovimentacaoAtivo.Edicao;
    }
}
