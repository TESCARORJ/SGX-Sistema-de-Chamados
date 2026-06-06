import { beforeEach, describe, expect, it, vi } from 'vitest'

const getMock = vi.fn()
const postMock = vi.fn()
const putMock = vi.fn()
const patchMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
    post: postMock,
    delete: vi.fn(),
    put: putMock,
    patch: patchMock,
  },
}))

describe('adminService', () => {
  beforeEach(() => {
    getMock.mockReset()
    postMock.mockReset()
    putMock.mockReset()
    patchMock.mockReset()
  })

  it('deve enviar naturezaChamado na listagem admin quando filtro for informado', async () => {
    const { adminService } = await import('./adminService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await adminService.listarChamadosAdmin({
      naturezaChamado: 1,
      pagina: 1,
      tamanhoPagina: 20,
    })

    const url = String(getMock.mock.calls[0]?.[0] ?? '')
    expect(url).toContain('/api/admin/chamados?')
    expect(url).toContain('naturezaChamado=1')
  })

  it('nao deve enviar naturezaChamado quando filtro for Todos', async () => {
    const { adminService } = await import('./adminService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await adminService.listarChamadosAdmin({
      pagina: 1,
      tamanhoPagina: 20,
    })

    const url = String(getMock.mock.calls[0]?.[0] ?? '')
    expect(url).toContain('/api/admin/chamados?')
    expect(url).not.toContain('naturezaChamado=')
  })

  it('deve enviar filtros de grupo tecnico e fila na listagem admin quando informados', async () => {
    const { adminService } = await import('./adminService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await adminService.listarChamadosAdmin({
      grupoTecnicoId: 'grupo-1',
      filaAtendimentoId: 'fila-1',
    })

    const url = String(getMock.mock.calls[0]?.[0] ?? '')
    expect(url).toContain('/api/admin/chamados?')
    expect(url).toContain('grupoTecnicoId=grupo-1')
    expect(url).toContain('filaAtendimentoId=fila-1')
  })

  it('deve listar grupos tecnicos com filtros administrativos', async () => {
    const { adminService } = await import('./adminService')
    getMock.mockResolvedValueOnce({ items: [], total: 0, pagina: 1, tamanhoPagina: 20 })

    await adminService.listarGruposTecnicos({
      texto: 'suporte',
      ativo: true,
      pagina: 2,
      tamanhoPagina: 10,
    })

    const url = String(getMock.mock.calls[0]?.[0] ?? '')
    expect(url).toContain('/api/admin/grupos-tecnicos?')
    expect(url).toContain('texto=suporte')
    expect(url).toContain('ativo=true')
    expect(url).toContain('pagina=2')
    expect(url).toContain('tamanhoPagina=10')
  })

  it('deve expor operacoes administrativas de grupo tecnico', async () => {
    const { adminService } = await import('./adminService')

    getMock.mockResolvedValueOnce({})
    postMock.mockResolvedValueOnce({})
    putMock.mockResolvedValueOnce({})
    patchMock.mockResolvedValueOnce({})

    await adminService.obterGrupoTecnico('grupo-1')
    await adminService.criarGrupoTecnico({ nome: 'Suporte', descricao: 'Atendimento' })
    await adminService.atualizarGrupoTecnico('grupo-1', { nome: 'Suporte N1', descricao: null })
    await adminService.atualizarStatusGrupoTecnico('grupo-1', { ativo: false })

    expect(getMock).toHaveBeenCalledWith('/api/admin/grupos-tecnicos/grupo-1')
    expect(postMock).toHaveBeenCalledWith('/api/admin/grupos-tecnicos', {
      nome: 'Suporte',
      descricao: 'Atendimento',
    })
    expect(putMock).toHaveBeenCalledWith('/api/admin/grupos-tecnicos/grupo-1', {
      nome: 'Suporte N1',
      descricao: null,
    })
    expect(patchMock).toHaveBeenCalledWith('/api/admin/grupos-tecnicos/grupo-1/status', { ativo: false })
  })

  it('deve expor operacoes administrativas de membros de grupo tecnico', async () => {
    const { adminService } = await import('./adminService')

    getMock.mockResolvedValueOnce([])
    postMock.mockResolvedValueOnce({})
    patchMock.mockResolvedValueOnce({})
    getMock.mockResolvedValueOnce([])

    await adminService.listarMembrosGrupoTecnico('grupo-1', { ativo: true })
    await adminService.adicionarMembroGrupoTecnico('grupo-1', { usuarioId: 'usuario-1' })
    await adminService.alterarStatusMembroGrupoTecnico('grupo-1', 'membro-1', { ativo: false })
    await adminService.listarGruposTecnicosDoUsuario('usuario-1', true)

    expect(getMock).toHaveBeenNthCalledWith(1, '/api/admin/grupos-tecnicos/grupo-1/membros?ativo=true')
    expect(postMock).toHaveBeenCalledWith('/api/admin/grupos-tecnicos/grupo-1/membros', { usuarioId: 'usuario-1' })
    expect(patchMock).toHaveBeenCalledWith('/api/admin/grupos-tecnicos/grupo-1/membros/membro-1/status', {
      ativo: false,
    })
    expect(getMock).toHaveBeenNthCalledWith(2, '/api/admin/usuarios/usuario-1/grupos-tecnicos?ativo=true')
  })

  it('deve listar filas de atendimento do grupo tecnico com filtros suportados', async () => {
    const { adminService } = await import('./adminService')

    getMock.mockResolvedValueOnce([])

    await adminService.listarFilasAtendimentoGrupoTecnico('grupo-1', {
      ativo: true,
      busca: 'Incidentes',
    })

    expect(getMock).toHaveBeenCalledWith('/api/admin/grupos-tecnicos/grupo-1/filas?ativo=true&busca=Incidentes')
  })

  it('deve assumir chamado pela fila enviando usuario autenticado', async () => {
    const { adminService } = await import('./adminService')

    postMock.mockResolvedValueOnce({})

    await adminService.assumirChamadoFila('ch-1', { usuarioId: 'usuario-1' })

    expect(postMock).toHaveBeenCalledWith('/api/admin/chamados/ch-1/assumir-fila', { usuarioId: 'usuario-1' })
  })

  it('deve transferir chamado para outro grupo tecnico', async () => {
    const { adminService } = await import('./adminService')

    postMock.mockResolvedValueOnce({})

    await adminService.transferirGrupoTecnicoChamado('ch-1', {
      grupoTecnicoId: 'grupo-2',
      filaAtendimentoId: 'fila-2',
    })

    expect(postMock).toHaveBeenCalledWith('/api/admin/chamados/ch-1/transferir-grupo-tecnico', {
      grupoTecnicoId: 'grupo-2',
      filaAtendimentoId: 'fila-2',
    })
  })

  it('deve listar relacionamentos do chamado administrativo', async () => {
    const { adminService } = await import('./adminService')
    getMock.mockResolvedValueOnce([])

    await adminService.listarRelacionamentosChamado('ch-1')

    expect(getMock).toHaveBeenCalledWith('/api/admin/chamados/ch-1/relacionamentos?incluirInativos=false')
  })

  it('deve listar relacionamentos inativos quando solicitado', async () => {
    const { adminService } = await import('./adminService')
    getMock.mockResolvedValueOnce([])

    await adminService.listarRelacionamentosChamado('ch-1', true)

    expect(getMock).toHaveBeenCalledWith('/api/admin/chamados/ch-1/relacionamentos?incluirInativos=true')
  })

  it('deve listar tarefas vinculadas do chamado administrativo', async () => {
    const { adminService } = await import('./adminService')
    getMock.mockResolvedValueOnce([])

    await adminService.listarTarefasChamado('ch-1')

    expect(getMock).toHaveBeenCalledWith('/api/admin/chamados/ch-1/tarefas?incluirInativas=false')
  })

  it('deve listar aprovacoes vinculadas do chamado administrativo', async () => {
    const { adminService } = await import('./adminService')
    getMock.mockResolvedValueOnce([])

    await adminService.listarAprovacoesChamado('ch-1', true)

    expect(getMock).toHaveBeenCalledWith('/api/admin/chamados/ch-1/aprovacoes?incluirInativas=true')
  })
})
