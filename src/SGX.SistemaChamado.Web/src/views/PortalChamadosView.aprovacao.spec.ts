import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('PortalChamadosView - aprovacao', () => {
  it('deve exibir indicador visual para chamados com aprovacao pendente', () => {
    const caminho = new URL('./PortalChamadosView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('textoStatusAprovacao')
    expect(fonte).toContain('Aguardando aprovacao')
    expect(fonte).toContain('body-cell-aprovacao')
  })
})
