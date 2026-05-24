using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class InventarioAtivosAdminUseCasesTests
{
    [Fact]
    public async Task CriarAtivoValido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var response = await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "NB-001",
            Nome = "Notebook Diretoria",
            TipoAtivoInventarioId = seed.TipoNotebook.Id,
            DepartamentoId = seed.DepartamentoTi.Id,
            LocalUnidadeId = seed.LocalMatriz.Id,
            UsuarioResponsavelId = seed.ResponsavelA.Id,
            StatusOperacional = StatusOperacionalAtivo.Operacional,
            StatusPatrimonial = StatusPatrimonialAtivo.EmUso,
            Criticidade = CriticidadeAtivo.Alta,
            NumeroPatrimonio = "PAT-0001",
            NumeroSerie = "SER-0001"
        });

        Assert.Equal("NB-001", response.Codigo);
        Assert.Equal("Notebook Diretoria", response.Nome);
        Assert.True(response.Ativo);
        Assert.Equal(seed.Admin.Id, response.CriadoPorUsuarioId);
    }

    [Fact]
    public async Task ImpedirCriacaoSemCodigo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = " ",
            Nome = "Ativo sem codigo",
            TipoAtivoInventarioId = seed.TipoNotebook.Id
        }));
    }

    [Fact]
    public async Task ImpedirCriacaoSemNome()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "SEM-NOME",
            Nome = " ",
            TipoAtivoInventarioId = seed.TipoNotebook.Id
        }));
    }

    [Fact]
    public async Task ImpedirCriacaoComTipoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "TIPO-INVALIDO",
            Nome = "Ativo invalido",
            TipoAtivoInventarioId = Guid.NewGuid()
        }));
    }

    [Fact]
    public async Task ImpedirCriacaoComCodigoDuplicado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "DUP-001",
            Nome = "Ativo 1",
            TipoAtivoInventarioId = seed.TipoNotebook.Id
        });

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "dup-001",
            Nome = "Ativo 2",
            TipoAtivoInventarioId = seed.TipoMonitor.Id
        }));
    }

    [Fact]
    public async Task ImpedirCriacaoComPatrimonioDuplicadoQuandoPreenchido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "PAT-ATIVO-1",
            Nome = "Ativo patrimonio 1",
            TipoAtivoInventarioId = seed.TipoNotebook.Id,
            NumeroPatrimonio = "PAT-123"
        });

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "PAT-ATIVO-2",
            Nome = "Ativo patrimonio 2",
            TipoAtivoInventarioId = seed.TipoMonitor.Id,
            NumeroPatrimonio = "PAT-123"
        }));
    }

    [Fact]
    public async Task PermitirCriacaoSemPatrimonio()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var response = await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "SEM-PAT",
            Nome = "Ativo sem patrimonio",
            TipoAtivoInventarioId = seed.TipoNotebook.Id,
            NumeroPatrimonio = null
        });

        Assert.Null(response.NumeroPatrimonio);
    }

    [Fact]
    public async Task ImpedirCriacaoComNumeroSerieDuplicadoQuandoPreenchido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "SER-ATIVO-1",
            Nome = "Ativo serie 1",
            TipoAtivoInventarioId = seed.TipoNotebook.Id,
            NumeroSerie = "SER-XYZ"
        });

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "SER-ATIVO-2",
            Nome = "Ativo serie 2",
            TipoAtivoInventarioId = seed.TipoMonitor.Id,
            NumeroSerie = "SER-XYZ"
        }));
    }

    [Fact]
    public async Task PermitirCriacaoSemNumeroSerie()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var response = await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "SEM-SERIE",
            Nome = "Ativo sem serie",
            TipoAtivoInventarioId = seed.TipoNotebook.Id
        });

        Assert.Null(response.NumeroSerie);
    }

    [Fact]
    public async Task EditarAtivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var criado = await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "EDIT-001",
            Nome = "Ativo original",
            TipoAtivoInventarioId = seed.TipoNotebook.Id,
            UsuarioResponsavelId = seed.ResponsavelA.Id
        });

        var atualizado = await useCase.AtualizarAsync(criado.Id, new AtualizarInventarioAtivoRequest
        {
            Codigo = "EDIT-002",
            Nome = "Ativo atualizado",
            TipoAtivoInventarioId = seed.TipoMonitor.Id,
            DepartamentoId = seed.DepartamentoRh.Id,
            LocalUnidadeId = seed.LocalFilial.Id,
            UsuarioResponsavelId = seed.ResponsavelB.Id,
            StatusOperacional = StatusOperacionalAtivo.EmManutencao,
            StatusPatrimonial = StatusPatrimonialAtivo.EmEstoque,
            Criticidade = CriticidadeAtivo.Baixa,
            NumeroPatrimonio = "PAT-EDIT",
            NumeroSerie = "SER-EDIT"
        });

        Assert.Equal("EDIT-002", atualizado.Codigo);
        Assert.Equal(seed.TipoMonitor.Id, atualizado.TipoAtivoInventarioId);
        Assert.Equal(seed.ResponsavelB.Id, atualizado.UsuarioResponsavelId);
        Assert.Equal(StatusOperacionalAtivo.EmManutencao, atualizado.StatusOperacional);
    }

    [Fact]
    public async Task ImpedirEdicaoDeAtivoInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var criado = await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "INAT-EDIT",
            Nome = "Ativo para inativar",
            TipoAtivoInventarioId = seed.TipoNotebook.Id
        });
        await useCase.InativarAsync(criado.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.AtualizarAsync(criado.Id, new AtualizarInventarioAtivoRequest
        {
            Codigo = "INAT-EDIT-2",
            Nome = "Nao deve atualizar",
            TipoAtivoInventarioId = seed.TipoNotebook.Id,
            StatusOperacional = StatusOperacionalAtivo.Operacional,
            StatusPatrimonial = StatusPatrimonialAtivo.EmUso,
            Criticidade = CriticidadeAtivo.Media
        }));
    }

    [Fact]
    public async Task ValidarUnicidadeAoEditar()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var ativoA = await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "UNQ-A",
            Nome = "Ativo A",
            TipoAtivoInventarioId = seed.TipoNotebook.Id,
            NumeroPatrimonio = "PAT-A",
            NumeroSerie = "SER-A"
        });

        var ativoB = await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "UNQ-B",
            Nome = "Ativo B",
            TipoAtivoInventarioId = seed.TipoMonitor.Id,
            NumeroPatrimonio = "PAT-B",
            NumeroSerie = "SER-B"
        });

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.AtualizarAsync(ativoA.Id, new AtualizarInventarioAtivoRequest
        {
            Codigo = ativoB.Codigo,
            Nome = "Ativo A novo",
            TipoAtivoInventarioId = seed.TipoNotebook.Id,
            NumeroPatrimonio = "PAT-A",
            NumeroSerie = "SER-A",
            StatusOperacional = StatusOperacionalAtivo.Operacional,
            StatusPatrimonial = StatusPatrimonialAtivo.EmUso,
            Criticidade = CriticidadeAtivo.Media
        }));
    }

    [Fact]
    public async Task InativarAtivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var criado = await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "INAT-001",
            Nome = "Ativo para inativacao",
            TipoAtivoInventarioId = seed.TipoNotebook.Id
        });

        var response = await useCase.InativarAsync(criado.Id);
        var salvo = await context.InventarioAtivos.FirstAsync(x => x.Id == criado.Id);

        Assert.False(response.Ativo);
        Assert.False(salvo.Ativo);
        Assert.NotNull(salvo.InativadoEm);
        Assert.Equal(seed.Admin.Id, salvo.InativadoPorUsuarioId);
    }

    [Fact]
    public async Task ReativarAtivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var criado = await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "REAT-001",
            Nome = "Ativo para reativacao",
            TipoAtivoInventarioId = seed.TipoNotebook.Id
        });
        await useCase.InativarAsync(criado.Id);

        var response = await useCase.ReativarAsync(criado.Id);
        var salvo = await context.InventarioAtivos.FirstAsync(x => x.Id == criado.Id);

        Assert.True(response.Ativo);
        Assert.True(salvo.Ativo);
        Assert.NotNull(salvo.InativadoEm);
        Assert.NotNull(salvo.InativadoPorUsuarioId);
    }

    [Fact]
    public async Task FiltrarPorTipo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        await CriarAtivosParaFiltroAsync(useCase, seed);

        var response = await useCase.ListarAsync(new FiltroInventarioAtivoRequest
        {
            TipoAtivoInventarioId = seed.TipoMonitor.Id
        });

        Assert.Single(response.Items);
        Assert.Equal("AT-MON-001", response.Items.Single().Codigo);
    }

    [Fact]
    public async Task FiltrarPorDepartamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        await CriarAtivosParaFiltroAsync(useCase, seed);

        var response = await useCase.ListarAsync(new FiltroInventarioAtivoRequest
        {
            DepartamentoId = seed.DepartamentoRh.Id
        });

        Assert.Single(response.Items);
        Assert.Equal("AT-MON-001", response.Items.Single().Codigo);
    }

    [Fact]
    public async Task FiltrarPorLocal()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        await CriarAtivosParaFiltroAsync(useCase, seed);

        var response = await useCase.ListarAsync(new FiltroInventarioAtivoRequest
        {
            LocalUnidadeId = seed.LocalFilial.Id
        });

        Assert.Single(response.Items);
        Assert.Equal("AT-MON-001", response.Items.Single().Codigo);
    }

    [Fact]
    public async Task FiltrarPorUsuarioResponsavel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        await CriarAtivosParaFiltroAsync(useCase, seed);

        var response = await useCase.ListarAsync(new FiltroInventarioAtivoRequest
        {
            UsuarioResponsavelId = seed.ResponsavelB.Id
        });

        Assert.Single(response.Items);
        Assert.Equal("AT-MON-001", response.Items.Single().Codigo);
    }

    [Fact]
    public async Task FiltrarPorStatusOperacional()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        await CriarAtivosParaFiltroAsync(useCase, seed);

        var response = await useCase.ListarAsync(new FiltroInventarioAtivoRequest
        {
            StatusOperacional = StatusOperacionalAtivo.EmManutencao
        });

        Assert.Single(response.Items);
        Assert.Equal("AT-MON-001", response.Items.Single().Codigo);
    }

    [Fact]
    public async Task FiltrarPorStatusPatrimonial()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        await CriarAtivosParaFiltroAsync(useCase, seed);

        var response = await useCase.ListarAsync(new FiltroInventarioAtivoRequest
        {
            StatusPatrimonial = StatusPatrimonialAtivo.EmEstoque
        });

        Assert.Single(response.Items);
        Assert.Equal("AT-MON-001", response.Items.Single().Codigo);
    }

    [Fact]
    public async Task FiltrarPorCriticidade()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        await CriarAtivosParaFiltroAsync(useCase, seed);

        var response = await useCase.ListarAsync(new FiltroInventarioAtivoRequest
        {
            Criticidade = CriticidadeAtivo.Critica
        });

        Assert.Single(response.Items);
        Assert.Equal("AT-MON-001", response.Items.Single().Codigo);
    }

    [Fact]
    public async Task BuscarPorTermoEmCodigo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        await CriarAtivosParaFiltroAsync(useCase, seed);

        var response = await useCase.ListarAsync(new FiltroInventarioAtivoRequest
        {
            Termo = "mon-001"
        });

        Assert.Single(response.Items);
        Assert.Equal("AT-MON-001", response.Items.Single().Codigo);
    }

    [Fact]
    public async Task BuscarPorTermoEmNome()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        await CriarAtivosParaFiltroAsync(useCase, seed);

        var response = await useCase.ListarAsync(new FiltroInventarioAtivoRequest
        {
            Termo = "monitor almoxarifado"
        });

        Assert.Single(response.Items);
        Assert.Equal("AT-MON-001", response.Items.Single().Codigo);
    }

    [Fact]
    public async Task RegistrarAuditoriaNasOperacoesPrincipais()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var auditoria = new FakeAuditoriaService();
        var useCase = CriarUseCase(context, seed.Admin, auditoria);

        var criado = await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "AUD-001",
            Nome = "Ativo auditado",
            TipoAtivoInventarioId = seed.TipoNotebook.Id
        });

        await useCase.AtualizarAsync(criado.Id, new AtualizarInventarioAtivoRequest
        {
            Codigo = "AUD-001",
            Nome = "Ativo auditado v2",
            TipoAtivoInventarioId = seed.TipoNotebook.Id,
            StatusOperacional = StatusOperacionalAtivo.Operacional,
            StatusPatrimonial = StatusPatrimonialAtivo.EmUso,
            Criticidade = CriticidadeAtivo.Media
        });

        await useCase.InativarAsync(criado.Id);
        await useCase.ReativarAsync(criado.Id);

        Assert.True(auditoria.Eventos.Count >= 4);
    }

    [Fact]
    public async Task RegistrarHistoricoNaCriacaoDeAtivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var criado = await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "HIS-CRI-001",
            Nome = "Ativo historico criacao",
            TipoAtivoInventarioId = seed.TipoNotebook.Id
        });

        var historico = await context.HistoricosInventarioAtivo
            .AsNoTracking()
            .Where(x => x.InventarioAtivoId == criado.Id)
            .OrderByDescending(x => x.CriadoEm)
            .FirstAsync();

        Assert.Equal(TipoMovimentacaoAtivo.Criacao, historico.TipoMovimentacao);
    }

    [Fact]
    public async Task RegistrarHistoricoNaEdicaoComMudancaDeDepartamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "HIS-DEP-001");

        await useCase.AtualizarAsync(criado.Id, new AtualizarInventarioAtivoRequest
        {
            Codigo = criado.Codigo,
            Nome = criado.Nome,
            TipoAtivoInventarioId = criado.TipoAtivoInventarioId,
            DepartamentoId = seed.DepartamentoRh.Id,
            LocalUnidadeId = criado.LocalUnidadeId,
            UsuarioResponsavelId = criado.UsuarioResponsavelId,
            StatusOperacional = criado.StatusOperacional,
            StatusPatrimonial = criado.StatusPatrimonial,
            Criticidade = criado.Criticidade
        });

        var historico = await ObterUltimoHistoricoAsync(context, criado.Id);
        Assert.Equal(TipoMovimentacaoAtivo.TransferenciaDepartamento, historico.TipoMovimentacao);
        Assert.Equal(seed.DepartamentoTi.Id, historico.DepartamentoOrigemId);
        Assert.Equal(seed.DepartamentoRh.Id, historico.DepartamentoDestinoId);
    }

    [Fact]
    public async Task RegistrarHistoricoNaEdicaoComMudancaDeLocal()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "HIS-LOC-001");

        await useCase.AtualizarAsync(criado.Id, new AtualizarInventarioAtivoRequest
        {
            Codigo = criado.Codigo,
            Nome = criado.Nome,
            TipoAtivoInventarioId = criado.TipoAtivoInventarioId,
            DepartamentoId = criado.DepartamentoId,
            LocalUnidadeId = seed.LocalFilial.Id,
            UsuarioResponsavelId = criado.UsuarioResponsavelId,
            StatusOperacional = criado.StatusOperacional,
            StatusPatrimonial = criado.StatusPatrimonial,
            Criticidade = criado.Criticidade
        });

        var historico = await ObterUltimoHistoricoAsync(context, criado.Id);
        Assert.Equal(TipoMovimentacaoAtivo.TransferenciaLocal, historico.TipoMovimentacao);
        Assert.Equal(seed.LocalMatriz.Id, historico.LocalUnidadeOrigemId);
        Assert.Equal(seed.LocalFilial.Id, historico.LocalUnidadeDestinoId);
    }

    [Fact]
    public async Task RegistrarHistoricoNaEdicaoComMudancaDeResponsavel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "HIS-RES-001");

        await useCase.AtualizarAsync(criado.Id, new AtualizarInventarioAtivoRequest
        {
            Codigo = criado.Codigo,
            Nome = criado.Nome,
            TipoAtivoInventarioId = criado.TipoAtivoInventarioId,
            DepartamentoId = criado.DepartamentoId,
            LocalUnidadeId = criado.LocalUnidadeId,
            UsuarioResponsavelId = seed.ResponsavelB.Id,
            StatusOperacional = criado.StatusOperacional,
            StatusPatrimonial = criado.StatusPatrimonial,
            Criticidade = criado.Criticidade
        });

        var historico = await ObterUltimoHistoricoAsync(context, criado.Id);
        Assert.Equal(TipoMovimentacaoAtivo.AlteracaoResponsavel, historico.TipoMovimentacao);
        Assert.Equal(seed.ResponsavelA.Id, historico.UsuarioResponsavelOrigemId);
        Assert.Equal(seed.ResponsavelB.Id, historico.UsuarioResponsavelDestinoId);
    }

    [Fact]
    public async Task RegistrarHistoricoNaEdicaoComMudancaDeStatusOperacional()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "HIS-SOP-001");

        await useCase.AtualizarAsync(criado.Id, new AtualizarInventarioAtivoRequest
        {
            Codigo = criado.Codigo,
            Nome = criado.Nome,
            TipoAtivoInventarioId = criado.TipoAtivoInventarioId,
            DepartamentoId = criado.DepartamentoId,
            LocalUnidadeId = criado.LocalUnidadeId,
            UsuarioResponsavelId = criado.UsuarioResponsavelId,
            StatusOperacional = StatusOperacionalAtivo.ComDefeito,
            StatusPatrimonial = criado.StatusPatrimonial,
            Criticidade = criado.Criticidade
        });

        var historico = await ObterUltimoHistoricoAsync(context, criado.Id);
        Assert.Equal(TipoMovimentacaoAtivo.AlteracaoStatusOperacional, historico.TipoMovimentacao);
        Assert.Equal(StatusOperacionalAtivo.Operacional, historico.StatusOperacionalAnterior);
        Assert.Equal(StatusOperacionalAtivo.ComDefeito, historico.StatusOperacionalNovo);
    }

    [Fact]
    public async Task RegistrarHistoricoNaEdicaoComMudancaDeStatusPatrimonial()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "HIS-SPA-001");

        await useCase.AtualizarAsync(criado.Id, new AtualizarInventarioAtivoRequest
        {
            Codigo = criado.Codigo,
            Nome = criado.Nome,
            TipoAtivoInventarioId = criado.TipoAtivoInventarioId,
            DepartamentoId = criado.DepartamentoId,
            LocalUnidadeId = criado.LocalUnidadeId,
            UsuarioResponsavelId = criado.UsuarioResponsavelId,
            StatusOperacional = criado.StatusOperacional,
            StatusPatrimonial = StatusPatrimonialAtivo.EmTransferencia,
            Criticidade = criado.Criticidade
        });

        var historico = await ObterUltimoHistoricoAsync(context, criado.Id);
        Assert.Equal(TipoMovimentacaoAtivo.AlteracaoStatusPatrimonial, historico.TipoMovimentacao);
        Assert.Equal(StatusPatrimonialAtivo.EmUso, historico.StatusPatrimonialAnterior);
        Assert.Equal(StatusPatrimonialAtivo.EmTransferencia, historico.StatusPatrimonialNovo);
    }

    [Fact]
    public async Task NaoRegistrarHistoricoEmEdicaoSemMudancaRelevante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "HIS-SEM-001");

        var totalAntes = await context.HistoricosInventarioAtivo.CountAsync(x => x.InventarioAtivoId == criado.Id);

        await useCase.AtualizarAsync(criado.Id, new AtualizarInventarioAtivoRequest
        {
            Codigo = criado.Codigo,
            Nome = "Nome alterado sem mudanca relevante",
            TipoAtivoInventarioId = criado.TipoAtivoInventarioId,
            DepartamentoId = criado.DepartamentoId,
            LocalUnidadeId = criado.LocalUnidadeId,
            UsuarioResponsavelId = criado.UsuarioResponsavelId,
            StatusOperacional = criado.StatusOperacional,
            StatusPatrimonial = criado.StatusPatrimonial,
            Criticidade = criado.Criticidade
        });

        var totalDepois = await context.HistoricosInventarioAtivo.CountAsync(x => x.InventarioAtivoId == criado.Id);
        Assert.Equal(totalAntes, totalDepois);
    }

    [Fact]
    public async Task RegistrarHistoricoNaInativacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "HIS-INA-001");

        await useCase.InativarAsync(criado.Id);

        var historico = await ObterUltimoHistoricoAsync(context, criado.Id);
        Assert.Equal(TipoMovimentacaoAtivo.Inativacao, historico.TipoMovimentacao);
    }

    [Fact]
    public async Task RegistrarHistoricoNaReativacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "HIS-REA-001");
        await useCase.InativarAsync(criado.Id);

        await useCase.ReativarAsync(criado.Id);

        var historico = await ObterUltimoHistoricoAsync(context, criado.Id);
        Assert.Equal(TipoMovimentacaoAtivo.Reativacao, historico.TipoMovimentacao);
    }

    [Fact]
    public async Task ListarHistoricoDoAtivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "HIS-LST-001");

        await useCase.MovimentarAsync(criado.Id, new MovimentarInventarioAtivoRequest
        {
            DepartamentoId = seed.DepartamentoRh.Id,
            Observacao = "Movimentacao para teste de listagem"
        });

        var historico = await useCase.ListarHistoricoAsync(criado.Id, new FiltroHistoricoInventarioAtivoRequest());
        Assert.True(historico.Total >= 2);
        Assert.Equal(criado.Id, historico.Items.First().InventarioAtivoId);
    }

    [Fact]
    public async Task MovimentarAtivoAlterandoDepartamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "MOV-DEP-001");

        var response = await useCase.MovimentarAsync(criado.Id, new MovimentarInventarioAtivoRequest
        {
            DepartamentoId = seed.DepartamentoRh.Id,
            Observacao = "Transferencia de departamento"
        });

        Assert.Equal(seed.DepartamentoRh.Id, response.DepartamentoId);
        var historico = await ObterUltimoHistoricoAsync(context, criado.Id);
        Assert.Equal(TipoMovimentacaoAtivo.TransferenciaDepartamento, historico.TipoMovimentacao);
    }

    [Fact]
    public async Task MovimentarAtivoAlterandoLocal()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "MOV-LOC-001");

        var response = await useCase.MovimentarAsync(criado.Id, new MovimentarInventarioAtivoRequest
        {
            LocalUnidadeId = seed.LocalFilial.Id,
            Observacao = "Transferencia de local"
        });

        Assert.Equal(seed.LocalFilial.Id, response.LocalUnidadeId);
        var historico = await ObterUltimoHistoricoAsync(context, criado.Id);
        Assert.Equal(TipoMovimentacaoAtivo.TransferenciaLocal, historico.TipoMovimentacao);
    }

    [Fact]
    public async Task MovimentarAtivoAlterandoResponsavel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "MOV-RES-001");

        var response = await useCase.MovimentarAsync(criado.Id, new MovimentarInventarioAtivoRequest
        {
            UsuarioResponsavelId = seed.ResponsavelB.Id,
            Observacao = "Alteracao de responsavel"
        });

        Assert.Equal(seed.ResponsavelB.Id, response.UsuarioResponsavelId);
        var historico = await ObterUltimoHistoricoAsync(context, criado.Id);
        Assert.Equal(TipoMovimentacaoAtivo.AlteracaoResponsavel, historico.TipoMovimentacao);
    }

    [Fact]
    public async Task MovimentarAtivoAlterandoStatusOperacional()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "MOV-SOP-001");

        var response = await useCase.MovimentarAsync(criado.Id, new MovimentarInventarioAtivoRequest
        {
            StatusOperacional = StatusOperacionalAtivo.EmManutencao,
            Observacao = "Entrada em manutencao"
        });

        Assert.Equal(StatusOperacionalAtivo.EmManutencao, response.StatusOperacional);
        var historico = await ObterUltimoHistoricoAsync(context, criado.Id);
        Assert.Equal(TipoMovimentacaoAtivo.Manutencao, historico.TipoMovimentacao);
    }

    [Fact]
    public async Task MovimentarAtivoAlterandoStatusPatrimonial()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "MOV-SPA-001");

        var response = await useCase.MovimentarAsync(criado.Id, new MovimentarInventarioAtivoRequest
        {
            StatusPatrimonial = StatusPatrimonialAtivo.EmTransferencia,
            Observacao = "Em transferencia"
        });

        Assert.Equal(StatusPatrimonialAtivo.EmTransferencia, response.StatusPatrimonial);
        var historico = await ObterUltimoHistoricoAsync(context, criado.Id);
        Assert.Equal(TipoMovimentacaoAtivo.AlteracaoStatusPatrimonial, historico.TipoMovimentacao);
    }

    [Fact]
    public async Task ImpedirMovimentacaoDeAtivoInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "MOV-INA-001");
        await useCase.InativarAsync(criado.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.MovimentarAsync(criado.Id, new MovimentarInventarioAtivoRequest
        {
            DepartamentoId = seed.DepartamentoRh.Id
        }));
    }

    [Fact]
    public async Task ImpedirMovimentacaoVazia()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var criado = await CriarAtivoBaseAsync(useCase, seed, "MOV-VAZ-001");

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.MovimentarAsync(criado.Id, new MovimentarInventarioAtivoRequest
        {
            DepartamentoId = criado.DepartamentoId
        }));
    }

    private static async Task<HistoricoInventarioAtivo> ObterUltimoHistoricoAsync(SGXSistemaChamadoDbContext context, Guid ativoId)
        => await context.HistoricosInventarioAtivo
            .AsNoTracking()
            .Where(x => x.InventarioAtivoId == ativoId)
            .OrderByDescending(x => x.CriadoEm)
            .FirstAsync();

    private static Task<InventarioAtivoDetalheDto> CriarAtivoBaseAsync(
        InventarioAtivosAdminUseCases useCase,
        SeedInventarioContexto seed,
        string codigo)
        => useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = codigo,
            Nome = $"Ativo {codigo}",
            TipoAtivoInventarioId = seed.TipoNotebook.Id,
            DepartamentoId = seed.DepartamentoTi.Id,
            LocalUnidadeId = seed.LocalMatriz.Id,
            UsuarioResponsavelId = seed.ResponsavelA.Id,
            StatusOperacional = StatusOperacionalAtivo.Operacional,
            StatusPatrimonial = StatusPatrimonialAtivo.EmUso,
            Criticidade = CriticidadeAtivo.Media
        });

    private static InventarioAtivosAdminUseCases CriarUseCase(
        SGXSistemaChamadoDbContext context,
        Usuario admin,
        FakeAuditoriaService? auditoria = null)
        => new(
            PortalUseCasesTestFactory.Repo<InventarioAtivo>(context),
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoInventarioAtivo>(context),
            PortalUseCasesTestFactory.Repo<TipoAtivoInventario>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);

    private static async Task CriarAtivosParaFiltroAsync(InventarioAtivosAdminUseCases useCase, SeedInventarioContexto seed)
    {
        await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "AT-NB-001",
            Nome = "Notebook operacional",
            TipoAtivoInventarioId = seed.TipoNotebook.Id,
            DepartamentoId = seed.DepartamentoTi.Id,
            LocalUnidadeId = seed.LocalMatriz.Id,
            UsuarioResponsavelId = seed.ResponsavelA.Id,
            StatusOperacional = StatusOperacionalAtivo.Operacional,
            StatusPatrimonial = StatusPatrimonialAtivo.EmUso,
            Criticidade = CriticidadeAtivo.Media
        });

        await useCase.CriarAsync(new CriarInventarioAtivoRequest
        {
            Codigo = "AT-MON-001",
            Nome = "Monitor almoxarifado",
            TipoAtivoInventarioId = seed.TipoMonitor.Id,
            DepartamentoId = seed.DepartamentoRh.Id,
            LocalUnidadeId = seed.LocalFilial.Id,
            UsuarioResponsavelId = seed.ResponsavelB.Id,
            StatusOperacional = StatusOperacionalAtivo.EmManutencao,
            StatusPatrimonial = StatusPatrimonialAtivo.EmEstoque,
            Criticidade = CriticidadeAtivo.Critica
        });
    }

    private static async Task<SeedInventarioContexto> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Admin Inventario",
            $"admin.inventario.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Administrador);

        var responsavelA = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Responsavel A",
            $"responsavel.a.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Atendente);

        var responsavelB = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Responsavel B",
            $"responsavel.b.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Atendente);

        var departamentoTi = new Departamento("Tecnologia", "TI", null, "teste");
        var departamentoRh = new Departamento("Recursos Humanos", "RH", null, "teste");
        context.Departamentos.AddRange(departamentoTi, departamentoRh);

        var localMatriz = new LocalUnidade("Matriz", null, null, "teste");
        var localFilial = new LocalUnidade("Filial", null, null, "teste");
        context.LocaisUnidade.AddRange(localMatriz, localFilial);

        var tipoNotebook = new TipoAtivoInventario("Notebook Teste", "Tipo para testes", "teste");
        var tipoMonitor = new TipoAtivoInventario("Monitor Teste", "Tipo para testes", "teste");
        context.TiposAtivoInventario.AddRange(tipoNotebook, tipoMonitor);

        await context.SaveChangesAsync();

        return new SeedInventarioContexto(
            admin,
            responsavelA,
            responsavelB,
            departamentoTi,
            departamentoRh,
            localMatriz,
            localFilial,
            tipoNotebook,
            tipoMonitor);
    }

    private sealed record SeedInventarioContexto(
        Usuario Admin,
        Usuario ResponsavelA,
        Usuario ResponsavelB,
        Departamento DepartamentoTi,
        Departamento DepartamentoRh,
        LocalUnidade LocalMatriz,
        LocalUnidade LocalFilial,
        TipoAtivoInventario TipoNotebook,
        TipoAtivoInventario TipoMonitor);
}
