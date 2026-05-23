import { beforeEach, describe, expect, it, vi } from 'vitest'

const getMock = vi.fn()
const postMock = vi.fn()
const deleteMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
    post: postMock,
    delete: deleteMock,
  },
}))

describe('chamadoBaseConhecimentoService', () => {
  beforeEach(() => {
    getMock.mockReset()
    postMock.mockReset()
    deleteMock.mockReset()
  })

  it('deve listar artigos vinculados do chamado', async () => {
    const { chamadoBaseConhecimentoService } = await import('./chamadoBaseConhecimentoService')
    getMock.mockResolvedValueOnce([])

    await chamadoBaseConhecimentoService.listarArtigosDoChamado('ch-1')

    expect(getMock).toHaveBeenCalledWith('/api/admin/chamados/ch-1/artigos-conhecimento')
  })

  it('deve vincular artigo ao chamado', async () => {
    const { chamadoBaseConhecimentoService } = await import('./chamadoBaseConhecimentoService')
    postMock.mockResolvedValueOnce({})

    await chamadoBaseConhecimentoService.vincularArtigoAoChamado('ch-1', 'art-1', 'Usar no retorno ao solicitante')

    expect(postMock).toHaveBeenCalledWith('/api/admin/chamados/ch-1/artigos-conhecimento/art-1', {
      observacao: 'Usar no retorno ao solicitante',
    })
  })

  it('deve remover vinculo de artigo do chamado', async () => {
    const { chamadoBaseConhecimentoService } = await import('./chamadoBaseConhecimentoService')
    deleteMock.mockResolvedValueOnce({})

    await chamadoBaseConhecimentoService.removerArtigoDoChamado('ch-1', 'art-1')

    expect(deleteMock).toHaveBeenCalledWith('/api/admin/chamados/ch-1/artigos-conhecimento/art-1')
  })

  it('deve montar query na busca de artigos disponiveis', async () => {
    const { chamadoBaseConhecimentoService } = await import('./chamadoBaseConhecimentoService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await chamadoBaseConhecimentoService.buscarArtigosDisponiveisParaVinculo('ch-1', {
      termo: 'vpn',
      categoriaId: 'cat-1',
      page: 2,
      pageSize: 8,
    })

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/chamados/ch-1/artigos-conhecimento/disponiveis?termo=vpn&categoriaId=cat-1&page=2&pageSize=8'
    )
  })
})
