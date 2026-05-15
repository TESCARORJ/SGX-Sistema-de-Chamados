import { describe, expect, it, vi, beforeEach } from 'vitest'

const getMock = vi.fn()
const postMock = vi.fn()
const getFileMock = vi.fn()

vi.mock('./httpClient', () => ({
  httpClient: {
    get: getMock,
    post: postMock,
    getFile: getFileMock,
  },
}))

describe('chamadosService (anexos)', () => {
  beforeEach(() => {
    getMock.mockReset()
    postMock.mockReset()
    getFileMock.mockReset()
  })

  it('deve chamar endpoint de listagem de anexos', async () => {
    const { chamadosService } = await import('./chamadosService')
    getMock.mockResolvedValueOnce([])

    await chamadosService.listarAnexosChamado('abc')

    expect(getMock).toHaveBeenCalledWith('/api/chamados/abc/anexos')
  })

  it('deve chamar endpoint de listagem da linha do tempo', async () => {
    const { chamadosService } = await import('./chamadosService')
    getMock.mockResolvedValueOnce({ chamadoId: 'abc', codigo: 'CH-1', items: [] })

    await chamadosService.listarLinhaTempoChamado('abc')

    expect(getMock).toHaveBeenCalledWith('/api/chamados/abc/linha-do-tempo')
  })

  it('deve chamar endpoint de upload de anexo', async () => {
    const { chamadosService } = await import('./chamadosService')
    postMock.mockResolvedValueOnce({})
    const arquivo = new File(['conteudo'], 'teste.txt', { type: 'text/plain' })

    await chamadosService.enviarAnexoChamado('abc', arquivo)

    expect(postMock).toHaveBeenCalledTimes(1)
    expect(postMock.mock.calls[0][0]).toBe('/api/chamados/abc/anexos')
  })

  it('deve chamar endpoint de download de anexo', async () => {
    const { chamadosService } = await import('./chamadosService')
    getFileMock.mockResolvedValueOnce({ blob: new Blob(), nomeArquivo: 'a.pdf', contentType: 'application/pdf' })

    await chamadosService.baixarAnexoChamado('abc', 'anx1')

    expect(getFileMock).toHaveBeenCalledWith('/api/chamados/abc/anexos/anx1/download')
  })

  it('nao deve expor funcoes de exclusao de anexo', async () => {
    const { chamadosService } = await import('./chamadosService')
    const metodos = Object.keys(chamadosService).map((item) => item.toLowerCase())

    expect(metodos).not.toContain('removeranexochamado')
    expect(metodos).not.toContain('excluiranexochamado')
    expect(metodos).not.toContain('deletaranexochamado')
  })
})
