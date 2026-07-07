import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('FormularioDinamicoCatalogoSection', () => {
  it('deve renderizar titulo e descricao do formulario quando existir', () => {
    const caminho = new URL('./FormularioDinamicoCatalogoSection.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('formulario?.nome')
    expect(fonte).toContain('formulario?.descricao')
    expect(fonte).toContain('Versao {{ formulario?.versao.numero }}')
  })

  it('deve ordenar e filtrar campos e opcoes por seguranca', () => {
    const caminho = new URL('./FormularioDinamicoCatalogoSection.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain(".filter((campo) => campo.ativo !== false && campo.visivel !== false)")
    expect(fonte).toContain('.sort((a, b) => a.ordem - b.ordem')
    expect(fonte).toContain(".filter((opcao) => opcao.ativo !== false)")
  })

  it('deve renderizar controles para todos os tipos suportados', () => {
    const caminho = new URL('./FormularioDinamicoCatalogoSection.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('isTextoCurto(campo)')
    expect(fonte).toContain('isTextoLongo(campo)')
    expect(fonte).toContain('isNumero(campo)')
    expect(fonte).toContain('isData(campo)')
    expect(fonte).toContain('isBooleano(campo)')
    expect(fonte).toContain('isSelecaoUnica(campo)')
    expect(fonte).toContain('isSelecaoMultipla(campo)')
    expect(fonte).toContain('<q-input')
    expect(fonte).toContain('type="textarea"')
    expect(fonte).toContain('type="number"')
    expect(fonte).toContain('type="date"')
    expect(fonte).toContain('<q-toggle')
    expect(fonte).toContain('<q-select')
    expect(fonte).toContain('<q-option-group')
  })

  it('deve exibir obrigatoriedade, ajuda e opcoes ordenadas', () => {
    const caminho = new URL('./FormularioDinamicoCatalogoSection.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("return campo.obrigatorio ? `${campo.rotulo} *` : campo.rotulo")
    expect(fonte).toContain('campo.textoAjuda')
    expect(fonte).toContain('obterOpcoesSelect(campo)')
  })

  it('deve expor estado controlado por v-model para a tela serializar respostas', () => {
    const caminho = new URL('./FormularioDinamicoCatalogoSection.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('modelValue: RespostaFormularioState')
    expect(fonte).toContain("'update:modelValue'")
    expect(fonte).toContain('atualizarResposta(campo.id')
  })
})
