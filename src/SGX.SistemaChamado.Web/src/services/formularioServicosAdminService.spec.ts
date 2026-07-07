import { beforeEach, describe, expect, it, vi } from 'vitest'

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

describe('formularioServicosAdminService', () => {
  beforeEach(() => {
    getMock.mockReset()
    postMock.mockReset()
    putMock.mockReset()
  })

  it('deve listar formularios por catalogo', async () => {
    const { formularioServicosAdminService } = await import('./formularioServicosAdminService')
    getMock.mockResolvedValueOnce([])

    await formularioServicosAdminService.listarFormularios({ catalogoServicoId: 'cat-1' })

    expect(getMock).toHaveBeenCalledWith('/api/admin/formulario-servicos?catalogoServicoId=cat-1')
  })

  it('deve criar formulario', async () => {
    const { formularioServicosAdminService } = await import('./formularioServicosAdminService')
    postMock.mockResolvedValueOnce({ id: 'form-1' })

    await formularioServicosAdminService.criarFormulario({
      catalogoServicoId: 'cat-1',
      nome: 'Formulario',
      descricao: 'Descricao',
      ativo: true,
    })

    expect(postMock).toHaveBeenCalledWith('/api/admin/formulario-servicos', {
      catalogoServicoId: 'cat-1',
      nome: 'Formulario',
      descricao: 'Descricao',
      ativo: true,
    })
  })

  it('deve atualizar versao, campo e opcao usando as rotas administrativas esperadas', async () => {
    const { formularioServicosAdminService } = await import('./formularioServicosAdminService')
    putMock.mockResolvedValue({})

    await formularioServicosAdminService.atualizarVersao('ver-1', {
      numero: 2,
      publicada: false,
      publicadoEm: null,
      ativo: true,
    })

    await formularioServicosAdminService.atualizarCampo('campo-1', {
      nome: 'motivo',
      rotulo: 'Motivo',
      tipo: 2,
      obrigatorio: true,
      ordem: 1,
      textoAjuda: 'Detalhe a solicitacao',
      visivel: true,
      ativo: true,
    })

    await formularioServicosAdminService.atualizarOpcao('opcao-1', {
      valor: 'vpn',
      rotulo: 'VPN',
      ordem: 1,
      ativo: true,
    })

    expect(putMock).toHaveBeenNthCalledWith(1, '/api/admin/formulario-servicos/versoes/ver-1', {
      numero: 2,
      publicada: false,
      publicadoEm: null,
      ativo: true,
    })
    expect(putMock).toHaveBeenNthCalledWith(2, '/api/admin/formulario-servicos/campos/campo-1', {
      nome: 'motivo',
      rotulo: 'Motivo',
      tipo: 2,
      obrigatorio: true,
      ordem: 1,
      textoAjuda: 'Detalhe a solicitacao',
      visivel: true,
      ativo: true,
    })
    expect(putMock).toHaveBeenNthCalledWith(3, '/api/admin/formulario-servicos/opcoes/opcao-1', {
      valor: 'vpn',
      rotulo: 'VPN',
      ordem: 1,
      ativo: true,
    })
  })

  it('deve consultar detalhe e acionar rotas de ativacao e inativacao administrativas', async () => {
    const { formularioServicosAdminService } = await import('./formularioServicosAdminService')
    getMock.mockResolvedValue({})
    postMock.mockResolvedValue({})

    await formularioServicosAdminService.obterFormulario('form-1')
    await formularioServicosAdminService.inativarFormulario('form-1')
    await formularioServicosAdminService.reativarFormulario('form-1')
    await formularioServicosAdminService.inativarVersao('ver-1')
    await formularioServicosAdminService.reativarVersao('ver-1')
    await formularioServicosAdminService.inativarCampo('campo-1')
    await formularioServicosAdminService.reativarCampo('campo-1')
    await formularioServicosAdminService.inativarOpcao('opcao-1')
    await formularioServicosAdminService.reativarOpcao('opcao-1')

    expect(getMock).toHaveBeenCalledWith('/api/admin/formulario-servicos/form-1')
    expect(postMock).toHaveBeenNthCalledWith(1, '/api/admin/formulario-servicos/form-1/inativar')
    expect(postMock).toHaveBeenNthCalledWith(2, '/api/admin/formulario-servicos/form-1/reativar')
    expect(postMock).toHaveBeenNthCalledWith(3, '/api/admin/formulario-servicos/versoes/ver-1/inativar')
    expect(postMock).toHaveBeenNthCalledWith(4, '/api/admin/formulario-servicos/versoes/ver-1/reativar')
    expect(postMock).toHaveBeenNthCalledWith(5, '/api/admin/formulario-servicos/campos/campo-1/inativar')
    expect(postMock).toHaveBeenNthCalledWith(6, '/api/admin/formulario-servicos/campos/campo-1/reativar')
    expect(postMock).toHaveBeenNthCalledWith(7, '/api/admin/formulario-servicos/opcoes/opcao-1/inativar')
    expect(postMock).toHaveBeenNthCalledWith(8, '/api/admin/formulario-servicos/opcoes/opcao-1/reativar')
  })
})
