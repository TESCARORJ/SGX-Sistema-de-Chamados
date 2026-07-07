using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IAdminFormularioServicosUseCases
{
    Task<IReadOnlyCollection<FormularioServicoAdminDto>> ListarAsync(Guid? catalogoServicoId = null, CancellationToken cancellationToken = default);
    Task<FormularioServicoDetalheAdminDto> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FormularioServicoDetalheAdminDto> CriarAsync(CriarFormularioServicoRequest request, CancellationToken cancellationToken = default);
    Task<FormularioServicoDetalheAdminDto> AtualizarAsync(Guid id, AtualizarFormularioServicoRequest request, CancellationToken cancellationToken = default);
    Task<AlterarSituacaoCadastroResponse> InativarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AlterarSituacaoCadastroResponse> ReativarAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<FormularioServicoVersaoAdminDto>> ListarVersoesAsync(Guid formularioServicoId, CancellationToken cancellationToken = default);
    Task<FormularioServicoVersaoAdminDto> CriarVersaoAsync(CriarFormularioServicoVersaoRequest request, CancellationToken cancellationToken = default);
    Task<FormularioServicoVersaoAdminDto> AtualizarVersaoAsync(Guid id, AtualizarFormularioServicoVersaoRequest request, CancellationToken cancellationToken = default);
    Task<AlterarSituacaoCadastroResponse> InativarVersaoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AlterarSituacaoCadastroResponse> ReativarVersaoAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CampoFormularioServicoAdminDto>> ListarCamposAsync(Guid formularioServicoVersaoId, CancellationToken cancellationToken = default);
    Task<CampoFormularioServicoAdminDto> CriarCampoAsync(CriarCampoFormularioServicoRequest request, CancellationToken cancellationToken = default);
    Task<CampoFormularioServicoAdminDto> AtualizarCampoAsync(Guid id, AtualizarCampoFormularioServicoRequest request, CancellationToken cancellationToken = default);
    Task<AlterarSituacaoCadastroResponse> InativarCampoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AlterarSituacaoCadastroResponse> ReativarCampoAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<OpcaoCampoFormularioServicoAdminDto>> ListarOpcoesAsync(Guid campoFormularioServicoId, CancellationToken cancellationToken = default);
    Task<OpcaoCampoFormularioServicoAdminDto> CriarOpcaoAsync(CriarOpcaoCampoFormularioServicoRequest request, CancellationToken cancellationToken = default);
    Task<OpcaoCampoFormularioServicoAdminDto> AtualizarOpcaoAsync(Guid id, AtualizarOpcaoCampoFormularioServicoRequest request, CancellationToken cancellationToken = default);
    Task<AlterarSituacaoCadastroResponse> InativarOpcaoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AlterarSituacaoCadastroResponse> ReativarOpcaoAsync(Guid id, CancellationToken cancellationToken = default);
}
