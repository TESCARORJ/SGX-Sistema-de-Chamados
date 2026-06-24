using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Chamados;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminChamadosController(
    IObterAdminContextoUseCase obterAdminContextoUseCase,
    IListarChamadosAdminUseCase listarChamadosAdminUseCase,
    IDetalharChamadoAdminUseCase detalharChamadoAdminUseCase,
    IAssumirChamadoUseCase assumirChamadoUseCase,
    IAtribuirChamadoUseCase atribuirChamadoUseCase,
    IDirecionarChamadoGrupoTecnicoAdminUseCase direcionarChamadoGrupoTecnicoUseCase,
    IAssumirChamadoFilaAdminUseCase assumirChamadoFilaUseCase,
    ITransferirGrupoTecnicoChamadoUseCase transferirGrupoTecnicoChamadoUseCase,
    IAlterarStatusChamadoUseCase alterarStatusChamadoUseCase,
    IAlterarPrioridadeChamadoUseCase alterarPrioridadeChamadoUseCase,
    IAlterarCategoriaChamadoUseCase alterarCategoriaChamadoUseCase,
    IComentarChamadoAdminUseCase comentarChamadoAdminUseCase,
    IResolverChamadoUseCase resolverChamadoUseCase,
    IEncerrarChamadoUseCase encerrarChamadoUseCase,
    IReabrirChamadoUseCase reabrirChamadoUseCase,
    IFecharChamadosAutomaticamentePorPrazoAceiteUseCase fecharChamadosAutomaticamentePorPrazoAceiteUseCase,
    IVincularInventarioAtivoChamadoUseCase vincularInventarioAtivoChamadoUseCase,
    IRemoverInventarioAtivoChamadoUseCase removerInventarioAtivoChamadoUseCase,
    IListarArtigosConhecimentoDoChamadoUseCase listarArtigosConhecimentoDoChamadoUseCase,
    IVincularArtigoConhecimentoAoChamadoUseCase vincularArtigoConhecimentoAoChamadoUseCase,
    IRemoverArtigoConhecimentoDoChamadoUseCase removerArtigoConhecimentoDoChamadoUseCase,
    IBuscarArtigosConhecimentoParaVinculoUseCase buscarArtigosConhecimentoParaVinculoUseCase,
    IAdminRelacionamentosChamadoUseCases relacionamentosChamadoUseCases,
    IAdminChamadoTarefasUseCases chamadoTarefasUseCases,
    IAdminChamadoAprovacoesUseCases chamadoAprovacoesUseCases,
    IValidator<FiltroChamadosAdminRequest> filtroValidator,
    IValidator<AtribuirChamadoRequest> atribuirValidator,
    IValidator<AlterarStatusChamadoRequest> alterarStatusValidator,
    IValidator<AlterarPrioridadeChamadoRequest> alterarPrioridadeValidator,
    IValidator<AlterarCategoriaChamadoRequest> alterarCategoriaValidator,
    IValidator<ComentarioAdminChamadoRequest> comentarioValidator,
    IValidator<ResolverChamadoRequest> resolverValidator,
    IValidator<EncerrarChamadoRequest> encerrarValidator,
    IValidator<ReabrirChamadoRequest> reabrirValidator) : ControllerBase
{
    [HttpGet("contexto")]
    public async Task<IActionResult> ObterContexto(CancellationToken cancellationToken)
    {
        try
        {
            var contexto = await obterAdminContextoUseCase.ExecutarAsync(cancellationToken);
            return Ok(contexto);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("chamados")]
    public async Task<IActionResult> ListarChamados([FromQuery] FiltroChamadosAdminRequest request, CancellationToken cancellationToken)
    {
        var validation = await filtroValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await listarChamadosAdminUseCase.ExecutarAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("chamados/{id:guid}")]
    public async Task<IActionResult> ObterChamado(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await detalharChamadoAdminUseCase.ExecutarAsync(id, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/assumir")]
    [Authorize(Policy = PermissionPolicies.ChamadosAssumir)]
    public async Task<IActionResult> AssumirChamado(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await assumirChamadoUseCase.ExecutarAsync(id, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/atribuir")]
    [Authorize(Policy = PermissionPolicies.ChamadosAtribuir)]
    public async Task<IActionResult> AtribuirChamado(Guid id, [FromBody] AtribuirChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await atribuirValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await atribuirChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/direcionar-grupo-tecnico")]
    public async Task<IActionResult> DirecionarGrupoTecnico(Guid id, [FromBody] DirecionarChamadoGrupoTecnicoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await direcionarChamadoGrupoTecnicoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/assumir-fila")]
    public async Task<IActionResult> AssumirChamadoFila(Guid id, [FromBody] AssumirChamadoFilaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await assumirChamadoFilaUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/transferir-grupo-tecnico")]
    public async Task<IActionResult> TransferirGrupoTecnico(Guid id, [FromBody] TransferirGrupoTecnicoChamadoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await transferirGrupoTecnicoChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/alterar-status")]
    public async Task<IActionResult> AlterarStatus(Guid id, [FromBody] AlterarStatusChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await alterarStatusValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await alterarStatusChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/alterar-prioridade")]
    public async Task<IActionResult> AlterarPrioridade(Guid id, [FromBody] AlterarPrioridadeChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await alterarPrioridadeValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await alterarPrioridadeChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/alterar-categoria")]
    public async Task<IActionResult> AlterarCategoria(Guid id, [FromBody] AlterarCategoriaChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await alterarCategoriaValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await alterarCategoriaChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/comentarios")]
    public async Task<IActionResult> Comentar(Guid id, [FromBody] ComentarioAdminChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await comentarioValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await comentarChamadoAdminUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/resolver")]
    public async Task<IActionResult> Resolver(Guid id, [FromBody] ResolverChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await resolverValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await resolverChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/encerrar")]
    [Authorize(Policy = PermissionPolicies.ChamadosEncerrar)]
    public async Task<IActionResult> Encerrar(Guid id, [FromBody] EncerrarChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await encerrarValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await encerrarChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/reabrir")]
    public async Task<IActionResult> Reabrir(Guid id, [FromBody] ReabrirChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await reabrirValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await reabrirChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/fechamento-automatico/prazo-aceite/executar")]
    [Authorize(Policy = Policies.Administrador)]
    public async Task<IActionResult> ExecutarFechamentoAutomaticoPrazoAceite(
        [FromBody] FecharChamadosAutomaticamentePorPrazoAceiteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await fecharChamadosAutomaticamentePorPrazoAceiteUseCase.ExecutarAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{chamadoId:guid}/ativo/{ativoId:guid}")]
    [Authorize(Policy = PermissionPolicies.InventarioAtivosVincularChamado)]
    public async Task<IActionResult> VincularAtivoChamado(Guid chamadoId, Guid ativoId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await vincularInventarioAtivoChamadoUseCase.ExecutarAsync(chamadoId, ativoId, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("chamados/{chamadoId:guid}/ativo")]
    [Authorize(Policy = PermissionPolicies.InventarioAtivosVincularChamado)]
    public async Task<IActionResult> RemoverVinculoAtivoChamado(Guid chamadoId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await removerInventarioAtivoChamadoUseCase.ExecutarAsync(chamadoId, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("chamados/{chamadoId:guid}/artigos-conhecimento")]
    [Authorize(Policy = PermissionPolicies.BaseConhecimentoVincularChamado)]
    public async Task<IActionResult> ListarArtigosConhecimento(Guid chamadoId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await listarArtigosConhecimentoDoChamadoUseCase.ExecutarAsync(chamadoId, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpGet("chamados/{chamadoId:guid}/artigos-conhecimento/disponiveis")]
    [Authorize(Policy = PermissionPolicies.BaseConhecimentoVincularChamado)]
    public async Task<IActionResult> BuscarArtigosConhecimentoDisponiveis(
        Guid chamadoId,
        [FromQuery] string? termo,
        [FromQuery] Guid? categoriaId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await buscarArtigosConhecimentoParaVinculoUseCase.ExecutarAsync(chamadoId, new BuscarArtigosParaVinculoChamadoRequest
            {
                Termo = termo,
                CategoriaId = categoriaId,
                Pagina = page,
                TamanhoPagina = pageSize
            }, cancellationToken);

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{chamadoId:guid}/artigos-conhecimento/{artigoId:guid}")]
    [Authorize(Policy = PermissionPolicies.BaseConhecimentoVincularChamado)]
    public async Task<IActionResult> VincularArtigoConhecimento(
        Guid chamadoId,
        Guid artigoId,
        [FromBody] VincularArtigoChamadoRequest? request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await vincularArtigoConhecimentoAoChamadoUseCase.ExecutarAsync(chamadoId, artigoId, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("chamados/{chamadoId:guid}/artigos-conhecimento/{artigoId:guid}")]
    [Authorize(Policy = PermissionPolicies.BaseConhecimentoVincularChamado)]
    public async Task<IActionResult> RemoverArtigoConhecimento(Guid chamadoId, Guid artigoId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await removerArtigoConhecimentoDoChamadoUseCase.ExecutarAsync(chamadoId, artigoId, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpGet("chamados/{chamadoId:guid}/relacionamentos")]
    public async Task<IActionResult> ListarRelacionamentos(
        Guid chamadoId,
        [FromQuery] bool incluirInativos = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await relacionamentosChamadoUseCases.ListarPorChamadoAsync(chamadoId, incluirInativos, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{chamadoId:guid}/relacionamentos")]
    public async Task<IActionResult> CriarRelacionamento(
        Guid chamadoId,
        [FromBody] CriarChamadoRelacionamentoAdminRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await relacionamentosChamadoUseCases.CriarAsync(new CriarChamadoRelacionamentoRequest
            {
                ChamadoOrigemId = chamadoId,
                ChamadoDestinoId = request.ChamadoDestinoId,
                TipoRelacionamento = request.TipoRelacionamento,
                Justificativa = request.Justificativa
            }, cancellationToken);

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("chamados/{chamadoId:guid}/relacionamentos/{relacionamentoId:guid}")]
    public async Task<IActionResult> RemoverRelacionamento(
        Guid chamadoId,
        Guid relacionamentoId,
        [FromBody] RemoverChamadoRelacionamentoAdminRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await relacionamentosChamadoUseCases.RemoverAsync(new RemoverChamadoRelacionamentoRequest
            {
                ChamadoId = chamadoId,
                RelacionamentoId = relacionamentoId,
                Motivo = request?.MotivoRemocao
            }, cancellationToken);

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("chamados/{chamadoId:guid}/tarefas")]
    public async Task<IActionResult> ListarTarefas(
        Guid chamadoId,
        [FromQuery] bool incluirInativas = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await chamadoTarefasUseCases.ListarPorChamadoAsync(chamadoId, incluirInativas, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{chamadoId:guid}/tarefas")]
    public async Task<IActionResult> CriarTarefa(
        Guid chamadoId,
        [FromBody] CriarChamadoTarefaAdminRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await chamadoTarefasUseCases.CriarAsync(chamadoId, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPatch("chamados/{chamadoId:guid}/tarefas/{tarefaId:guid}/status")]
    public async Task<IActionResult> AtualizarStatusTarefa(
        Guid chamadoId,
        Guid tarefaId,
        [FromBody] AtualizarStatusChamadoTarefaAdminRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await chamadoTarefasUseCases.AtualizarStatusAsync(chamadoId, tarefaId, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("chamados/{chamadoId:guid}/tarefas/{tarefaId:guid}")]
    public async Task<IActionResult> CancelarTarefa(
        Guid chamadoId,
        Guid tarefaId,
        [FromBody] CancelarChamadoTarefaAdminRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await chamadoTarefasUseCases.CancelarAsync(chamadoId, tarefaId, request, cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("chamados/{chamadoId:guid}/aprovacoes")]
    public async Task<IActionResult> ListarAprovacoesVinculadas(
        Guid chamadoId,
        [FromQuery] bool incluirInativas = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await chamadoAprovacoesUseCases.ListarPorChamadoAsync(chamadoId, incluirInativas, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{chamadoId:guid}/aprovacoes")]
    public async Task<IActionResult> CriarAprovacaoVinculada(
        Guid chamadoId,
        [FromBody] CriarChamadoAprovacaoAdminRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await chamadoAprovacoesUseCases.CriarAsync(chamadoId, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{chamadoId:guid}/aprovacoes/{aprovacaoId:guid}/aprovar")]
    public async Task<IActionResult> AprovarAprovacaoVinculada(
        Guid chamadoId,
        Guid aprovacaoId,
        [FromBody] DecidirChamadoAprovacaoAdminRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await chamadoAprovacoesUseCases.AprovarAsync(chamadoId, aprovacaoId, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{chamadoId:guid}/aprovacoes/{aprovacaoId:guid}/reprovar")]
    public async Task<IActionResult> ReprovarAprovacaoVinculada(
        Guid chamadoId,
        Guid aprovacaoId,
        [FromBody] DecidirChamadoAprovacaoAdminRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await chamadoAprovacoesUseCases.ReprovarAsync(chamadoId, aprovacaoId, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("chamados/{chamadoId:guid}/aprovacoes/{aprovacaoId:guid}")]
    public async Task<IActionResult> CancelarAprovacaoVinculada(
        Guid chamadoId,
        Guid aprovacaoId,
        [FromBody] CancelarChamadoAprovacaoAdminRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await chamadoAprovacoesUseCases.CancelarAsync(chamadoId, aprovacaoId, request, cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
