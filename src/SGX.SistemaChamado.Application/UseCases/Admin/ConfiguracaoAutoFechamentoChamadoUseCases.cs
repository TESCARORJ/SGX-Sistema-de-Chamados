using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using System.Text.Json;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ObterConfiguracaoAutoFechamentoChamadoUseCase(
    IRepository<ParametroSistema> parametroRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterConfiguracaoAutoFechamentoChamadoUseCase
{
    public async Task<ObterConfiguracaoAutoFechamentoChamadoResponse> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var parametro = await parametroRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Chave == ConfiguracaoAutoFechamentoChamadoConstantes.ChaveParametroPrazoAceiteHoras,
                cancellationToken);

        return MapObter(parametro);
    }

    internal static ObterConfiguracaoAutoFechamentoChamadoResponse MapObter(ParametroSistema? parametro)
    {
        var prazo = ResolverPrazo(parametro);
        return new ObterConfiguracaoAutoFechamentoChamadoResponse(
            PrazoAutoFechamentoHoras: prazo,
            PrazoAutoFechamentoDias: Math.Round(prazo / 24m, 2),
            Ativo: parametro?.Ativo ?? true,
            CriadoEm: parametro?.CriadoEm ?? DateTime.UtcNow,
            CriadoPor: parametro?.CriadoPor ?? "seed.sistema",
            AtualizadoEm: parametro?.AtualizadoEm,
            AtualizadoPor: parametro?.AtualizadoPor);
    }

    internal static int ResolverPrazo(ParametroSistema? parametro)
    {
        if (parametro is null || string.IsNullOrWhiteSpace(parametro.Valor))
        {
            return ConfiguracaoAutoFechamentoChamadoConstantes.PrazoPadraoHoras;
        }

        if (!int.TryParse(parametro.Valor, out var prazo))
        {
            throw new InvalidOperationException("A configuracao administrativa do prazo de auto-fechamento esta invalida.");
        }

        ValidarPrazo(prazo, "configuracao administrativa");
        return prazo;
    }

    internal static void ValidarPrazo(int prazo, string origem)
    {
        if (prazo < ConfiguracaoAutoFechamentoChamadoConstantes.PrazoMinimoHoras ||
            prazo > ConfiguracaoAutoFechamentoChamadoConstantes.PrazoMaximoHoras)
        {
            throw new InvalidOperationException(
                $"O prazo de auto-fechamento informado na {origem} deve estar entre {ConfiguracaoAutoFechamentoChamadoConstantes.PrazoMinimoHoras} e {ConfiguracaoAutoFechamentoChamadoConstantes.PrazoMaximoHoras} horas.");
        }
    }
}

public sealed class AtualizarConfiguracaoAutoFechamentoChamadoUseCase(
    IRepository<ParametroSistema> parametroRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAtualizarConfiguracaoAutoFechamentoChamadoUseCase
{
    public async Task<AtualizarConfiguracaoAutoFechamentoChamadoResponse> ExecutarAsync(
        AtualizarConfiguracaoAutoFechamentoChamadoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        ObterConfiguracaoAutoFechamentoChamadoUseCase.ValidarPrazo(
            request.PrazoAutoFechamentoHoras,
            "atualizacao administrativa");

        var parametro = await parametroRepository.Query()
            .FirstOrDefaultAsync(
                x => x.Chave == ConfiguracaoAutoFechamentoChamadoConstantes.ChaveParametroPrazoAceiteHoras,
                cancellationToken);

        var dadosAntes = parametro is null
            ? null
            : JsonSerializer.Serialize(new
            {
                parametro.Chave,
                parametro.Valor,
                parametro.Ativo,
                parametro.AtualizadoEm,
                parametro.AtualizadoPor
            });

        if (parametro is null)
        {
            parametro = new ParametroSistema(
                ConfiguracaoAutoFechamentoChamadoConstantes.ChaveParametroPrazoAceiteHoras,
                request.PrazoAutoFechamentoHoras.ToString(),
                ConfiguracaoAutoFechamentoChamadoConstantes.DescricaoParametroPrazoAceiteHoras,
                false,
                usuarioAtual.Login);

            await parametroRepository.AddAsync(parametro, cancellationToken);
        }
        else
        {
            parametro.Ativar(usuarioAtual.Login);
            parametro.AtualizarValor(request.PrazoAutoFechamentoHoras.ToString(), usuarioAtual.Login);
            parametro.DefinirDescricao(
                ConfiguracaoAutoFechamentoChamadoConstantes.DescricaoParametroPrazoAceiteHoras,
                usuarioAtual.Login);
            parametro.DefinirSensivel(false, usuarioAtual.Login);
            parametroRepository.Update(parametro);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dadosDepois = JsonSerializer.Serialize(new
        {
            parametro.Chave,
            parametro.Valor,
            parametro.Ativo,
            parametro.AtualizadoEm,
            parametro.AtualizadoPor,
            request.ObservacaoAlteracao
        });

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                modulo: "Configuracoes",
                entidade: "ParametroSistema",
                entidadeId: parametro.Id.ToString(),
                descricao: "Configuracao administrativa do prazo de auto-fechamento de chamados atualizada.",
                dadosAntes: dadosAntes,
                dadosDepois: dadosDepois,
                metadados: JsonSerializer.Serialize(new
                {
                    parametro.Chave,
                    request.PrazoAutoFechamentoHoras,
                    request.ObservacaoAlteracao,
                    UsuarioAdministrador = usuarioAtual.Login
                }),
                cancellationToken: cancellationToken);
        }

        return new AtualizarConfiguracaoAutoFechamentoChamadoResponse(
            PrazoAutoFechamentoHoras: request.PrazoAutoFechamentoHoras,
            PrazoAutoFechamentoDias: Math.Round(request.PrazoAutoFechamentoHoras / 24m, 2),
            Ativo: parametro.Ativo,
            CriadoEm: parametro.CriadoEm,
            CriadoPor: parametro.CriadoPor,
            AtualizadoEm: parametro.AtualizadoEm,
            AtualizadoPor: parametro.AtualizadoPor,
            ObservacaoAlteracao: string.IsNullOrWhiteSpace(request.ObservacaoAlteracao) ? null : request.ObservacaoAlteracao.Trim());
    }
}
