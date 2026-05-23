import { beforeEach, describe, expect, it, vi } from 'vitest'

const getMock = vi.fn()
const postMock = vi.fn()
const putMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
    post: postMock,
    put: putMock,
  },
}))

describe('baseConhecimentoAdminService', () => {
  beforeEach(() => {
    getMock.mockReset()
    postMock.mockReset()
    putMock.mockReset()
  })

  it('deve montar query de listagem com filtros', async () => {
    const { baseConhecimentoAdminService } = await import('./baseConhecimentoAdminService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await baseConhecimentoAdminService.listarArtigos({
      termo: 'vpn',
      status: 1,
      visibilidade: 2,
      categoriaId: 'cat-1',
      ativo: true,
      pagina: 2,
      tamanhoPagina: 10,
      ordenarPor: 'criadoEm',
      direcaoOrdenacao: 'asc',
    })

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/base-conhecimento/artigos?termo=vpn&status=1&visibilidade=2&categoriaId=cat-1&ativo=true&pagina=2&tamanhoPagina=10&ordenarPor=criadoEm&direcaoOrdenacao=asc'
    )
  })

  it('deve chamar endpoint de criacao de artigo', async () => {
    const { baseConhecimentoAdminService } = await import('./baseConhecimentoAdminService')
    postMock.mockResolvedValueOnce({ id: '1' })

    await baseConhecimentoAdminService.criarArtigo({
      titulo: 'Titulo',
      conteudo: 'Conteudo',
      visibilidade: 1,
    })

    expect(postMock).toHaveBeenCalledWith('/api/admin/base-conhecimento/artigos', {
      titulo: 'Titulo',
      conteudo: 'Conteudo',
      visibilidade: 1,
    })
  })

  it('deve chamar endpoint de publicar artigo', async () => {
    const { baseConhecimentoAdminService } = await import('./baseConhecimentoAdminService')
    postMock.mockResolvedValueOnce({})

    await baseConhecimentoAdminService.publicarArtigo('art-1')

    expect(postMock).toHaveBeenCalledWith('/api/admin/base-conhecimento/artigos/art-1/publicar')
  })
})
