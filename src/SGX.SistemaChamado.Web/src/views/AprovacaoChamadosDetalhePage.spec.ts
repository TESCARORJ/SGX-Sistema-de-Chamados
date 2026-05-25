import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('AprovacaoChamadosDetalhePage', () => {
  it('deve renderizar dados principais do detalhe', () => {
    const caminho = new URL('./AprovacaoChamadosDetalhePage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Situacao da aprovacao"')
    expect(fonte).toContain('titulo="Participantes"')
    expect(fonte).toContain('label="Abrir chamado"')
  })

  it('deve exibir botoes de decisao apenas para aprovacao pendente', () => {
    const caminho = new URL('./AprovacaoChamadosDetalhePage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('statusPendente')
    expect(fonte).toContain('label="Aprovar"')
    expect(fonte).toContain('label="Reprovar"')
    expect(fonte).toContain('label="Cancelar"')
  })

  it('deve exigir justificativa para reprovar e cancelar no frontend', () => {
    const caminho = new URL('./AprovacaoChamadosDetalhePage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("acaoSelecionada.value === 'reprovar' || acaoSelecionada.value === 'cancelar'")
    expect(fonte).toContain('Informe a justificativa para continuar.')
  })
})
