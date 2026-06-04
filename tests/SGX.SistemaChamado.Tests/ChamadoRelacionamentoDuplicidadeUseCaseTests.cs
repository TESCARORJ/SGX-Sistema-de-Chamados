using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ChamadoRelacionamentoDuplicidadeUseCaseTests
{
    private const string MensagemEsperadaDuplicidade = "Ja existe um relacionamento ativo entre estes chamados com este tipo de vinculo.";
    private const string MensagemEsperadaCiclo = "Este relacionamento criaria um ciclo indevido entre chamados.";

    [Fact]
    public async Task DeveBloquearCriacaoDeRelacionamentoAtivoDuplicadoComMesmoTipo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        context.ChamadosRelacionamentos.Add(new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Relacionado,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var request = new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoOrigem.Id,
            ChamadoDestinoId = dados.ChamadoDestino.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Relacionado
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(request));
        Assert.Equal(MensagemEsperadaDuplicidade, ex.Message);
    }

    [Fact]
    public async Task DevePermitirRelacionamentoComMesmoParQuandoTipoForDiferente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        context.ChamadosRelacionamentos.Add(new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Relacionado,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoOrigem.Id,
            ChamadoDestinoId = dados.ChamadoDestino.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Bloqueia
        });

        Assert.Equal(TipoRelacionamentoChamadoEnum.Bloqueia, response.TipoRelacionamento);
        Assert.Equal(2, context.ChamadosRelacionamentos.Count());
    }

    [Fact]
    public async Task DevePermitirCriacaoQuandoRelacionamentoAnteriorEstiverInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamentoInativo = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Relacionado,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        relacionamentoInativo.Inativar(dados.AdminUsuario.Id, dados.AdminUsuario.Login, "Encerrado.");
        context.ChamadosRelacionamentos.Add(relacionamentoInativo);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoOrigem.Id,
            ChamadoDestinoId = dados.ChamadoDestino.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Relacionado
        });

        Assert.Equal(TipoRelacionamentoChamadoEnum.Relacionado, response.TipoRelacionamento);
        Assert.Equal(2, context.ChamadosRelacionamentos.Count());
        Assert.Equal(1, context.ChamadosRelacionamentos.Count(x => x.Ativo));
    }

    [Fact]
    public async Task DevePermitirCriacaoComOrigemEDestinoInvertidos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        context.ChamadosRelacionamentos.Add(new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Duplicado,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoDestino.Id,
            ChamadoDestinoId = dados.ChamadoOrigem.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Duplicado
        });

        Assert.Equal(dados.ChamadoDestino.Id, response.ChamadoOrigemId);
        Assert.Equal(dados.ChamadoOrigem.Id, response.ChamadoDestinoId);
        Assert.Equal(2, context.ChamadosRelacionamentos.Count());
    }

    [Fact]
    public async Task DeveRetornarMensagemControladaQuandoBancoApontarViolacaoDeIndiceUnico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var unitOfWorkComFalha = new FakeUnitOfWorkErroIndiceRelacionamentoDuplicado();

        var useCase = new RelacionamentosChamadoUseCases(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ChamadoRelacionamento>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.ContextoAdmin),
            unitOfWorkComFalha);

        var request = new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoOrigem.Id,
            ChamadoDestinoId = dados.ChamadoDestino.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Relacionado
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(request));
        Assert.Equal(MensagemEsperadaDuplicidade, ex.Message);
    }

    [Fact]
    public async Task DeveBloquearCicloDiretoComTipoProtegido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        await AdicionarRelacionamentoAsync(context, dados, TipoRelacionamentoChamadoEnum.Bloqueia);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoDestino.Id,
            ChamadoDestinoId = dados.ChamadoOrigem.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Bloqueia
        }));

        Assert.Equal(MensagemEsperadaCiclo, ex.Message);
    }

    [Fact]
    public async Task DeveBloquearCicloIndiretoComTipoProtegido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedCadeiaAsync(context);

        await AdicionarRelacionamentoAsync(context, dados.ChamadoA, dados.ChamadoB, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Pai);
        await AdicionarRelacionamentoAsync(context, dados.ChamadoB, dados.ChamadoC, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Pai);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoC.Id,
            ChamadoDestinoId = dados.ChamadoA.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Pai
        }));

        Assert.Equal(MensagemEsperadaCiclo, ex.Message);
    }

    [Fact]
    public async Task DevePermitirCadeiaValidaSemFechamentoDeCiclo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedCadeiaAsync(context);

        await AdicionarRelacionamentoAsync(context, dados.ChamadoA, dados.ChamadoB, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Bloqueia);
        await AdicionarRelacionamentoAsync(context, dados.ChamadoB, dados.ChamadoC, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Bloqueia);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoC.Id,
            ChamadoDestinoId = dados.ChamadoD.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Bloqueia
        });

        Assert.Equal(dados.ChamadoC.Id, response.ChamadoOrigemId);
        Assert.Equal(dados.ChamadoD.Id, response.ChamadoDestinoId);
        Assert.Equal(3, context.ChamadosRelacionamentos.Count());
    }

    [Fact]
    public async Task DeveIgnorarRelacionamentoInativoNaDeteccaoDeCiclo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamentoInativo = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        relacionamentoInativo.Inativar(dados.AdminUsuario.Id, dados.AdminUsuario.Login, "Encerrado.");
        context.ChamadosRelacionamentos.Add(relacionamentoInativo);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoDestino.Id,
            ChamadoDestinoId = dados.ChamadoOrigem.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Bloqueia
        });

        Assert.Equal(dados.ChamadoDestino.Id, response.ChamadoOrigemId);
        Assert.Equal(dados.ChamadoOrigem.Id, response.ChamadoDestinoId);
    }

    [Fact]
    public async Task NaoDeveBloquearCicloParaTipoRelacionadoNestaEtapa()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedCadeiaAsync(context);

        await AdicionarRelacionamentoAsync(context, dados.ChamadoA, dados.ChamadoB, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Relacionado);
        await AdicionarRelacionamentoAsync(context, dados.ChamadoB, dados.ChamadoC, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Relacionado);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoC.Id,
            ChamadoDestinoId = dados.ChamadoA.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Relacionado
        });

        Assert.Equal(TipoRelacionamentoChamadoEnum.Relacionado, response.TipoRelacionamento);
    }

    [Fact]
    public async Task NaoDeveBloquearCicloParaTipoDuplicadoNestaEtapa()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedCadeiaAsync(context);

        await AdicionarRelacionamentoAsync(context, dados.ChamadoA, dados.ChamadoB, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Duplicado);
        await AdicionarRelacionamentoAsync(context, dados.ChamadoB, dados.ChamadoC, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Duplicado);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoC.Id,
            ChamadoDestinoId = dados.ChamadoA.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Duplicado
        });

        Assert.Equal(TipoRelacionamentoChamadoEnum.Duplicado, response.TipoRelacionamento);
    }

    [Fact]
    public async Task DeveBloquearCicloComParInversoPaiFilho()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedCadeiaAsync(context);

        await AdicionarRelacionamentoAsync(context, dados.ChamadoA, dados.ChamadoB, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Pai);
        await AdicionarRelacionamentoAsync(context, dados.ChamadoB, dados.ChamadoC, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Pai);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoA.Id,
            ChamadoDestinoId = dados.ChamadoC.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Filho
        }));

        Assert.Equal(MensagemEsperadaCiclo, ex.Message);
    }

    [Fact]
    public async Task DeveBloquearCicloComParInversoBloqueiaBloqueadoPor()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedCadeiaAsync(context);

        await AdicionarRelacionamentoAsync(context, dados.ChamadoA, dados.ChamadoB, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Bloqueia);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoA.Id,
            ChamadoDestinoId = dados.ChamadoB.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.BloqueadoPor
        }));

        Assert.Equal(MensagemEsperadaCiclo, ex.Message);
    }

    [Fact]
    public async Task DeveBloquearCicloComParInversoOriginaDerivadoDe()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedCadeiaAsync(context);

        await AdicionarRelacionamentoAsync(context, dados.ChamadoA, dados.ChamadoB, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Origina);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoA.Id,
            ChamadoDestinoId = dados.ChamadoB.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.DerivadoDe
        }));

        Assert.Equal(MensagemEsperadaCiclo, ex.Message);
    }

    [Fact]
    public async Task DeveRegistrarHistoricoNaOrigemEDestinoQuandoCriarVinculo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoOrigem.Id,
            ChamadoDestinoId = dados.ChamadoDestino.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Bloqueia,
            Justificativa = "Dependencia operacional."
        });

        var historicoOrigem = context.HistoricosChamado
            .Single(x => x.ChamadoId == dados.ChamadoOrigem.Id && x.Tipo == TipoHistoricoChamado.RelacionamentoCriado);
        var historicoDestino = context.HistoricosChamado
            .Single(x => x.ChamadoId == dados.ChamadoDestino.Id && x.Tipo == TipoHistoricoChamado.RelacionamentoRecebido);

        Assert.Contains(dados.ChamadoDestino.Id.ToString(), historicoOrigem.Descricao, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(TipoRelacionamentoChamadoEnum.Bloqueia.ToString(), historicoOrigem.Descricao, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Justificativa: Dependencia operacional.", historicoOrigem.Descricao, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(response.Id.ToString(), historicoOrigem.Descricao, StringComparison.OrdinalIgnoreCase);

        Assert.Contains(dados.ChamadoOrigem.Id.ToString(), historicoDestino.Descricao, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(TipoRelacionamentoChamadoEnum.Bloqueia.ToString(), historicoDestino.Descricao, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Justificativa: Dependencia operacional.", historicoDestino.Descricao, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(response.Id.ToString(), historicoDestino.Descricao, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task NaoDeveRegistrarHistoricoQuandoCriacaoFalharPorDuplicidade()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        await AdicionarRelacionamentoAsync(context, dados, TipoRelacionamentoChamadoEnum.Relacionado);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        _ = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoOrigem.Id,
            ChamadoDestinoId = dados.ChamadoDestino.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Relacionado
        }));

        Assert.DoesNotContain(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.RelacionamentoCriado || x.Tipo == TipoHistoricoChamado.RelacionamentoRecebido);
    }

    [Fact]
    public async Task NaoDeveRegistrarHistoricoQuandoCriacaoFalharPorCiclo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedCadeiaAsync(context);

        await AdicionarRelacionamentoAsync(context, dados.ChamadoA, dados.ChamadoB, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Pai);
        await AdicionarRelacionamentoAsync(context, dados.ChamadoB, dados.ChamadoC, dados.AdminUsuario, TipoRelacionamentoChamadoEnum.Pai);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        _ = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoC.Id,
            ChamadoDestinoId = dados.ChamadoA.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Pai
        }));

        Assert.DoesNotContain(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.RelacionamentoCriado || x.Tipo == TipoHistoricoChamado.RelacionamentoRecebido);
    }

    private static RelacionamentosChamadoUseCases CriarUseCase(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ChamadoRelacionamento>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task AdicionarRelacionamentoAsync(
        SGXSistemaChamadoDbContext context,
        (Chamado ChamadoOrigem, Chamado ChamadoDestino, Usuario AdminUsuario, UsuarioContextoAplicacao ContextoAdmin) dados,
        TipoRelacionamentoChamadoEnum tipoRelacionamento)
    {
        context.ChamadosRelacionamentos.Add(new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            tipoRelacionamento,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login));
        await context.SaveChangesAsync();
    }

    private static async Task AdicionarRelacionamentoAsync(
        SGXSistemaChamadoDbContext context,
        Chamado origem,
        Chamado destino,
        Usuario adminUsuario,
        TipoRelacionamentoChamadoEnum tipoRelacionamento)
    {
        context.ChamadosRelacionamentos.Add(new ChamadoRelacionamento(
            origem.Id,
            destino.Id,
            tipoRelacionamento,
            adminUsuario.Id,
            adminUsuario.Login));
        await context.SaveChangesAsync();
    }

    private static async Task<(Chamado ChamadoOrigem, Chamado ChamadoDestino, Usuario AdminUsuario, UsuarioContextoAplicacao ContextoAdmin)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Admin Relacionamento",
            $"admin.rel.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante Relacionamento",
            $"sol.rel.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria Rel {Guid.NewGuid():N}");

        var chamadoOrigem = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "REL-A");
        var chamadoDestino = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "REL-B");

        return (chamadoOrigem, chamadoDestino, admin, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }

    private static async Task<(Chamado ChamadoA, Chamado ChamadoB, Chamado ChamadoC, Chamado ChamadoD, Usuario AdminUsuario, UsuarioContextoAplicacao ContextoAdmin)> SeedCadeiaAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Admin Cadeia",
            $"admin.cadeia.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante Cadeia",
            $"sol.cadeia.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria Cadeia {Guid.NewGuid():N}");

        var chamadoA = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "REL-CA");
        var chamadoB = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "REL-CB");
        var chamadoC = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "REL-CC");
        var chamadoD = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "REL-CD");

        return (chamadoA, chamadoB, chamadoC, chamadoD, admin, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }

    private sealed class FakeUnitOfWorkErroIndiceRelacionamentoDuplicado : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new DbUpdateException(
                "Falha de persistencia.",
                new InvalidOperationException("duplicate key value violates unique constraint \"ux_chamados_relacionamentos_origem_destino_tipo_ativo\""));
        }
    }
}
