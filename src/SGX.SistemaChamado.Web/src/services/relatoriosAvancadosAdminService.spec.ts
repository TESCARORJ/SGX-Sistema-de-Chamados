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
      naturezaChamado: 1 as any,
      apenasAtivos: true,
      limiteRanking: 10,
    })

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/relatorios-avancados/chamados/resumo?DataInicial=2026-01-01&DataFinal=2026-01-31&DepartamentoId=dep-1&NaturezaChamado=1&ApenasAtivos=true&LimiteRanking=10'
    )
  })

  it('nao deve enviar filtros vazios, guid vazio e agrupamentos em endpoint de resumo', async () => {
    const { relatoriosAvancadosAdminService } = await import('./relatoriosAvancadosAdminService')
    getMock.mockResolvedValueOnce({})

    await relatoriosAvancadosAdminService.obterResumoChamados({
      dataInicial: '2026-05-01T00:00:00',
      dataFinal: '2026-05-31T23:59:59',
      departamentoId: '00000000-0000-0000-0000-000000000000',
      status: '',
      agrupamento: 0 as any,
      agruparPor: 0 as any,
      // cobertura de limpeza para valores nao utilizados
      subcategoriaId: undefined,
    } as any)

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/relatorios-avancados/chamados/resumo?DataInicial=2026-05-01T00%3A00%3A00&DataFinal=2026-05-31T23%3A59%3A59'
    )
  })

  it('nao deve enviar naturezaChamado quando filtro for Todos', async () => {
    const { relatoriosAvancadosAdminService } = await import('./relatoriosAvancadosAdminService')
    getMock.mockResolvedValueOnce({})

    await relatoriosAvancadosAdminService.obterResumoChamados({
      dataInicial: '2026-05-01',
      dataFinal: '2026-05-31',
      naturezaChamado: 0 as any,
    })

    const url = getMock.mock.calls.at(-1)?.[0] as string
    expect(url).not.toContain('NaturezaChamado=')
  })

  it('deve obter resumo de sla', async () => {
    const { relatoriosAvancadosAdminService } = await import('./relatoriosAvancadosAdminService')
    getMock.mockResolvedValueOnce({})

    await relatoriosAvancadosAdminService.obterResumoSla({ dataInicial: '2026-02-01', dataFinal: '2026-02-28' })

    expect(getMock).toHaveBeenCalledWith('/api/admin/relatorios-avancados/sla/resumo?DataInicial=2026-02-01&DataFinal=2026-02-28')
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
      '/api/admin/relatorios-avancados/aprovacoes/resumo?DataInicial=2026-03-01&DataFinal=2026-03-31&StatusAprovacao=Aprovada'
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
      '/api/admin/relatorios-avancados/inventario-ativos/resumo?DataInicial=2026-04-01&DataFinal=2026-04-30&StatusOperacional=Operacional'
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
      '/api/admin/relatorios-avancados/catalogo-servicos/mais-solicitados?DataInicial=2026-05-01&DataFinal=2026-05-31&LimiteRanking=5&ApenasAtivos=false'
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
      '/api/admin/relatorios-avancados/auditoria/resumo?DataInicial=2026-05-01&DataFinal=2026-05-20&Entidade=Chamado'
    )
  })

  it('nao deve enviar campos de outro contrato no endpoint de inventario', async () => {
    const { relatoriosAvancadosAdminService } = await import('./relatoriosAvancadosAdminService')
    getMock.mockResolvedValueOnce([])

    await relatoriosAvancadosAdminService.obterInventarioAtivosChamadosRecorrentes({
      dataInicial: '2026-05-01',
      dataFinal: '2026-05-31',
      limiteRanking: 5,
      // nao pertence ao contrato de inventario
      apenasAtivos: false as any,
      agrupamento: 3 as any,
    } as any)

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/relatorios-avancados/inventario-ativos/chamados-recorrentes?DataInicial=2026-05-01&DataFinal=2026-05-31&LimiteRanking=5'
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
