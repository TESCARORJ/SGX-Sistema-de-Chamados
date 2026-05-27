import { beforeEach, describe, expect, it, vi } from 'vitest'

const getMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
  },
}))

describe('dashboardAdminService', () => {
  beforeEach(() => {
    getMock.mockReset()
  })

  it('deve enviar naturezaChamado nos filtros do dashboard quando informado', async () => {
    const { dashboardAdminService } = await import('./dashboardAdminService')
    getMock.mockResolvedValueOnce({})

    await dashboardAdminService.obterDashboard({
      dataInicio: '2026-01-01',
      dataFim: '2026-01-31',
      naturezaChamado: 3,
    })

    const url = String(getMock.mock.calls[0]?.[0] ?? '')
    expect(url).toContain('/api/admin/dashboard?')
    expect(url).toContain('naturezaChamado=3')
  })

  it('nao deve enviar naturezaChamado quando filtro nao for informado', async () => {
    const { dashboardAdminService } = await import('./dashboardAdminService')
    getMock.mockResolvedValueOnce({})

    await dashboardAdminService.obterDashboard({ dataInicio: '2026-01-01' })

    const url = String(getMock.mock.calls[0]?.[0] ?? '')
    expect(url).toContain('/api/admin/dashboard?')
    expect(url).not.toContain('naturezaChamado=')
  })
})
