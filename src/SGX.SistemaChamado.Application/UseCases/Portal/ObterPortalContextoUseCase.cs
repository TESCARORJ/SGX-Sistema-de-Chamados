using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Application.Options;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class ObterPortalContextoUseCase(
    IRepository<Departamento> departamentoRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<PrioridadeChamado> prioridadeRepository,
    IRepository<StatusChamado> statusRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IOptions<ArquivosOptions> arquivosOptions) : IObterPortalContextoUseCase
{
    public async Task<PortalContextoResponse> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);

        var departamentos = await departamentoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.Nome)
            .Select(x => new DepartamentoPortalResponse(x.Id, x.Nome, x.Sigla))
            .ToListAsync(cancellationToken);

        var categorias = await categoriaRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.Nome)
            .Select(x => new CategoriaPortalResponse(x.Id, x.Nome, x.DepartamentoId))
            .ToListAsync(cancellationToken);

        var prioridades = await prioridadeRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.Nivel)
            .Select(x => new PrioridadePortalResponse(x.Id, x.Nome, (int)x.Nivel))
            .ToListAsync(cancellationToken);

        var status = await statusRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.Codigo)
            .Select(x => new StatusPortalResponse(x.Id, x.Nome, (int)x.Codigo))
            .ToListAsync(cancellationToken);

        var configuracaoAnexos = CriarConfiguracaoAnexos(arquivosOptions.Value);

        return new PortalContextoResponse
        {
            Usuario = new UsuarioPortalResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.Login, usuario.Perfis),
            Departamentos = departamentos,
            Categorias = categorias,
            Prioridades = prioridades,
            Status = status,
            ConfiguracaoAnexos = configuracaoAnexos
        };
    }

    private static ConfiguracaoAnexoPortalResponse? CriarConfiguracaoAnexos(ArquivosOptions options)
    {
        var tiposPermitidos = options.ContentTypesPermitidos
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        long? tamanhoMaximo = options.TamanhoMaximoBytes > 0 ? options.TamanhoMaximoBytes : null;

        if (tiposPermitidos.Length == 0 && tamanhoMaximo is null)
        {
            return null;
        }

        return new ConfiguracaoAnexoPortalResponse(tiposPermitidos, tamanhoMaximo);
    }
}
