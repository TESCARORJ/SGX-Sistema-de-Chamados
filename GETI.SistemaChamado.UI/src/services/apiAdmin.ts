import { obterCabecalhosAutenticacaoLocal } from '@/services/sessaoUsuario';

export type DepartamentoAdministrativo = {
  id: string;
  nome: string;
  ativo: boolean;
  dataCriacao: string;
  dataAtualizacao: string | null;
};

export type CaixaDeEmailAdministrativa = {
  id: string;
  enderecoEmail: string;
  nomeExibicao: string;
  ativa: boolean;
  departamentoId: string;
  departamentoNome: string;
  dataCriacao: string;
  dataAtualizacao: string | null;
};

export type CategoriaAdministrativa = {
  id: string;
  nome: string;
  descricao: string | null;
  ativo: boolean;
  dataCriacao: string;
  dataAtualizacao: string | null;
};

export type GrupoAtendimentoAdministrativo = {
  id: string;
  nome: string;
  descricao: string | null;
  ativo: boolean;
  departamentoId: string;
  departamentoNome: string;
  dataCriacao: string;
  dataAtualizacao: string | null;
};

export type ServicoAdministrativo = {
  id: string;
  nome: string;
  descricao: string | null;
  ativo: boolean;
  categoriaId: string;
  categoriaNome: string;
  departamentoId: string;
  departamentoNome: string;
  dataCriacao: string;
  dataAtualizacao: string | null;
};

type MetodoHttp = 'GET' | 'POST' | 'PUT' | 'PATCH';

async function requisicaoJson<TResposta>(
  url: string,
  metodo: MetodoHttp,
  corpo?: unknown
): Promise<TResposta> {
  const resposta = await fetch(url, {
    method: metodo,
    headers: {
      ...(corpo ? { 'Content-Type': 'application/json' } : {}),
      ...obterCabecalhosAutenticacaoLocal()
    },
    body: corpo ? JSON.stringify(corpo) : undefined
  });

  if (!resposta.ok) {
    let mensagem = `Falha na requisicao (${resposta.status}).`;
    try {
      const erro = (await resposta.json()) as { mensagem?: string };
      if (erro.mensagem) {
        mensagem = erro.mensagem;
      }
    } catch {
      // noop
    }
    throw new Error(mensagem);
  }

  if (resposta.status === 204) {
    return undefined as TResposta;
  }

  return (await resposta.json()) as TResposta;
}

export function listarDepartamentos(): Promise<DepartamentoAdministrativo[]> {
  return requisicaoJson<DepartamentoAdministrativo[]>('/api/admin/cadastros/departamentos', 'GET');
}

export function criarDepartamento(nome: string): Promise<DepartamentoAdministrativo> {
  return requisicaoJson<DepartamentoAdministrativo>('/api/admin/cadastros/departamentos', 'POST', { nome });
}

export function atualizarDepartamento(id: string, nome: string): Promise<DepartamentoAdministrativo> {
  return requisicaoJson<DepartamentoAdministrativo>(`/api/admin/cadastros/departamentos/${id}`, 'PUT', { nome });
}

export function inativarDepartamento(id: string): Promise<void> {
  return requisicaoJson<void>(`/api/admin/cadastros/departamentos/${id}/inativacao`, 'PATCH');
}

export function listarCaixasEmail(): Promise<CaixaDeEmailAdministrativa[]> {
  return requisicaoJson<CaixaDeEmailAdministrativa[]>('/api/admin/cadastros/caixas-email', 'GET');
}

export function criarCaixaEmail(
  enderecoEmail: string,
  nomeExibicao: string,
  departamentoId: string
): Promise<CaixaDeEmailAdministrativa> {
  return requisicaoJson<CaixaDeEmailAdministrativa>('/api/admin/cadastros/caixas-email', 'POST', {
    enderecoEmail,
    nomeExibicao,
    departamentoId
  });
}

export function atualizarCaixaEmail(
  id: string,
  enderecoEmail: string,
  nomeExibicao: string,
  departamentoId: string
): Promise<CaixaDeEmailAdministrativa> {
  return requisicaoJson<CaixaDeEmailAdministrativa>(`/api/admin/cadastros/caixas-email/${id}`, 'PUT', {
    enderecoEmail,
    nomeExibicao,
    departamentoId
  });
}

export function inativarCaixaEmail(id: string): Promise<void> {
  return requisicaoJson<void>(`/api/admin/cadastros/caixas-email/${id}/inativacao`, 'PATCH');
}

export function listarCategorias(): Promise<CategoriaAdministrativa[]> {
  return requisicaoJson<CategoriaAdministrativa[]>('/api/admin/cadastros/categorias', 'GET');
}

export function criarCategoria(nome: string, descricao: string | null): Promise<CategoriaAdministrativa> {
  return requisicaoJson<CategoriaAdministrativa>('/api/admin/cadastros/categorias', 'POST', {
    nome,
    descricao
  });
}

export function atualizarCategoria(
  id: string,
  nome: string,
  descricao: string | null
): Promise<CategoriaAdministrativa> {
  return requisicaoJson<CategoriaAdministrativa>(`/api/admin/cadastros/categorias/${id}`, 'PUT', {
    nome,
    descricao
  });
}

export function inativarCategoria(id: string): Promise<void> {
  return requisicaoJson<void>(`/api/admin/cadastros/categorias/${id}/inativacao`, 'PATCH');
}

export function listarGruposAtendimento(): Promise<GrupoAtendimentoAdministrativo[]> {
  return requisicaoJson<GrupoAtendimentoAdministrativo[]>('/api/admin/cadastros/grupos-atendimento', 'GET');
}

export function criarGrupoAtendimento(
  nome: string,
  descricao: string | null,
  departamentoId: string
): Promise<GrupoAtendimentoAdministrativo> {
  return requisicaoJson<GrupoAtendimentoAdministrativo>('/api/admin/cadastros/grupos-atendimento', 'POST', {
    nome,
    descricao,
    departamentoId
  });
}

export function atualizarGrupoAtendimento(
  id: string,
  nome: string,
  descricao: string | null,
  departamentoId: string
): Promise<GrupoAtendimentoAdministrativo> {
  return requisicaoJson<GrupoAtendimentoAdministrativo>(`/api/admin/cadastros/grupos-atendimento/${id}`, 'PUT', {
    nome,
    descricao,
    departamentoId
  });
}

export function inativarGrupoAtendimento(id: string): Promise<void> {
  return requisicaoJson<void>(`/api/admin/cadastros/grupos-atendimento/${id}/inativacao`, 'PATCH');
}

export function listarServicos(): Promise<ServicoAdministrativo[]> {
  return requisicaoJson<ServicoAdministrativo[]>('/api/admin/cadastros/servicos', 'GET');
}

export function criarServico(
  nome: string,
  descricao: string | null,
  categoriaId: string,
  departamentoId: string
): Promise<ServicoAdministrativo> {
  return requisicaoJson<ServicoAdministrativo>('/api/admin/cadastros/servicos', 'POST', {
    nome,
    descricao,
    categoriaId,
    departamentoId
  });
}

export function atualizarServico(
  id: string,
  nome: string,
  descricao: string | null,
  categoriaId: string,
  departamentoId: string
): Promise<ServicoAdministrativo> {
  return requisicaoJson<ServicoAdministrativo>(`/api/admin/cadastros/servicos/${id}`, 'PUT', {
    nome,
    descricao,
    categoriaId,
    departamentoId
  });
}

export function inativarServico(id: string): Promise<void> {
  return requisicaoJson<void>(`/api/admin/cadastros/servicos/${id}/inativacao`, 'PATCH');
}
