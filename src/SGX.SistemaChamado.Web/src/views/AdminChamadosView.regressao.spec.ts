import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('AdminChamadosView - regressao abertura e atribuicao', () => {
  it('deve manter acao legado de assumir chamado na listagem sem usar fluxo de fila', () => {
    const fonte = readFileSync(new URL('./AdminChamadosView.vue', import.meta.url), 'utf-8')

    expect(fonte).toContain('async function assumir(id: string): Promise<void> {')
    expect(fonte).toContain('await adminService.assumirChamado(id)')
    expect(fonte).toContain('await carregarChamados()')
    expect(fonte).toContain('@assumir="assumir"')
    expect(fonte).not.toContain('assumirChamadoFila(id')
    expect(fonte).not.toContain('transferirGrupoTecnicoChamado(id')
  })
})
