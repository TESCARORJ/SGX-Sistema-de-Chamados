import { beforeEach, describe, expect, it, vi } from 'vitest'

const getMock = vi.fn()
const patchMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
    patch: patchMock,
  },
}))

describe('notificacoesService', () => {
  beforeEach(() => {
    getMock.mockReset()
    patchMock.mockReset()
  })

  it('deve listar notificacoes da caixa propria sem enviar usuarioId', async () => {
    const { notificacoesService } = await import('./notificacoesService')
    getMock.mockResolvedValueOnce({ itens: [], pagina: 1, tamanhoPagina: 10, total: 0, totalPaginas: 0, totalNaoLidas: 0 })

    await notificacoesService.listarMinhasNotificacoes({
      pagina: 2,
      tamanhoPagina: 20,
      lida: false,
    })

    expect(getMock).toHaveBeenCalledWith('/api/notificacoes/minhas?pagina=2&tamanhoPagina=20&lida=false')
    expect(getMock.mock.calls[0]?.[0]).not.toContain('usuarioId')
    expect(getMock.mock.calls[0]?.[0]).not.toContain('destinatarioUsuarioId')
  })

  it('deve consultar detalhe e contagem usando rotas autenticadas da caixa propria', async () => {
    const { notificacoesService } = await import('./notificacoesService')
    getMock.mockResolvedValue({})

    await notificacoesService.obterMinhaNotificacao('notif-1')
    await notificacoesService.contarMinhasNotificacoesNaoLidas()

    expect(getMock).toHaveBeenNthCalledWith(1, '/api/notificacoes/minhas/notif-1')
    expect(getMock).toHaveBeenNthCalledWith(2, '/api/notificacoes/minhas/nao-lidas/contagem')
  })

  it('deve marcar notificacao como lida e nao lida sem payload de ownership', async () => {
    const { notificacoesService } = await import('./notificacoesService')
    patchMock.mockResolvedValue({})

    await notificacoesService.marcarMinhaNotificacaoComoLida('notif-2')
    await notificacoesService.marcarMinhaNotificacaoComoNaoLida('notif-2')

    expect(patchMock).toHaveBeenNthCalledWith(1, '/api/notificacoes/minhas/notif-2/lida')
    expect(patchMock).toHaveBeenNthCalledWith(2, '/api/notificacoes/minhas/notif-2/nao-lida')
    expect(patchMock.mock.calls[0]?.[1]).toBeUndefined()
    expect(patchMock.mock.calls[1]?.[1]).toBeUndefined()
  })
})
