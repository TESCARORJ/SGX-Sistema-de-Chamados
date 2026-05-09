import { defineStore } from 'pinia'
import type { Notificacao } from '../types/notificacao'

function criarNotificacoesMock(): Notificacao[] {
  return [
    {
      id: 'ntf-1',
      titulo: 'Novo chamado aberto',
      descricao: '#CHM-2026-000128 - Problema de acesso ao sistema financeiro',
      tipo: 'info',
      lida: false,
      dataHora: '2026-05-07T10:28:00-03:00',
      tempoRelativo: 'há 2 min',
      chamadoCodigo: 'CHM-2026-000128',
    },
    {
      id: 'ntf-2',
      titulo: 'SLA próximo do vencimento',
      descricao: '#CHM-2026-000121 vence em 45 minutos',
      tipo: 'warning',
      lida: false,
      dataHora: '2026-05-07T10:22:00-03:00',
      tempoRelativo: 'há 8 min',
      chamadoCodigo: 'CHM-2026-000121',
    },
    {
      id: 'ntf-3',
      titulo: 'SLA vencido',
      descricao: '#CHM-2026-000119 ultrapassou o prazo de resolução',
      tipo: 'negative',
      lida: false,
      dataHora: '2026-05-07T10:15:00-03:00',
      tempoRelativo: 'há 15 min',
      chamadoCodigo: 'CHM-2026-000119',
    },
    {
      id: 'ntf-4',
      titulo: 'Novo comentário do solicitante',
      descricao: 'Ana Paula respondeu no chamado #CHM-2026-000117',
      tipo: 'message',
      lida: true,
      dataHora: '2026-05-07T10:08:00-03:00',
      tempoRelativo: 'há 22 min',
      chamadoCodigo: 'CHM-2026-000117',
      remetente: 'Ana Paula',
    },
    {
      id: 'ntf-5',
      titulo: 'Chamado atribuído a você',
      descricao: '#CHM-2026-000115 foi atribuído para Atendente Demo',
      tipo: 'assignment',
      lida: true,
      dataHora: '2026-05-07T09:50:00-03:00',
      tempoRelativo: 'há 40 min',
      chamadoCodigo: 'CHM-2026-000115',
    },
    {
      id: 'ntf-6',
      titulo: 'E-mail processado',
      descricao: 'Novo chamado criado a partir de suporte@empresa.com.br',
      tipo: 'email',
      lida: true,
      dataHora: '2026-05-07T09:30:00-03:00',
      tempoRelativo: 'há 1 h',
      remetente: 'suporte@empresa.com.br',
    },
    {
      id: 'ntf-7',
      titulo: 'Falha ao processar e-mail',
      descricao: 'Não foi possível processar mensagem recebida via IMAP',
      tipo: 'negative',
      lida: false,
      dataHora: '2026-05-07T09:12:00-03:00',
      tempoRelativo: 'há 1 h',
      detalheTecnico: 'Erro de autenticação ou timeout no servidor IMAP',
    },
    {
      id: 'ntf-8',
      titulo: 'Chamado encerrado',
      descricao: '#CHM-2026-000104 foi encerrado por Carlos Augusto',
      tipo: 'positive',
      lida: true,
      dataHora: '2026-05-07T08:40:00-03:00',
      tempoRelativo: 'há 2 h',
      chamadoCodigo: 'CHM-2026-000104',
      remetente: 'Carlos Augusto',
    },
  ]
}

export const useNotificacoesStore = defineStore('notificacoes', {
  state: () => ({
    notificacoes: [] as Notificacao[],
    loading: false,
    erro: null as string | null,
    carregado: false,
  }),

  getters: {
    naoLidas(state): Notificacao[] {
      return state.notificacoes.filter((notificacao) => !notificacao.lida)
    },
    totalNaoLidas(): number {
      return this.naoLidas.length
    },
    notificacoesOrdenadas(state): Notificacao[] {
      return [...state.notificacoes].sort(
        (a, b) => new Date(b.dataHora).getTime() - new Date(a.dataHora).getTime()
      )
    },
  },

  actions: {
    async carregarNotificacoes(force = false): Promise<void> {
      if (this.loading) {
        return
      }

      if (this.carregado && !force) {
        return
      }

      this.loading = true
      this.erro = null

      try {
        const origem = criarNotificacoesMock()
        const leituraAtual = new Map(this.notificacoes.map((notificacao) => [notificacao.id, notificacao.lida]))
        this.notificacoes = origem.map((notificacao) => ({
          ...notificacao,
          lida: leituraAtual.get(notificacao.id) ?? notificacao.lida,
        }))
        this.carregado = true
      } catch (error) {
        this.erro = error instanceof Error ? error.message : 'Não foi possível carregar os dados.'
        this.notificacoes = []
      } finally {
        this.loading = false
      }
    },

    async atualizar(): Promise<void> {
      await this.carregarNotificacoes(true)
    },

    obterPorId(id: string): Notificacao | undefined {
      return this.notificacoes.find((notificacao) => notificacao.id === id)
    },

    marcarComoLida(id: string): void {
      const notificacao = this.notificacoes.find((item) => item.id === id)
      if (!notificacao || notificacao.lida) {
        return
      }

      notificacao.lida = true
    },

    marcarTodasComoLidas(): void {
      this.notificacoes = this.notificacoes.map((notificacao) => ({
        ...notificacao,
        lida: true,
      }))
    },
  },
})
