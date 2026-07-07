import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('DetalheChamadoView - respostas do formulario', () => {
  function lerFonte(): string {
    const caminho = new URL('./DetalheChamadoView.vue', import.meta.url)
    return readFileSync(caminho, 'utf-8')
  }

  it('deve manter o detalhe compativel quando nao houver respostas', () => {
    const fonte = lerFonte()

    expect(fonte).toContain('const possuiRespostasFormulario = computed(() => (detalhe.value?.respostasFormulario?.length ?? 0) > 0)')
    expect(fonte).toContain('v-if="possuiRespostasFormulario"')
  })

  it('deve exibir secao com rotulo e valor de resposta simples', () => {
    const fonte = lerFonte()

    expect(fonte).toContain('titulo="Respostas do formulario"')
    expect(fonte).toContain('{{ resposta.rotulo }}')
    expect(fonte).toContain('v-if="resposta.valor"')
    expect(fonte).toContain('{{ resposta.valor }}')
  })

  it('deve exibir todos os valores de resposta multipla', () => {
    const fonte = lerFonte()

    expect(fonte).toContain('v-else-if="resposta.valores.length"')
    expect(fonte).toContain('v-for="valor in resposta.valores"')
    expect(fonte).toContain(':key="`${resposta.campoFormularioServicoId}-${valor}`"')
    expect(fonte).toContain('{{ valor }}')
  })

  it('deve respeitar a ordem recebida do backend sem reordenar no frontend', () => {
    const fonte = lerFonte()
    const blocoRespostas = fonte.slice(fonte.indexOf('titulo="Respostas do formulario"'))

    expect(fonte).toContain('v-for="resposta in detalhe.respostasFormulario"')
    expect(blocoRespostas).not.toContain('.sort(')
    expect(blocoRespostas).not.toContain('computed(() => [...detalhe.value.respostasFormulario]')
  })

  it('deve exibir o tipo do campo conforme o layout atual', () => {
    const fonte = lerFonte()

    expect(fonte).toContain("icon=\"fact_check\"")
    expect(fonte).toContain('{{ formatarTipoCampoFormulario(resposta.tipo) }}')
    expect(fonte).toContain('function formatarTipoCampoFormulario(tipo: TipoCampoFormularioServico): string')
  })

  it('deve continuar usando apenas o endpoint existente de detalhe', () => {
    const fonte = lerFonte()

    expect(fonte).toContain('portalService.obterChamado(id)')
    expect(fonte).not.toContain('fetch(')
    expect(fonte).not.toContain('respostas-formulario')
    expect(fonte).not.toContain('portalService.obterRespostasFormulario')
  })
})
