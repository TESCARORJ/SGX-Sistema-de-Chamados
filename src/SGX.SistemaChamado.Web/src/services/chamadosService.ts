import { httpClient } from './httpClient'
import type { Chamado, CriarChamadoRequest } from '../types/chamado'
import type { ApiListResponse } from '../types/common'
import type { ComentarioChamado } from '../types/comentario'
import type { ComentarChamadoPayload } from '../types/portal'
import type { AnexoChamado } from '../types/anexo'
import type { LinhaTempoChamadoResponse } from '../types/linhaTempo'

export const chamadosService = {
  listar: () => httpClient.get<ApiListResponse<Chamado>>('/api/chamados'),
  obterPorId: (id: string) => httpClient.get<Chamado>(`/api/chamados/${id}`),
  criar: (payload: CriarChamadoRequest) => httpClient.post<Chamado>('/api/chamados', payload),
  listarComentariosChamado: (chamadoId: string) =>
    httpClient.get<ComentarioChamado[]>(`/api/chamados/${chamadoId}/comentarios`),
  adicionarComentarioChamado: (chamadoId: string, payload: ComentarChamadoPayload) =>
    httpClient.post<ComentarioChamado>(`/api/chamados/${chamadoId}/comentarios`, payload),
  listarLinhaTempoChamado: (chamadoId: string) =>
    httpClient.get<LinhaTempoChamadoResponse>(`/api/chamados/${chamadoId}/linha-do-tempo`),
  listarAnexosChamado: (chamadoId: string) =>
    httpClient.get<AnexoChamado[]>(`/api/chamados/${chamadoId}/anexos`),
  enviarAnexoChamado: (chamadoId: string, arquivo: File) => {
    const form = new FormData()
    form.append('arquivo', arquivo)
    return httpClient.post<AnexoChamado>(`/api/chamados/${chamadoId}/anexos`, form)
  },
  baixarAnexoChamado: (chamadoId: string, anexoId: string) =>
    httpClient.getFile(`/api/chamados/${chamadoId}/anexos/${anexoId}/download`),
}
