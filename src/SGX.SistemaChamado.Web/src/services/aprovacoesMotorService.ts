import { httpClient } from './httpClient'
import type { InstanciaAprovacaoChamadoResumoResponse, AprovarAprovacaoChamadoRequest, AprovarAprovacaoChamadoResponse, ReprovarAprovacaoChamadoRequest, ReprovarAprovacaoChamadoResponse } from '../types/aprovacoesMotor'
import type { PagedResponse } from '../types/common'

export const aprovacoesMotorService = {
  listarPendenciasPorChamado: (chamadoId: string) =>
    httpClient.get<PagedResponse<InstanciaAprovacaoChamadoResumoResponse>>(`/api/admin/aprovacoes-motor/chamados/${chamadoId}/pendencias`),

  listarMinhasPendencias: (params?: Record<string, any>) =>
    httpClient.get<PagedResponse<InstanciaAprovacaoChamadoResumoResponse>>('/api/admin/aprovacoes-motor/pendencias/minhas', { params }),

  aprovarAprovacao: (request: AprovarAprovacaoChamadoRequest) =>
    httpClient.post<AprovarAprovacaoChamadoResponse>('/api/admin/aprovacoes-motor/aprovar', request),

  reprovarAprovacao: (request: ReprovarAprovacaoChamadoRequest) =>
    httpClient.post<ReprovarAprovacaoChamadoResponse>('/api/admin/aprovacoes-motor/reprovar', request),
}
