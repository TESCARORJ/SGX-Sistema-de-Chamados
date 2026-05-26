import { httpClient } from './httpClient'
import type {
  ActiveDirectoryIntegracaoResponse,
  AtualizarActiveDirectoryIntegracaoRequest,
  TestarAutenticacaoActiveDirectoryRequest,
  TestarAutenticacaoActiveDirectoryResponse,
  TestarConexaoActiveDirectoryRequest,
  TestarConexaoActiveDirectoryResponse,
} from '../types/integracoesActiveDirectory'

export const integracoesActiveDirectoryService = {
  obterConfiguracao: () =>
    httpClient.get<ActiveDirectoryIntegracaoResponse>('/api/admin/integracoes/active-directory'),

  atualizarConfiguracao: (payload: AtualizarActiveDirectoryIntegracaoRequest) =>
    httpClient.put<ActiveDirectoryIntegracaoResponse>('/api/admin/integracoes/active-directory', payload),

  testarConexao: (payload: TestarConexaoActiveDirectoryRequest) =>
    httpClient.post<TestarConexaoActiveDirectoryResponse>(
      '/api/admin/integracoes/active-directory/testar-conexao',
      payload
    ),

  testarAutenticacao: (payload: TestarAutenticacaoActiveDirectoryRequest) =>
    httpClient.post<TestarAutenticacaoActiveDirectoryResponse>(
      '/api/admin/integracoes/active-directory/testar-autenticacao',
      payload
    ),
}
