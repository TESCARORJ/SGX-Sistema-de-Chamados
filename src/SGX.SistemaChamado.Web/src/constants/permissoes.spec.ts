import { describe, expect, it } from 'vitest'
import { permissoes } from './permissoes'

describe('permissoes.ts', () => {
  it('deve expor as chaves de permissoes essenciais do sistema', () => {
    expect(permissoes.dashboardVisualizar).toBe('Dashboard.Visualizar')
    expect(permissoes.chamadosVisualizar).toBe('Chamados.Visualizar')
    expect(permissoes.chamadosVisualizarTodos).toBe('Chamados.VisualizarTodos')
    expect(permissoes.usuariosGerenciar).toBe('Usuarios.Gerenciar')
    expect(permissoes.perfisGerenciar).toBe('Perfis.Gerenciar')
    expect(permissoes.relatoriosAvancadosVisualizar).toBe('RelatoriosAvancados.Visualizar')
    expect(permissoes.auditoriaVisualizar).toBe('Auditoria.Visualizar')
  })

  it('deve conter as novas permissoes do ITSM (Problemas, Mudancas, Tarefas)', () => {
    expect(permissoes.problemasVisualizar).toBe('Problemas.Visualizar')
    expect(permissoes.problemasGerenciar).toBe('Problemas.Gerenciar')
    expect(permissoes.mudancasVisualizar).toBe('Mudancas.Visualizar')
    expect(permissoes.mudancasGerenciar).toBe('Mudancas.Gerenciar')
    expect(permissoes.tarefasVisualizar).toBe('Tarefas.Visualizar')
    expect(permissoes.tarefasGerenciar).toBe('Tarefas.Gerenciar')
  })

  it('deve garantir que nao existem codigos de permissao duplicados no catalogo', () => {
    const codigos = Object.values(permissoes)
    const distinctCodigos = [...new Set(codigos)]
    expect(codigos.length).toBe(distinctCodigos.length)
  })
})
