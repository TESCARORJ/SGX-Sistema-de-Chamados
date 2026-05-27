import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('TabelaChamados - ITSM', () => {
  it('deve exibir classificacao ITSM na listagem administrativa', () => {
    const caminho = new URL('./TabelaChamados.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("name: 'itsm'")
    expect(fonte).toContain('Classificação ITSM')
    expect(fonte).toContain('labelNaturezaChamado')
    expect(fonte).toContain('labelImpactoChamado')
    expect(fonte).toContain('labelUrgenciaChamado')
  })
})
