export type SessaoLocalDesenvolvimento = {
  login: string;
  nome?: string;
  email?: string;
};

const CHAVE_SESSAO_LOCAL = 'geti.sistema-chamado.sessao-local';

export function obterSessaoLocalDesenvolvimento(): SessaoLocalDesenvolvimento | null {
  if (typeof window === 'undefined') {
    return null;
  }

  const valor = window.localStorage.getItem(CHAVE_SESSAO_LOCAL);
  if (!valor) {
    return null;
  }

  try {
    const desserializado = JSON.parse(valor) as Partial<SessaoLocalDesenvolvimento>;
    if (!desserializado.login || !desserializado.login.trim()) {
      return null;
    }
    return {
      login: desserializado.login.trim(),
      nome: desserializado.nome?.trim(),
      email: desserializado.email?.trim()
    };
  } catch {
    return null;
  }
}

export function salvarSessaoLocalDesenvolvimento(sessao: SessaoLocalDesenvolvimento): void {
  if (typeof window === 'undefined') {
    return;
  }

  const login = sessao.login.trim();
  if (!login) {
    window.localStorage.removeItem(CHAVE_SESSAO_LOCAL);
    return;
  }

  window.localStorage.setItem(
    CHAVE_SESSAO_LOCAL,
    JSON.stringify({
      login,
      nome: sessao.nome?.trim() || login,
      email: sessao.email?.trim() || (login.includes('@') ? login : `${login}@local.geti`)
    })
  );
}

export function limparSessaoLocalDesenvolvimento(): void {
  if (typeof window === 'undefined') {
    return;
  }
  window.localStorage.removeItem(CHAVE_SESSAO_LOCAL);
}
