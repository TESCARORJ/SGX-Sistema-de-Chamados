using System;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed record AtualizarConfiguracaoAutoFechamentoChamadoRequest(
    int PrazoAutoFechamentoHoras,
    string? ObservacaoAlteracao);

public sealed record ObterConfiguracaoAutoFechamentoChamadoResponse(
    int PrazoAutoFechamentoHoras,
    decimal PrazoAutoFechamentoDias,
    bool Ativo,
    DateTime CriadoEm,
    string CriadoPor,
    DateTime? AtualizadoEm,
    string? AtualizadoPor);

public sealed record AtualizarConfiguracaoAutoFechamentoChamadoResponse(
    int PrazoAutoFechamentoHoras,
    decimal PrazoAutoFechamentoDias,
    bool Ativo,
    DateTime CriadoEm,
    string CriadoPor,
    DateTime? AtualizadoEm,
    string? AtualizadoPor,
    string? ObservacaoAlteracao);
