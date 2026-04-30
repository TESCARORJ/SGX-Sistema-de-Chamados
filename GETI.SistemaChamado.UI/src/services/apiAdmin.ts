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

export type OpcaoCatalogoAdminChamado = {
  id: string;
  nome: string;
};

export type ResponsavelAdminChamado = {
  id: string;
  nome: string;
  login: string;
  perfilAcesso: string;
};

export type CatalogoAdminChamado = {
  departamentos: OpcaoCatalogoAdminChamado[];
  categorias: OpcaoCatalogoAdminChamado[];
  servicos: OpcaoCatalogoAdminChamado[];
  responsaveis: ResponsavelAdminChamado[];
  situacoes: string[];
  prioridades: string[];
  origens: string[];
};

export type IndicadorDashboardAdminChamado = {
  chave: string;
  total: number;
};

export type ChamadoPendenteDashboardAdmin = {
  id: string;
  numero: string;
  titulo: string;
  situacao: string;
  prioridade: string;
  departamento: string;
  responsavel: string;
  dataCriacao: string;
};

export type DashboardAdminChamado = {
  porSituacao: IndicadorDashboardAdminChamado[];
  porPrioridade: IndicadorDashboardAdminChamado[];
  porDepartamento: IndicadorDashboardAdminChamado[];
  pendentesRecentes: ChamadoPendenteDashboardAdmin[];
};

export type ChamadoFilaAdmin = {
  id: string;
  numero: string;
  titulo: string;
  situacao: string;
  prioridade: string;
  origem: string;
  solicitanteNome: string;
  departamentoNome: string;
  categoriaNome: string;
  servicoNome: string;
  responsavelId: string | null;
  responsavelNome: string | null;
  dataCriacao: string;
  dataAtualizacao: string | null;
};

export type InteracaoChamadoAdmin = {
  id: string;
  tipoInteracao: string;
  mensagem: string;
  visivelSolicitante: boolean;
  autorId: string;
  autorNome: string;
  dataCriacao: string;
};

export type HistoricoChamadoAdmin = {
  id: string;
  descricao: string;
  situacaoAnterior: string | null;
  situacaoNova: string;
  visivelSolicitante: boolean;
  dataCriacao: string;
};

export type AnexoChamadoAdmin = {
  id: string;
  nomeArquivo: string;
  tipoConteudo: string;
  tamanhoBytes: number;
  autorId: string;
  autorNome: string;
  dataCriacao: string;
};

export type ChamadoDetalheAdmin = {
  id: string;
  numero: string;
  titulo: string;
  descricao: string;
  situacao: string;
  prioridade: string;
  origem: string;
  solicitanteId: string;
  solicitanteNome: string;
  solicitanteLogin: string;
  solicitanteEmail: string;
  responsavelId: string | null;
  responsavelNome: string | null;
  departamentoId: string;
  departamentoNome: string;
  categoriaId: string;
  categoriaNome: string;
  servicoId: string;
  servicoNome: string;
  dataCriacao: string;
  dataAtualizacao: string | null;
  interacoes: InteracaoChamadoAdmin[];
  historicos: HistoricoChamadoAdmin[];
  anexos: AnexoChamadoAdmin[];
};

export type FiltroFilaAdminChamado = {
  situacao?: string | null;
  prioridade?: string | null;
  departamentoId?: string | null;
  origem?: string | null;
  responsavelId?: string | null;
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

export function consultarCatalogoChamadoAdmin(): Promise<CatalogoAdminChamado> {
  return requisicaoJson<CatalogoAdminChamado>('/api/admin/chamados/catalogo', 'GET');
}

export function consultarDashboardChamadoAdmin(): Promise<DashboardAdminChamado> {
  return requisicaoJson<DashboardAdminChamado>('/api/admin/chamados/dashboard', 'GET');
}

export function listarFilaChamadoAdmin(filtro: FiltroFilaAdminChamado): Promise<ChamadoFilaAdmin[]> {
  const params = new URLSearchParams();
  if (filtro.situacao) {
    params.set('situacao', filtro.situacao);
  }
  if (filtro.prioridade) {
    params.set('prioridade', filtro.prioridade);
  }
  if (filtro.departamentoId) {
    params.set('departamentoId', filtro.departamentoId);
  }
  if (filtro.origem) {
    params.set('origem', filtro.origem);
  }
  if (filtro.responsavelId) {
    params.set('responsavelId', filtro.responsavelId);
  }

  const sufixo = params.toString() ? `?${params.toString()}` : '';
  return requisicaoJson<ChamadoFilaAdmin[]>(`/api/admin/chamados${sufixo}`, 'GET');
}

export function detalharChamadoAdmin(id: string): Promise<ChamadoDetalheAdmin> {
  return requisicaoJson<ChamadoDetalheAdmin>(`/api/admin/chamados/${id}`, 'GET');
}

export function atribuirChamadoAdmin(id: string, responsavelId: string): Promise<ChamadoDetalheAdmin> {
  return requisicaoJson<ChamadoDetalheAdmin>(`/api/admin/chamados/${id}/atribuicao`, 'PATCH', {
    responsavelId
  });
}

export function alterarSituacaoChamadoAdmin(id: string, novaSituacao: string): Promise<ChamadoDetalheAdmin> {
  return requisicaoJson<ChamadoDetalheAdmin>(`/api/admin/chamados/${id}/situacao`, 'PATCH', {
    novaSituacao
  });
}

export function encaminharChamadoAdmin(
  id: string,
  departamentoId: string,
  categoriaId: string,
  servicoId: string
): Promise<ChamadoDetalheAdmin> {
  return requisicaoJson<ChamadoDetalheAdmin>(`/api/admin/chamados/${id}/encaminhamento`, 'PATCH', {
    departamentoId,
    categoriaId,
    servicoId
  });
}

export function comentarPublicamenteChamadoAdmin(id: string, mensagem: string): Promise<ChamadoDetalheAdmin> {
  return requisicaoJson<ChamadoDetalheAdmin>(`/api/admin/chamados/${id}/comentarios/publico`, 'POST', {
    mensagem
  });
}

export function comentarInternamenteChamadoAdmin(id: string, mensagem: string): Promise<ChamadoDetalheAdmin> {
  return requisicaoJson<ChamadoDetalheAdmin>(`/api/admin/chamados/${id}/comentarios/interno`, 'POST', {
    mensagem
  });
}
