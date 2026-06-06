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

  it('deve exibir atendimento com fallback para grupo e fila', () => {
    const caminho = new URL('./TabelaChamados.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("name: 'atendimento'")
    expect(fonte).toContain("label: 'Atendimento'")
    expect(fonte).toContain("slotProps.row.grupoTecnicoNome || 'Sem grupo'")
    expect(fonte).toContain("slotProps.row.filaAtendimentoNome || 'Sem fila'")
    expect(fonte).toContain("Grupo: {{ slotProps.row.grupoTecnicoNome || 'Sem grupo' }}")
    expect(fonte).toContain("Fila: {{ slotProps.row.filaAtendimentoNome || 'Sem fila' }}")
  })

  it('nao deve criar acoes operacionais novas de grupo e fila na listagem', () => {
    const caminho = new URL('./TabelaChamados.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).not.toContain('Transferir grupo')
    expect(fonte).not.toContain('Direcionar grupo')
    expect(fonte).not.toContain('Atribuir tecnico')
    expect(fonte).not.toContain("emit('transferir'")
    expect(fonte).not.toContain("emit('direcionar'")
    expect(fonte).not.toContain("emit('atribuir'")
  })
})
