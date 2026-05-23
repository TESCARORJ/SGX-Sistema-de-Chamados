import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('AdminDetalheChamadoView - base de conhecimento', () => {
  it('deve exibir secao de base de conhecimento no detalhe administrativo', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Base de conhecimento"')
    expect(fonte).toContain('label="Vincular artigo"')
    expect(fonte).toContain('label="Remover vínculo"')
  })

  it('deve consumir service de vinculo, busca e remocao de artigos', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('listarArtigosDoChamado')
    expect(fonte).toContain('buscarArtigosDisponiveisParaVinculo')
    expect(fonte).toContain('vincularArtigoAoChamado')
    expect(fonte).toContain('removerArtigoDoChamado')
  })
})
