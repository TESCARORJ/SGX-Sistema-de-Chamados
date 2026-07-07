import { beforeEach, describe, expect, it, vi } from 'vitest'

const getMock = vi.fn()
const postMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
    post: postMock,
  },
}))

describe('portalService', () => {
  beforeEach(() => {
    getMock.mockReset()
    postMock.mockReset()
  })

  it('deve enviar catalogoServicoId na criacao de chamado quando informado', async () => {
    const { portalService } = await import('./portalService')
    postMock.mockResolvedValueOnce({ id: 'chamado-1', codigo: 'CH-1' })

    await portalService.criarChamado({
      titulo: 'Solicitar VPN',
      descricao: 'Preciso de acesso remoto',
      catalogoServicoId: 'srv-1',
      categoriaId: 'cat-1',
      prioridadeId: 'prio-1',
      naturezaChamado: 2,
      impactoChamado: 1,
      urgenciaChamado: 1,
    })

    expect(postMock).toHaveBeenCalledWith('/api/portal/chamados', {
      titulo: 'Solicitar VPN',
      descricao: 'Preciso de acesso remoto',
      catalogoServicoId: 'srv-1',
      categoriaId: 'cat-1',
      prioridadeId: 'prio-1',
      naturezaChamado: 2,
      impactoChamado: 1,
      urgenciaChamado: 1,
    })

    const payloadEnviado = postMock.mock.calls[0]?.[1]
    expect(payloadEnviado).toMatchObject({
      naturezaChamado: 2,
      impactoChamado: 1,
      urgenciaChamado: 1,
    })
  })

  it('deve enviar apenas contrato minimo na abertura guiada por catalogo', async () => {
    const { portalService } = await import('./portalService')
    postMock.mockResolvedValueOnce({ id: 'chamado-1', codigo: 'CH-1' })

    await portalService.abrirRequisicaoServicoCatalogo({
      catalogoServicoId: 'srv-1',
      titulo: 'Solicitar VPN',
      descricao: 'Preciso de acesso remoto',
    })

    expect(postMock).toHaveBeenCalledWith('/api/portal/catalogo-servicos/requisicoes', {
      catalogoServicoId: 'srv-1',
      titulo: 'Solicitar VPN',
      descricao: 'Preciso de acesso remoto',
    })

    const payloadEnviado = postMock.mock.calls[0]?.[1] as Record<string, unknown>
    expect(payloadEnviado.naturezaChamado).toBeUndefined()
    expect(payloadEnviado.categoriaId).toBeUndefined()
    expect(payloadEnviado.subcategoriaId).toBeUndefined()
    expect(payloadEnviado.prioridadeId).toBeUndefined()
    expect(payloadEnviado.departamentoId).toBeUndefined()
    expect(payloadEnviado.grupoTecnicoId).toBeUndefined()
    expect(payloadEnviado.slaId).toBeUndefined()
    expect(payloadEnviado.requerAprovacao).toBeUndefined()
    expect(payloadEnviado.aprovacaoPendente).toBeUndefined()
    expect(payloadEnviado.aprovacaoChamadoId).toBeUndefined()
  })

  it('deve encaminhar respostas de formulario quando informadas na abertura guiada', async () => {
    const { portalService } = await import('./portalService')
    postMock.mockResolvedValueOnce({ id: 'chamado-1', codigo: 'CH-1' })

    await portalService.abrirRequisicaoServicoCatalogo({
      catalogoServicoId: 'srv-1',
      titulo: 'Solicitar acessos',
      descricao: 'Preciso de acessos adicionais',
      respostasFormulario: [
        {
          campoFormularioServicoId: 'campo-1',
          valor: 'vpn',
        },
        {
          campoFormularioServicoId: 'campo-2',
          valores: ['email', 'teams'],
        },
      ],
    })

    expect(postMock).toHaveBeenCalledWith('/api/portal/catalogo-servicos/requisicoes', {
      catalogoServicoId: 'srv-1',
      titulo: 'Solicitar acessos',
      descricao: 'Preciso de acessos adicionais',
      respostasFormulario: [
        {
          campoFormularioServicoId: 'campo-1',
          valor: 'vpn',
        },
        {
          campoFormularioServicoId: 'campo-2',
          valores: ['email', 'teams'],
        },
      ],
    })
  })

  it('deve propagar erro do backend na abertura guiada com resposta invalida', async () => {
    const { portalService } = await import('./portalService')
    const erro = new Error('HTTP 400: {"mensagem":"Resposta invalida."}')
    postMock.mockRejectedValueOnce(erro)

    await expect(
      portalService.abrirRequisicaoServicoCatalogo({
        catalogoServicoId: 'srv-1',
        titulo: 'Solicitar acessos',
        respostasFormulario: [
          {
            campoFormularioServicoId: 'campo-1',
            valor: 'abc',
          },
        ],
      })
    ).rejects.toBe(erro)
  })

  it('deve manter payload legado de abertura sem grupo tecnico ou fila', async () => {
    const { portalService } = await import('./portalService')
    postMock.mockResolvedValueOnce({ id: 'chamado-1', codigo: 'CH-1' })

    await portalService.criarChamado({
      titulo: 'Erro no portal',
      descricao: 'Falha ao acessar o portal',
      categoriaId: 'cat-1',
      prioridadeId: 'prio-1',
      naturezaChamado: 1,
      impactoChamado: 1,
      urgenciaChamado: 1,
    })

    expect(postMock).toHaveBeenCalledWith('/api/portal/chamados', {
      titulo: 'Erro no portal',
      descricao: 'Falha ao acessar o portal',
      categoriaId: 'cat-1',
      prioridadeId: 'prio-1',
      naturezaChamado: 1,
      impactoChamado: 1,
      urgenciaChamado: 1,
    })

    const payloadEnviado = postMock.mock.calls[0]?.[1] as Record<string, unknown>
    expect(payloadEnviado.grupoTecnicoId).toBeUndefined()
    expect(payloadEnviado.filaAtendimentoId).toBeUndefined()
  })

  it('deve consultar status de aprovacao de chamado no portal', async () => {
    const { portalService } = await import('./portalService')
    getMock.mockResolvedValueOnce({ chamadoId: 'ch-1', requerAprovacao: true })

    await portalService.obterStatusAprovacaoChamado('ch-1')

    expect(getMock).toHaveBeenCalledWith('/api/portal/chamados/ch-1/aprovacao')
  })
})
