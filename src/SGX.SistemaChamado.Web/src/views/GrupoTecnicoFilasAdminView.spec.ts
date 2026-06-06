import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('GrupoTecnicoFilasAdminView', () => {
  it('deve renderizar cabecalho, filtros, tabela e estado vazio de filas', () => {
    const caminho = new URL('./GrupoTecnicoFilasAdminView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Filas do grupo tecnico"')
    expect(fonte).toContain('Identificacao do grupo')
    expect(fonte).toContain('label="Buscar por nome ou descricao"')
    expect(fonte).toContain('label="Status"')
    expect(fonte).toContain('Nome')
    expect(fonte).toContain('Descricao')
    expect(fonte).toContain('Grupo tecnico')
    expect(fonte).toContain('Nenhuma fila cadastrada para este grupo tecnico.')
  })

  it('deve consumir endpoint de filas sem criar acoes administrativas de fila', () => {
    const caminho = new URL('./GrupoTecnicoFilasAdminView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('adminService.listarFilasAtendimentoGrupoTecnico')
    expect(fonte).toContain('adminService.obterGrupoTecnico')
    expect(fonte).toContain('podeVisualizar')
    expect(fonte).not.toContain('criarFila')
    expect(fonte).not.toContain('editarFila')
    expect(fonte).not.toContain('alterarStatusFila')
  })
})
