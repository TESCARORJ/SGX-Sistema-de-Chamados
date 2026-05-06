using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Validators;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/info")]
public sealed class InfoController : ControllerBase
{
    private readonly IApiInfoUseCase _apiInfoUseCase;
    private readonly IValidator<ApiInfoRequest> _validator;

    public InfoController(IApiInfoUseCase apiInfoUseCase, IValidator<ApiInfoRequest> validator)
    {
        _apiInfoUseCase = apiInfoUseCase;
        _validator = validator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var request = new ApiInfoRequest
        {
            Ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        };

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        var response = _apiInfoUseCase.Executar(request.Ambiente);
        return Ok(response);
    }
}
