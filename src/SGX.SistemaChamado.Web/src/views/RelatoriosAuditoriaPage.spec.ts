import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('RelatoriosAuditoriaPage', () => {
  it('deve exibir estrutura de auditoria', () => {
    const caminho = new URL('./RelatoriosAuditoriaPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Relatórios - Auditoria')
    expect(fonte).toContain('Auditoria por usuário')
    expect(fonte).toContain('Auditoria por entidade')
    expect(fonte).toContain('totalAcoesAuditadas')
  })

  it('deve respeitar bloqueio de permissao e estados de feedback', () => {
    const caminho = new URL('./RelatoriosAuditoriaPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('!podeVisualizar || !podeAuditoria')
    expect(fonte).toContain('LoadingState')
    expect(fonte).toContain('ErrorState')
    expect(fonte).toContain('EmptyState')
  })
})
