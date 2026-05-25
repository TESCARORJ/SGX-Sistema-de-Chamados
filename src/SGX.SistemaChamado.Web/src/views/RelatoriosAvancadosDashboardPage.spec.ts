import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('RelatoriosAvancadosDashboardPage', () => {
  it('deve renderizar cards de resumo e acesso rapido', () => {
    const caminho = new URL('./RelatoriosAvancadosDashboardPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Relatorios avancados"')
    expect(fonte).toContain('Acesso rapido')
    expect(fonte).toContain('Total de chamados')
    expect(fonte).toContain('Cumprimento de SLA')
    expect(fonte).toContain('Informacoes tecnicas dos relatorios')
  })

  it('deve carregar metadados e indicadores consolidados', () => {
    const caminho = new URL('./RelatoriosAvancadosDashboardPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('obterMetadados(')
    expect(fonte).toContain('obterResumoChamados(')
    expect(fonte).toContain('obterResumoAprovacoes(')
    expect(fonte).toContain('obterCatalogoServicosMaisSolicitados(')
    expect(fonte).toContain('obterInventarioAtivosChamadosRecorrentes(')
    expect(fonte).toContain('criarFiltroPeriodoPadrao')
    expect(fonte).toContain('T00:00:00')
    expect(fonte).toContain('T23:59:59')
    expect(fonte).toContain('apenasAtivos: false')
  })

  it('deve tratar estados claros dos cards', () => {
    const caminho = new URL('./RelatoriosAvancadosDashboardPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('LoadingState')
    expect(fonte).toContain('ErrorState')
    expect(fonte).toContain('EmptyState')
    expect(fonte).toContain('Sem dados no periodo')
    expect(fonte).toContain('Sem permissao')
    expect(fonte).toContain('Erro ao carregar')
    expect(fonte).toContain('Filtro invalido')
    expect(fonte).toContain('Endpoint nao encontrado')
    expect(fonte).toContain('Erro interno ao carregar')
    expect(fonte).toContain('API indisponivel')
  })

  it('deve chamar auditoria apenas com permissao', () => {
    const caminho = new URL('./RelatoriosAvancadosDashboardPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('podeAuditoria.value')
    expect(fonte).toContain('obterResumoAuditoria')
  })

  it('deve exibir metadados tecnicos em secao recolhivel com nomes amigaveis', () => {
    const caminho = new URL('./RelatoriosAvancadosDashboardPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('q-expansion-item')
    expect(fonte).not.toContain('default-opened')
    expect(fonte).toContain('Capacidades informadas pelo backend para montagem dos relatorios.')
    expect(fonte).toContain("DataInicial: 'Periodo'")
    expect(fonte).toContain("CatalogoServicoId: 'Servico do catalogo'")
    expect(fonte).toContain("'RelatoriosAvancados.Visualizar': 'Visualizar relatorios'")
    expect(fonte).toContain("Ultimos30Dias: 'Ultimos 30 dias'")
    expect(fonte).not.toContain('metadados.filtrosDisponiveis')
    expect(fonte).not.toContain('metadados.permissoesRelevantes')
  })
})
