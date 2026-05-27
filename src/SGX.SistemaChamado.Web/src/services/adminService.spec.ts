import { beforeEach, describe, expect, it, vi } from 'vitest'

const getMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
    post: vi.fn(),
    delete: vi.fn(),
    put: vi.fn(),
    patch: vi.fn(),
  },
}))

describe('adminService', () => {
  beforeEach(() => {
    getMock.mockReset()
  })

  it('deve enviar naturezaChamado na listagem admin quando filtro for informado', async () => {
    const { adminService } = await import('./adminService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await adminService.listarChamadosAdmin({
      naturezaChamado: 1,
      pagina: 1,
      tamanhoPagina: 20,
    })

    const url = String(getMock.mock.calls[0]?.[0] ?? '')
    expect(url).toContain('/api/admin/chamados?')
    expect(url).toContain('naturezaChamado=1')
  })

  it('nao deve enviar naturezaChamado quando filtro for Todos', async () => {
    const { adminService } = await import('./adminService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await adminService.listarChamadosAdmin({
      pagina: 1,
      tamanhoPagina: 20,
    })

    const url = String(getMock.mock.calls[0]?.[0] ?? '')
    expect(url).toContain('/api/admin/chamados?')
    expect(url).not.toContain('naturezaChamado=')
  })
})
