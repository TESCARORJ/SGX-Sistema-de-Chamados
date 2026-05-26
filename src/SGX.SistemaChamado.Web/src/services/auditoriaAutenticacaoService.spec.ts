import { beforeEach, describe, expect, it, vi } from 'vitest'

const getMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
  },
}))

describe('auditoriaAutenticacaoService', () => {
  beforeEach(() => {
    getMock.mockReset()
  })

  it('deve consultar endpoint de auditoria de autenticacao com filtros e paginacao', async () => {
    const { auditoriaAutenticacaoService } = await import('./auditoriaAutenticacaoService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await auditoriaAutenticacaoService.listarEventos({
      dataInicio: '2026-05-01',
      dataFim: '2026-05-31',
      usuarioEmail: 'admin@empresa.com',
      provedor: 'ActiveDirectory',
      tipoEventoAutenticacao: 'LoginActiveDirectorySucesso',
      resultadoAutenticacao: 'Sucesso',
      pagina: 2,
      tamanhoPagina: 15,
    })

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/auditoria/autenticacao?dataInicio=2026-05-01&dataFim=2026-05-31&usuarioEmail=admin%40empresa.com&provedor=ActiveDirectory&tipoEventoAutenticacao=LoginActiveDirectorySucesso&resultadoAutenticacao=Sucesso&pagina=2&tamanhoPagina=15'
    )
  })

  it('nao deve enviar filtros vazios', async () => {
    const { auditoriaAutenticacaoService } = await import('./auditoriaAutenticacaoService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await auditoriaAutenticacaoService.listarEventos({
      usuarioEmail: '   ',
      provedor: '' as any,
      pagina: 1,
      tamanhoPagina: 20,
    })

    expect(getMock).toHaveBeenCalledWith('/api/admin/auditoria/autenticacao?pagina=1&tamanhoPagina=20')
  })
})
