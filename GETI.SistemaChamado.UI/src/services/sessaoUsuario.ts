import { ErroHttpApi, requisitarApiJson } from '@/services/http/clienteHttp';

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

export async function obterUsuarioAtual(forcarAtualizacao = false): Promise<UsuarioAtual | null> {
  if (!forcarAtualizacao && cacheUsuarioAtual !== undefined) {
    return cacheUsuarioAtual;
  }

  try {
    const usuario = await requisitarApiJson<UsuarioAtual>('/api/me');
    cacheUsuarioAtual = usuario;
    return usuario;
  } catch (ex) {
    if (ex instanceof ErroHttpApi && ex.status === 401) {
      cacheUsuarioAtual = null;
      return null;
    }
    throw ex;
  }
}

export function limparCacheUsuarioAtual(): void {
  cacheUsuarioAtual = undefined;
}

export function possuiAcessoAdministrativo(usuario: UsuarioAtual): boolean {
  return PERFIS_ADMINISTRATIVOS.has(usuario.perfilAcesso);
}
