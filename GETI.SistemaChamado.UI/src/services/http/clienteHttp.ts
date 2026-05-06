import { obterCabecalhosAutenticacaoLocal } from '@/services/autenticacaoLocalCabecalhos';

export type CodigoErroHttp = 'NAO_AUTENTICADO' | 'ACESSO_NEGADO' | 'ERRO_INTERNO' | 'FALHA_CONEXAO' | 'ERRO_HTTP';

export class ErroHttpApi extends Error {
  readonly codigo: CodigoErroHttp;
  readonly status?: number;

  constructor(mensagem: string, codigo: CodigoErroHttp, status?: number) {
    super(mensagem);
    this.name = 'ErroHttpApi';
    this.codigo = codigo;
    this.status = status;
  }
}

type MetodoHttp = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';

type OpcoesRequisicao = {
  metodo?: MetodoHttp;
  corpo?: unknown;
  cabecalhos?: HeadersInit;
};

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:18080').replace(/\/$/, '');

function montarUrl(url: string): string {
  if (url.startsWith('http://') || url.startsWith('https://')) {
    return url;
  }
  return `${API_BASE_URL}${url.startsWith('/') ? url : `/${url}`}`;
}

function mapearErroPorStatus(status: number, mensagem: string): ErroHttpApi {
  if (status === 401) {
    return new ErroHttpApi(mensagem || 'Usuario nao autenticado.', 'NAO_AUTENTICADO', status);
  }
  if (status === 403) {
    return new ErroHttpApi(mensagem || 'Acesso negado para esta operacao.', 'ACESSO_NEGADO', status);
  }
  if (status >= 500) {
    return new ErroHttpApi(mensagem || 'Erro interno no backend.', 'ERRO_INTERNO', status);
  }
  return new ErroHttpApi(mensagem || `Falha na requisicao HTTP (${status}).`, 'ERRO_HTTP', status);
}

async function lerMensagemErro(resposta: Response): Promise<string> {
  const tipoConteudo = resposta.headers.get('content-type') || '';
  if (tipoConteudo.includes('application/json')) {
    try {
      const corpo = (await resposta.json()) as { mensagem?: string; error?: string };
      return corpo.mensagem || corpo.error || '';
    } catch {
      return '';
    }
  }
  try {
    return await resposta.text();
  } catch {
    return '';
  }
}

export async function requisitarApiJson<TResposta>(url: string, opcoes: OpcoesRequisicao = {}): Promise<TResposta> {
  const { metodo = 'GET', corpo, cabecalhos } = opcoes;
  const urlCompleta = montarUrl(url);
  const cabecalhosBase: HeadersInit = {
    ...obterCabecalhosAutenticacaoLocal(),
    ...cabecalhos
  };

  const init: RequestInit = {
    method: metodo,
    headers: cabecalhosBase
  };

  if (corpo !== undefined && corpo !== null) {
    if (corpo instanceof FormData) {
      init.body = corpo;
    } else {
      init.body = JSON.stringify(corpo);
      init.headers = {
        'Content-Type': 'application/json',
        ...cabecalhosBase
      };
    }
  }

  let resposta: Response;
  try {
    resposta = await fetch(urlCompleta, init);
  } catch {
    throw new ErroHttpApi(
      'Falha de conexao com o backend. Verifique se a API esta ativa em http://localhost:18080.',
      'FALHA_CONEXAO'
    );
  }

  if (!resposta.ok) {
    const mensagem = await lerMensagemErro(resposta);
    throw mapearErroPorStatus(resposta.status, mensagem);
  }

  if (resposta.status === 204) {
    return undefined as TResposta;
  }

  const tipoConteudo = resposta.headers.get('content-type') || '';
  if (!tipoConteudo.includes('application/json')) {
    return (await resposta.text()) as TResposta;
  }

  return (await resposta.json()) as TResposta;
}
