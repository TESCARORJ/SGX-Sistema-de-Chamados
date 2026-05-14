import { httpClient } from './httpClient'
import type {
  AtualizarMicrosoftEntraIdIntegracaoRequest,
  MicrosoftEntraIdIntegracaoResponse,
} from '../types/integracoesMicrosoft'

export const integracoesMicrosoftService = {
  obterConfiguracao: () =>
    httpClient.get<MicrosoftEntraIdIntegracaoResponse>('/api/admin/integracoes/microsoft-entra-id'),

  atualizarConfiguracao: (payload: AtualizarMicrosoftEntraIdIntegracaoRequest) =>
    httpClient.put<MicrosoftEntraIdIntegracaoResponse>('/api/admin/integracoes/microsoft-entra-id', payload),
}

