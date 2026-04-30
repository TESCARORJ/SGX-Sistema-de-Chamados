import { obterSessaoLocalDesenvolvimento } from '@/services/sessaoLocalDesenvolvimento';

function obterCabecalhosPorCredencialBasica(): HeadersInit {
  const email = import.meta.env.VITE_AUTH_LOCAL_EMAIL;
  const senha = import.meta.env.VITE_AUTH_LOCAL_SENHA;
  if (!email || !senha) {
    return {};
  }

  const token = btoa(`${email}:${senha}`);
  return {
    Authorization: `Basic ${token}`
  };
}

function obterCabecalhosPorSessaoLocal(): HeadersInit {
  const sessaoLocal = obterSessaoLocalDesenvolvimento();
  if (!sessaoLocal) {
    return {};
  }

  return {
    'X-Auth-Login': sessaoLocal.login,
    'X-Auth-Nome': sessaoLocal.nome || sessaoLocal.login,
    'X-Auth-Email': sessaoLocal.email || (sessaoLocal.login.includes('@') ? sessaoLocal.login : `${sessaoLocal.login}@local.geti`)
  };
}

export function obterCabecalhosAutenticacaoLocal(): HeadersInit {
  const porCredencialBasica = obterCabecalhosPorCredencialBasica();
  if (Object.keys(porCredencialBasica).length > 0) {
    return porCredencialBasica;
  }
  return obterCabecalhosPorSessaoLocal();
}
