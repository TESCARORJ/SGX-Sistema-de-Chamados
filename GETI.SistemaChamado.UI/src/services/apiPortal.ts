import { requisitarApiJson } from '@/services/http/clienteHttp';

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

export function consultarCatalogoAberturaChamado(): Promise<CatalogoAberturaChamadoPortal> {
  return requisitarApiJson<CatalogoAberturaChamadoPortal>('/api/portal/chamados/catalogo-abertura');
}

export function abrirChamadoPortal(entrada: AberturaChamadoPortalEntrada): Promise<ChamadoPortalDetalhe> {
  return requisitarApiJson<ChamadoPortalDetalhe>('/api/portal/chamados', {
    metodo: 'POST',
    corpo: entrada
  });
}

export function listarChamadosPortal(): Promise<ChamadoPortalResumo[]> {
  return requisitarApiJson<ChamadoPortalResumo[]>('/api/portal/chamados');
}

export function detalharChamadoPortal(id: string): Promise<ChamadoPortalDetalhe> {
  return requisitarApiJson<ChamadoPortalDetalhe>(`/api/portal/chamados/${id}`);
}

export function anexarArquivoChamadoPortal(id: string, arquivo: File): Promise<AnexoChamadoPortal> {
  const formData = new FormData();
  formData.append('arquivo', arquivo);

  return requisitarApiJson<AnexoChamadoPortal>(`/api/portal/chamados/${id}/anexos`, {
    metodo: 'POST',
    corpo: formData
  });
}
