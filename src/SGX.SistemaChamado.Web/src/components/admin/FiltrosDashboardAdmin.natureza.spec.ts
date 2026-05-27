import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('FiltrosDashboardAdmin - natureza', () => {
  it('deve exibir filtro de Natureza ITSM no dashboard administrativo', () => {
    const caminho = new URL('./FiltrosDashboardAdmin.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('label="Natureza ITSM"')
    expect(fonte).toContain('NaturezaChamado.EventoAlerta')
    expect(fonte).toContain('NaturezaChamado.TarefaOperacional')
  })
})
