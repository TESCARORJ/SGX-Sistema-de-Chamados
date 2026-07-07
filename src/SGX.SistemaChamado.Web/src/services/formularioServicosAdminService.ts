import { httpClient } from './httpClient'
import type { AlterarSituacaoCadastroResponse } from '../types/adminCadastros'
import type {
  AtualizarCampoFormularioServicoRequest,
  AtualizarFormularioServicoRequest,
  AtualizarFormularioServicoVersaoRequest,
  AtualizarOpcaoCampoFormularioServicoRequest,
  CampoFormularioServicoAdminDto,
  CriarCampoFormularioServicoRequest,
  CriarFormularioServicoRequest,
  CriarFormularioServicoVersaoRequest,
  CriarOpcaoCampoFormularioServicoRequest,
  FormularioServicoAdminDto,
  FormularioServicoDetalheAdminDto,
  FormularioServicoVersaoAdminDto,
  ListarFormularioServicoRequest,
  OpcaoCampoFormularioServicoAdminDto,
} from '../types/formularioServicos'

function buildQuery(params: ListarFormularioServicoRequest = {}): string {
  const search = new URLSearchParams()

  if (params.catalogoServicoId) {
    search.set('catalogoServicoId', params.catalogoServicoId)
  }

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const formularioServicosAdminService = {
  listarFormularios: (filtros: ListarFormularioServicoRequest = {}) =>
    httpClient.get<FormularioServicoAdminDto[]>(`/api/admin/formulario-servicos${buildQuery(filtros)}`),

  obterFormulario: (id: string) =>
    httpClient.get<FormularioServicoDetalheAdminDto>(`/api/admin/formulario-servicos/${id}`),

  criarFormulario: (payload: CriarFormularioServicoRequest) =>
    httpClient.post<FormularioServicoDetalheAdminDto>('/api/admin/formulario-servicos', payload),

  atualizarFormulario: (id: string, payload: AtualizarFormularioServicoRequest) =>
    httpClient.put<FormularioServicoDetalheAdminDto>(`/api/admin/formulario-servicos/${id}`, payload),

  inativarFormulario: (id: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/formulario-servicos/${id}/inativar`),

  reativarFormulario: (id: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/formulario-servicos/${id}/reativar`),

  listarVersoes: (formularioId: string) =>
    httpClient.get<FormularioServicoVersaoAdminDto[]>(`/api/admin/formulario-servicos/${formularioId}/versoes`),

  criarVersao: (formularioId: string, payload: CriarFormularioServicoVersaoRequest) =>
    httpClient.post<FormularioServicoVersaoAdminDto>(`/api/admin/formulario-servicos/${formularioId}/versoes`, payload),

  atualizarVersao: (versaoId: string, payload: AtualizarFormularioServicoVersaoRequest) =>
    httpClient.put<FormularioServicoVersaoAdminDto>(`/api/admin/formulario-servicos/versoes/${versaoId}`, payload),

  inativarVersao: (versaoId: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/formulario-servicos/versoes/${versaoId}/inativar`),

  reativarVersao: (versaoId: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/formulario-servicos/versoes/${versaoId}/reativar`),

  listarCampos: (versaoId: string) =>
    httpClient.get<CampoFormularioServicoAdminDto[]>(`/api/admin/formulario-servicos/versoes/${versaoId}/campos`),

  criarCampo: (versaoId: string, payload: CriarCampoFormularioServicoRequest) =>
    httpClient.post<CampoFormularioServicoAdminDto>(`/api/admin/formulario-servicos/versoes/${versaoId}/campos`, payload),

  atualizarCampo: (campoId: string, payload: AtualizarCampoFormularioServicoRequest) =>
    httpClient.put<CampoFormularioServicoAdminDto>(`/api/admin/formulario-servicos/campos/${campoId}`, payload),

  inativarCampo: (campoId: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/formulario-servicos/campos/${campoId}/inativar`),

  reativarCampo: (campoId: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/formulario-servicos/campos/${campoId}/reativar`),

  listarOpcoes: (campoId: string) =>
    httpClient.get<OpcaoCampoFormularioServicoAdminDto[]>(`/api/admin/formulario-servicos/campos/${campoId}/opcoes`),

  criarOpcao: (campoId: string, payload: CriarOpcaoCampoFormularioServicoRequest) =>
    httpClient.post<OpcaoCampoFormularioServicoAdminDto>(`/api/admin/formulario-servicos/campos/${campoId}/opcoes`, payload),

  atualizarOpcao: (opcaoId: string, payload: AtualizarOpcaoCampoFormularioServicoRequest) =>
    httpClient.put<OpcaoCampoFormularioServicoAdminDto>(`/api/admin/formulario-servicos/opcoes/${opcaoId}`, payload),

  inativarOpcao: (opcaoId: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/formulario-servicos/opcoes/${opcaoId}/inativar`),

  reativarOpcao: (opcaoId: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/formulario-servicos/opcoes/${opcaoId}/reativar`),
}
