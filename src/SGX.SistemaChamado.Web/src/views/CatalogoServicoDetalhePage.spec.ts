import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('CatalogoServicoDetalhePage', () => {
  it('deve preparar abertura antes de redirecionar para novo chamado', () => {
    const caminho = new URL('./CatalogoServicoDetalhePage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('prepararAberturaChamado(servico.value.slug)')
    expect(fonte).toContain("path: '/portal/chamados/novo'")
    expect(fonte).toContain('catalogoServicoId: preparado.catalogoServicoId')
  })

  it('deve exibir mensagem amigavel para servico apenas de consulta', () => {
    const caminho = new URL('./CatalogoServicoDetalhePage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Este servico esta disponivel apenas para consulta.')
    expect(fonte).toContain('Nao foi possivel iniciar a abertura do chamado para este servico.')
  })
})
