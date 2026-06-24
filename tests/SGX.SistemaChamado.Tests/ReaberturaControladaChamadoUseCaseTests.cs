using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;
using System.Reflection;

namespace SGX.SistemaChamado.Tests;

public sealed class ReaberturaControladaChamadoUseCaseTests
{
    [Fact]
    public async Task Reabre_Chamado_Encerrado_Dentro_Do_Prazo_Permitido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var chamado = await SeedEncerradoAsync(context, "RCP-001", DateTime.UtcNow.AddHours(-24));

        var response = await CriarUseCase(context).ExecutarAsync(chamado.Id, new ReabrirChamadoRequest
        {
            Mensagem = "Motivo de reabertura controlada."
        });

        var atualizado = context.Chamados.Single(x => x.Id == chamado.Id);
        Assert.Equal("Em Atendimento", response.Status);
        Assert.Equal(SeedData.StatusEmAtendimentoId, atualizado.StatusId);
        Assert.Null(atualizado.EncerradoEm);
        Assert.NotNull(atualizado.ResolvidoEm);
    }

    [Fact]
    public async Task Bloqueia_Reabertura_De_Chamado_Encerrado_Fora_Do_Prazo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        DefinirParametro(context, "48");
        var chamado = await SeedEncerradoAsync(context, "RCP-002", DateTime.UtcNow.AddHours(-200));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CriarUseCase(context).ExecutarAsync(chamado.Id, new ReabrirChamadoRequest
            {
                Mensagem = "Motivo valido."
            }));

        Assert.Contains("prazo maximo de reabertura", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Usa_Prazo_Configurado_Em_ParametroSistema()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        DefinirParametro(context, "24");
        var chamado = await SeedEncerradoAsync(context, "RCP-003", DateTime.UtcNow.AddHours(-30));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CriarUseCase(context).ExecutarAsync(chamado.Id, new ReabrirChamadoRequest
            {
                Mensagem = "Reabrir com prazo configurado."
            }));

        Assert.Contains("24 horas", ex.Message);
    }

    [Fact]
    public async Task Usa_Prazo_Padrao_Quando_Parametro_Nao_Esta_Ativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var parametro = context.ParametrosSistema.SingleOrDefault(x => x.Id == SeedData.ParametroPrazoReaberturaChamadoId);
        if (parametro is null)
        {
            parametro = new ParametroSistema(
                "chamados.reabertura.prazo_maximo_horas",
                "48",
                "Prazo maximo em horas para reabertura de chamado encerrado",
                false,
                "teste");
            DefinirPropriedade(parametro, "Id", SeedData.ParametroPrazoReaberturaChamadoId);
            context.ParametrosSistema.Add(parametro);
        }
        parametro.Desativar("teste");
        await context.SaveChangesAsync();

        var chamado = await SeedEncerradoAsync(context, "RCP-004", DateTime.UtcNow.AddHours(-100));

        var response = await CriarUseCase(context).ExecutarAsync(chamado.Id, new ReabrirChamadoRequest
        {
            Mensagem = "Usando fallback padrao."
        });

        Assert.Equal("Em Atendimento", response.Status);
    }

    [Fact]
    public async Task Rejeita_Configuracao_Invalida_De_Forma_Segura()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        DefinirParametro(context, "abc");
        var chamado = await SeedEncerradoAsync(context, "RCP-005", DateTime.UtcNow.AddHours(-10));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CriarUseCase(context).ExecutarAsync(chamado.Id, new ReabrirChamadoRequest
            {
                Mensagem = "Motivo valido."
            }));

        Assert.Contains("configuracao administrativa do prazo maximo de reabertura", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Registra_Historico_E_Auditoria_Da_Reabertura_Controlada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        DefinirParametro(context, "48");
        var auditoria = new FakeAuditoriaService();
        var chamado = await SeedEncerradoAsync(context, "RCP-006", DateTime.UtcNow.AddHours(-12));

        await CriarUseCase(context, auditoria).ExecutarAsync(chamado.Id, new ReabrirChamadoRequest
        {
            Mensagem = "Motivo auditado."
        });

        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == chamado.Id && x.Tipo == TipoHistoricoChamado.Reaberto);
        var evento = Assert.Single(auditoria.Eventos);
        Assert.Equal("Chamado reaberto por politica de prazo.", evento.Descricao);
        Assert.Equal(TipoAcaoAuditoria.ReabrirChamado, evento.Acao);
        Assert.Contains("Motivo auditado.", evento.DadosDepois ?? string.Empty);
    }

    private static ReabrirChamadoUseCase CriarUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        FakeAuditoriaService? auditoria = null)
    {
        var admin = new UsuarioContextoAplicacao(Guid.NewGuid(), "Administrador", "admin@empresa.com", "admin", ["Administrador"]);
        return new ReabrirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(admin),
            PortalUseCasesTestFactory.Uow(context),
            auditoria ?? new FakeAuditoriaService(),
            validarBloqueioMovimentacaoUseCase: null,
            parametroRepository: PortalUseCasesTestFactory.Repo<ParametroSistema>(context));
    }

    private static async Task<Chamado> SeedEncerradoAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        string codigo,
        DateTime encerradoEm)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Admin-{codigo}", $"admin-{codigo.ToLowerInvariant()}@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Solicitante-{codigo}", $"{codigo.ToLowerInvariant()}@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria-{codigo}");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Encerrado, null, codigo);

        DefinirPropriedade(chamado, nameof(Chamado.ResponsavelId), admin.Id);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), encerradoEm.AddHours(-6));
        DefinirPropriedade(chamado, nameof(Chamado.EncerradoEm), encerradoEm);

        context.Chamados.Update(chamado);
        await context.SaveChangesAsync();
        return chamado;
    }

    private static void DefinirParametro(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        string valor)
    {
        var parametro = context.ParametrosSistema.SingleOrDefault(x => x.Id == SeedData.ParametroPrazoReaberturaChamadoId);
        if (parametro is null)
        {
            parametro = new ParametroSistema(
                "chamados.reabertura.prazo_maximo_horas",
                valor,
                "Prazo maximo em horas para reabertura de chamado encerrado",
                false,
                "teste");
            DefinirPropriedade(parametro, "Id", SeedData.ParametroPrazoReaberturaChamadoId);
            context.ParametrosSistema.Add(parametro);
        }
        else
        {
            parametro.Ativar("teste");
            parametro.AtualizarValor(valor, "teste");
        }
        context.SaveChanges();
    }

    private static void DefinirPropriedade(object alvo, string propriedade, object? valor)
    {
        var propertyInfo = alvo.GetType().GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        propertyInfo!.SetValue(alvo, valor);
    }
}
