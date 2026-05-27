import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('NovoChamadoView (catalogo)', () => {
  it('deve carregar servico selecionado a partir da querystring', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("route.query.catalogoServicoSlug")
    expect(fonte).toContain("route.query.catalogoServicoId")
    expect(fonte).toContain('Servico selecionado')
  })

  it('deve enviar catalogoServicoId no payload de abertura', () => {
    const caminho = new URL('./NovoChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('catalogoServicoId: form.catalogoServicoId ?? undefined')
    expect(fonte).toContain('catalogoServicoSlug: form.catalogoServicoSlug ?? undefined')
    expect(fonte).toContain(':disable="aberturaPorCatalogo"')
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
})
