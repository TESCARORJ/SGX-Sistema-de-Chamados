using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Interfaces.Sla;

public interface ISlaService
{
    Task InicializarNaAberturaAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default);
    Task InicializarNaAberturaAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, Guid? politicaSlaIdPreferencial, CancellationToken cancellationToken = default);
    Task RegistrarPrimeiraRespostaAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default);
    Task AplicarMudancaPrioridadeAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default);
    Task AplicarMudancaCategoriaAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default);
    Task AplicarMudancaStatusAsync(Chamado chamado, StatusChamado statusAnterior, StatusChamado statusAtual, string usuarioLogin, DateTime agoraUtc);
    Task RegistrarEncerramentoAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc);
    Task ReabrirAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default);
    bool EstaProximoDoVencimento(ChamadoSla? chamadoSla, DateTime agoraUtc);
}
