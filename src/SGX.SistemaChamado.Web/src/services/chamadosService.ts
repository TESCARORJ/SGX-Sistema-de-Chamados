import { httpClient } from './httpClient'
import type { Chamado, CriarChamadoRequest } from '../types/chamado'
import type { ApiListResponse } from '../types/common'

export const chamadosService = {
  listar: () => httpClient.get<ApiListResponse<Chamado>>('/api/chamados'),
  obterPorId: (id: string) => httpClient.get<Chamado>(`/api/chamados/${id}`),
  criar: (payload: CriarChamadoRequest) => httpClient.post<Chamado>('/api/chamados', payload),
}
