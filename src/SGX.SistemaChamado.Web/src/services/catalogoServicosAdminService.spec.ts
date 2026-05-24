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

describe('catalogoServicosAdminService', () => {
  beforeEach(() => {
    getMock.mockReset()
    postMock.mockReset()
    putMock.mockReset()
  })

  it('deve listar servicos admin com filtros', async () => {
    const { catalogoServicosAdminService } = await import('./catalogoServicosAdminService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await catalogoServicosAdminService.listarServicos({
      termo: 'vpn',
      departamentoResponsavelId: 'dep-1',
      categoriaId: 'cat-1',
      subcategoriaId: 'sub-1',
      status: 1,
      visibilidade: 2,
      ativo: true,
      permiteAberturaChamado: true,
      pagina: 2,
      tamanhoPagina: 10,
      ordenarPor: 'nome',
      direcaoOrdenacao: 'asc',
    })

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/catalogo-servicos?termo=vpn&departamentoResponsavelId=dep-1&categoriaId=cat-1&subcategoriaId=sub-1&status=1&visibilidade=2&ativo=true&permiteAberturaChamado=true&pagina=2&tamanhoPagina=10&ordenarPor=nome&direcaoOrdenacao=asc'
    )
  })

  it('deve obter servico admin por id', async () => {
    const { catalogoServicosAdminService } = await import('./catalogoServicosAdminService')
    getMock.mockResolvedValueOnce({ id: 'srv-1' })

    await catalogoServicosAdminService.obterServico('srv-1')

    expect(getMock).toHaveBeenCalledWith('/api/admin/catalogo-servicos/srv-1')
  })

  it('deve criar servico', async () => {
    const { catalogoServicosAdminService } = await import('./catalogoServicosAdminService')
    postMock.mockResolvedValueOnce({ id: 'srv-1' })

    await catalogoServicosAdminService.criarServico({
      nome: 'Servico',
      descricao: 'Descricao',
      departamentoResponsavelId: 'dep-1',
      visibilidade: 1,
      requerAprovacao: false,
      ordem: 1,
    })

    expect(postMock).toHaveBeenCalledWith('/api/admin/catalogo-servicos', {
      nome: 'Servico',
      descricao: 'Descricao',
      departamentoResponsavelId: 'dep-1',
      visibilidade: 1,
      requerAprovacao: false,
      ordem: 1,
    })
  })

  it('deve atualizar servico', async () => {
    const { catalogoServicosAdminService } = await import('./catalogoServicosAdminService')
    putMock.mockResolvedValueOnce({ id: 'srv-1' })

    await catalogoServicosAdminService.atualizarServico('srv-1', {
      nome: 'Servico atualizado',
      descricao: 'Descricao atualizada',
      departamentoResponsavelId: 'dep-1',
      visibilidade: 2,
      permiteAberturaChamado: true,
      requerAprovacao: false,
      ordem: 2,
      ativo: true,
    })

    expect(putMock).toHaveBeenCalledWith('/api/admin/catalogo-servicos/srv-1', {
      nome: 'Servico atualizado',
      descricao: 'Descricao atualizada',
      departamentoResponsavelId: 'dep-1',
      visibilidade: 2,
      permiteAberturaChamado: true,
      requerAprovacao: false,
      ordem: 2,
      ativo: true,
    })
  })

  it('deve publicar servico', async () => {
    const { catalogoServicosAdminService } = await import('./catalogoServicosAdminService')
    postMock.mockResolvedValueOnce({})

    await catalogoServicosAdminService.publicarServico('srv-1')

    expect(postMock).toHaveBeenCalledWith('/api/admin/catalogo-servicos/srv-1/publicar')
  })

  it('deve arquivar servico', async () => {
    const { catalogoServicosAdminService } = await import('./catalogoServicosAdminService')
    postMock.mockResolvedValueOnce({})

    await catalogoServicosAdminService.arquivarServico('srv-1')

    expect(postMock).toHaveBeenCalledWith('/api/admin/catalogo-servicos/srv-1/arquivar')
  })

  it('deve reativar servico', async () => {
    const { catalogoServicosAdminService } = await import('./catalogoServicosAdminService')
    postMock.mockResolvedValueOnce({})

    await catalogoServicosAdminService.reativarServico('srv-1')

    expect(postMock).toHaveBeenCalledWith('/api/admin/catalogo-servicos/srv-1/reativar')
  })
})
