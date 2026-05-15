import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('DetalheChamadoView - linha do tempo', () => {
  it('deve exibir secao de linha do tempo e eventos de anexo com download', () => {
    const caminho = new URL('./DetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Linha do tempo"')
    expect(fonte).toContain("item.tipo === 'anexo'")
    expect(fonte).toContain('Baixar anexo')
  })

  it('deve manter filtro de eventos internos para solicitante e indicador interno para admin/atendente', () => {
    const caminho = new URL('./DetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('linhaTempo.value.filter((item) => !item.interno)')
    expect(fonte).toContain('item.interno && podeComentarInterno')
  })

  it('deve recarregar a linha do tempo apos comentario e upload de anexo', () => {
    const caminho = new URL('./DetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('await carregar()')
    expect(fonte).toContain('listarLinhaTempoChamado')
  })
})
