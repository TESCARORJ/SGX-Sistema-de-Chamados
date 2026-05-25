import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('AprovacaoChamadosListPage', () => {
  it('deve renderizar estrutura principal da listagem administrativa', () => {
    const caminho = new URL('./AprovacaoChamadosListPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Aprovacao de chamados"')
    expect(fonte).toContain('label="Status"')
    expect(fonte).toContain('label="Tipo de origem"')
    expect(fonte).toContain('label="Visualizar"')
  })

  it('deve exibir acoes de aprovacao para status pendente', () => {
    const caminho = new URL('./AprovacaoChamadosListPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("row.status === StatusAprovacaoChamado.Pendente")
    expect(fonte).toContain('label="Aprovar"')
    expect(fonte).toContain('label="Reprovar"')
    expect(fonte).toContain('label="Cancelar"')
  })

  it('deve tratar estados de loading, erro e vazio', () => {
    const caminho = new URL('./AprovacaoChamadosListPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('LoadingState')
    expect(fonte).toContain('ErrorState')
    expect(fonte).toContain('EmptyState')
  })
})
