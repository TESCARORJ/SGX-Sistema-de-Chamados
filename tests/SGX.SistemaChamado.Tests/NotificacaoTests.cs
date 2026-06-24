using System.Reflection;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class NotificacaoTests
{
    private static readonly Guid UsuarioId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid ChamadoId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly DateTime DataBaseUtc = new(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void DeveCriarNotificacaoValidaParaUsuarioInterno()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);

        Assert.Equal(UsuarioId, notificacao.DestinatarioUsuarioId);
        Assert.Null(notificacao.DestinatarioEndereco);
        Assert.Equal(StatusNotificacao.Pendente, notificacao.Status);
    }

    [Fact]
    public void DeveCriarNotificacaoValidaParaEnderecoExterno()
    {
        var notificacao = CriarNotificacao(destinatarioEndereco: "externo@cliente.com");

        Assert.Equal("externo@cliente.com", notificacao.DestinatarioEndereco);
        Assert.Null(notificacao.DestinatarioUsuarioId);
    }

    [Fact]
    public void DeveCriarNotificacaoValidaComUsuarioEEndereco()
    {
        var notificacao = CriarNotificacao(
            destinatarioUsuarioId: UsuarioId,
            destinatarioEndereco: "usuario.externo@cliente.com");

        Assert.Equal(UsuarioId, notificacao.DestinatarioUsuarioId);
        Assert.Equal("usuario.externo@cliente.com", notificacao.DestinatarioEndereco);
    }

    [Fact]
    public void DeveCriarNotificacaoValidaSemChamado()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId, chamadoId: null);

        Assert.Null(notificacao.ChamadoId);
    }

    [Fact]
    public void DeveCriarNotificacaoValidaComChamado()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId, chamadoId: ChamadoId);

        Assert.Equal(ChamadoId, notificacao.ChamadoId);
    }

    [Fact]
    public void DeveIniciarEmPendente()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);

        Assert.Equal(StatusNotificacao.Pendente, notificacao.Status);
    }

    [Fact]
    public void DeveIniciarComZeroTentativas()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);

        Assert.Equal(0, notificacao.QuantidadeTentativas);
    }

    [Fact]
    public void NaoDevePermitirCriacaoSemDestinatario()
    {
        var ex = Assert.Throws<ArgumentException>(() => CriarNotificacao());

        Assert.Contains("destinatario", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NaoDevePermitirConteudoVazio()
    {
        var ex = Assert.Throws<ArgumentException>(() => CriarNotificacao(destinatarioUsuarioId: UsuarioId, conteudo: " "));

        Assert.Contains("conteudo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NaoDevePermitirChaveIdempotenciaVazia()
    {
        var ex = Assert.Throws<ArgumentException>(() => CriarNotificacao(destinatarioUsuarioId: UsuarioId, chaveIdempotencia: " "));

        Assert.Contains("idempotencia", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NaoDevePermitirTipoEventoInvalido()
    {
        var ex = Assert.Throws<ArgumentException>(() => CriarNotificacao(
            destinatarioUsuarioId: UsuarioId,
            tipoEvento: (TipoEventoNotificacao)999));

        Assert.Contains("tipo de evento", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NaoDevePermitirCanalInvalido()
    {
        var ex = Assert.Throws<ArgumentException>(() => CriarNotificacao(
            destinatarioUsuarioId: UsuarioId,
            canal: (CanalNotificacao)999));

        Assert.Contains("canal", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveNormalizarStringsComTrim()
    {
        var notificacao = CriarNotificacao(
            destinatarioUsuarioId: UsuarioId,
            conteudo: "  Conteudo com espacos  ",
            chaveCorrelacao: "  corr-123  ",
            chaveIdempotencia: "  idem-123  ",
            assunto: "  Assunto  ",
            destinatarioEndereco: "  externo@cliente.com  ");

        Assert.Equal("Conteudo com espacos", notificacao.Conteudo);
        Assert.Equal("corr-123", notificacao.ChaveCorrelacao);
        Assert.Equal("idem-123", notificacao.ChaveIdempotencia);
        Assert.Equal("Assunto", notificacao.Assunto);
        Assert.Equal("externo@cliente.com", notificacao.DestinatarioEndereco);
    }

    [Fact]
    public void DeveAceitarAssuntoOpcional()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId, assunto: null);

        Assert.Null(notificacao.Assunto);
    }

    [Fact]
    public void DeveAceitarCorrelacaoOpcional()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId, chaveCorrelacao: null);

        Assert.Null(notificacao.ChaveCorrelacao);
    }

    [Fact]
    public void DeveAgendarNotificacaoPendente()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);

        notificacao.Agendar(DataBaseUtc, "teste");

        Assert.Equal(StatusNotificacao.Agendada, notificacao.Status);
        Assert.Equal(DataBaseUtc, notificacao.AgendadaEm);
    }

    [Fact]
    public void NaoDevePermitirAgendarEstadoIncompativel()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.IniciarProcessamento(DataBaseUtc, "teste");

        Assert.Throws<InvalidOperationException>(() => notificacao.Agendar(DataBaseUtc.AddMinutes(1), "teste"));
    }

    [Fact]
    public void DeveIniciarProcessamentoAPartirDePendente()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);

        notificacao.IniciarProcessamento(DataBaseUtc, "teste");

        Assert.Equal(StatusNotificacao.EmProcessamento, notificacao.Status);
        Assert.Equal(DataBaseUtc, notificacao.ProcessadaEm);
    }

    [Fact]
    public void DeveIniciarProcessamentoAPartirDeAgendada()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.Agendar(DataBaseUtc, "teste");

        notificacao.IniciarProcessamento(DataBaseUtc.AddMinutes(5), "teste");

        Assert.Equal(StatusNotificacao.EmProcessamento, notificacao.Status);
        Assert.Equal(DataBaseUtc.AddMinutes(5), notificacao.ProcessadaEm);
    }

    [Fact]
    public void DevePermitirReprocessamentoExplicitoAPartirDeFalhou()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.IniciarProcessamento(DataBaseUtc, "teste");
        notificacao.RegistrarFalha("Falha SMTP", DataBaseUtc.AddMinutes(1), "teste");

        notificacao.IniciarProcessamento(DataBaseUtc.AddMinutes(2), "teste");

        Assert.Equal(StatusNotificacao.EmProcessamento, notificacao.Status);
        Assert.Equal(2, notificacao.QuantidadeTentativas);
        Assert.Null(notificacao.UltimoErro);
        Assert.Null(notificacao.FalhouEm);
    }

    [Fact]
    public void DeveIncrementarTentativaAoIniciarProcessamento()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);

        notificacao.IniciarProcessamento(DataBaseUtc, "teste");

        Assert.Equal(1, notificacao.QuantidadeTentativas);
    }

    [Fact]
    public void DeveConsiderarPendenteComoProcessavel()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);

        Assert.True(notificacao.EstaProcessavel(DataBaseUtc, 5));
    }

    [Fact]
    public void DeveConsiderarAgendadaVencidaComoProcessavel()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.Agendar(DataBaseUtc.AddMinutes(-1), "teste");

        Assert.True(notificacao.EstaProcessavel(DataBaseUtc, 5));
    }

    [Fact]
    public void NaoDeveConsiderarAgendadaFuturaComoProcessavel()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.Agendar(DataBaseUtc.AddMinutes(10), "teste");

        Assert.False(notificacao.EstaProcessavel(DataBaseUtc, 5));
    }

    [Fact]
    public void NaoDeveConsiderarFalhouComoProcessavelAutomaticamente()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.IniciarProcessamento(DataBaseUtc, "teste");
        notificacao.RegistrarFalha("Falha temporaria", DataBaseUtc.AddMinutes(1), "teste");

        Assert.False(notificacao.EstaProcessavel(DataBaseUtc.AddMinutes(2), 5));
    }

    [Fact]
    public void NaoDeveConsiderarNotificacaoComLimiteDeTentativasAtingidoComoProcessavel()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);

        for (var tentativa = 0; tentativa < 5; tentativa++)
        {
            notificacao.IniciarProcessamento(DataBaseUtc.AddMinutes(tentativa), "teste");
            notificacao.RegistrarFalha($"Falha {tentativa}", DataBaseUtc.AddMinutes(tentativa + 1), "teste");
        }

        Assert.False(notificacao.EstaProcessavel(DataBaseUtc.AddMinutes(10), 5));
    }

    [Fact]
    public void DeveRegistrarEnvioApenasAPartirDeEmProcessamento()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.IniciarProcessamento(DataBaseUtc, "teste");

        notificacao.RegistrarEnvio(DataBaseUtc.AddMinutes(1), "teste");

        Assert.Equal(StatusNotificacao.Enviada, notificacao.Status);
        Assert.Equal(DataBaseUtc.AddMinutes(1), notificacao.EnviadaEm);
        Assert.Null(notificacao.UltimoErro);
    }

    [Fact]
    public void NotificacaoSistemaEnviadaDeveIniciarNaoLida()
    {
        var notificacao = CriarNotificacaoSistemaEnviada();

        Assert.False(notificacao.Lida);
        Assert.Null(notificacao.LidaEm);
    }

    [Fact]
    public void DeveMarcarNotificacaoSistemaComoLida()
    {
        var notificacao = CriarNotificacaoSistemaEnviada();
        var dataLeitura = notificacao.EnviadaEm!.Value.AddMinutes(1);

        notificacao.MarcarComoLida(dataLeitura, "teste");

        Assert.True(notificacao.Lida);
        Assert.Equal(dataLeitura, notificacao.LidaEm);
        Assert.Equal(StatusNotificacao.Enviada, notificacao.Status);
        Assert.NotNull(notificacao.EnviadaEm);
        Assert.True(notificacao.EnviadaEm.Value <= dataLeitura);
    }

    [Fact]
    public void NaoDevePermitirLeituraAntesDaDisponibilizacao()
    {
        var notificacao = CriarNotificacaoSistemaEnviada();

        Assert.Throws<InvalidOperationException>(() => notificacao.MarcarComoLida(notificacao.EnviadaEm!.Value.AddMinutes(-1), "teste"));
    }

    [Fact]
    public void DevePreservarPrimeiraDataAoMarcarComoLidaNovamente()
    {
        var notificacao = CriarNotificacaoSistemaEnviada();
        var primeiraLeitura = notificacao.EnviadaEm!.Value.AddMinutes(1);

        notificacao.MarcarComoLida(primeiraLeitura, "teste");
        notificacao.MarcarComoLida(primeiraLeitura.AddMinutes(3), "teste");

        Assert.Equal(primeiraLeitura, notificacao.LidaEm);
    }

    [Fact]
    public void DevePermitirMarcarComoNaoLidaDeFormaIdempotente()
    {
        var notificacao = CriarNotificacaoSistemaEnviada();
        notificacao.MarcarComoLida(notificacao.EnviadaEm!.Value.AddMinutes(1), "teste");

        notificacao.MarcarComoNaoLida("teste");
        notificacao.MarcarComoNaoLida("teste");

        Assert.False(notificacao.Lida);
        Assert.Null(notificacao.LidaEm);
        Assert.Equal(StatusNotificacao.Enviada, notificacao.Status);
    }

    [Fact]
    public void CanalEmailNaoAceitaControleDeLeitura()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.IniciarProcessamento(DataBaseUtc, "teste");
        notificacao.RegistrarEnvio(DataBaseUtc.AddMinutes(1), "teste");

        Assert.Throws<InvalidOperationException>(() => notificacao.MarcarComoLida(DataBaseUtc.AddMinutes(2), "teste"));
    }

    [Fact]
    public void EstadosNaoEnviadosNaoAceitamControleDeLeitura()
    {
        var pendente = CriarNotificacao(destinatarioUsuarioId: UsuarioId, canal: CanalNotificacao.Sistema);
        var agendada = CriarNotificacao(destinatarioUsuarioId: UsuarioId, canal: CanalNotificacao.Sistema);
        agendada.Agendar(DataBaseUtc, "teste");
        var emProcessamento = CriarNotificacao(destinatarioUsuarioId: UsuarioId, canal: CanalNotificacao.Sistema);
        emProcessamento.IniciarProcessamento(DataBaseUtc, "teste");
        var falhou = CriarNotificacao(destinatarioUsuarioId: UsuarioId, canal: CanalNotificacao.Sistema);
        falhou.IniciarProcessamento(DataBaseUtc, "teste");
        falhou.RegistrarFalha("falha", DataBaseUtc.AddMinutes(1), "teste");
        var cancelada = CriarNotificacao(destinatarioUsuarioId: UsuarioId, canal: CanalNotificacao.Sistema);
        cancelada.Cancelar(DataBaseUtc, "teste", "cancelada");

        Assert.Throws<InvalidOperationException>(() => pendente.MarcarComoLida(DataBaseUtc.AddMinutes(2), "teste"));
        Assert.Throws<InvalidOperationException>(() => agendada.MarcarComoLida(DateTime.UtcNow.AddMinutes(2), "teste"));
        Assert.Throws<InvalidOperationException>(() => emProcessamento.MarcarComoLida(DateTime.UtcNow.AddMinutes(2), "teste"));
        Assert.Throws<InvalidOperationException>(() => falhou.MarcarComoLida(DateTime.UtcNow.AddMinutes(2), "teste"));
        Assert.Throws<InvalidOperationException>(() => cancelada.MarcarComoLida(DateTime.UtcNow.AddMinutes(2), "teste"));
    }

    [Fact]
    public void NaoDevePermitirEnvioDiretoDePendente()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);

        Assert.Throws<InvalidOperationException>(() => notificacao.RegistrarEnvio(DataBaseUtc, "teste"));
    }

    [Fact]
    public void DeveRegistrarFalhaApenasAPartirDeEmProcessamento()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.IniciarProcessamento(DataBaseUtc, "teste");

        notificacao.RegistrarFalha("Falha SMTP", DataBaseUtc.AddMinutes(1), "teste");

        Assert.Equal(StatusNotificacao.Falhou, notificacao.Status);
        Assert.Equal("Falha SMTP", notificacao.UltimoErro);
        Assert.Equal(DataBaseUtc.AddMinutes(1), notificacao.FalhouEm);
    }

    [Fact]
    public void DevePermitirReagendarAposFalhaMantendoUltimoErro()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.IniciarProcessamento(DataBaseUtc, "teste");
        notificacao.RegistrarFalha("Falha transitoria", DataBaseUtc.AddMinutes(1), "teste");

        notificacao.ReagendarAposFalha(DataBaseUtc.AddMinutes(6), "teste");

        Assert.Equal(StatusNotificacao.Agendada, notificacao.Status);
        Assert.Equal(DataBaseUtc.AddMinutes(6), notificacao.AgendadaEm);
        Assert.Equal("Falha transitoria", notificacao.UltimoErro);
        Assert.Equal(DataBaseUtc.AddMinutes(1), notificacao.FalhouEm);
    }

    [Fact]
    public void NaoDevePermitirReagendarSemFalhaAnterior()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);

        Assert.Throws<InvalidOperationException>(() => notificacao.ReagendarAposFalha(DataBaseUtc.AddMinutes(1), "teste"));
    }

    [Fact]
    public void NaoDevePermitirFalhaForaDeProcessamento()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);

        Assert.Throws<InvalidOperationException>(() => notificacao.RegistrarFalha("Falha", DataBaseUtc, "teste"));
    }

    [Fact]
    public void DeveCancelarNotificacaoPendente()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);

        notificacao.Cancelar(DataBaseUtc, "teste", "Cancelada por regra");

        Assert.Equal(StatusNotificacao.Cancelada, notificacao.Status);
        Assert.Equal(DataBaseUtc, notificacao.CanceladaEm);
        Assert.Equal("Cancelada por regra", notificacao.MotivoCancelamento);
    }

    [Fact]
    public void DeveCancelarNotificacaoAgendada()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.Agendar(DataBaseUtc, "teste");

        notificacao.Cancelar(DataBaseUtc.AddMinutes(1), "teste", "Cancelada antes do envio");

        Assert.Equal(StatusNotificacao.Cancelada, notificacao.Status);
        Assert.Equal(DataBaseUtc.AddMinutes(1), notificacao.CanceladaEm);
    }

    [Fact]
    public void NaoDevePermitirCancelamentoDeEnviada()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.IniciarProcessamento(DataBaseUtc, "teste");
        notificacao.RegistrarEnvio(DataBaseUtc.AddMinutes(1), "teste");

        Assert.Throws<InvalidOperationException>(() => notificacao.Cancelar(DataBaseUtc.AddMinutes(2), "teste"));
    }

    [Fact]
    public void NaoDevePermitirCancelamentoDeCancelada()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.Cancelar(DataBaseUtc, "teste", "Primeiro cancelamento");

        Assert.Throws<InvalidOperationException>(() => notificacao.Cancelar(DataBaseUtc.AddMinutes(1), "teste"));
    }

    [Fact]
    public void NaoDevePermitirCancelamentoEmProcessamento()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.IniciarProcessamento(DataBaseUtc, "teste");

        Assert.Throws<InvalidOperationException>(() => notificacao.Cancelar(DataBaseUtc.AddMinutes(1), "teste"));
    }

    [Fact]
    public void NaoDevePermitirTransicaoAposCancelamento()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.Cancelar(DataBaseUtc, "teste", "Cancelada");

        Assert.Throws<InvalidOperationException>(() => notificacao.IniciarProcessamento(DataBaseUtc.AddMinutes(1), "teste"));
        Assert.Throws<InvalidOperationException>(() => notificacao.RegistrarEnvio(DataBaseUtc.AddMinutes(1), "teste"));
    }

    [Fact]
    public void DeveValidarLimiteDeDestinatarioEndereco()
    {
        var enderecoLongo = new string('a', 64) + "@" + new string('b', 252) + ".com";

        var ex = Assert.Throws<ArgumentException>(() => CriarNotificacao(destinatarioEndereco: enderecoLongo));

        Assert.Contains("maximo 320", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveValidarLimiteDeAssunto()
    {
        var ex = Assert.Throws<ArgumentException>(() => CriarNotificacao(
            destinatarioUsuarioId: UsuarioId,
            assunto: new string('a', 301)));

        Assert.Contains("maximo 300", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveValidarLimiteDeConteudo()
    {
        var ex = Assert.Throws<ArgumentException>(() => CriarNotificacao(
            destinatarioUsuarioId: UsuarioId,
            conteudo: new string('a', 10001)));

        Assert.Contains("maximo 10000", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveValidarLimiteDeChaveCorrelacao()
    {
        var ex = Assert.Throws<ArgumentException>(() => CriarNotificacao(
            destinatarioUsuarioId: UsuarioId,
            chaveCorrelacao: new string('a', 201)));

        Assert.Contains("maximo 200", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveValidarLimiteDeChaveIdempotencia()
    {
        var ex = Assert.Throws<ArgumentException>(() => CriarNotificacao(
            destinatarioUsuarioId: UsuarioId,
            chaveIdempotencia: new string('a', 201)));

        Assert.Contains("maximo 200", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveValidarLimiteDeUltimoErro()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        notificacao.IniciarProcessamento(DataBaseUtc, "teste");

        var ex = Assert.Throws<ArgumentException>(() => notificacao.RegistrarFalha(new string('a', 2001), DataBaseUtc.AddMinutes(1), "teste"));

        Assert.Contains("maximo 2000", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveValidarLimiteDeMotivoCancelamento()
    {
        var ex = Assert.Throws<ArgumentException>(() => CriarNotificacao(destinatarioUsuarioId: UsuarioId)
            .Cancelar(DataBaseUtc, "teste", new string('a', 1001)));

        Assert.Contains("maximo 1000", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveDiferenciarCorrelacaoDeIdempotencia()
    {
        var notificacao = CriarNotificacao(
            destinatarioUsuarioId: UsuarioId,
            chaveCorrelacao: "correlacao-123",
            chaveIdempotencia: "idempotencia-456");

        Assert.Equal("correlacao-123", notificacao.ChaveCorrelacao);
        Assert.Equal("idempotencia-456", notificacao.ChaveIdempotencia);
        Assert.NotEqual(notificacao.ChaveCorrelacao, notificacao.ChaveIdempotencia);
    }

    [Fact]
    public void NaoDevePermitirQuantidadeNegativaPorConstrucao()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId);
        var property = typeof(Notificacao).GetProperty(nameof(Notificacao.QuantidadeTentativas), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(property);
        Assert.False(property!.SetMethod?.IsPublic ?? false);
        Assert.True(notificacao.QuantidadeTentativas >= 0);
    }

    private static Notificacao CriarNotificacao(
        Guid? destinatarioUsuarioId = null,
        string? destinatarioEndereco = null,
        string conteudo = "Conteudo materializado da notificacao",
        string chaveIdempotencia = "notif:0001",
        string? chaveCorrelacao = "evt:0001",
        Guid? chamadoId = null,
        string? assunto = "Assunto",
        TipoEventoNotificacao tipoEvento = TipoEventoNotificacao.EventoChamado,
        CanalNotificacao canal = CanalNotificacao.Email)
    {
        return new Notificacao(
            tipoEvento,
            canal,
            conteudo,
            chaveIdempotencia,
            "teste",
            destinatarioUsuarioId,
            destinatarioEndereco,
            chamadoId,
            assunto,
            chaveCorrelacao,
            UsuarioId);
    }

    private static Notificacao CriarNotificacaoSistemaEnviada()
    {
        var notificacao = CriarNotificacao(destinatarioUsuarioId: UsuarioId, canal: CanalNotificacao.Sistema);
        var inicio = DateTime.UtcNow.AddMinutes(1);
        notificacao.IniciarProcessamento(inicio, "teste");
        notificacao.RegistrarEnvio(inicio.AddMinutes(1), "teste");
        return notificacao;
    }
}
