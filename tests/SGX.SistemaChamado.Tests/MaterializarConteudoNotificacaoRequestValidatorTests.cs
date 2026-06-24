using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class MaterializarConteudoNotificacaoRequestValidatorTests
{
    private readonly MaterializarConteudoNotificacaoRequestValidator _validator = new();

    [Fact]
    public void DeveAceitarRequestValido()
    {
        var result = _validator.Validate(CriarRequest());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeveRejeitarVariaveisNulas()
    {
        var request = new MaterializarConteudoNotificacaoRequest(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            new DateTime(2026, 6, 21, 21, 0, 0, DateTimeKind.Utc),
            null!,
            null);
        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void DeveRejeitarTipoEventoOuCanalInvalidos()
    {
        Assert.False(_validator.Validate(CriarRequest(tipoEvento: (TipoEventoNotificacao)999)).IsValid);
        Assert.False(_validator.Validate(CriarRequest(canal: (CanalNotificacao)999)).IsValid);
    }

    [Fact]
    public void DeveRejeitarDataReferenciaPadraoOuTemplateVazio()
    {
        var requestDataPadrao = new MaterializarConteudoNotificacaoRequest(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            DateTime.MinValue,
            new Dictionary<string, string> { ["chamado.codigo"] = "CH-001" },
            null);

        Assert.False(_validator.Validate(requestDataPadrao).IsValid);
        Assert.False(_validator.Validate(CriarRequest(templateNotificacaoId: Guid.Empty)).IsValid);
    }

    [Fact]
    public void DeveRejeitarChaveVaziaOuValorMuitoGrande()
    {
        var variaveisComChaveVazia = new Dictionary<string, string> { [" "] = "valor" };
        var variaveisComValorGrande = new Dictionary<string, string> { ["chamado.codigo"] = new string('x', 5001) };

        Assert.False(_validator.Validate(CriarRequest(variaveis: variaveisComChaveVazia)).IsValid);
        Assert.False(_validator.Validate(CriarRequest(variaveis: variaveisComValorGrande)).IsValid);
    }

    [Fact]
    public void DeveRejeitarQuantidadeExcessivaDeVariaveis()
    {
        var variaveis = Enumerable.Range(1, 101)
            .ToDictionary(x => $"variavel.{x}", x => "valor", StringComparer.Ordinal);

        var result = _validator.Validate(CriarRequest(variaveis: variaveis));

        Assert.False(result.IsValid);
    }

    private static MaterializarConteudoNotificacaoRequest CriarRequest(
        TipoEventoNotificacao tipoEvento = TipoEventoNotificacao.EventoChamado,
        CanalNotificacao canal = CanalNotificacao.Email,
        DateTime? dataReferencia = null,
        IReadOnlyDictionary<string, string>? variaveis = null,
        Guid? templateNotificacaoId = null)
    {
        return new MaterializarConteudoNotificacaoRequest(
            tipoEvento,
            canal,
            dataReferencia ?? new DateTime(2026, 6, 21, 21, 0, 0, DateTimeKind.Utc),
            variaveis ?? new Dictionary<string, string>
            {
                ["chamado.codigo"] = "CH-001"
            },
            templateNotificacaoId);
    }
}
