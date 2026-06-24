import { defineStore } from 'pinia'
import { notificacoesService } from '../services/notificacoesService'

function extrairMensagemErro(error: unknown, fallback: string): string {
  if (!(error instanceof Error)) {
    return fallback
  }

  const mensagem = error.message?.trim()
  return mensagem || fallback
}

export const useNotificacoesStore = defineStore('notificacoes', {
  state: () => ({
    totalNaoLidas: 0,
    carregandoContagem: false,
    erroContagem: null as string | null,
    contagemCarregada: false,
  }),

  actions: {
    async carregarContagem(force = false): Promise<number> {
      if (this.carregandoContagem) {
        return this.totalNaoLidas
      }

      if (this.contagemCarregada && !force) {
        return this.totalNaoLidas
      }

      this.carregandoContagem = true
      this.erroContagem = null

      try {
        const response = await notificacoesService.contarMinhasNotificacoesNaoLidas()
        this.totalNaoLidas = response.totalNaoLidas
        this.contagemCarregada = true
        return this.totalNaoLidas
      } catch (error) {
        this.erroContagem = extrairMensagemErro(error, 'Nao foi possivel carregar a contagem de notificacoes.')
        throw error
      } finally {
        this.carregandoContagem = false
      }
    },

    sincronizarTotal(totalNaoLidas: number): void {
      this.totalNaoLidas = Math.max(0, totalNaoLidas)
      this.contagemCarregada = true
      this.erroContagem = null
    },

    resetar(): void {
      this.totalNaoLidas = 0
      this.carregandoContagem = false
      this.erroContagem = null
      this.contagemCarregada = false
    },
  },
})
