import { beforeEach, describe, expect, it, vi } from 'vitest'

const postMock = vi.fn()
const deleteMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    post: postMock,
    delete: deleteMock,
  },
}))

describe('chamadoInventarioAtivoService', () => {
  beforeEach(() => {
    postMock.mockReset()
    deleteMock.mockReset()
  })

  it('deve vincular ativo ao chamado', async () => {
    const { chamadoInventarioAtivoService } = await import('./chamadoInventarioAtivoService')
    postMock.mockResolvedValueOnce({})

    await chamadoInventarioAtivoService.vincularAtivo('ch-1', 'atv-1')

    expect(postMock).toHaveBeenCalledWith('/api/admin/chamados/ch-1/ativo/atv-1')
  })

  it('deve remover ativo do chamado', async () => {
    const { chamadoInventarioAtivoService } = await import('./chamadoInventarioAtivoService')
    deleteMock.mockResolvedValueOnce({})

    await chamadoInventarioAtivoService.removerAtivo('ch-1')

    expect(deleteMock).toHaveBeenCalledWith('/api/admin/chamados/ch-1/ativo')
  })
})
