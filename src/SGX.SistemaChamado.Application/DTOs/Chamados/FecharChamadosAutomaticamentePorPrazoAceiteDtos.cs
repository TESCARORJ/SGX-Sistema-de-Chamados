using System;
using System.Collections.Generic;

namespace SGX.SistemaChamado.Application.DTOs.Chamados;

public sealed class FecharChamadosAutomaticamentePorPrazoAceiteRequest
{
    public DateTime DataReferencia { get; set; } = DateTime.UtcNow;
    public int? PrazoAceiteHoras { get; set; }
    public Guid? UsuarioSistemaId { get; set; }
    public int? LimiteProcessamento { get; set; }
}

public sealed class FecharChamadosAutomaticamentePorPrazoAceiteResponse
{
    public int TotalAnalisados { get; set; }
    public int TotalFechados { get; set; }
    public int TotalIgnorados { get; set; }
    public int TotalBloqueadosPorAprovacao { get; set; }
    public List<FechamentoAutomaticoChamadoResultadoResponse> ChamadosFechados { get; set; } = [];
    public List<FechamentoAutomaticoChamadoResultadoResponse> ChamadosIgnorados { get; set; } = [];
}

public sealed class FechamentoAutomaticoChamadoResultadoResponse
{
    public Guid ChamadoId { get; set; }
    public string CodigoChamado { get; set; } = string.Empty;
    public string StatusAnterior { get; set; } = string.Empty;
    public string StatusNovo { get; set; } = string.Empty;
    public DateTime? ResolvidoEm { get; set; }
    public DateTime? EncerradoEm { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public bool BloqueadoPorAprovacao { get; set; }
}
