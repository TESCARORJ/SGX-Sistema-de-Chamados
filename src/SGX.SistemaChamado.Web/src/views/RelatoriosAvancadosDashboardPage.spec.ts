import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'
import { HttpRequestError } from '../services/httpClient'
import {
  criarFiltrosResumoDashboard,
  criarFiltroPeriodoPadrao,
  mapearMensagemErroDashboard,
} from './relatoriosAvancadosDashboard.helpers'

describe('relatoriosAvancadosDashboard.helpers', () => {
  it('deve criar periodo padrao de ultimos 30 dias em formato YYYY-MM-DD', () => {
    const filtro = criarFiltroPeriodoPadrao(new Date('2026-05-25T12:00:00.000Z'))

    expect(filtro).toEqual({
      dataInicial: '2026-04-25T00:00:00.000Z',
      dataFinal: '2026-05-25T23:59:59.999Z',
    })
  })

  it('deve criar filtros validos para os endpoints do dashboard', () => {
    const filtros = criarFiltrosResumoDashboard(new Date('2026-05-25T12:00:00.000Z'))

    expect(filtros.chamados).toEqual({ dataInicial: '2026-04-25T00:00:00.000Z', dataFinal: '2026-05-25T23:59:59.999Z' })
    expect(filtros.sla).toEqual({ dataInicial: '2026-04-25T00:00:00.000Z', dataFinal: '2026-05-25T23:59:59.999Z' })
    expect(filtros.aprovacoes).toEqual({ dataInicial: '2026-04-25T00:00:00.000Z', dataFinal: '2026-05-25T23:59:59.999Z' })
    expect(filtros.catalogo).toEqual({ dataInicial: '2026-04-25T00:00:00.000Z', dataFinal: '2026-05-25T23:59:59.999Z', limiteRanking: 5 })
    expect(filtros.inventario).toEqual({ dataInicial: '2026-04-25T00:00:00.000Z', dataFinal: '2026-05-25T23:59:59.999Z', limiteRanking: 5 })
    expect(filtros.auditoria).toEqual({ dataInicial: '2026-04-25T00:00:00.000Z', dataFinal: '2026-05-25T23:59:59.999Z', limiteRanking: 5 })
  })

  it('deve enviar DataInicial e DataFinal juntas no periodo padrao', () => {
    const filtros = criarFiltrosResumoDashboard(new Date('2026-05-25T12:00:00.000Z'))

    for (const filtro of [filtros.chamados, filtros.sla, filtros.aprovacoes, filtros.catalogo, filtros.inventario, filtros.auditoria]) {
      expect(Boolean(filtro.dataInicial)).toBe(true)
      expect(Boolean(filtro.dataFinal)).toBe(true)
    }
  })

  it('deve mapear erros HTTP e de rede para mensagens amigaveis', () => {
    expect(mapearMensagemErroDashboard(new HttpRequestError(400, 'bad request'))).toBe('Filtro inválido')
    expect(mapearMensagemErroDashboard(new HttpRequestError(403, 'forbidden'))).toBe('Sem permissão')
    expect(mapearMensagemErroDashboard(new HttpRequestError(404, 'not found'))).toBe('Endpoint não encontrado')
    expect(mapearMensagemErroDashboard(new HttpRequestError(500, 'server error'))).toBe('Erro interno ao carregar')
    expect(mapearMensagemErroDashboard(new Error('Failed to fetch'))).toBe('API indisponível')
  })
})

describe('RelatoriosAvancadosDashboardPage.vue', () => {
  it('nao deve exibir atalho clicavel para a propria tela de relatorios avancados', () => {
    const caminho = new URL('./RelatoriosAvancadosDashboardPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("const rotaAtualRelatoriosAvancados = '/admin/relatorios/avancados'")
    expect(fonte).toContain('rotaAtualNormalizada === rotaDashboardNormalizada')
    expect(fonte).not.toContain("titulo: 'Relatórios avançados'")
    expect(fonte).not.toContain("rota: '/admin/relatorios/avancados'")
  })

  it('deve exibir atalhos apenas para relatorios detalhados', () => {
    const caminho = new URL('./RelatoriosAvancadosDashboardPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("titulo: 'Chamados'")
    expect(fonte).toContain("titulo: 'SLA'")
    expect(fonte).toContain("titulo: 'Aprovações'")
    expect(fonte).toContain("titulo: 'Catálogo de serviços'")
    expect(fonte).toContain("titulo: 'Inventário/Ativos'")
    expect(fonte).toContain("titulo: 'Base de conhecimento'")
    expect(fonte).toContain("titulo: 'Auditoria'")
  })

  it('deve exibir texto atualizado da secao de acesso rapido', () => {
    const caminho = new URL('./RelatoriosAvancadosDashboardPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Painel administrativo consolidado de relatórios operacionais, gerenciais e institucionais.')
    expect(fonte).toContain('Escolha um relatório para detalhar filtros, distribuições e rankings.')
  })

  it('deve chamar os endpoints principais com filtros padrao do dashboard', () => {
    const caminho = new URL('./RelatoriosAvancadosDashboardPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('criarFiltrosResumoDashboard()')
    expect(fonte).toContain('obterResumoChamados(filtrosResumo.chamados)')
    expect(fonte).toContain('obterResumoSla(filtrosResumo.sla)')
    expect(fonte).toContain('obterResumoAprovacoes(filtrosResumo.aprovacoes)')
    expect(fonte).toContain('obterCatalogoServicosMaisSolicitados(filtrosResumo.catalogo)')
    expect(fonte).toContain('obterInventarioAtivosChamadosRecorrentes(filtrosResumo.inventario)')
    expect(fonte).toContain('obterResumoAuditoria(filtrosResumo.auditoria)')
  })

  it('deve respeitar permissoes e nao chamar auditoria sem acesso', () => {
    const caminho = new URL('./RelatoriosAvancadosDashboardPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('podeConsultarAuditoria.value')
    expect(fonte).toContain('Promise.resolve(null)')
    expect(fonte).toContain("valor: !podeConsultarAuditoria.value")
    expect(fonte).toContain("'Sem permissão'")
  })

  it('deve exibir estados consistentes de dados e erro nos cards', () => {
    const caminho = new URL('./RelatoriosAvancadosDashboardPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Sem dados no período')
    expect(fonte).toContain('Sem permissão')
    expect(fonte).toContain("resumoSla.value?.totalChamadosComSla === 0")
    expect(fonte).toContain("'0%'")
    expect(fonte).toContain('item.nomeServico')
    expect(fonte).toContain('item.codigo')
  })

  it('nao deve exibir secao tecnica de metadados no dashboard principal', () => {
    const caminho = new URL('./RelatoriosAvancadosDashboardPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).not.toContain('Informacoes tecnicas dos relatorios')
    expect(fonte).not.toContain('Informações técnicas dos relatórios')
    expect(fonte).not.toContain('Detalhes tecnicos')
    expect(fonte).not.toContain('Detalhes técnicos')
    expect(fonte).not.toContain('Periodos suportados')
    expect(fonte).not.toContain('Períodos suportados')
    expect(fonte).not.toContain('Filtros disponiveis')
    expect(fonte).not.toContain('Filtros disponíveis')
    expect(fonte).not.toContain('Permissoes relevantes')
    expect(fonte).not.toContain('Permissões relevantes')
    expect(fonte).not.toContain('q-expansion-item')
  })
})
