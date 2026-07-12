import { beforeEach, describe, expect, it, vi } from 'vitest'

function criarFetchAbortavel(): ReturnType<typeof vi.fn> {
  return vi.fn((_url: RequestInfo | URL, init?: RequestInit) => {
    return new Promise<Response>((_resolve, reject) => {
      const signal = init?.signal
      if (!signal) {
        return
      }

      const abortar = () => {
        signal.removeEventListener('abort', abortar)
        reject(new DOMException('The operation was aborted.', 'AbortError'))
      }

      if (signal.aborted) {
        abortar()
        return
      }

      signal.addEventListener('abort', abortar, { once: true })
    })
  })
}

describe('httpClient', () => {
  beforeEach(() => {
    vi.resetModules()
    vi.unstubAllGlobals()
    vi.unstubAllEnvs()
    vi.useRealTimers()
    vi.stubEnv('VITE_API_BASE_URL', '')
  })

  it('deve usar rota relativa /api por padrao', async () => {
    const { httpClient } = await import('./httpClient')
    const fetchMock = vi.fn().mockResolvedValue(
      new Response(JSON.stringify({ ok: true }), {
        status: 200,
        headers: { 'content-type': 'application/json' },
      })
    )
    vi.stubGlobal('fetch', fetchMock)

    await httpClient.get('/api/saude')

    expect(fetchMock).toHaveBeenCalledTimes(1)
    expect(fetchMock.mock.calls[0]?.[0]).toBe('/api/saude')
  })

  it('deve concluir a requisicao antes do timeout', async () => {
    vi.useFakeTimers()
    const { httpClient } = await import('./httpClient')
    const fetchMock = vi.fn().mockResolvedValue(
      new Response(JSON.stringify({ ok: true }), {
        status: 200,
        headers: { 'content-type': 'application/json' },
      })
    )
    vi.stubGlobal('fetch', fetchMock)

    await expect(httpClient.get('/api/saude')).resolves.toEqual({ ok: true })
    expect(vi.getTimerCount()).toBe(0)
  })

  it('deve abortar requisicoes JSON quando o timeout expira', async () => {
    vi.useFakeTimers()
    const { httpClient } = await import('./httpClient')
    const fetchMock = criarFetchAbortavel()
    vi.stubGlobal('fetch', fetchMock)

    const promessa = httpClient.get('/api/saude')
    const erroPromise = promessa.catch((error: unknown) => error)
    await vi.advanceTimersByTimeAsync(30_000)
    const erro = await erroPromise

    expect(erro).toMatchObject({
      status: 408,
    })
    expect(erro).toBeInstanceOf(Error)
    expect((erro as Error).message).toBe(
      'A API demorou mais de 30 segundos para responder.'
    )
  })

  it('deve abortar downloads quando o timeout expira', async () => {
    vi.useFakeTimers()
    const { httpClient } = await import('./httpClient')
    const fetchMock = criarFetchAbortavel()
    vi.stubGlobal('fetch', fetchMock)

    const promessa = httpClient.getFile('/api/chamados/1/anexos/2/download')
    const erroPromise = promessa.catch((error: unknown) => error)
    await vi.advanceTimersByTimeAsync(30_000)
    const erro = await erroPromise

    expect(erro).toMatchObject({
      status: 408,
    })
    expect(erro).toBeInstanceOf(Error)
    expect((erro as Error).message).toBe(
      'A API demorou mais de 30 segundos para responder.'
    )
  })

  it('deve limpar o temporizador apos sucesso', async () => {
    vi.useFakeTimers()
    const { httpClient } = await import('./httpClient')
    const fetchMock = vi.fn().mockResolvedValue(
      new Response(JSON.stringify({ ok: true }), {
        status: 200,
        headers: { 'content-type': 'application/json' },
      })
    )
    vi.stubGlobal('fetch', fetchMock)

    await httpClient.get('/api/saude')

    expect(vi.getTimerCount()).toBe(0)
  })

  it('deve limpar o temporizador apos erro HTTP', async () => {
    vi.useFakeTimers()
    const { httpClient } = await import('./httpClient')
    const fetchMock = vi.fn().mockResolvedValue(
      new Response('Falha controlada', {
        status: 500,
      })
    )
    vi.stubGlobal('fetch', fetchMock)

    await expect(httpClient.get('/api/saude')).rejects.toMatchObject({
      status: 500,
      message: 'HTTP 500: Falha controlada',
    })

    expect(vi.getTimerCount()).toBe(0)
  })

  it('deve preservar o redirect e o erro 401', async () => {
    const assignMock = vi.fn()
    vi.stubGlobal('window', {
      location: {
        pathname: '/painel',
        assign: assignMock,
      },
    } as unknown as Window & typeof globalThis)

    const { httpClient } = await import('./httpClient')
    const fetchMock = vi.fn().mockResolvedValue(new Response('', { status: 401 }))
    vi.stubGlobal('fetch', fetchMock)

    await expect(httpClient.get('/api/saude')).rejects.toMatchObject({
      status: 401,
      message: 'Acesso não autenticado (401).',
    })

    expect(assignMock).toHaveBeenCalledWith('/login')
  })

  it('deve preservar o redirect e o erro 403', async () => {
    const assignMock = vi.fn()
    vi.stubGlobal('window', {
      location: {
        pathname: '/painel',
        assign: assignMock,
      },
    } as unknown as Window & typeof globalThis)

    const { httpClient } = await import('./httpClient')
    const fetchMock = vi.fn().mockResolvedValue(new Response('', { status: 403 }))
    vi.stubGlobal('fetch', fetchMock)

    await expect(httpClient.get('/api/saude')).rejects.toMatchObject({
      status: 403,
      message: 'Acesso negado (403).',
    })

    expect(assignMock).toHaveBeenCalledWith('/acesso-negado')
  })

  it('deve preservar envio de FormData sem forcar content-type JSON', async () => {
    const { httpClient } = await import('./httpClient')
    const fetchMock = vi.fn().mockResolvedValue(
      new Response(JSON.stringify({ ok: true }), {
        status: 200,
        headers: { 'content-type': 'application/json' },
      })
    )
    vi.stubGlobal('fetch', fetchMock)

    const formData = new FormData()
    formData.append('arquivo', new Blob(['conteudo']), 'teste.txt')

    await httpClient.post('/api/anexos', formData)

    expect(fetchMock).toHaveBeenCalledTimes(1)
    expect(fetchMock.mock.calls[0]?.[1]).toMatchObject({
      method: 'POST',
      body: formData,
    })
    expect((fetchMock.mock.calls[0]?.[1] as RequestInit).headers).not.toMatchObject({
      'Content-Type': 'application/json',
    })
  })
})
