import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('AdminDetalheChamadoView - aprovacao', () => {
  it('deve exibir secao de aprovacao no detalhe administrativo', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Aprovacao legada"')
    expect(fonte).toContain('titulo="Aprovacao do motor ITSM"')
    expect(fonte).toContain('Requer aprovacao')
    expect(fonte).toContain('Aprovacao pendente')
    expect(fonte).toContain('Status da aprovacao')
  })

  it('deve permitir acoes rapidas e solicitacao manual quando aplicavel', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('label="Solicitar aprovacao"')
    expect(fonte).toContain('label="Aprovar"')
    expect(fonte).toContain('label="Reprovar"')
    expect(fonte).toContain('label="Cancelar"')
    expect(fonte).toContain('Solicitar aprovacao manual')
  })
})
