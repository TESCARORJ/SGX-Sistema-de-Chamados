import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('CadastroDetalheBaseView - Status ITSM', () => {
  it('deve listar os novos status especificos na selecao de codigo', () => {
    const caminho = new URL('./CadastroDetalheBaseView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("{ label: 'Em Analise', value: 7 }")
    expect(fonte).toContain("{ label: 'Aguardando Aprovacao', value: 8 }")
    expect(fonte).toContain("{ label: 'Aprovada', value: 9 }")
    expect(fonte).toContain("{ label: 'Reprovada', value: 10 }")
    expect(fonte).toContain("{ label: 'Em Execucao', value: 11 }")
    expect(fonte).toContain("{ label: 'Concluida', value: 12 }")
    expect(fonte).toContain("{ label: 'Causa Raiz Identificada', value: 13 }")
    expect(fonte).toContain("{ label: 'Solucao de Contorno', value: 14 }")
    expect(fonte).toContain("{ label: 'Correlacionado', value: 15 }")
    expect(fonte).toContain("{ label: 'Tratado', value: 16 }")
    expect(fonte).toContain("{ label: 'Planejada', value: 17 }")
  })
})
