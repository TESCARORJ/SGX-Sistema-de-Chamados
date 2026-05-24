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
})
