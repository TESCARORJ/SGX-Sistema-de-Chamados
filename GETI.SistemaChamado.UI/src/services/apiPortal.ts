import { obterCabecalhosAutenticacaoLocal } from '@/services/sessaoUsuario';

export type OpcaoCatalogoPortal = {
  id: string;
  nome: string;
};

export type CatalogoAberturaChamadoPortal = {
  departamentos: OpcaoCatalogoPortal[];
  categorias: OpcaoCatalogoPortal[];
  servicos: OpcaoCatalogoPortal[];
};

export type ChamadoPortalResumo = {
  id: string;
  numero: string;
  titulo: string;
  situacao: string;
  prioridade: string;
  categoria: string;
  servico: string;
  dataCriacao: string;
  dataAtualizacao: string | null;
};

export type InteracaoChamadoPortal = {
  id: string;
  tipoInteracao: string;
  mensagem: string;
  autor: string;
  dataCriacao: string;
};

export type HistoricoChamadoPortal = {
  id: string;
  descricao: string;
  situacaoAnterior: string | null;
  situacaoNova: string;
  dataCriacao: string;
};

export type AnexoChamadoPortal = {
  id: string;
  nomeArquivo: string;
  tipoConteudo: string;
  tamanhoBytes: number;
  dataCriacao: string;
};

export type ChamadoPortalDetalhe = {
  id: string;
  numero: string;
  titulo: string;
  descricao: string;
  situacao: string;
  prioridade: string;
  origem: string;
  departamentoId: string;
  departamento: string;
  categoriaId: string;
  categoria: string;
  servicoId: string;
  servico: string;
  dataCriacao: string;
  dataAtualizacao: string | null;
  interacoes: InteracaoChamadoPortal[];
  historicos: HistoricoChamadoPortal[];
  anexos: AnexoChamadoPortal[];
};

export type AberturaChamadoPortalEntrada = {
  titulo: string;
  descricao: string;
  prioridade: 'BAIXA' | 'MEDIA' | 'ALTA' | 'CRITICA';
  departamentoId: string;
  categoriaId: string;
  servicoId: string;
};

async function requisicaoJson<TResposta>(url: string, init?: RequestInit): Promise<TResposta> {
  const resposta = await fetch(url, {
    ...init,
    headers: {
      ...obterCabecalhosAutenticacaoLocal(),
      ...(init?.headers ?? {})
    }
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

  return (await resposta.json()) as TResposta;
}

export function consultarCatalogoAberturaChamado(): Promise<CatalogoAberturaChamadoPortal> {
  return requisicaoJson<CatalogoAberturaChamadoPortal>('/api/portal/chamados/catalogo-abertura');
}

export function abrirChamadoPortal(entrada: AberturaChamadoPortalEntrada): Promise<ChamadoPortalDetalhe> {
  return requisicaoJson<ChamadoPortalDetalhe>('/api/portal/chamados', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(entrada)
  });
}

export function listarChamadosPortal(): Promise<ChamadoPortalResumo[]> {
  return requisicaoJson<ChamadoPortalResumo[]>('/api/portal/chamados');
}

export function detalharChamadoPortal(id: string): Promise<ChamadoPortalDetalhe> {
  return requisicaoJson<ChamadoPortalDetalhe>(`/api/portal/chamados/${id}`);
}

export function anexarArquivoChamadoPortal(id: string, arquivo: File): Promise<AnexoChamadoPortal> {
  const formData = new FormData();
  formData.append('arquivo', arquivo);

  return requisicaoJson<AnexoChamadoPortal>(`/api/portal/chamados/${id}/anexos`, {
    method: 'POST',
    body: formData
  });
}
