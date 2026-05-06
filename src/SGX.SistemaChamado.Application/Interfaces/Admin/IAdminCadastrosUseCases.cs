using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IListarUsuariosAdminUseCase
{
    Task<PagedResultResponse<UsuarioAdminResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default);
}

public interface IObterUsuarioAdminUseCase
{
    Task<UsuarioAdminDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarUsuarioAdminUseCase
{
    Task<UsuarioAdminDetalheResponse> ExecutarAsync(CriarUsuarioAdminRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarUsuarioAdminUseCase
{
    Task<UsuarioAdminDetalheResponse> ExecutarAsync(Guid id, AtualizarUsuarioAdminRequest request, CancellationToken cancellationToken = default);
}

public interface IInativarUsuarioAdminUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReativarUsuarioAdminUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IAlterarPerfisUsuarioUseCase
{
    Task<UsuarioAdminDetalheResponse> ExecutarAsync(Guid id, AlterarPerfisUsuarioRequest request, CancellationToken cancellationToken = default);
}

public interface IListarPerfisAcessoUseCase
{
    Task<PagedResultResponse<PerfilAcessoResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default);
}

public interface IObterPerfilAcessoUseCase
{
    Task<PerfilAcessoDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarPerfilAcessoUseCase
{
    Task<PerfilAcessoDetalheResponse> ExecutarAsync(CriarPerfilAcessoRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarPerfilAcessoUseCase
{
    Task<PerfilAcessoDetalheResponse> ExecutarAsync(Guid id, AtualizarPerfilAcessoRequest request, CancellationToken cancellationToken = default);
}

public interface IInativarPerfilAcessoUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReativarPerfilAcessoUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IListarDepartamentosAdminUseCase
{
    Task<PagedResultResponse<DepartamentoResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default);
}

public interface IObterDepartamentoAdminUseCase
{
    Task<DepartamentoDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarDepartamentoUseCase
{
    Task<DepartamentoDetalheResponse> ExecutarAsync(CriarDepartamentoRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarDepartamentoUseCase
{
    Task<DepartamentoDetalheResponse> ExecutarAsync(Guid id, AtualizarDepartamentoRequest request, CancellationToken cancellationToken = default);
}

public interface IInativarDepartamentoUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReativarDepartamentoUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IListarCategoriasAdminUseCase
{
    Task<PagedResultResponse<CategoriaChamadoResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default);
}

public interface IObterCategoriaAdminUseCase
{
    Task<CategoriaChamadoDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarCategoriaUseCase
{
    Task<CategoriaChamadoDetalheResponse> ExecutarAsync(CriarCategoriaChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarCategoriaUseCase
{
    Task<CategoriaChamadoDetalheResponse> ExecutarAsync(Guid id, AtualizarCategoriaChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IInativarCategoriaUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReativarCategoriaUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IListarPrioridadesAdminUseCase
{
    Task<PagedResultResponse<PrioridadeChamadoResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default);
}

public interface IObterPrioridadeAdminUseCase
{
    Task<PrioridadeChamadoDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarPrioridadeUseCase
{
    Task<PrioridadeChamadoDetalheResponse> ExecutarAsync(CriarPrioridadeChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarPrioridadeUseCase
{
    Task<PrioridadeChamadoDetalheResponse> ExecutarAsync(Guid id, AtualizarPrioridadeChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IInativarPrioridadeUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReativarPrioridadeUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IListarStatusAdminUseCase
{
    Task<PagedResultResponse<StatusChamadoResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default);
}

public interface IObterStatusAdminUseCase
{
    Task<StatusChamadoDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarStatusUseCase
{
    Task<StatusChamadoDetalheResponse> ExecutarAsync(CriarStatusChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarStatusUseCase
{
    Task<StatusChamadoDetalheResponse> ExecutarAsync(Guid id, AtualizarStatusChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IInativarStatusUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReativarStatusUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IListarParametrosSistemaUseCase
{
    Task<PagedResultResponse<ParametroSistemaResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default);
}

public interface IObterParametroSistemaUseCase
{
    Task<ParametroSistemaDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarParametroSistemaUseCase
{
    Task<ParametroSistemaDetalheResponse> ExecutarAsync(CriarParametroSistemaRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarParametroSistemaUseCase
{
    Task<ParametroSistemaDetalheResponse> ExecutarAsync(Guid id, AtualizarParametroSistemaRequest request, CancellationToken cancellationToken = default);
}

public interface IInativarParametroSistemaUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReativarParametroSistemaUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}
