using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Interfaces;

public interface ICamposObrigatoriosChamadoService
{
    IReadOnlyCollection<ErroCampoObrigatorioChamado> ValidarCriacao(CamposObrigatoriosChamadoInput input);
}
