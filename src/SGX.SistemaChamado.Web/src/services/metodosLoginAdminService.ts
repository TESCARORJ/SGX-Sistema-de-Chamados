import { httpClient } from './httpClient'
import type {
  AtualizarMetodosLoginAdminRequest,
  MetodosLoginAdminResponse,
} from '../types/metodosLogin'

export const metodosLoginAdminService = {
  obterConfiguracao: () =>
    httpClient.get<MetodosLoginAdminResponse>('/api/admin/autenticacao/provedores'),

  atualizarConfiguracao: (payload: AtualizarMetodosLoginAdminRequest) =>
    httpClient.put<MetodosLoginAdminResponse>('/api/admin/autenticacao/provedores', payload),
}

