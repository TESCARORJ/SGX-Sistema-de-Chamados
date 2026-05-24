import { httpClient } from './httpClient'
import type { ChamadoAdminDetalhe } from '../types/admin'

export const chamadoInventarioAtivoService = {
  vincularAtivo: (chamadoId: string, ativoId: string) =>
    httpClient.post<ChamadoAdminDetalhe>(`/api/admin/chamados/${chamadoId}/ativo/${ativoId}`),

  removerAtivo: (chamadoId: string) =>
    httpClient.delete<ChamadoAdminDetalhe>(`/api/admin/chamados/${chamadoId}/ativo`),
}
