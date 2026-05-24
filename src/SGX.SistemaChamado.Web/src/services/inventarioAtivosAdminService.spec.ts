import { beforeEach, describe, expect, it, vi } from 'vitest'
import {
  CriticidadeAtivo,
  StatusOperacionalAtivo,
  StatusPatrimonialAtivo,
} from '../types/inventarioAtivos'

const getMock = vi.fn()
const postMock = vi.fn()
const putMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
    post: postMock,
    put: putMock,
  },
}))

describe('inventarioAtivosAdminService', () => {
  beforeEach(() => {
    getMock.mockReset()
    postMock.mockReset()
    putMock.mockReset()
  })

  it('deve listar ativos com filtros', async () => {
    const { inventarioAtivosAdminService } = await import('./inventarioAtivosAdminService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await inventarioAtivosAdminService.listar({
      termo: 'notebook',
      tipoAtivoInventarioId: 'tipo-1',
      departamentoId: 'dep-1',
      localUnidadeId: 'loc-1',
      usuarioResponsavelId: 'usr-1',
      statusOperacional: StatusOperacionalAtivo.Operacional,
      statusPatrimonial: StatusPatrimonialAtivo.EmUso,
      criticidade: CriticidadeAtivo.Media,
      ativo: true,
      pagina: 2,
      tamanhoPagina: 10,
      ordenarPor: 'nome',
      direcaoOrdenacao: 'asc',
    })

    expect(getMock).toHaveBeenCalledWith(
      '/api/admin/inventario-ativos?termo=notebook&tipoAtivoInventarioId=tipo-1&departamentoId=dep-1&localUnidadeId=loc-1&usuarioResponsavelId=usr-1&statusOperacional=1&statusPatrimonial=1&criticidade=2&ativo=true&pagina=2&tamanhoPagina=10&ordenarPor=nome&direcaoOrdenacao=asc'
    )
  })

  it('deve obter ativo por id', async () => {
    const { inventarioAtivosAdminService } = await import('./inventarioAtivosAdminService')
    getMock.mockResolvedValueOnce({ id: 'ativo-1' })

    await inventarioAtivosAdminService.obterPorId('ativo-1')

    expect(getMock).toHaveBeenCalledWith('/api/admin/inventario-ativos/ativo-1')
  })

  it('deve criar ativo', async () => {
    const { inventarioAtivosAdminService } = await import('./inventarioAtivosAdminService')
    postMock.mockResolvedValueOnce({ id: 'ativo-1' })

    await inventarioAtivosAdminService.criar({
      codigo: 'ATV-0001',
      nome: 'Notebook RH',
      tipoAtivoInventarioId: 'tipo-1',
    })

    expect(postMock).toHaveBeenCalledWith('/api/admin/inventario-ativos', {
      codigo: 'ATV-0001',
      nome: 'Notebook RH',
      tipoAtivoInventarioId: 'tipo-1',
    })
  })

  it('deve atualizar ativo', async () => {
    const { inventarioAtivosAdminService } = await import('./inventarioAtivosAdminService')
    putMock.mockResolvedValueOnce({ id: 'ativo-1' })

    await inventarioAtivosAdminService.atualizar('ativo-1', {
      codigo: 'ATV-0001',
      nome: 'Notebook RH',
      tipoAtivoInventarioId: 'tipo-1',
      statusOperacional: StatusOperacionalAtivo.Operacional,
      statusPatrimonial: StatusPatrimonialAtivo.EmUso,
      criticidade: CriticidadeAtivo.Media,
    })

    expect(putMock).toHaveBeenCalledWith('/api/admin/inventario-ativos/ativo-1', {
      codigo: 'ATV-0001',
      nome: 'Notebook RH',
      tipoAtivoInventarioId: 'tipo-1',
      statusOperacional: StatusOperacionalAtivo.Operacional,
      statusPatrimonial: StatusPatrimonialAtivo.EmUso,
      criticidade: CriticidadeAtivo.Media,
    })
  })

  it('deve inativar ativo', async () => {
    const { inventarioAtivosAdminService } = await import('./inventarioAtivosAdminService')
    postMock.mockResolvedValueOnce({})

    await inventarioAtivosAdminService.inativar('ativo-1')

    expect(postMock).toHaveBeenCalledWith('/api/admin/inventario-ativos/ativo-1/inativar')
  })

  it('deve reativar ativo', async () => {
    const { inventarioAtivosAdminService } = await import('./inventarioAtivosAdminService')
    postMock.mockResolvedValueOnce({})

    await inventarioAtivosAdminService.reativar('ativo-1')

    expect(postMock).toHaveBeenCalledWith('/api/admin/inventario-ativos/ativo-1/reativar')
  })

  it('deve listar tipos de ativo', async () => {
    const { inventarioAtivosAdminService } = await import('./inventarioAtivosAdminService')
    getMock.mockResolvedValueOnce([])

    await inventarioAtivosAdminService.listarTipos()

    expect(getMock).toHaveBeenCalledWith('/api/admin/inventario-ativos/tipos')
  })

  it('deve listar historico paginado', async () => {
    const { inventarioAtivosAdminService } = await import('./inventarioAtivosAdminService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await inventarioAtivosAdminService.listarHistorico('ativo-1', { pagina: 2, tamanhoPagina: 15 })

    expect(getMock).toHaveBeenCalledWith('/api/admin/inventario-ativos/ativo-1/historico?pagina=2&tamanhoPagina=15')
  })

  it('deve movimentar ativo', async () => {
    const { inventarioAtivosAdminService } = await import('./inventarioAtivosAdminService')
    postMock.mockResolvedValueOnce({})

    await inventarioAtivosAdminService.movimentar('ativo-1', {
      statusOperacional: StatusOperacionalAtivo.EmManutencao,
      observacao: 'Enviado para manutencao',
    })

    expect(postMock).toHaveBeenCalledWith('/api/admin/inventario-ativos/ativo-1/movimentar', {
      statusOperacional: StatusOperacionalAtivo.EmManutencao,
      observacao: 'Enviado para manutencao',
    })
  })

  it('deve listar chamados relacionados', async () => {
    const { inventarioAtivosAdminService } = await import('./inventarioAtivosAdminService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await inventarioAtivosAdminService.listarChamados('ativo-1', { pagina: 3, tamanhoPagina: 5 })

    expect(getMock).toHaveBeenCalledWith('/api/admin/inventario-ativos/ativo-1/chamados?pagina=3&tamanhoPagina=5')
  })
})
