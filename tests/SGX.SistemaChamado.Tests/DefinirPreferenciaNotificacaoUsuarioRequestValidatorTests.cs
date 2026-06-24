using FluentValidation.TestHelper;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class DefinirPreferenciaNotificacaoUsuarioRequestValidatorTests
{
    private readonly DefinirPreferenciaNotificacaoUsuarioRequestValidator _validator = new();

    [Fact]
    public void DeveValidarRequestCorreto()
    {
        var result = _validator.TestValidate(CriarRequest());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DeveExigirUsuario()
    {
        var result = _validator.TestValidate(CriarRequest(usuarioId: Guid.Empty));
        result.ShouldHaveValidationErrorFor(x => x.UsuarioId);
    }

    [Fact]
    public void DeveExigirEventoValido()
    {
        var result = _validator.TestValidate(CriarRequest(tipoEvento: (TipoEventoNotificacao)0));
        result.ShouldHaveValidationErrorFor(x => x.TipoEvento);
    }

    [Fact]
    public void DeveExigirCanalValido()
    {
        var result = _validator.TestValidate(CriarRequest(canal: (CanalNotificacao)0));
        result.ShouldHaveValidationErrorFor(x => x.Canal);
    }

    private static DefinirPreferenciaNotificacaoUsuarioRequest CriarRequest(
        Guid? usuarioId = null,
        TipoEventoNotificacao tipoEvento = TipoEventoNotificacao.EventoChamado,
        CanalNotificacao canal = CanalNotificacao.Email,
        bool habilitada = true)
        => new(usuarioId ?? Guid.NewGuid(), tipoEvento, canal, habilitada);
}
