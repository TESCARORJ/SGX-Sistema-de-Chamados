using Microsoft.AspNetCore.Mvc;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/saude")]
public sealed class SaudeController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            sistema = "SGX.SistemaChamado",
            dataHoraUtc = DateTime.UtcNow
        });
    }
}
