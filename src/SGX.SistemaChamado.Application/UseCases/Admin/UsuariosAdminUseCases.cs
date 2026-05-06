using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarUsuariosAdminUseCase(
    IRepository<Usuario> usuarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarUsuariosAdminUseCase
{
    public async Task<PagedResultResponse<UsuarioAdminResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = usuarioRepository.Query()
            .AsNoTracking()
            .Include(x => x.Departamento)
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .AsQueryable();

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim();
            query = query.Where(x =>
                x.Nome.Contains(texto) ||
                x.Email.Contains(texto) ||
                x.Login.Contains(texto));
        }

        query = ApplyOrder(query, request.OrdenarPor, request.DirecaoOrdenacao);
        var (pagina, tamanho) = AdminCadastrosHelpers.NormalizarPaginacao(request);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);

        return new PagedResultResponse<UsuarioAdminResumoResponse>
        {
            Items = items.Select(MapResumo).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanho
        };
    }

    private static IQueryable<Usuario> ApplyOrder(IQueryable<Usuario> query, string? ordenarPor, string? direcao)
    {
        var campo = (ordenarPor ?? "nome").Trim().ToLowerInvariant();
        var desc = AdminCadastrosHelpers.DirecaoDesc(direcao);
        return campo switch
        {
            "email" => desc ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email),
            "login" => desc ? query.OrderByDescending(x => x.Login) : query.OrderBy(x => x.Login),
            "situacao" => desc ? query.OrderByDescending(x => x.Situacao) : query.OrderBy(x => x.Situacao),
            "criadoem" => desc ? query.OrderByDescending(x => x.CriadoEm) : query.OrderBy(x => x.CriadoEm),
            _ => desc ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome)
        };
    }

    internal static UsuarioAdminResumoResponse MapResumo(Usuario x)
        => new(
            x.Id,
            x.Nome,
            x.Email,
            x.Login,
            x.Situacao.ToString(),
            x.DepartamentoId,
            x.Departamento?.Nome,
            x.Ativo,
            x.UsuarioPerfis
                .Where(p => p.PerfilAcesso.Ativo)
                .Select(p => AdminCadastrosHelpers.MapPerfilResumo(p.PerfilAcesso))
                .OrderBy(p => p.Nome)
                .ToArray());
}

public sealed class ObterUsuarioAdminUseCase(
    IRepository<Usuario> usuarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterUsuarioAdminUseCase
{
    public async Task<UsuarioAdminDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var usuario = await usuarioRepository.Query()
            .AsNoTracking()
            .Include(x => x.Departamento)
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario nao encontrado.");

        return MapDetalhe(usuario);
    }

    internal static UsuarioAdminDetalheResponse MapDetalhe(Usuario x)
        => new(
            x.Id,
            x.Nome,
            x.Email,
            x.Login,
            x.Situacao.ToString(),
            x.UltimoAcessoEm,
            x.DepartamentoId,
            x.Departamento?.Nome,
            x.Ativo,
            x.UsuarioPerfis
                .Where(p => p.PerfilAcesso.Ativo)
                .Select(p => AdminCadastrosHelpers.MapPerfilResumo(p.PerfilAcesso))
                .OrderBy(p => p.Nome)
                .ToArray());
}

public sealed class CriarUsuarioAdminUseCase(
    IRepository<Usuario> usuarioRepository,
    IRepository<Departamento> departamentoRepository,
    IRepository<PerfilAcesso> perfilAcessoRepository,
    IRepository<UsuarioPerfilAcesso> usuarioPerfilAcessoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : ICriarUsuarioAdminUseCase
{
    public async Task<UsuarioAdminDetalheResponse> ExecutarAsync(CriarUsuarioAdminRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var email = request.Email.Trim().ToLowerInvariant();
        var login = string.IsNullOrWhiteSpace(request.Login)
            ? DerivarLogin(email)
            : request.Login.Trim().ToLowerInvariant();

        var existeEmail = await usuarioRepository.Query().AnyAsync(x => x.Email == email, cancellationToken);
        if (existeEmail)
        {
            throw new InvalidOperationException("Ja existe usuario com este e-mail.");
        }

        var existeLogin = await usuarioRepository.Query().AnyAsync(x => x.Login == login, cancellationToken);
        if (existeLogin)
        {
            throw new InvalidOperationException("Ja existe usuario com este login.");
        }

        if (request.DepartamentoId.HasValue)
        {
            var departamentoValido = await departamentoRepository.Query()
                .AnyAsync(x => x.Id == request.DepartamentoId.Value && x.Ativo, cancellationToken);
            if (!departamentoValido)
            {
                throw new InvalidOperationException("Departamento informado nao encontrado ou inativo.");
            }
        }

        var perfis = await CarregarPerfisAsync(perfilAcessoRepository, request.PerfilIds, cancellationToken);
        if (perfis.Count == 0)
        {
            throw new InvalidOperationException("Usuario deve possuir ao menos um perfil.");
        }

        var usuario = new Usuario(request.Nome, email, login, usuarioAtual.Login, request.DepartamentoId);
        await usuarioRepository.AddAsync(usuario, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var perfil in perfis)
        {
            await usuarioPerfilAcessoRepository.AddAsync(new UsuarioPerfilAcesso(usuario.Id, perfil.Id, usuarioAtual.Login), cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var criado = await usuarioRepository.Query()
            .AsNoTracking()
            .Include(x => x.Departamento)
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstAsync(x => x.Id == usuario.Id, cancellationToken);

        return ObterUsuarioAdminUseCase.MapDetalhe(criado);
    }

    internal static string DerivarLogin(string email)
    {
        var at = email.IndexOf('@');
        return at > 0 ? email[..at] : email;
    }

    internal static async Task<List<PerfilAcesso>> CarregarPerfisAsync(
        IRepository<PerfilAcesso> perfilAcessoRepository,
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        var perfilIds = ids.Where(x => x != Guid.Empty).Distinct().ToArray();
        if (perfilIds.Length == 0)
        {
            return [];
        }

        var perfis = await perfilAcessoRepository.Query()
            .Where(x => perfilIds.Contains(x.Id) && x.Ativo)
            .ToListAsync(cancellationToken);

        if (perfis.Count != perfilIds.Length)
        {
            throw new InvalidOperationException("Um ou mais perfis informados nao existem ou estao inativos.");
        }

        return perfis;
    }
}

public sealed class AtualizarUsuarioAdminUseCase(
    IRepository<Usuario> usuarioRepository,
    IRepository<Departamento> departamentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarUsuarioAdminUseCase
{
    public async Task<UsuarioAdminDetalheResponse> ExecutarAsync(Guid id, AtualizarUsuarioAdminRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var usuario = await usuarioRepository.Query()
            .Include(x => x.Departamento)
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario nao encontrado.");

        var email = request.Email.Trim().ToLowerInvariant();
        var login = string.IsNullOrWhiteSpace(request.Login)
            ? CriarUsuarioAdminUseCase.DerivarLogin(email)
            : request.Login.Trim().ToLowerInvariant();

        var existeEmail = await usuarioRepository.Query().AnyAsync(x => x.Id != id && x.Email == email, cancellationToken);
        if (existeEmail)
        {
            throw new InvalidOperationException("Ja existe usuario com este e-mail.");
        }

        var existeLogin = await usuarioRepository.Query().AnyAsync(x => x.Id != id && x.Login == login, cancellationToken);
        if (existeLogin)
        {
            throw new InvalidOperationException("Ja existe usuario com este login.");
        }

        if (request.DepartamentoId.HasValue)
        {
            var departamentoValido = await departamentoRepository.Query()
                .AnyAsync(x => x.Id == request.DepartamentoId.Value && x.Ativo, cancellationToken);
            if (!departamentoValido)
            {
                throw new InvalidOperationException("Departamento informado nao encontrado ou inativo.");
            }
        }

        usuario.DefinirNome(request.Nome);
        usuario.DefinirEmail(email);
        usuario.DefinirLogin(login);
        usuario.DefinirDepartamento(request.DepartamentoId, usuarioAtual.Login);
        usuario.AlterarSituacao(request.Situacao, usuarioAtual.Login);
        usuarioRepository.Update(usuario);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await usuarioRepository.Query()
            .AsNoTracking()
            .Include(x => x.Departamento)
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstAsync(x => x.Id == id, cancellationToken);

        return ObterUsuarioAdminUseCase.MapDetalhe(atualizado);
    }
}

public sealed class InativarUsuarioAdminUseCase(
    IRepository<Usuario> usuarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IInativarUsuarioAdminUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var usuario = await usuarioRepository.Query()
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario nao encontrado.");

        if (!usuario.Ativo)
        {
            return new AlterarSituacaoCadastroResponse(usuario.Id, false, "Usuario ja esta inativo.");
        }

        var usuarioEhAdminAtivo = usuario.Situacao == SituacaoUsuario.Ativo &&
                                  usuario.UsuarioPerfis.Any(x => x.PerfilAcesso.Ativo && x.PerfilAcesso.TipoPerfil == TipoPerfil.Administrador);

        if (usuarioEhAdminAtivo)
        {
            var totalAdminsAtivos = await AdminCadastrosHelpers.ContarAdministradoresAtivosAsync(
                usuarioRepository.Query().Include(x => x.UsuarioPerfis).ThenInclude(x => x.PerfilAcesso));

            if (totalAdminsAtivos <= 1)
            {
                throw new InvalidOperationException("Nao e permitido inativar o ultimo Administrador ativo.");
            }
        }

        usuario.Desativar(usuarioAtual.Login);
        usuario.AlterarSituacao(SituacaoUsuario.Inativo, usuarioAtual.Login);
        usuarioRepository.Update(usuario);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AlterarSituacaoCadastroResponse(usuario.Id, false, "Usuario inativado com sucesso.");
    }
}

public sealed class ReativarUsuarioAdminUseCase(
    IRepository<Usuario> usuarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IReativarUsuarioAdminUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var usuario = await usuarioRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario nao encontrado.");

        usuario.Ativar(usuarioAtual.Login);
        usuario.AlterarSituacao(SituacaoUsuario.Ativo, usuarioAtual.Login);
        usuarioRepository.Update(usuario);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AlterarSituacaoCadastroResponse(usuario.Id, true, "Usuario reativado com sucesso.");
    }
}

public sealed class AlterarPerfisUsuarioUseCase(
    IRepository<Usuario> usuarioRepository,
    IRepository<PerfilAcesso> perfilAcessoRepository,
    IRepository<UsuarioPerfilAcesso> usuarioPerfilAcessoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAlterarPerfisUsuarioUseCase
{
    public async Task<UsuarioAdminDetalheResponse> ExecutarAsync(Guid id, AlterarPerfisUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var usuario = await usuarioRepository.Query()
            .Include(x => x.Departamento)
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario nao encontrado.");

        var perfis = await CriarUsuarioAdminUseCase.CarregarPerfisAsync(perfilAcessoRepository, request.PerfilIds, cancellationToken);
        if (perfis.Count == 0)
        {
            throw new InvalidOperationException("Usuario deve possuir ao menos um perfil.");
        }

        var tinhaAdmin = usuario.UsuarioPerfis.Any(x => x.PerfilAcesso.Ativo && x.PerfilAcesso.TipoPerfil == TipoPerfil.Administrador);
        var ficaraAdmin = perfis.Any(x => x.TipoPerfil == TipoPerfil.Administrador);
        if (tinhaAdmin && !ficaraAdmin && usuario.Ativo && usuario.Situacao == SituacaoUsuario.Ativo)
        {
            var totalAdminsAtivos = await AdminCadastrosHelpers.ContarAdministradoresAtivosAsync(
                usuarioRepository.Query().Include(x => x.UsuarioPerfis).ThenInclude(x => x.PerfilAcesso));
            if (totalAdminsAtivos <= 1)
            {
                throw new InvalidOperationException("Nao e permitido remover o ultimo Administrador ativo.");
            }
        }

        var perfilIdsDesejados = perfis.Select(x => x.Id).ToHashSet();
        var atuais = usuario.UsuarioPerfis.ToArray();

        foreach (var item in atuais)
        {
            if (!perfilIdsDesejados.Contains(item.PerfilAcessoId))
            {
                usuarioPerfilAcessoRepository.Remove(item);
            }
        }

        var idsAtuais = atuais.Select(x => x.PerfilAcessoId).ToHashSet();
        foreach (var perfil in perfis)
        {
            if (!idsAtuais.Contains(perfil.Id))
            {
                await usuarioPerfilAcessoRepository.AddAsync(new UsuarioPerfilAcesso(usuario.Id, perfil.Id, usuarioAtual.Login), cancellationToken);
            }
        }

        usuario.AtualizarAuditoria(usuarioAtual.Login);
        usuarioRepository.Update(usuario);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await usuarioRepository.Query()
            .AsNoTracking()
            .Include(x => x.Departamento)
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstAsync(x => x.Id == id, cancellationToken);

        return ObterUsuarioAdminUseCase.MapDetalhe(atualizado);
    }
}
