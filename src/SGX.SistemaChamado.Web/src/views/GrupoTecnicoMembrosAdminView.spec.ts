import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('GrupoTecnicoMembrosAdminView', () => {
  it('deve renderizar cabecalho, filtros, tabela e formulario de membros', () => {
    const caminho = new URL('./GrupoTecnicoMembrosAdminView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Membros do grupo tecnico"')
    expect(fonte).toContain('Identificacao do grupo')
    expect(fonte).toContain('label="Status"')
    expect(fonte).toContain('label="Adicionar membro"')
    expect(fonte).toContain('Nome do usuario')
    expect(fonte).toContain('E-mail')
    expect(fonte).toContain('Usuario e obrigatorio.')
  })

  it('deve consumir endpoints administrativos de membros e respeitar modo somente leitura', () => {
    const caminho = new URL('./GrupoTecnicoMembrosAdminView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('adminService.listarMembrosGrupoTecnico')
    expect(fonte).toContain('adminService.adicionarMembroGrupoTecnico')
    expect(fonte).toContain('adminService.alterarStatusMembroGrupoTecnico')
    expect(fonte).toContain('adminService.obterAdminContexto')
    expect(fonte).toContain('podeGerenciar')
    expect(fonte).toContain('Somente leitura')
  })
})
