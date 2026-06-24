using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Services;

public sealed class AcoesChamadoService(IFluxoStatusChamadoService fluxoStatusChamadoService) : IAcoesChamadoService
{
    private const string PermissaoChamadosAssumir = "Chamados.Assumir";
    private const string PermissaoChamadosAtribuir = "Chamados.Atribuir";
    private const string PermissaoChamadosAlterarStatus = "Chamados.AlterarStatus";
    private const string PermissaoChamadosAlterarPrioridade = "Chamados.AlterarPrioridade";
    private const string PermissaoChamadosAlterarCategoria = "Chamados.AlterarCategoria";
    private const string PermissaoChamadosEncerrar = "Chamados.Encerrar";
    private const string PermissaoChamadosReabrir = "Chamados.Reabrir";
    private const string PermissaoChamadosComentar = "Chamados.Comentar";
    private const string PermissaoChamadosAnexar = "Chamados.Anexar";

    public IReadOnlyCollection<AcaoChamadoEnum> ObterAcoesDisponiveis(Chamado chamado, UsuarioContextoAplicacao usuario)
    {
        ArgumentNullException.ThrowIfNull(chamado);
        ArgumentNullException.ThrowIfNull(usuario);

        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            return [];
        }

        var acoes = new HashSet<AcaoChamadoEnum>();
        var statusAtual = chamado.Status.Codigo;
        var chamadoEmStatusFinal = chamado.Status.EhStatusFinal
            || statusAtual is StatusChamadoEnum.Encerrado
                or StatusChamadoEnum.Cancelado
                or StatusChamadoEnum.Resolvido
                or StatusChamadoEnum.Concluida
                or StatusChamadoEnum.Reprovada
                or StatusChamadoEnum.Tratado;
        var chamadoReabrivel = chamadoEmStatusFinal;
        var statusPermitidos = fluxoStatusChamadoService.ObterStatusPermitidos(chamado.NaturezaChamado);
        var statusAtualPermitidoPelaNatureza = statusPermitidos.Contains(statusAtual);
        var estadoAprovacao = AprovacaoChamadoHelper.ObterEstado(chamado);

        var podeAlterarStatus = !chamadoEmStatusFinal
            && statusAtualPermitidoPelaNatureza
            && PodeExecutarComPermissaoOuPerfil(usuario, PermissaoChamadosAlterarStatus);
        var podeResolver = !chamadoEmStatusFinal
            && statusAtualPermitidoPelaNatureza
            && PodeExecutarComPermissaoOuPerfil(usuario, PermissaoChamadosAlterarStatus);
        var podeEncerrar = !chamadoEmStatusFinal
            && PodeExecutarComPermissaoOuPerfil(usuario, PermissaoChamadosEncerrar);
        var podeReabrir = chamadoReabrivel
            && PodeExecutarComPermissaoOuPerfil(usuario, PermissaoChamadosReabrir);
        var podeComentar = !chamadoEmStatusFinal
            && PodeExecutarComPermissaoOuPerfil(usuario, PermissaoChamadosComentar);
        var podeAnexar = !chamadoEmStatusFinal
            && PodeExecutarComPermissaoOuPerfil(usuario, PermissaoChamadosAnexar);
        var podeAssumir = !chamadoEmStatusFinal
            && !estadoAprovacao.BloqueiaAvancoAtendimento
            && PodeExecutarComPermissaoOuPerfil(usuario, PermissaoChamadosAssumir)
            && (AdminUseCaseHelpers.EhAdministrador(usuario) || !chamado.ResponsavelId.HasValue || chamado.ResponsavelId == usuario.Id);
        var podeAtribuir = !chamadoEmStatusFinal
            && AdminUseCaseHelpers.EhAdministrador(usuario)
            && PodeExecutarComPermissaoOuPerfil(usuario, PermissaoChamadosAtribuir);
        var podeCancelar = !chamadoEmStatusFinal;

        if (podeAssumir)
        {
            acoes.Add(AcaoChamadoEnum.Assumir);
        }

        if (podeAtribuir)
        {
            acoes.Add(AcaoChamadoEnum.Atribuir);
        }

        if (podeCancelar)
        {
            acoes.Add(AcaoChamadoEnum.Cancelar);
        }

        if (podeComentar)
        {
            acoes.Add(AcaoChamadoEnum.Comentar);
        }

        if (podeAnexar)
        {
            acoes.Add(AcaoChamadoEnum.Anexar);
        }

        if (podeAlterarStatus)
        {
            acoes.Add(AcaoChamadoEnum.AlterarStatus);
        }

        if (podeResolver)
        {
            acoes.Add(AcaoChamadoEnum.Resolver);
        }

        if (podeEncerrar)
        {
            acoes.Add(AcaoChamadoEnum.Encerrar);
        }

        if (podeReabrir)
        {
            acoes.Add(AcaoChamadoEnum.Reabrir);
        }

        if (NaturezaPermiteAlterarPrioridadeECategoria(chamado.NaturezaChamado))
        {
            if (!chamadoEmStatusFinal && PodeExecutarComPermissaoOuPerfil(usuario, PermissaoChamadosAlterarPrioridade))
            {
                acoes.Add(AcaoChamadoEnum.AlterarPrioridade);
            }

            if (!chamadoEmStatusFinal && PodeExecutarComPermissaoOuPerfil(usuario, PermissaoChamadosAlterarCategoria))
            {
                acoes.Add(AcaoChamadoEnum.AlterarCategoria);
            }
        }

        return acoes.OrderBy(x => (int)x).ToArray();
    }

    public bool AcaoEstaDisponivel(Chamado chamado, AcaoChamadoEnum acao, UsuarioContextoAplicacao usuario)
        => ObterAcoesDisponiveis(chamado, usuario).Contains(acao);

    public void ValidarAcaoDisponivel(Chamado chamado, AcaoChamadoEnum acao, UsuarioContextoAplicacao usuario)
    {
        var estadoAprovacao = AprovacaoChamadoHelper.ObterEstado(chamado);
        if (estadoAprovacao.BloqueiaAvancoAtendimento
            && acao is AcaoChamadoEnum.Assumir
                or AcaoChamadoEnum.Reabrir)
        {
            throw new InvalidOperationException(estadoAprovacao.MensagemBloqueio ?? AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente);
        }

        if (AcaoEstaDisponivel(chamado, acao, usuario))
        {
            return;
        }

        throw new InvalidOperationException($"A acao '{acao}' nao esta disponivel para o chamado no estado atual.");
    }

    private static bool NaturezaPermiteAlterarPrioridadeECategoria(NaturezaChamadoEnum natureza)
        => natureza is NaturezaChamadoEnum.Incidente
            or NaturezaChamadoEnum.Requisicao
            or NaturezaChamadoEnum.Mudanca
            or NaturezaChamadoEnum.Problema;

    private static bool PodeExecutarComPermissaoOuPerfil(UsuarioContextoAplicacao usuario, string permissao)
        => AdminUseCaseHelpers.EhAdministrador(usuario) || usuario.PossuiPermissao(permissao);
}
