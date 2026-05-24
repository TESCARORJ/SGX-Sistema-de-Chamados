import { beforeEach, describe, expect, it, vi } from 'vitest'

const getMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
  },
}))

describe('catalogoServicosPortalService', () => {
  beforeEach(() => {
    getMock.mockReset()
  })

  it('deve listar catalogo do portal com filtros', async () => {
    const { catalogoServicosPortalService } = await import('./catalogoServicosPortalService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await catalogoServicosPortalService.listarServicos({
      termo: 'vpn',
      departamentoResponsavelId: 'dep-1',
      categoriaId: 'cat-1',
      subcategoriaId: 'sub-1',
      permiteAberturaChamado: true,
      pagina: 3,
      tamanhoPagina: 12,
    })

    expect(getMock).toHaveBeenCalledWith(
      '/api/portal/catalogo-servicos?termo=vpn&departamentoResponsavelId=dep-1&categoriaId=cat-1&subcategoriaId=sub-1&permiteAberturaChamado=true&pagina=3&tamanhoPagina=12'
    )
  })

  it('deve obter servico do portal por slug', async () => {
    const { catalogoServicosPortalService } = await import('./catalogoServicosPortalService')
    getMock.mockResolvedValueOnce({ id: 'srv-1' })

    await catalogoServicosPortalService.obterServicoPorSlug('solicitar-vpn')

    expect(getMock).toHaveBeenCalledWith('/api/portal/catalogo-servicos/solicitar-vpn')
  })

  it('deve preparar abertura de chamado por slug', async () => {
    const { catalogoServicosPortalService } = await import('./catalogoServicosPortalService')
    getMock.mockResolvedValueOnce({ catalogoServicoId: 'srv-1' })

    await catalogoServicosPortalService.prepararAberturaChamado('solicitar-vpn')

    expect(getMock).toHaveBeenCalledWith('/api/portal/catalogo-servicos/solicitar-vpn/preparar-chamado')
  })
})
