import { beforeEach, describe, expect, it, vi } from 'vitest'
import { StatusAprovacaoChamado, TipoOrigemAprovacaoChamado } from '../types/aprovacaoChamados'

const getMock = vi.fn()
const postMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
    post: postMock,
  },
}))

describe('aprovacaoChamadosAdminService', () => {
  beforeEach(() => {
    getMock.mockReset()
    postMock.mockReset()
  })

  it('deve listar aprovacoes com filtros', async () => {
    const { aprovacaoChamadosAdminService } = await import('./aprovacaoChamadosAdminService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await aprovacaoChamadosAdminService.listar({
      chamadoId: 'ch-1',
      status: StatusAprovacaoChamado.Pendente,
      tipoOrigem: TipoOrigemAprovacaoChamado.CatalogoServico,
      solicitanteId: 'usr-sol',
      aprovadorId: 'usr-apr',
      dataSolicitacaoInicial: '2026-01-01',
      dataSolicitacaoFinal: '2026-01-31',
      dataDecisaoInicial: '2026-02-01',
      dataDecisaoFinal: '2026-02-28',
      termo: 'VPN',
      pagina: 2,
      tamanhoPagina: 15,
      ordenarPor: 'solicitadaEm',
      direcaoOrdenacao: 'desc',
    })

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/aprovacao-chamados?chamadoId=ch-1&status=0&tipoOrigem=1&solicitanteId=usr-sol&aprovadorId=usr-apr&dataSolicitacaoInicial=2026-01-01&dataSolicitacaoFinal=2026-01-31&dataDecisaoInicial=2026-02-01&dataDecisaoFinal=2026-02-28&termo=VPN&pagina=2&tamanhoPagina=15&ordenarPor=solicitadaEm&direcaoOrdenacao=desc'
    )
  })

  it('deve obter aprovacao por id', async () => {
    const { aprovacaoChamadosAdminService } = await import('./aprovacaoChamadosAdminService')
    getMock.mockResolvedValueOnce({ id: 'apr-1' })

    await aprovacaoChamadosAdminService.obterPorId('apr-1')

    expect(getMock).toHaveBeenCalledWith('/api/admin/aprovacao-chamados/apr-1')
  })

  it('deve solicitar aprovacao', async () => {
    const { aprovacaoChamadosAdminService } = await import('./aprovacaoChamadosAdminService')
    postMock.mockResolvedValueOnce({ id: 'apr-1' })

    await aprovacaoChamadosAdminService.solicitar('ch-1', {
      tipoOrigem: TipoOrigemAprovacaoChamado.Manual,
      origemDescricao: 'Gestor da area',
      justificativaSolicitacao: 'Validacao administrativa',
    })

    expect(postMock).toHaveBeenCalledWith('/api/admin/chamados/ch-1/aprovacao/solicitar', {
      tipoOrigem: TipoOrigemAprovacaoChamado.Manual,
      origemDescricao: 'Gestor da area',
      justificativaSolicitacao: 'Validacao administrativa',
    })
  })

  it('deve aprovar aprovacao', async () => {
    const { aprovacaoChamadosAdminService } = await import('./aprovacaoChamadosAdminService')
    postMock.mockResolvedValueOnce({ id: 'apr-1' })

    await aprovacaoChamadosAdminService.aprovar('apr-1', {
      justificativaDecisao: 'Aprovado para execucao',
    })

    expect(postMock).toHaveBeenCalledWith('/api/admin/aprovacao-chamados/apr-1/aprovar', {
      justificativaDecisao: 'Aprovado para execucao',
    })
  })

  it('deve reprovar aprovacao', async () => {
    const { aprovacaoChamadosAdminService } = await import('./aprovacaoChamadosAdminService')
    postMock.mockResolvedValueOnce({ id: 'apr-1' })

    await aprovacaoChamadosAdminService.reprovar('apr-1', {
      justificativaDecisao: 'Nao atende ao criterio',
    })

    expect(postMock).toHaveBeenCalledWith('/api/admin/aprovacao-chamados/apr-1/reprovar', {
      justificativaDecisao: 'Nao atende ao criterio',
    })
  })

  it('deve cancelar aprovacao', async () => {
    const { aprovacaoChamadosAdminService } = await import('./aprovacaoChamadosAdminService')
    postMock.mockResolvedValueOnce({ id: 'apr-1' })

    await aprovacaoChamadosAdminService.cancelar('apr-1', {
      justificativaDecisao: 'Solicitacao substituida',
    })

    expect(postMock).toHaveBeenCalledWith('/api/admin/aprovacao-chamados/apr-1/cancelar', {
      justificativaDecisao: 'Solicitacao substituida',
    })
  })
})
