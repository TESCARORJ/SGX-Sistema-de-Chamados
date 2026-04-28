export type UsuarioAtual = {
  usuarioId: string;
  login: string;
  nome: string;
  email: string;
  perfilAcesso: 'Solicitante' | 'Atendente' | 'Supervisor' | 'Administrador' | string;
  departamentoId: string | null;
};

const PERFIS_ADMINISTRATIVOS = new Set(['Atendente', 'Supervisor', 'Administrador']);

let cacheUsuarioAtual: UsuarioAtual | null | undefined;

function headersAutenticacaoLocal(): HeadersInit {
  const login = import.meta.env.VITE_AUTH_LOCAL_LOGIN;
  if (!login) {
    return {};
  }

  return {
    'X-Auth-Login': login,
    'X-Auth-Nome': import.meta.env.VITE_AUTH_LOCAL_NOME ?? login,
    'X-Auth-Email': import.meta.env.VITE_AUTH_LOCAL_EMAIL ?? login
  };
}

export async function obterUsuarioAtual(forcarAtualizacao = false): Promise<UsuarioAtual | null> {
  if (!forcarAtualizacao && cacheUsuarioAtual !== undefined) {
    return cacheUsuarioAtual;
  }

  const resposta = await fetch('/api/me', {
    method: 'GET',
    headers: {
      ...headersAutenticacaoLocal()
    }
  });

  if (resposta.status === 401) {
    cacheUsuarioAtual = null;
    return null;
  }

  if (!resposta.ok) {
    throw new Error(`Falha ao consultar usuario autenticado: ${resposta.status}`);
  }

  const usuario = (await resposta.json()) as UsuarioAtual;
  cacheUsuarioAtual = usuario;
  return usuario;
}

export function limparCacheUsuarioAtual(): void {
  cacheUsuarioAtual = undefined;
}

export function possuiAcessoAdministrativo(usuario: UsuarioAtual): boolean {
  return PERFIS_ADMINISTRATIVOS.has(usuario.perfilAcesso);
}
