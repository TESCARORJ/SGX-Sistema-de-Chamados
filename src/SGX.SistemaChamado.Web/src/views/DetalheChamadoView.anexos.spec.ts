import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('DetalheChamadoView - anexos', () => {
  it('nao deve renderizar botao de exclusao de anexo', () => {
    const caminho = new URL('./DetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8').toLowerCase()

    expect(fonte).not.toContain('excluir anexo')
    expect(fonte).not.toContain('remover anexo')
    expect(fonte).not.toContain('deletar anexo')
  })
})
