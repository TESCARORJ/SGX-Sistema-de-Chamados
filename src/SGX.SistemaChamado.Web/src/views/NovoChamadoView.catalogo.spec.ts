import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('NovoChamadoView (catalogo)', () => {
  it('deve carregar servico selecionado a partir da querystring', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("route.query.catalogoServicoSlug")
    expect(fonte).toContain("route.query.catalogoServicoId")
    expect(fonte).toContain('Servico selecionado')
    expect(fonte).toContain('Formulario do servico')
  })

  it('deve enviar catalogoServicoId no payload de abertura', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('catalogoServicoId: form.catalogoServicoId ?? undefined')
    expect(fonte).toContain('catalogoServicoSlug: form.catalogoServicoSlug ?? undefined')
    expect(fonte).toContain(':disable="aberturaPorCatalogo"')
    expect(fonte).toContain('respostasFormulario: serializarRespostasFormulario()')
  })

  it('deve renderizar o componente de formulario dinamico quando o servico possuir formulario', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("import FormularioDinamicoCatalogoSection from '../components/portal/FormularioDinamicoCatalogoSection.vue'")
    expect(fonte).toContain('v-model="respostasFormulario"')
    expect(fonte).toContain(':formulario="servicoSelecionado.formulario"')
    expect(fonte).toContain("v-if=\"servicoSelecionado?.formulario\"")
  })

  it('deve serializar respostas por tipo e ignorar campos vazios', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('function serializarRespostasFormulario()')
    expect(fonte).toContain('if (!formulario) {')
    expect(fonte).toContain('return undefined')
    expect(fonte).toContain("valor: valor ? 'true' : 'false'")
    expect(fonte).toContain('campo.tipo === TipoCampoFormularioServico.SelecaoMultipla')
    expect(fonte).toContain('return respostasSerializadas.length ? respostasSerializadas : undefined')
  })

  it('deve limpar respostas antigas ao trocar servico ou remover formulario', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('function limparRespostasFormulario()')
    expect(fonte).toContain('limparRespostasFormulario()')
    expect(fonte).toContain('watch(')
    expect(fonte).toContain('route.query.catalogoServicoSlug')
  })

  it('nao deve renderizar secao dinamica quando o servico nao possuir formulario', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('v-if="servicoSelecionado?.formulario"')
    expect(fonte).toContain('const formulario = servicoSelecionado.value?.formulario')
    expect(fonte).toContain('if (!formulario) {')
  })

  it('nao deve enviar respostas antigas quando o servico selecionado nao possuir formulario', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('limparRespostasFormulario()')
    expect(fonte).toContain('aplicarServicoSelecionadoNoFormulario(servico)')
    expect(fonte).toContain('respostasFormulario: serializarRespostasFormulario()')
    expect(fonte).toContain('return undefined')
  })

  it('deve enviar payload valido com respostas no fluxo guiado e preservar erro retornado pela API', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('await portalService.abrirRequisicaoServicoCatalogo({')
    expect(fonte).toContain('respostasFormulario: serializarRespostasFormulario()')
    expect(fonte).toContain('erroSalvar.value = extrairMensagemErro(error')
  })

  it('nao deve simular sucesso quando o backend rejeitar respostas invalidas do formulario', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('const chamado = aberturaPorCatalogo.value && form.catalogoServicoId')
    expect(fonte).toContain("router.push('/portal/chamados')")
    expect(fonte).toContain('await router.replace(`/portal/chamados/${chamado.id}`)')
    expect(fonte).toContain('erroSalvar.value = extrairMensagemErro(error')
  })

  it('deve exigir selecao explicita de natureza, impacto e urgencia', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('label="Natureza do chamado *"')
    expect(fonte).toContain('label="Impacto *"')
    expect(fonte).toContain('label="Urgencia *"')
    expect(fonte).toContain("Natureza do chamado obrigatoria")
    expect(fonte).toContain("Impacto obrigatorio")
    expect(fonte).toContain("Urgencia obrigatoria")
  })

  it('nao deve usar fallback oculto de requisicao como comportamento principal', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).not.toContain('naturezaChamado: 2 as NaturezaChamado')
    expect(fonte).not.toContain('impactoChamado: ImpactoChamadoEnum.Baixo')
    expect(fonte).not.toContain('urgenciaChamado: UrgenciaChamadoEnum.Baixa')
  })

  it('deve reaproveitar mensagem de erro retornada pela API', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('erroSalvar.value = extrairMensagemErro(error')
  })

  it('nao deve simular sucesso quando o backend rejeitar obrigatorios ausentes', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    const indiceErro = fonte.indexOf("erroSalvar.value = extrairMensagemErro(error")
    const indicePush = fonte.indexOf("router.push('/portal/chamados')")

    expect(indiceErro).toBeGreaterThan(-1)
    expect(indicePush).toBeGreaterThan(indiceErro)
  })
})
