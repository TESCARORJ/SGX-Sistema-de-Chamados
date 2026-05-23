import { beforeEach, describe, expect, it, vi } from 'vitest'

const getMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
  },
}))

describe('baseConhecimentoPortalService', () => {
  beforeEach(() => {
    getMock.mockReset()
  })

  it('deve montar query da listagem de portal', async () => {
    const { baseConhecimentoPortalService } = await import('./baseConhecimentoPortalService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await baseConhecimentoPortalService.listarArtigos({
      termo: 'senha',
      categoriaId: 'cat-1',
      pagina: 3,
      tamanhoPagina: 12,
    })

    expect(getMock).toHaveBeenCalledWith('/api/portal/base-conhecimento/artigos?termo=senha&categoriaId=cat-1&pagina=3&tamanhoPagina=12')
  })

  it('deve chamar endpoint de detalhe por slug', async () => {
    const { baseConhecimentoPortalService } = await import('./baseConhecimentoPortalService')
    getMock.mockResolvedValueOnce({ id: '1' })

    await baseConhecimentoPortalService.obterArtigoPorSlug('como-configurar-vpn')

    expect(getMock).toHaveBeenCalledWith('/api/portal/base-conhecimento/artigos/como-configurar-vpn')
  })
})
