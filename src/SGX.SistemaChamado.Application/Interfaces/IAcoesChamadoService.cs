using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Interfaces;

public interface IAcoesChamadoService
{
    IReadOnlyCollection<AcaoChamadoEnum> ObterAcoesDisponiveis(Chamado chamado, UsuarioContextoAplicacao usuario);
    bool AcaoEstaDisponivel(Chamado chamado, AcaoChamadoEnum acao, UsuarioContextoAplicacao usuario);
    void ValidarAcaoDisponivel(Chamado chamado, AcaoChamadoEnum acao, UsuarioContextoAplicacao usuario);
}
