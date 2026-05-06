namespace SGX.SistemaChamado.Api.Authorization;

public static class Policies
{
    public const string Administrador = "Administrador";
    public const string Atendente = "Atendente";
    public const string Solicitante = "Solicitante";
    public const string AdminOuAtendente = "AdminOuAtendente";
}
