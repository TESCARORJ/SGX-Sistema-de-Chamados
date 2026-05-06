import { httpClient } from './httpClient'
import type { Categoria } from '../types/categoria'
import type { Departamento } from '../types/departamento'
import type { Prioridade } from '../types/prioridade'
import type { Status } from '../types/status'

export const cadastrosService = {
  listarDepartamentos: () => httpClient.get<Departamento[]>('/api/cadastros/departamentos'),
  listarCategorias: () => httpClient.get<Categoria[]>('/api/cadastros/categorias'),
  listarPrioridades: () => httpClient.get<Prioridade[]>('/api/cadastros/prioridades'),
  listarStatus: () => httpClient.get<Status[]>('/api/cadastros/status'),
}
