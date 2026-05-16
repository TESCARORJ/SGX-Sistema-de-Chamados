import { httpClient } from './httpClient'
import type { Categoria } from '../types/categoria'
import type { Departamento } from '../types/departamento'
import type { Prioridade } from '../types/prioridade'
import type { Status } from '../types/status'

export const cadastrosService = {
  listarDepartamentos: () => httpClient.get<Departamento[]>('/api/cadastros/departamentos'),
  listarDepartamentosAtivos: () => httpClient.get<Departamento[]>('/api/cadastros/departamentos/ativos'),
  listarCategorias: () => httpClient.get<Categoria[]>('/api/cadastros/categorias'),
  listarCategoriasAtivas: () => httpClient.get<Categoria[]>('/api/cadastros/categorias/ativas'),
  listarSubcategoriasAtivasPorCategoria: (categoriaId: string) =>
    httpClient.get(`/api/cadastros/categorias/${categoriaId}/subcategorias/ativas`),
  listarPrioridades: () => httpClient.get<Prioridade[]>('/api/cadastros/prioridades'),
  listarPrioridadesAtivas: () => httpClient.get<Prioridade[]>('/api/cadastros/prioridades/ativas'),
  listarTiposSolicitacaoAtivos: () => httpClient.get('/api/cadastros/tipos-solicitacao/ativos'),
  listarLocaisAtivos: () => httpClient.get('/api/cadastros/locais/ativos'),
  listarStatus: () => httpClient.get<Status[]>('/api/cadastros/status'),
}
