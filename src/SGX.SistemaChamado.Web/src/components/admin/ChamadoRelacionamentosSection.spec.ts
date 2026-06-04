import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('ChamadoRelacionamentosSection', () => {
  it('deve declarar estados visuais, lista operacional e acoes preparadas', () => {
    const caminho = new URL('./ChamadoRelacionamentosSection.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Relacionamentos')
    expect(fonte).toContain('LoadingState')
    expect(fonte).toContain('ErrorState')
    expect(fonte).toContain('EmptyState')
    expect(fonte).toContain('Nenhum relacionamento registrado para este chamado.')
    expect(fonte).toContain('Mostrar inativos')
    expect(fonte).toContain('Abrir chamado')
    expect(fonte).toContain('obterDescricaoDirecionalRelacionamento')
    expect(fonte).toContain('obterChamadoRelacionadoCodigo')
    expect(fonte).toContain('Removido em')
    expect(fonte).toContain('Motivo:')
    expect(fonte).toContain('Novo vinculo')
    expect(fonte).toContain('Nova tarefa')
    expect(fonte).toContain('Nova aprovacao')
    expect(fonte).toContain('Criar chamado derivado')
    expect(fonte).toContain('adminService.listarRelacionamentosChamado')
    expect(fonte).toContain('adminService.listarTarefasChamado')
    expect(fonte).toContain('adminService.listarAprovacoesChamado')
    expect(fonte).toContain('Promise.allSettled')
    expect(fonte).toContain('Bloqueios e dependencias')
    expect(fonte).toContain('Este chamado esta bloqueado por outro chamado.')
    expect(fonte).toContain('Este chamado bloqueia outros chamados.')
    expect(fonte).toContain('Nenhum bloqueio ativo identificado.')
    expect(fonte).toContain('Derivacoes')
    expect(fonte).toContain('Nenhuma derivacao registrada.')
    expect(fonte).toContain('Tarefas vinculadas')
    expect(fonte).toContain('Nenhuma tarefa vinculada a este chamado.')
    expect(fonte).toContain('Aprovacoes vinculadas')
    expect(fonte).toContain('Este chamado possui aprovacao pendente bloqueante.')
    expect(fonte).toContain('Nenhuma aprovacao pendente vinculada a este chamado.')
  })

  it('deve estar acoplado ao detalhe administrativo do chamado', () => {
    const caminho = new URL('../../views/AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('ChamadoRelacionamentosSection')
    expect(fonte).toContain(':chamado-id="detalhe.id"')
    expect(fonte).toContain(':can-manage="podeGerenciarOrquestracao"')
  })
})
