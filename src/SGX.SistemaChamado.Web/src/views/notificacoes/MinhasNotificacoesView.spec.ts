import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('MinhasNotificacoesView', () => {
  it('deve manter a central segura e responsiva', () => {
    const caminho = new URL('./MinhasNotificacoesView.vue', import.meta.url)
    const cardPath = new URL('../../components/notificacoes/NotificacaoCard.vue', import.meta.url)
    const dialogPath = new URL('../../components/notificacoes/NotificacaoDetalheDialog.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')
    const cardSource = readFileSync(cardPath, 'utf-8')
    const dialogSource = readFileSync(dialogPath, 'utf-8')

    expect(fonte).toContain('Notifica')
    expect(fonte).toContain('nao-lidas')
    expect(fonte).toContain("'lidas'")
    expect(fonte).toContain('q-pagination')
    expect(cardSource).toContain('Marcar como lida')
    expect(cardSource).toContain('Marcar como não lida')
    expect(dialogSource).toContain('white-space: pre-wrap')
    expect(fonte).not.toContain('v-html')
    expect(dialogSource).not.toContain('v-html')
    expect(dialogSource).not.toContain('chaveCorrelacao')
    expect(fonte).not.toContain('usuarioId')
    expect(fonte).not.toContain('destinatarioUsuarioId')
    expect(fonte).toContain('$q.notify({')
  })

  it('deve expor a rota autenticada no portal e na area administrativa com badge no layout', () => {
    const routerPath = new URL('../../router/index.ts', import.meta.url)
    const portalLayoutPath = new URL('../../layouts/PortalLayout.vue', import.meta.url)
    const adminLayoutPath = new URL('../../layouts/AdminLayout.vue', import.meta.url)

    const routerSource = readFileSync(routerPath, 'utf-8')
    const portalLayoutSource = readFileSync(portalLayoutPath, 'utf-8')
    const adminLayoutSource = readFileSync(adminLayoutPath, 'utf-8')

    expect(routerSource).toContain("path: 'notificacoes'")
    expect(routerSource).toContain("name: 'portal-notificacoes'")
    expect(routerSource).toContain("name: 'admin-notificacoes'")
    expect(portalLayoutSource).toContain('NotificacoesBadge')
    expect(adminLayoutSource).toContain('NotificacoesBadge')
  })
})
