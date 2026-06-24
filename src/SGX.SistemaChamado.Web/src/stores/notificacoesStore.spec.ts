import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'

const contarMinhasNotificacoesNaoLidasMock = vi.fn()

vi.mock('../services/notificacoesService', () => ({
  notificacoesService: {
    contarMinhasNotificacoesNaoLidas: contarMinhasNotificacoesNaoLidasMock,
  },
}))

describe('notificacoesStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    contarMinhasNotificacoesNaoLidasMock.mockReset()
  })

  it('deve carregar contagem e sincronizar o estado global', async () => {
    const { useNotificacoesStore } = await import('./notificacoesStore')
    const store = useNotificacoesStore()
    contarMinhasNotificacoesNaoLidasMock.mockResolvedValueOnce({ totalNaoLidas: 4 })

    await store.carregarContagem(true)

    expect(store.totalNaoLidas).toBe(4)
    expect(store.contagemCarregada).toBe(true)
    expect(store.carregandoContagem).toBe(false)
  })

  it('deve propagar erro de contagem sem perder o estado do loading', async () => {
    const { useNotificacoesStore } = await import('./notificacoesStore')
    const store = useNotificacoesStore()
    contarMinhasNotificacoesNaoLidasMock.mockRejectedValueOnce(new Error('falha de rede'))

    await expect(store.carregarContagem(true)).rejects.toThrow('falha de rede')

    expect(store.carregandoContagem).toBe(false)
    expect(store.erroContagem).toContain('falha de rede')
  })

  it('deve permitir sincronizacao local e reset do contador', async () => {
    const { useNotificacoesStore } = await import('./notificacoesStore')
    const store = useNotificacoesStore()

    store.sincronizarTotal(7)
    expect(store.totalNaoLidas).toBe(7)

    store.resetar()
    expect(store.totalNaoLidas).toBe(0)
    expect(store.contagemCarregada).toBe(false)
    expect(store.erroContagem).toBeNull()
  })
})
