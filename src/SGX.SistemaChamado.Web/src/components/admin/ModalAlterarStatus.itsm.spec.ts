import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('ModalAlterarStatus - ITSM', () => {
  it('deve bloquear selecao quando nao houver status compativeis', () => {
    const caminho = new URL('./ModalAlterarStatus.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Nao ha status compativeis para a natureza deste chamado.')
    expect(fonte).toContain(':disable="opcoesStatus.length === 0"')
    expect(fonte).toContain("if (!statusId.value || opcoesStatus.value.length === 0) return")
  })
})
