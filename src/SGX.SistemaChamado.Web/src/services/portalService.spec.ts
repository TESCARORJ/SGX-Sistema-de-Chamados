import { beforeEach, describe, expect, it, vi } from 'vitest'

const getMock = vi.fn()
const postMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
    post: postMock,
  },
}))

describe('portalService', () => {
  beforeEach(() => {
    getMock.mockReset()
    postMock.mockReset()
  })

  it('deve enviar catalogoServicoId na criacao de chamado quando informado', async () => {
    const { portalService } = await import('./portalService')
    postMock.mockResolvedValueOnce({ id: 'chamado-1', codigo: 'CH-1' })

    await portalService.criarChamado({
      titulo: 'Solicitar VPN',
      descricao: 'Preciso de acesso remoto',
      catalogoServicoId: 'srv-1',
      categoriaId: 'cat-1',
      prioridadeId: 'prio-1',
    })

    expect(postMock).toHaveBeenCalledWith('/api/portal/chamados', {
      titulo: 'Solicitar VPN',
      descricao: 'Preciso de acesso remoto',
      catalogoServicoId: 'srv-1',
      categoriaId: 'cat-1',
      prioridadeId: 'prio-1',
    })
  })

  it('deve consultar status de aprovacao de chamado no portal', async () => {
    const { portalService } = await import('./portalService')
    getMock.mockResolvedValueOnce({ chamadoId: 'ch-1', requerAprovacao: true })

    await portalService.obterStatusAprovacaoChamado('ch-1')

    expect(getMock).toHaveBeenCalledWith('/api/portal/chamados/ch-1/aprovacao')
  })
})
