import type { ProvedorPrincipal } from '../../types/auth'
import type { AtualizarMicrosoftEntraIdIntegracaoRequest } from '../../types/integracoesMicrosoft'

export interface ValidacaoIntegracaoMicrosoftResultado {
  valido: boolean
  erroGeral: string | null
  errosCampo: Record<string, string>
}

const CAMPOS_OBRIGATORIOS_MICROSOFT: ReadonlyArray<{
  campo: keyof Pick<
    AtualizarMicrosoftEntraIdIntegracaoRequest,
    'tenantId' | 'clientId' | 'audience' | 'issuer' | 'authority' | 'apiScope' | 'redirectUri'
  >
  mensagem: string
}> = [
  { campo: 'tenantId', mensagem: 'Tenant ID é obrigatório quando a integração Microsoft está habilitada.' },
  { campo: 'clientId', mensagem: 'Client ID é obrigatório quando a integração Microsoft está habilitada.' },
  { campo: 'audience', mensagem: 'Audience é obrigatória quando a integração Microsoft está habilitada.' },
  { campo: 'issuer', mensagem: 'Issuer é obrigatório quando a integração Microsoft está habilitada.' },
  { campo: 'authority', mensagem: 'Authority é obrigatória quando a integração Microsoft está habilitada.' },
  { campo: 'apiScope', mensagem: 'API Scope é obrigatório quando a integração Microsoft está habilitada.' },
  { campo: 'redirectUri', mensagem: 'Redirect URI é obrigatório quando a integração Microsoft está habilitada.' },
]

export function usaMicrosoftNoFluxo(provedorPrincipal: ProvedorPrincipal): boolean {
  return provedorPrincipal === 'MicrosoftEntraId' || provedorPrincipal === 'Hibrido'
}

export function usaLoginLocalNoFluxo(provedorPrincipal: ProvedorPrincipal): boolean {
  return provedorPrincipal === 'Local' || provedorPrincipal === 'Hibrido'
}

export function deveExigirCamposMicrosoft(
  habilitado: boolean,
  provedorPrincipal: ProvedorPrincipal
): boolean {
  return habilitado && usaMicrosoftNoFluxo(provedorPrincipal)
}

export function obterAvisoModo(provedorPrincipal: ProvedorPrincipal): string {
  if (provedorPrincipal === 'Local') {
    return 'Modo Local selecionado. O login Microsoft não será exibido para os usuários.'
  }

  if (provedorPrincipal === 'Hibrido') {
    return 'Modo Híbrido selecionado. O usuário poderá entrar com Microsoft Entra ID ou com login local SGX.'
  }

  return 'Modo Microsoft Entra ID selecionado. Garanta que a configuração esteja completa antes de salvar.'
}

export function validarIntegracaoMicrosoft(
  payload: AtualizarMicrosoftEntraIdIntegracaoRequest
): ValidacaoIntegracaoMicrosoftResultado {
  const errosCampo: Record<string, string> = {}
  const microsoftAtivo = payload.habilitado && usaMicrosoftNoFluxo(payload.provedorPrincipal)
  const localAtivo = payload.loginLocalHabilitado && usaLoginLocalNoFluxo(payload.provedorPrincipal)

  if (payload.provedorPrincipal === 'Local' && !payload.loginLocalHabilitado) {
    return {
      valido: false,
      erroGeral: 'Login local SGX deve permanecer habilitado quando o modo Local estiver selecionado.',
      errosCampo,
    }
  }

  if (!microsoftAtivo && !localAtivo) {
    return {
      valido: false,
      erroGeral: 'Ao menos um provedor de autenticação deve permanecer habilitado.',
      errosCampo,
    }
  }

  if (!microsoftAtivo) {
    return {
      valido: true,
      erroGeral: null,
      errosCampo,
    }
  }

  for (const campoObrigatorio of CAMPOS_OBRIGATORIOS_MICROSOFT) {
    const valor = payload[campoObrigatorio.campo]
    if (typeof valor !== 'string' || !valor.trim()) {
      errosCampo[campoObrigatorio.campo] = campoObrigatorio.mensagem
    }
  }

  return {
    valido: Object.keys(errosCampo).length === 0,
    erroGeral: null,
    errosCampo,
  }
}
