namespace SGX.SistemaChamado.Application.Options;

public sealed class SlaMonitoringOptions
{
    public const string SectionName = "SlaMonitoring";

    public bool Enabled { get; init; } = true;
    public int IntervalMinutes { get; init; } = 5;
}
