using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class FormularioServicoAdminUseCases(
    IRepository<CatalogoServico> catalogoServicoRepository,
    IRepository<FormularioServico> formularioServicoRepository,
    IRepository<FormularioServicoVersao> formularioServicoVersaoRepository,
    IRepository<CampoFormularioServico> campoFormularioServicoRepository,
    IRepository<OpcaoCampoFormularioServico> opcaoCampoFormularioServicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAdminFormularioServicosUseCases
{
    private static readonly CriarFormularioServicoRequestValidator CriarFormularioValidator = new();
    private static readonly AtualizarFormularioServicoRequestValidator AtualizarFormularioValidator = new();
    private static readonly CriarFormularioServicoVersaoRequestValidator CriarVersaoValidator = new();
    private static readonly AtualizarFormularioServicoVersaoRequestValidator AtualizarVersaoValidator = new();
    private static readonly CriarCampoFormularioServicoRequestValidator CriarCampoValidator = new();
    private static readonly AtualizarCampoFormularioServicoRequestValidator AtualizarCampoValidator = new();
    private static readonly CriarOpcaoCampoFormularioServicoRequestValidator CriarOpcaoValidator = new();
    private static readonly AtualizarOpcaoCampoFormularioServicoRequestValidator AtualizarOpcaoValidator = new();

    public async Task<IReadOnlyCollection<FormularioServicoAdminDto>> ListarAsync(
        Guid? catalogoServicoId = null,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = formularioServicoRepository.Query()
            .AsNoTracking()
            .AsQueryable();

        if (catalogoServicoId.HasValue)
        {
            query = query.Where(x => x.CatalogoServicoId == catalogoServicoId.Value);
        }

        var formularios = await query
            .OrderBy(x => x.Nome)
            .ToListAsync(cancellationToken);

        return formularios
            .Select(FormularioServicoAdminMapeamentos.MapFormulario)
            .ToArray();
    }

    public async Task<FormularioServicoDetalheAdminDto> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var formulario = await ObterFormularioCompletoPorIdAsync(id, true, cancellationToken)
            ?? throw new KeyNotFoundException("Formulario do servico nao encontrado.");

        return FormularioServicoAdminMapeamentos.MapDetalhe(formulario);
    }

    public async Task<FormularioServicoDetalheAdminDto> CriarAsync(
        CriarFormularioServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        await ValidarOuFalharAsync(CriarFormularioValidator, request, cancellationToken);

        await ValidarCatalogoExisteAsync(request.CatalogoServicoId, cancellationToken);
        await ValidarDuplicidadeFormularioAsync(request.CatalogoServicoId, null, cancellationToken);

        var formulario = new FormularioServico(
            request.CatalogoServicoId,
            request.Nome,
            request.Descricao,
            usuarioAtual.Login);

        if (!request.Ativo)
        {
            formulario.Inativar(usuarioAtual.Login);
        }

        await formularioServicoRepository.AddAsync(formulario, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await ObterFormularioCompletoPorIdAsync(formulario.Id, true, cancellationToken)
            ?? throw new KeyNotFoundException("Formulario do servico nao encontrado apos criacao.");

        return FormularioServicoAdminMapeamentos.MapDetalhe(completo);
    }

    public async Task<FormularioServicoDetalheAdminDto> AtualizarAsync(
        Guid id,
        AtualizarFormularioServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        await ValidarOuFalharAsync(AtualizarFormularioValidator, request, cancellationToken);

        var formulario = await ObterFormularioCompletoPorIdAsync(id, false, cancellationToken)
            ?? throw new KeyNotFoundException("Formulario do servico nao encontrado.");

        formulario.AlterarDados(request.Nome, request.Descricao, usuarioAtual.Login);

        if (!request.Ativo && formulario.Ativo)
        {
            formulario.Inativar(usuarioAtual.Login);
        }
        else if (request.Ativo && !formulario.Ativo)
        {
            formulario.Reativar(usuarioAtual.Login);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await ObterFormularioCompletoPorIdAsync(formulario.Id, true, cancellationToken)
            ?? throw new KeyNotFoundException("Formulario do servico nao encontrado apos atualizacao.");

        return FormularioServicoAdminMapeamentos.MapDetalhe(completo);
    }

    public async Task<AlterarSituacaoCadastroResponse> InativarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var formulario = await ObterFormularioCompletoPorIdAsync(id, false, cancellationToken)
            ?? throw new KeyNotFoundException("Formulario do servico nao encontrado.");

        if (!formulario.Ativo)
        {
            return new AlterarSituacaoCadastroResponse(formulario.Id, false, "Formulario do servico ja estava inativo.");
        }

        formulario.Inativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AlterarSituacaoCadastroResponse(formulario.Id, false, "Formulario do servico inativado com sucesso.");
    }

    public async Task<AlterarSituacaoCadastroResponse> ReativarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var formulario = await ObterFormularioCompletoPorIdAsync(id, false, cancellationToken)
            ?? throw new KeyNotFoundException("Formulario do servico nao encontrado.");

        if (formulario.Ativo)
        {
            return new AlterarSituacaoCadastroResponse(formulario.Id, true, "Formulario do servico ja estava ativo.");
        }

        await ValidarDuplicidadeFormularioAsync(formulario.CatalogoServicoId, formulario.Id, cancellationToken);
        formulario.Reativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AlterarSituacaoCadastroResponse(formulario.Id, true, "Formulario do servico reativado com sucesso.");
    }

    public async Task<IReadOnlyCollection<FormularioServicoVersaoAdminDto>> ListarVersoesAsync(
        Guid formularioServicoId,
        CancellationToken cancellationToken = default)
    {
        if (formularioServicoId == Guid.Empty)
        {
            throw new ArgumentException("FormularioServicoId invalido.", nameof(formularioServicoId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        await ValidarFormularioExisteAsync(formularioServicoId, cancellationToken);

        var versoes = await formularioServicoVersaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.FormularioServicoId == formularioServicoId)
            .Include(x => x.Campos)
            .ThenInclude(x => x.Opcoes)
            .OrderBy(x => x.Numero)
            .ToListAsync(cancellationToken);

        return versoes
            .Select(FormularioServicoAdminMapeamentos.MapVersao)
            .ToArray();
    }

    public async Task<FormularioServicoVersaoAdminDto> CriarVersaoAsync(
        CriarFormularioServicoVersaoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        await ValidarOuFalharAsync(CriarVersaoValidator, request, cancellationToken);
        await ValidarFormularioExisteAsync(request.FormularioServicoId, cancellationToken);
        await ValidarDuplicidadeVersaoAsync(request.FormularioServicoId, request.Numero, null, cancellationToken);

        var versao = new FormularioServicoVersao(
            request.FormularioServicoId,
            request.Numero,
            request.Publicada,
            request.PublicadoEm,
            usuarioAtual.Login);

        if (!request.Ativo)
        {
            versao.Inativar(usuarioAtual.Login);
        }

        await formularioServicoVersaoRepository.AddAsync(versao, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completa = await ObterVersaoPorIdAsync(versao.Id, true, cancellationToken)
            ?? throw new KeyNotFoundException("Versao do formulario nao encontrada apos criacao.");

        return FormularioServicoAdminMapeamentos.MapVersao(completa);
    }

    public async Task<FormularioServicoVersaoAdminDto> AtualizarVersaoAsync(
        Guid id,
        AtualizarFormularioServicoVersaoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        await ValidarOuFalharAsync(AtualizarVersaoValidator, request, cancellationToken);

        var versao = await ObterVersaoPorIdAsync(id, false, cancellationToken)
            ?? throw new KeyNotFoundException("Versao do formulario nao encontrada.");

        await ValidarDuplicidadeVersaoAsync(versao.FormularioServicoId, request.Numero, versao.Id, cancellationToken);

        versao.AlterarDados(request.Numero, request.Publicada, request.PublicadoEm, usuarioAtual.Login);

        if (!request.Ativo && versao.Ativo)
        {
            versao.Inativar(usuarioAtual.Login);
        }
        else if (request.Ativo && !versao.Ativo)
        {
            versao.Reativar(usuarioAtual.Login);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completa = await ObterVersaoPorIdAsync(versao.Id, true, cancellationToken)
            ?? throw new KeyNotFoundException("Versao do formulario nao encontrada apos atualizacao.");

        return FormularioServicoAdminMapeamentos.MapVersao(completa);
    }

    public async Task<AlterarSituacaoCadastroResponse> InativarVersaoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var versao = await ValidarEObterVersaoParaAlteracaoStatusAsync(id, cancellationToken);

        if (!versao.Ativo)
        {
            return new AlterarSituacaoCadastroResponse(versao.Id, false, "Versao do formulario ja estava inativa.");
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        versao.Inativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(versao.Id, false, "Versao do formulario inativada com sucesso.");
    }

    public async Task<AlterarSituacaoCadastroResponse> ReativarVersaoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var versao = await ValidarEObterVersaoParaAlteracaoStatusAsync(id, cancellationToken);

        if (versao.Ativo)
        {
            return new AlterarSituacaoCadastroResponse(versao.Id, true, "Versao do formulario ja estava ativa.");
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        versao.Reativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(versao.Id, true, "Versao do formulario reativada com sucesso.");
    }

    public async Task<IReadOnlyCollection<CampoFormularioServicoAdminDto>> ListarCamposAsync(
        Guid formularioServicoVersaoId,
        CancellationToken cancellationToken = default)
    {
        if (formularioServicoVersaoId == Guid.Empty)
        {
            throw new ArgumentException("FormularioServicoVersaoId invalido.", nameof(formularioServicoVersaoId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        await ValidarVersaoExisteAsync(formularioServicoVersaoId, cancellationToken);

        var campos = await campoFormularioServicoRepository.Query()
            .AsNoTracking()
            .Where(x => x.FormularioServicoVersaoId == formularioServicoVersaoId)
            .Include(x => x.Opcoes)
            .OrderBy(x => x.Ordem)
            .ThenBy(x => x.Nome)
            .ToListAsync(cancellationToken);

        return campos
            .Select(FormularioServicoAdminMapeamentos.MapCampo)
            .ToArray();
    }

    public async Task<CampoFormularioServicoAdminDto> CriarCampoAsync(
        CriarCampoFormularioServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        await ValidarOuFalharAsync(CriarCampoValidator, request, cancellationToken);
        await ValidarVersaoExisteAsync(request.FormularioServicoVersaoId, cancellationToken);
        await ValidarDuplicidadeCampoAsync(request.FormularioServicoVersaoId, request.Nome, request.Ordem, null, cancellationToken);

        var campo = new CampoFormularioServico(
            request.FormularioServicoVersaoId,
            request.Nome,
            request.Rotulo,
            request.Tipo,
            request.Obrigatorio,
            request.Ordem,
            request.TextoAjuda,
            request.Visivel,
            usuarioAtual.Login);

        if (!request.Ativo)
        {
            campo.Inativar(usuarioAtual.Login);
        }

        await campoFormularioServicoRepository.AddAsync(campo, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await ObterCampoPorIdAsync(campo.Id, true, cancellationToken)
            ?? throw new KeyNotFoundException("Campo do formulario nao encontrado apos criacao.");

        return FormularioServicoAdminMapeamentos.MapCampo(completo);
    }

    public async Task<CampoFormularioServicoAdminDto> AtualizarCampoAsync(
        Guid id,
        AtualizarCampoFormularioServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        await ValidarOuFalharAsync(AtualizarCampoValidator, request, cancellationToken);

        var campo = await ObterCampoPorIdAsync(id, false, cancellationToken)
            ?? throw new KeyNotFoundException("Campo do formulario nao encontrado.");

        await ValidarDuplicidadeCampoAsync(campo.FormularioServicoVersaoId, request.Nome, request.Ordem, campo.Id, cancellationToken);

        campo.AlterarDados(
            request.Nome,
            request.Rotulo,
            request.Tipo,
            request.Obrigatorio,
            request.Ordem,
            request.TextoAjuda,
            request.Visivel,
            usuarioAtual.Login);

        if (!request.Ativo && campo.Ativo)
        {
            campo.Inativar(usuarioAtual.Login);
        }
        else if (request.Ativo && !campo.Ativo)
        {
            campo.Reativar(usuarioAtual.Login);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await ObterCampoPorIdAsync(campo.Id, true, cancellationToken)
            ?? throw new KeyNotFoundException("Campo do formulario nao encontrado apos atualizacao.");

        return FormularioServicoAdminMapeamentos.MapCampo(completo);
    }

    public async Task<AlterarSituacaoCadastroResponse> InativarCampoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var campo = await ValidarEObterCampoParaAlteracaoStatusAsync(id, cancellationToken);

        if (!campo.Ativo)
        {
            return new AlterarSituacaoCadastroResponse(campo.Id, false, "Campo do formulario ja estava inativo.");
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        campo.Inativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(campo.Id, false, "Campo do formulario inativado com sucesso.");
    }

    public async Task<AlterarSituacaoCadastroResponse> ReativarCampoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var campo = await ValidarEObterCampoParaAlteracaoStatusAsync(id, cancellationToken);

        if (campo.Ativo)
        {
            return new AlterarSituacaoCadastroResponse(campo.Id, true, "Campo do formulario ja estava ativo.");
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        campo.Reativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(campo.Id, true, "Campo do formulario reativado com sucesso.");
    }

    public async Task<IReadOnlyCollection<OpcaoCampoFormularioServicoAdminDto>> ListarOpcoesAsync(
        Guid campoFormularioServicoId,
        CancellationToken cancellationToken = default)
    {
        if (campoFormularioServicoId == Guid.Empty)
        {
            throw new ArgumentException("CampoFormularioServicoId invalido.", nameof(campoFormularioServicoId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        await ValidarCampoExisteAsync(campoFormularioServicoId, cancellationToken);

        var opcoes = await opcaoCampoFormularioServicoRepository.Query()
            .AsNoTracking()
            .Where(x => x.CampoFormularioServicoId == campoFormularioServicoId)
            .OrderBy(x => x.Ordem)
            .ThenBy(x => x.Valor)
            .ToListAsync(cancellationToken);

        return opcoes
            .Select(FormularioServicoAdminMapeamentos.MapOpcao)
            .ToArray();
    }

    public async Task<OpcaoCampoFormularioServicoAdminDto> CriarOpcaoAsync(
        CriarOpcaoCampoFormularioServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        await ValidarOuFalharAsync(CriarOpcaoValidator, request, cancellationToken);

        var campo = await ObterCampoPorIdAsync(request.CampoFormularioServicoId, false, cancellationToken)
            ?? throw new KeyNotFoundException("Campo do formulario nao encontrado.");

        ValidarCampoEnumerado(campo);
        await ValidarDuplicidadeOpcaoAsync(campo.Id, request.Valor, request.Ordem, null, cancellationToken);

        var opcao = new OpcaoCampoFormularioServico(
            campo.Id,
            request.Valor,
            request.Rotulo,
            request.Ordem,
            usuarioAtual.Login);

        if (!request.Ativo)
        {
            opcao.Inativar(usuarioAtual.Login);
        }

        await opcaoCampoFormularioServicoRepository.AddAsync(opcao, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completa = await ObterOpcaoPorIdAsync(opcao.Id, true, cancellationToken)
            ?? throw new KeyNotFoundException("Opcao do campo nao encontrada apos criacao.");

        return FormularioServicoAdminMapeamentos.MapOpcao(completa);
    }

    public async Task<OpcaoCampoFormularioServicoAdminDto> AtualizarOpcaoAsync(
        Guid id,
        AtualizarOpcaoCampoFormularioServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        await ValidarOuFalharAsync(AtualizarOpcaoValidator, request, cancellationToken);

        var opcao = await ObterOpcaoPorIdAsync(id, false, cancellationToken)
            ?? throw new KeyNotFoundException("Opcao do campo nao encontrada.");

        var campo = await ObterCampoPorIdAsync(opcao.CampoFormularioServicoId, false, cancellationToken)
            ?? throw new KeyNotFoundException("Campo do formulario nao encontrado.");

        ValidarCampoEnumerado(campo);
        await ValidarDuplicidadeOpcaoAsync(opcao.CampoFormularioServicoId, request.Valor, request.Ordem, opcao.Id, cancellationToken);

        opcao.AlterarDados(request.Valor, request.Rotulo, request.Ordem, usuarioAtual.Login);

        if (!request.Ativo && opcao.Ativo)
        {
            opcao.Inativar(usuarioAtual.Login);
        }
        else if (request.Ativo && !opcao.Ativo)
        {
            opcao.Reativar(usuarioAtual.Login);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completa = await ObterOpcaoPorIdAsync(opcao.Id, true, cancellationToken)
            ?? throw new KeyNotFoundException("Opcao do campo nao encontrada apos atualizacao.");

        return FormularioServicoAdminMapeamentos.MapOpcao(completa);
    }

    public async Task<AlterarSituacaoCadastroResponse> InativarOpcaoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var opcao = await ValidarEObterOpcaoParaAlteracaoStatusAsync(id, cancellationToken);

        if (!opcao.Ativo)
        {
            return new AlterarSituacaoCadastroResponse(opcao.Id, false, "Opcao do campo ja estava inativa.");
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        opcao.Inativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(opcao.Id, false, "Opcao do campo inativada com sucesso.");
    }

    public async Task<AlterarSituacaoCadastroResponse> ReativarOpcaoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var opcao = await ValidarEObterOpcaoParaAlteracaoStatusAsync(id, cancellationToken);

        if (opcao.Ativo)
        {
            return new AlterarSituacaoCadastroResponse(opcao.Id, true, "Opcao do campo ja estava ativa.");
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        opcao.Reativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(opcao.Id, true, "Opcao do campo reativada com sucesso.");
    }

    private async Task<FormularioServico?> ObterFormularioCompletoPorIdAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken)
    {
        var query = formularioServicoRepository.Query()
            .Include(x => x.Versoes)
                .ThenInclude(x => x.Campos)
                    .ThenInclude(x => x.Opcoes)
            .AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    private async Task<FormularioServicoVersao?> ObterVersaoPorIdAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken)
    {
        var query = formularioServicoVersaoRepository.Query()
            .Include(x => x.Campos)
                .ThenInclude(x => x.Opcoes)
            .AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    private async Task<CampoFormularioServico?> ObterCampoPorIdAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken)
    {
        var query = campoFormularioServicoRepository.Query()
            .Include(x => x.Opcoes)
            .AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    private async Task<OpcaoCampoFormularioServico?> ObterOpcaoPorIdAsync(
        Guid id,
        bool asNoTracking,
        CancellationToken cancellationToken)
    {
        var query = opcaoCampoFormularioServicoRepository.Query()
            .AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    private async Task ValidarCatalogoExisteAsync(Guid catalogoServicoId, CancellationToken cancellationToken)
    {
        var existe = await catalogoServicoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == catalogoServicoId && x.Ativo, cancellationToken);

        if (!existe)
        {
            throw new InvalidOperationException("Catalogo de servico informado nao encontrado ou inativo.");
        }
    }

    private async Task ValidarFormularioExisteAsync(Guid formularioServicoId, CancellationToken cancellationToken)
    {
        var existe = await formularioServicoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == formularioServicoId, cancellationToken);

        if (!existe)
        {
            throw new InvalidOperationException("Formulario do servico informado nao encontrado.");
        }
    }

    private async Task ValidarVersaoExisteAsync(Guid formularioServicoVersaoId, CancellationToken cancellationToken)
    {
        var existe = await formularioServicoVersaoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == formularioServicoVersaoId, cancellationToken);

        if (!existe)
        {
            throw new InvalidOperationException("Versao do formulario informada nao encontrada.");
        }
    }

    private async Task ValidarCampoExisteAsync(Guid campoFormularioServicoId, CancellationToken cancellationToken)
    {
        var existe = await campoFormularioServicoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == campoFormularioServicoId, cancellationToken);

        if (!existe)
        {
            throw new InvalidOperationException("Campo do formulario informado nao encontrado.");
        }
    }

    private async Task ValidarDuplicidadeFormularioAsync(Guid catalogoServicoId, Guid? idIgnorado, CancellationToken cancellationToken)
    {
        var query = formularioServicoRepository.Query()
            .AsNoTracking()
            .Where(x => x.CatalogoServicoId == catalogoServicoId);

        if (idIgnorado.HasValue)
        {
            query = query.Where(x => x.Id != idIgnorado.Value);
        }

        if (await query.AnyAsync(cancellationToken))
        {
            throw new InvalidOperationException("Catalogo de servico ja possui formulario configurado.");
        }
    }

    private async Task ValidarDuplicidadeVersaoAsync(Guid formularioServicoId, int numero, Guid? idIgnorado, CancellationToken cancellationToken)
    {
        var query = formularioServicoVersaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.FormularioServicoId == formularioServicoId && x.Numero == numero);

        if (idIgnorado.HasValue)
        {
            query = query.Where(x => x.Id != idIgnorado.Value);
        }

        if (await query.AnyAsync(cancellationToken))
        {
            throw new InvalidOperationException("Ja existe versao com o mesmo numero para este formulario.");
        }
    }

    private async Task ValidarDuplicidadeCampoAsync(
        Guid formularioServicoVersaoId,
        string nome,
        int ordem,
        Guid? idIgnorado,
        CancellationToken cancellationToken)
    {
        var query = campoFormularioServicoRepository.Query()
            .AsNoTracking()
            .Where(x => x.FormularioServicoVersaoId == formularioServicoVersaoId);

        if (idIgnorado.HasValue)
        {
            query = query.Where(x => x.Id != idIgnorado.Value);
        }

        if (await query.AnyAsync(x => x.Nome == nome, cancellationToken))
        {
            throw new InvalidOperationException("Ja existe campo com o mesmo nome nesta versao.");
        }

        if (await query.AnyAsync(x => x.Ordem == ordem, cancellationToken))
        {
            throw new InvalidOperationException("Ja existe campo com a mesma ordem nesta versao.");
        }
    }

    private async Task ValidarDuplicidadeOpcaoAsync(
        Guid campoFormularioServicoId,
        string valor,
        int ordem,
        Guid? idIgnorado,
        CancellationToken cancellationToken)
    {
        var query = opcaoCampoFormularioServicoRepository.Query()
            .AsNoTracking()
            .Where(x => x.CampoFormularioServicoId == campoFormularioServicoId);

        if (idIgnorado.HasValue)
        {
            query = query.Where(x => x.Id != idIgnorado.Value);
        }

        if (await query.AnyAsync(x => x.Valor == valor, cancellationToken))
        {
            throw new InvalidOperationException("Ja existe opcao com o mesmo valor para este campo.");
        }

        if (await query.AnyAsync(x => x.Ordem == ordem, cancellationToken))
        {
            throw new InvalidOperationException("Ja existe opcao com a mesma ordem para este campo.");
        }
    }

    private static void ValidarCampoEnumerado(CampoFormularioServico campo)
    {
        if (campo.Tipo is not TipoCampoFormularioServico.SelecaoUnica and not TipoCampoFormularioServico.SelecaoMultipla)
        {
            throw new InvalidOperationException("Opcoes so podem ser configuradas para campos dos tipos SelecaoUnica ou SelecaoMultipla.");
        }
    }

    private async Task<FormularioServicoVersao> ValidarEObterVersaoParaAlteracaoStatusAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        return await ObterVersaoPorIdAsync(id, false, cancellationToken)
            ?? throw new KeyNotFoundException("Versao do formulario nao encontrada.");
    }

    private async Task<CampoFormularioServico> ValidarEObterCampoParaAlteracaoStatusAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        return await ObterCampoPorIdAsync(id, false, cancellationToken)
            ?? throw new KeyNotFoundException("Campo do formulario nao encontrado.");
    }

    private async Task<OpcaoCampoFormularioServico> ValidarEObterOpcaoParaAlteracaoStatusAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var opcao = await ObterOpcaoPorIdAsync(id, false, cancellationToken)
            ?? throw new KeyNotFoundException("Opcao do campo nao encontrada.");

        return opcao;
    }

    private static async Task ValidarOuFalharAsync<T>(IValidator<T> validator, T request, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}

internal static class FormularioServicoAdminMapeamentos
{
    public static FormularioServicoAdminDto MapFormulario(FormularioServico formulario)
        => new(
            formulario.Id,
            formulario.CatalogoServicoId,
            formulario.Nome,
            formulario.Descricao,
            formulario.Ativo,
            formulario.CriadoEm,
            formulario.AtualizadoEm);

    public static FormularioServicoDetalheAdminDto MapDetalhe(FormularioServico formulario)
        => new(
            formulario.Id,
            formulario.CatalogoServicoId,
            formulario.Nome,
            formulario.Descricao,
            formulario.Ativo,
            formulario.CriadoEm,
            formulario.AtualizadoEm,
            formulario.Versoes
                .OrderBy(x => x.Numero)
                .Select(MapVersao)
                .ToArray());

    public static FormularioServicoVersaoAdminDto MapVersao(FormularioServicoVersao versao)
        => new(
            versao.Id,
            versao.FormularioServicoId,
            versao.Numero,
            versao.Publicada,
            versao.PublicadoEm,
            versao.Ativo,
            versao.Campos
                .OrderBy(x => x.Ordem)
                .ThenBy(x => x.Nome)
                .Select(MapCampo)
                .ToArray());

    public static CampoFormularioServicoAdminDto MapCampo(CampoFormularioServico campo)
        => new(
            campo.Id,
            campo.FormularioServicoVersaoId,
            campo.Nome,
            campo.Rotulo,
            campo.Tipo,
            campo.Obrigatorio,
            campo.Ordem,
            campo.TextoAjuda,
            campo.Visivel,
            campo.Ativo,
            campo.Opcoes
                .OrderBy(x => x.Ordem)
                .ThenBy(x => x.Valor)
                .Select(MapOpcao)
                .ToArray());

    public static OpcaoCampoFormularioServicoAdminDto MapOpcao(OpcaoCampoFormularioServico opcao)
        => new(
            opcao.Id,
            opcao.CampoFormularioServicoId,
            opcao.Valor,
            opcao.Rotulo,
            opcao.Ordem,
            opcao.Ativo);
}
