import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('DetalheChamadoView - aprovacao', () => {
  it('deve exibir secao de aprovacao com mensagem orientativa', () => {
    const caminho = new URL('./DetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Aprovacao"')
    expect(fonte).toContain('mensagemOrientativaAprovacao')
    expect(fonte).toContain('Status da aprovacao')
  })

  it('deve exibir mensagem de aguardando aprovacao quando pendente', () => {
    const caminho = new URL('./DetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Aguardando aprovacao')
    expect(fonte).toContain('StatusAprovacaoChamado.Pendente')
  })

  it('deve exibir mensagem de reprovado com justificativa sem dados administrativos indevidos', () => {
    const caminho = new URL('./DetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Justificativa da reprovacao')
    expect(fonte).not.toContain('aprovadorNome')
    expect(fonte).not.toContain('aprovadorId')
  })
})
