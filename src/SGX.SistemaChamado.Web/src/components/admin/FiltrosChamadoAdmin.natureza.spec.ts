import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('FiltrosChamadoAdmin - natureza', () => {
  it('deve exibir filtro de Natureza ITSM na listagem administrativa', () => {
    const caminho = new URL('./FiltrosChamadoAdmin.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('label="Natureza ITSM"')
    expect(fonte).toContain('NaturezaChamado.Incidente')
    expect(fonte).toContain('NaturezaChamado.Requisicao')
  })

  it('deve exibir filtros dependentes de grupo tecnico e fila de atendimento', () => {
    const caminho = new URL('./FiltrosChamadoAdmin.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('label="Grupo tecnico"')
    expect(fonte).toContain('label="Fila de atendimento"')
    expect(fonte).toContain('filtros.grupoTecnicoId')
    expect(fonte).toContain('filtros.filaAtendimentoId')
    expect(fonte).toContain('adminService.listarGruposTecnicos({')
    expect(fonte).toContain('adminService.listarFilasAtendimentoGrupoTecnico(grupoTecnicoId, { ativo: true })')
    expect(fonte).toContain('filtros.filaAtendimentoId = undefined')
    expect(fonte).toContain('@update:model-value="onGrupoTecnicoChange"')
    expect(fonte).toContain(':disable="!filtros.grupoTecnicoId || loadingFilasAtendimento"')
    expect(fonte).toContain('filasAtendimento.value = []')
    expect(fonte).toContain('filtros.grupoTecnicoId = undefined')
  })
})
