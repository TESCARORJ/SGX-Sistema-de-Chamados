import { httpClient } from './httpClient'
import type { PagedResponse } from '../types/common'
import type {
  ListarConfiguracoesRegrasAprovacaoFiltro,
  CriarConfiguracaoRegraAprovacaoRequest,
  AtualizarConfiguracaoRegraAprovacaoRequest,
  AlterarStatusConfiguracaoRegraAprovacaoRequest,
  ValidarConfiguracaoRegraAprovacaoRequest,
  ValidarConfiguracaoRegraAprovacaoResponse,
  ConfiguracaoRegraAprovacaoResumoResponse,
  ConfiguracaoRegraAprovacaoResponse,
  ListaConfiguracoesRegrasAprovacaoResponse,
} from '../types/aprovacoesMotor'

const basePath = '/api/admin/regras-aprovacao'

export const configuracoesRegrasAprovacaoService = {
  listar: (params?: ListarConfiguracoesRegrasAprovacaoFiltro) =>
    httpClient.get<ListaConfiguracoesRegrasAprovacaoResponse>(basePath, { params }),

  obterPorId: (id: string) => httpClient.get<ConfiguracaoRegraAprovacaoResponse>(`${basePath}/${id}`),

  criar: (data: CriarConfiguracaoRegraAprovacaoRequest) =>
    httpClient.post<ConfiguracaoRegraAprovacaoResponse>(basePath, data),

  atualizar: (id: string, data: AtualizarConfiguracaoRegraAprovacaoRequest) =>
    httpClient.put<ConfiguracaoRegraAprovacaoResponse>(`${basePath}/${id}`, data),

  atualizarStatus: (id: string, data: AlterarStatusConfiguracaoRegraAprovacaoRequest) =>
    httpClient.patch<void>(`${basePath}/${id}/status`, data),

  validar: (data: ValidarConfiguracaoRegraAprovacaoRequest) =>
    httpClient.post<ValidarConfiguracaoRegraAprovacaoResponse>(`${basePath}/validar`, data),
}
