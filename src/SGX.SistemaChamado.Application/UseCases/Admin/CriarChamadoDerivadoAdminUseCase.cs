using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class CriarChamadoDerivadoAdminUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<Usuario> usuarioRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IRepository<PrioridadeChamado> prioridadeRepository,
    IRepository<TipoSolicitacao> tipoSolicitacaoRepository,
    IRepository<LocalUnidade> localUnidadeRepository,
    IRepository<Departamento> departamentoRepository,
    IRepository<StatusChamado> statusRepository,
    IRepository<HistoricoChamado> historicoRepository,
    ISlaService slaService,
    ICodigoChamadoService codigoChamadoService,
    IPrioridadeChamadoMatrizService prioridadeChamadoMatrizService,
    ICamposObrigatoriosChamadoService camposObrigatoriosChamadoService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IAdminRelacionamentosChamadoUseCases relacionamentosChamadoUseCases,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : ICriarChamadoDerivadoAdminUseCase
{
    public async Task<ChamadoDerivadoAdminResponse> ExecutarAsync(
        Guid chamadoOrigemId,
        CriarChamadoDerivadoAdminRequest request,
        CancellationToken cancellationToken = default)
    {
        if (chamadoOrigemId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado origem invalido.", nameof(chamadoOrigemId));
        }

        ArgumentNullException.ThrowIfNull(request);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuarioAtual))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var chamadoOrigem = await chamadoRepository.Query()
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.Id == chamadoOrigemId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado origem nao encontrado.");

        var naturezaChamado = request.NaturezaChamado ?? chamadoOrigem.NaturezaChamado;
        var impactoChamado = request.ImpactoChamado ?? chamadoOrigem.ImpactoChamado;
        var urgenciaChamado = request.UrgenciaChamado ?? chamadoOrigem.UrgenciaChamado;
        var categoriaId = request.CategoriaId ?? chamadoOrigem.CategoriaId;
        var departamentoId = request.DepartamentoId ?? chamadoOrigem.DepartamentoId;
        var solicitanteId = request.SolicitanteId ?? chamadoOrigem.SolicitanteId;

        ValidarCamposObrigatorios(request, naturezaChamado, impactoChamado, urgenciaChamado, categoriaId);

        var solicitante = await usuarioRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == solicitanteId && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Solicitante nao encontrado ou inativo.");

        var categoria = await categoriaRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == categoriaId && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Categoria nao encontrada ou inativa.");

        var prioridade = await prioridadeChamadoMatrizService.ObterPrioridadeAsync(
            impactoChamado,
            urgenciaChamado,
            cancellationToken);

        if (prioridade is null)
        {
            if (!request.PrioridadeId.HasValue || request.PrioridadeId.Value == Guid.Empty)
            {
                throw new InvalidOperationException("Prioridade obrigatoria.");
            }

            prioridade = await prioridadeRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.PrioridadeId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Prioridade nao encontrada ou inativa.");
        }

        SubcategoriaChamado? subcategoria = null;
        if (request.SubcategoriaId.HasValue)
        {
            subcategoria = await subcategoriaRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.SubcategoriaId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Subcategoria nao encontrada ou inativa.");

            if (subcategoria.CategoriaChamadoId != categoria.Id)
            {
                throw new InvalidOperationException("A subcategoria selecionada nao pertence a categoria informada.");
            }
        }

        if (request.TipoSolicitacaoId.HasValue)
        {
            _ = await tipoSolicitacaoRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.TipoSolicitacaoId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Tipo de solicitacao nao encontrado ou inativo.");
        }

        if (request.LocalUnidadeId.HasValue)
        {
            _ = await localUnidadeRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.LocalUnidadeId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Local/unidade nao encontrado ou inativo.");
        }

        if (departamentoId.HasValue)
        {
            _ = await departamentoRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == departamentoId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Departamento nao encontrado ou inativo.");
        }

        var statusAberto = await statusRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == StatusChamadoEnum.Aberto, cancellationToken)
            ?? throw new InvalidOperationException("Status inicial 'Aberto' nao encontrado.");

        var codigo = await codigoChamadoService.GerarAsync(cancellationToken);
        var chamadoDerivado = new Chamado(
            codigo,
            request.Titulo,
            request.Descricao,
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            statusAberto.Id,
            OrigemChamado.Admin,
            usuarioAtual.Login,
            departamentoId,
            subcategoria?.Id,
            request.TipoSolicitacaoId,
            request.LocalUnidadeId,
            naturezaChamado: naturezaChamado,
            impactoChamado: impactoChamado,
            urgenciaChamado: urgenciaChamado);

        await chamadoRepository.AddAsync(chamadoDerivado, cancellationToken);

        var justificativa = NormalizarJustificativa(request.JustificativaDerivacao);
        await historicoRepository.AddAsync(new HistoricoChamado(
            chamadoOrigem.Id,
            TipoHistoricoChamado.ChamadoDerivadoCriado,
            CriarDescricaoHistoricoOrigem(chamadoDerivado.Codigo, justificativa),
            usuarioAtual.Id,
            usuarioAtual.Login), cancellationToken);

        await historicoRepository.AddAsync(new HistoricoChamado(
            chamadoDerivado.Id,
            TipoHistoricoChamado.CriadoAPartirDeChamado,
            CriarDescricaoHistoricoDerivado(chamadoOrigem.Codigo, justificativa),
            usuarioAtual.Id,
            usuarioAtual.Login), cancellationToken);

        await slaService.InicializarNaAberturaAsync(chamadoDerivado, usuarioAtual.Login, DateTime.UtcNow, null, cancellationToken);
        var relacionamento = await relacionamentosChamadoUseCases.CriarNaUnidadeDeTrabalhoAsync(
            new CriarChamadoRelacionamentoRequest
            {
                ChamadoOrigemId = chamadoOrigem.Id,
                ChamadoDestinoId = chamadoDerivado.Id,
                TipoRelacionamento = TipoRelacionamentoChamadoEnum.Origina,
                Justificativa = CriarJustificativaRelacionamentoAutomatico(justificativa)
            },
            chamadoOrigem.Codigo,
            chamadoDerivado.Codigo,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await RegistrarAuditoriaAsync(chamadoOrigem, chamadoDerivado, usuarioAtual, justificativa, cancellationToken);
        }

        return new ChamadoDerivadoAdminResponse
        {
            ChamadoOrigemId = chamadoOrigem.Id,
            ChamadoOrigemCodigo = chamadoOrigem.Codigo,
            ChamadoDerivadoId = chamadoDerivado.Id,
            ChamadoDerivadoCodigo = chamadoDerivado.Codigo,
            RelacionamentoId = relacionamento.Id,
            TipoRelacionamento = relacionamento.TipoRelacionamento,
            Titulo = chamadoDerivado.Titulo,
            Status = statusAberto.Nome,
            CriadoEm = chamadoDerivado.CriadoEm
        };
    }

    private void ValidarCamposObrigatorios(
        CriarChamadoDerivadoAdminRequest request,
        NaturezaChamadoEnum naturezaChamado,
        ImpactoChamadoEnum impactoChamado,
        UrgenciaChamadoEnum urgenciaChamado,
        Guid categoriaId)
    {
        var validacoes = camposObrigatoriosChamadoService.ValidarCriacao(new CamposObrigatoriosChamadoInput
        {
            NaturezaChamado = naturezaChamado,
            ImpactoChamado = impactoChamado,
            UrgenciaChamado = urgenciaChamado,
            Titulo = request.Titulo,
            Descricao = request.Descricao,
            CategoriaId = categoriaId,
            TipoSolicitacaoId = request.TipoSolicitacaoId,
            Origem = "Admin"
        });

        var primeiraFalha = validacoes.FirstOrDefault();
        if (primeiraFalha is not null)
        {
            throw new InvalidOperationException(primeiraFalha.Mensagem);
        }

        if (categoriaId == Guid.Empty)
        {
            throw new InvalidOperationException("Categoria obrigatoria.");
        }
    }

    private async Task RegistrarAuditoriaAsync(
        Chamado chamadoOrigem,
        Chamado chamadoDerivado,
        UsuarioContextoAplicacao usuarioAtual,
        string? justificativa,
        CancellationToken cancellationToken)
    {
        var dadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
        {
            chamadoDerivado.Id,
            chamadoDerivado.Codigo,
            chamadoDerivado.Titulo,
            ChamadoOrigemId = chamadoOrigem.Id,
            ChamadoOrigemCodigo = chamadoOrigem.Codigo,
            JustificativaDerivacao = justificativa
        });

        await auditoriaService!.RegistrarCriacaoAsync(
            "Chamados",
            "Chamado",
            chamadoDerivado.Id.ToString(),
            "Chamado derivado criado.",
            dadosDepois: dadosDepois,
            metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                origem: "api",
                modulo: "Chamados",
                entidade: "Chamado",
                entidadeId: chamadoDerivado.Id.ToString(),
                codigo: chamadoDerivado.Codigo,
                nome: chamadoDerivado.Titulo,
                operacao: "CriacaoChamadoDerivado",
                resultado: "Sucesso",
                observacao: $"Derivado do chamado {chamadoOrigem.Codigo} por {usuarioAtual.Login}"),
            cancellationToken: cancellationToken);
    }

    private static string? NormalizarJustificativa(string? justificativa)
        => string.IsNullOrWhiteSpace(justificativa) ? null : justificativa.Trim();

    private static string CriarDescricaoHistoricoOrigem(string codigoChamadoDerivado, string? justificativa)
        => string.IsNullOrWhiteSpace(justificativa)
            ? $"Chamado derivado criado: #{codigoChamadoDerivado}."
            : $"Chamado derivado criado: #{codigoChamadoDerivado}. Justificativa: {justificativa}";

    private static string CriarDescricaoHistoricoDerivado(string codigoChamadoOrigem, string? justificativa)
        => string.IsNullOrWhiteSpace(justificativa)
            ? $"Chamado criado como derivado do chamado #{codigoChamadoOrigem}."
            : $"Chamado criado como derivado do chamado #{codigoChamadoOrigem}. Justificativa: {justificativa}";

    private static string CriarJustificativaRelacionamentoAutomatico(string? justificativaDerivacao)
        => string.IsNullOrWhiteSpace(justificativaDerivacao)
            ? "Vinculo automatico criado a partir do fluxo de chamado derivado."
            : $"Vinculo automatico criado a partir do fluxo de chamado derivado. Justificativa da derivacao: {justificativaDerivacao}";
}
