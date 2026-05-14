import { describe, expect, it } from 'vitest'
import {
  deveExigirCamposMicrosoft,
  obterAvisoModo,
  validarIntegracaoMicrosoft,
} from './validacaoIntegracaoMicrosoftEntraId'

describe('validacaoIntegracaoMicrosoftEntraId', () => {
  it('deve indicar campos Microsoft obrigatórios quando integração estiver habilitada em modo Microsoft', () => {
    expect(deveExigirCamposMicrosoft(true, 'MicrosoftEntraId')).toBe(true)
    expect(deveExigirCamposMicrosoft(true, 'Hibrido')).toBe(true)
    expect(deveExigirCamposMicrosoft(false, 'MicrosoftEntraId')).toBe(false)
    expect(deveExigirCamposMicrosoft(true, 'Local')).toBe(false)
  })

  it('deve retornar mensagens por campo ao validar configuração Microsoft incompleta', () => {
    const validacao = validarIntegracaoMicrosoft({
      habilitado: true,
      provedorPrincipal: 'MicrosoftEntraId',
      loginLocalHabilitado: false,
      tenantId: '',
      clientId: '',
      audience: '',
      issuer: '',
      authority: '',
      apiScope: '',
      redirectUri: '',
      dominiosPermitidos: [],
      criarUsuarioAutomaticamente: true,
      perfilPadraoUsuarioMicrosoft: 'Solicitante',
    })

    expect(validacao.valido).toBe(false)
    expect(validacao.errosCampo.tenantId).toContain('Tenant ID')
    expect(validacao.errosCampo.clientId).toContain('Client ID')
    expect(validacao.errosCampo.audience).toContain('Audience')
    expect(validacao.errosCampo.issuer).toContain('Issuer')
    expect(validacao.errosCampo.authority).toContain('Authority')
    expect(validacao.errosCampo.apiScope).toContain('API Scope')
    expect(validacao.errosCampo.redirectUri).toContain('Redirect URI')
  })

  it('deve retornar erro específico para modo Local sem login local habilitado', () => {
    const validacao = validarIntegracaoMicrosoft({
      habilitado: false,
      provedorPrincipal: 'Local',
      loginLocalHabilitado: false,
      tenantId: '',
      clientId: '',
      audience: '',
      issuer: '',
      authority: '',
      apiScope: '',
      redirectUri: '',
      dominiosPermitidos: [],
      criarUsuarioAutomaticamente: true,
      perfilPadraoUsuarioMicrosoft: 'Solicitante',
    })

    expect(validacao.valido).toBe(false)
    expect(validacao.erroGeral).toBe('Login local SGX deve permanecer habilitado quando o modo Local estiver selecionado.')
  })

  it('deve retornar aviso correto para modo Local', () => {
    expect(obterAvisoModo('Local')).toBe(
      'Modo Local selecionado. O login Microsoft não será exibido para os usuários.'
    )
  })

  it('deve retornar aviso correto para modo Híbrido', () => {
    expect(obterAvisoModo('Hibrido')).toBe(
      'Modo Híbrido selecionado. O usuário poderá entrar com Microsoft Entra ID ou com login local SGX.'
    )
  })

  it('deve retornar aviso correto para modo MicrosoftEntraId', () => {
    expect(obterAvisoModo('MicrosoftEntraId')).toBe(
      'Modo Microsoft Entra ID selecionado. Garanta que a configuração esteja completa antes de salvar.'
    )
  })
})
