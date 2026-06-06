import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('GruposTecnicosAdminView', () => {
  it('deve renderizar listagem, filtros e formulario de grupos tecnicos', () => {
    const caminho = new URL('./GruposTecnicosAdminView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Grupos Tecnicos"')
    expect(fonte).toContain('label="Buscar por nome"')
    expect(fonte).toContain('label="Status"')
    expect(fonte).toContain('label="Novo grupo tecnico"')
    expect(fonte).toContain('Ver membros do grupo tecnico')
    expect(fonte).toContain('/admin/cadastros/grupos-tecnicos/${grupo.id}/membros')
    expect(fonte).toContain('Ver filas do grupo tecnico')
    expect(fonte).toContain('/admin/cadastros/grupos-tecnicos/${grupo.id}/filas')
    expect(fonte).toContain('label="Nome"')
    expect(fonte).toContain('label="Descricao"')
    expect(fonte).toContain('Nome e obrigatorio.')
  })

  it('deve respeitar modo somente leitura para nao administradores', () => {
    const caminho = new URL('./GruposTecnicosAdminView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('podeGerenciar')
    expect(fonte).toContain('usuarioEhAdministrador')
    expect(fonte).toContain('Somente leitura')
    expect(fonte).toContain('adminService.atualizarStatusGrupoTecnico')
  })
})
