import { beforeEach, describe, expect, it, vi } from 'vitest'

const getMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
  },
}))

describe('relatoriosAvancadosAdminService', () => {
  beforeEach(() => {
    getMock.mockReset()
  })

  it('deve obter metadados', async () => {
    const { relatoriosAvancadosAdminService } = await import('./relatoriosAvancadosAdminService')
    getMock.mockResolvedValueOnce({})

    await relatoriosAvancadosAdminService.obterMetadados()

    expect(getMock).toHaveBeenCalledWith('/api/admin/relatorios-avancados/metadados')
  })

  it('deve obter resumo de chamados com filtros', async () => {
    const { relatoriosAvancadosAdminService } = await import('./relatoriosAvancadosAdminService')
    getMock.mockResolvedValueOnce({})

    await relatoriosAvancadosAdminService.obterResumoChamados({
      dataInicial: '2026-01-01',
      dataFinal: '2026-01-31',
      departamentoId: 'dep-1',
      apenasAtivos: true,
      limiteRanking: 10,
    })

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/relatorios-avancados/chamados/resumo?dataInicial=2026-01-01&dataFinal=2026-01-31&departamentoId=dep-1&apenasAtivos=true&limiteRanking=10'
    )
  })

  it('nao deve enviar filtros vazios ou guid vazio', async () => {
    const { relatoriosAvancadosAdminService } = await import('./relatoriosAvancadosAdminService')
    getMock.mockResolvedValueOnce({})

    await relatoriosAvancadosAdminService.obterResumoChamados({
      dataInicial: '2026-05-01T00:00:00',
      dataFinal: '2026-05-31T23:59:59',
      departamentoId: '00000000-0000-0000-0000-000000000000',
      status: '',
      // cobertura de limpeza para valores nao utilizados
      subcategoriaId: undefined,
    } as any)

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/relatorios-avancados/chamados/resumo?dataInicial=2026-05-01T00%3A00%3A00&dataFinal=2026-05-31T23%3A59%3A59'
    )
  })

  it('deve obter resumo de sla', async () => {
    const { relatoriosAvancadosAdminService } = await import('./relatoriosAvancadosAdminService')
    getMock.mockResolvedValueOnce({})

    await relatoriosAvancadosAdminService.obterResumoSla({ dataInicial: '2026-02-01', dataFinal: '2026-02-28' })

    expect(getMock).toHaveBeenCalledWith('/api/admin/relatorios-avancados/sla/resumo?dataInicial=2026-02-01&dataFinal=2026-02-28')
  })

  it('deve obter resumo de aprovacoes', async () => {
    const { relatoriosAvancadosAdminService } = await import('./relatoriosAvancadosAdminService')
    getMock.mockResolvedValueOnce({})

    await relatoriosAvancadosAdminService.obterResumoAprovacoes({
      dataInicial: '2026-03-01',
      dataFinal: '2026-03-31',
      statusAprovacao: 'Aprovada',
    })

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/relatorios-avancados/aprovacoes/resumo?dataInicial=2026-03-01&dataFinal=2026-03-31&statusAprovacao=Aprovada'
    )
  })

  it('deve obter resumo de inventario de ativos', async () => {
    const { relatoriosAvancadosAdminService } = await import('./relatoriosAvancadosAdminService')
    getMock.mockResolvedValueOnce({})

    await relatoriosAvancadosAdminService.obterResumoInventarioAtivos({
      dataInicial: '2026-04-01',
      dataFinal: '2026-04-30',
      statusOperacional: 'Operacional',
    })

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/relatorios-avancados/inventario-ativos/resumo?dataInicial=2026-04-01&dataFinal=2026-04-30&statusOperacional=Operacional'
    )
  })

  it('deve obter catalogo de servicos mais solicitados', async () => {
    const { relatoriosAvancadosAdminService } = await import('./relatoriosAvancadosAdminService')
    getMock.mockResolvedValueOnce([])

    await relatoriosAvancadosAdminService.obterCatalogoServicosMaisSolicitados({
      dataInicial: '2026-05-01',
      dataFinal: '2026-05-31',
      limiteRanking: 5,
      apenasAtivos: false,
    })

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/relatorios-avancados/catalogo-servicos/mais-solicitados?dataInicial=2026-05-01&dataFinal=2026-05-31&limiteRanking=5&apenasAtivos=false'
    )
  })

  it('deve obter resumo de auditoria', async () => {
    const { relatoriosAvancadosAdminService } = await import('./relatoriosAvancadosAdminService')
    getMock.mockResolvedValueOnce({})

    await relatoriosAvancadosAdminService.obterResumoAuditoria({
      dataInicial: '2026-05-01',
      dataFinal: '2026-05-20',
      entidade: 'Chamado',
    })

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/relatorios-avancados/auditoria/resumo?dataInicial=2026-05-01&dataFinal=2026-05-20&entidade=Chamado'
    )
  })
})

describe('exportCsv utilitario', () => {
  it('deve gerar csv com separador e escape', async () => {
    const { gerarConteudoCsv } = await import('../utils/exportCsv')

    const csv = gerarConteudoCsv(
      [
        { indicador: 'Total', valor: 10 },
        { indicador: 'Texto com ; e "aspas"', valor: 20 },
      ],
      [
        { key: 'indicador', label: 'Indicador' },
        { key: 'valor', label: 'Valor' },
      ]
    )

    expect(csv).toContain('Indicador;Valor')
    expect(csv).toContain('Total;10')
    expect(csv).toContain('"Texto com ; e ""aspas""";20')
  })
})
