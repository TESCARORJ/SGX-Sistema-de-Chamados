import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('AdminDetalheChamadoView - ITSM', () => {
  it('deve exibir natureza, impacto, urgencia e prioridade no detalhe administrativo', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Natureza ITSM')
    expect(fonte).toContain('Impacto')
    expect(fonte).toContain('Urgencia')
    expect(fonte).toContain('Prioridade')
  })

  it('deve consumir acoes disponiveis vindas do backend', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('const acoesDisponiveisSet = computed(() => new Set(detalhe.value?.acoesDisponiveisCodigos ?? []))')
    expect(fonte).toContain(':can-alterar-status="podeAlterarStatus"')
    expect(fonte).toContain(':can-encerrar="podeEncerrar"')
    expect(fonte).toContain(':can-reabrir="podeReabrir"')
  })

  it('deve filtrar visualmente status pelo conjunto permitido e manter tratamento de erro da API', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('const statusDisponiveisParaNatureza = computed(() => {')
    expect(fonte).toContain(':status="statusDisponiveisParaNatureza"')
    expect(fonte).toContain("registrarErro(error, 'Não foi possível concluir a ação.')")
    expect(fonte).toContain("A ação Encerrar não está disponível para este chamado no estado atual.")
  })
})
