using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Enums;
using Xunit;

namespace SGX.SistemaChamado.Tests;

public class AbrirRequisicaoServicoCatalogoUseCaseTests
{
    private class FakeAbrirChamadoUseCase : IAbrirChamadoUseCase
    {
        public CriarChamadoRequest? RequestCapturado { get; private set; }
        public CancellationToken TokenCapturado { get; private set; }
        public Exception? ExcecaoParaLancar { get; set; }

        public Task<ChamadoDetalheResponse> ExecutarAsync(CriarChamadoRequest request, CancellationToken cancellationToken = default)
        {
            if (ExcecaoParaLancar != null)
                throw ExcecaoParaLancar;

            RequestCapturado = request;
            TokenCapturado = cancellationToken;
            return Task.FromResult(new ChamadoDetalheResponse { Id = Guid.NewGuid() });
        }
    }

    [Fact]
    public async Task ExecutarAsync_DeveMapearRequestEForcarNaturezaRequisicao()
    {
        var fakeAbrirChamadoUseCase = new FakeAbrirChamadoUseCase();
        var sut = new AbrirRequisicaoServicoCatalogoUseCase(fakeAbrirChamadoUseCase);

        var request = new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Requisicao de Software",
            Descricao = "Preciso de um software"
        };

        var result = await sut.ExecutarAsync(request);

        Assert.NotNull(result);
        var reqCap = fakeAbrirChamadoUseCase.RequestCapturado;
        Assert.NotNull(reqCap);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, reqCap!.NaturezaChamado);
        Assert.Equal(request.CatalogoServicoId, reqCap.CatalogoServicoId);
        Assert.Equal(request.Titulo, reqCap.Titulo);
        Assert.Equal(request.Descricao, reqCap.Descricao);
    }

    [Fact]
    public async Task ExecutarAsync_DescricaoNula_DeveMapearParaStringVazia()
    {
        var fakeAbrirChamadoUseCase = new FakeAbrirChamadoUseCase();
        var sut = new AbrirRequisicaoServicoCatalogoUseCase(fakeAbrirChamadoUseCase);

        var request = new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Requisicao sem descricao",
            Descricao = null
        };

        await sut.ExecutarAsync(request);

        var reqCap = fakeAbrirChamadoUseCase.RequestCapturado;
        Assert.NotNull(reqCap);
        Assert.Equal(string.Empty, reqCap!.Descricao);
    }

    [Fact]
    public async Task ExecutarAsync_DevePropagarCancellationToken()
    {
        var fakeAbrirChamadoUseCase = new FakeAbrirChamadoUseCase();
        var sut = new AbrirRequisicaoServicoCatalogoUseCase(fakeAbrirChamadoUseCase);

        var request = new AbrirRequisicaoServicoCatalogoRequest();
        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        await sut.ExecutarAsync(request, token);

        Assert.Equal(token, fakeAbrirChamadoUseCase.TokenCapturado);
    }

    [Fact]
    public async Task ExecutarAsync_DevePreservarExcecoesDoFluxoInterno()
    {
        var fakeAbrirChamadoUseCase = new FakeAbrirChamadoUseCase();
        fakeAbrirChamadoUseCase.ExcecaoParaLancar = new InvalidOperationException("Erro de negocio");
        var sut = new AbrirRequisicaoServicoCatalogoUseCase(fakeAbrirChamadoUseCase);

        var request = new AbrirRequisicaoServicoCatalogoRequest();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ExecutarAsync(request));
        Assert.Equal("Erro de negocio", ex.Message);
    }
}
