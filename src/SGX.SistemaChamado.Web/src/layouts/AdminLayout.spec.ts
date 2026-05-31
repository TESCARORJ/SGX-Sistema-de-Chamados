import { readFileSync } from 'node:fs'
import { webcrypto } from 'node:crypto'
import { describe, expect, it } from 'vitest'

globalThis.window = {
  location: {
    origin: 'http://localhost',
    href: 'http://localhost/',
    host: 'localhost',
    pathname: '/'
  },
  crypto: webcrypto
} as any

describe('Layouts - Menus Dinâmicos por Ator', () => {
  it('deve possuir a estrutura do menu dinâmico para Administrador', () => {
    const caminho = new URL('./AdminLayout.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("perfis.includes('Administrador')")
    expect(fonte).toContain("label: 'Usuários'")
    expect(fonte).toContain("label: 'Perfis e permissões'")
    expect(fonte).toContain("label: 'Cadastros administrativos'")
    expect(fonte).toContain("label: 'SLA'")
    expect(fonte).toContain("label: 'Integrações'")
    expect(fonte).toContain("label: 'Configurações'")
    expect(fonte).toContain("label: 'Roadmap'")
  })

  it('deve possuir a estrutura do menu dinâmico para Coordenador Service Desk', () => {
    const caminho = new URL('./AdminLayout.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("perfis.includes('Coordenador Service Desk')")
    expect(fonte).toContain("label: 'Dashboard operacional'")
    expect(fonte).toContain("label: 'Fila geral'")
    expect(fonte).toContain("label: 'Chamados críticos'")
    expect(fonte).toContain("label: 'Atribuições'")
    expect(fonte).toContain("label: 'Relatórios operacionais'")
  })

  it('deve possuir a estrutura do menu dinâmico para Técnico N2', () => {
    const caminho = new URL('./AdminLayout.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("perfis.includes('Técnico N2')")
    expect(fonte).toContain("label: 'Minha fila técnica'")
    expect(fonte).toContain("label: 'Chamados escalados'")
    expect(fonte).toContain("label: 'Problemas'")
    expect(fonte).toContain("label: 'Mudanças'")
    expect(fonte).toContain("label: 'Tarefas operacionais'")
  })

  it('deve possuir a estrutura do menu dinâmico para Atendente N1', () => {
    const caminho = new URL('./AdminLayout.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("perfis.includes('Atendente N1')")
    expect(fonte).toContain("label: 'Fila de atendimento'")
    expect(fonte).toContain("label: 'Meus atendimentos'")
    expect(fonte).toContain("label: 'Triagem'")
  })

  it('deve possuir a estrutura do menu dinâmico para Gestor TI', () => {
    const caminho = new URL('./AdminLayout.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("perfis.includes('Gestor TI')")
    expect(fonte).toContain("label: 'Dashboard executivo'")
    expect(fonte).toContain("label: 'Indicadores ITSM'")
    expect(fonte).toContain("label: 'Relatórios'")
    expect(fonte).toContain("label: 'Problemas recorrentes'")
  })

  it('deve possuir a estrutura do menu dinâmico para Auditor Governança', () => {
    const caminho = new URL('./AdminLayout.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("perfis.includes('Auditor Governança')")
    expect(fonte).toContain("label: 'Consulta de chamados'")
    expect(fonte).toContain("label: 'Histórico e auditoria'")
    expect(fonte).toContain("label: 'Logs/Auditoria'")
  })

  it('deve mapear corretamente os novos itens de menu no resolverGrupoMenu', () => {
    const caminho = new URL('./AdminLayout.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("case 'Minha fila técnica':")
    expect(fonte).toContain("case 'Triagem':")
    expect(fonte).toContain("case 'Fila de atendimento':")
    expect(fonte).toContain("case 'Fila geral':")
    expect(fonte).toContain("case 'Dashboard executivo':")
    expect(fonte).toContain("case 'Logs/Auditoria':")
  })

  it('deve conter exatamente os quatro itens de menu do Solicitante no PortalLayout', () => {
    const caminho = new URL('./PortalLayout.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("label: 'Meus chamados'")
    expect(fonte).toContain("label: 'Abrir chamado'")
    expect(fonte).toContain("label: 'Base de conhecimento'")
    expect(fonte).toContain("label: 'Minha conta'")
    expect(fonte).not.toContain("label: 'Dashboard'")
    expect(fonte).not.toContain("label: 'Catalogo de servicos'")
  })

  it('deve possuir o q-select para alternar visão de homologação e não ter os botões fixos', () => {
    const caminho = new URL('./AdminLayout.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('drawer-emulacao-select')
    expect(fonte).toContain('podeEmularQualquerPerfil')
    expect(fonte).toContain('opcoesEmulacao')
    expect(fonte).toContain('perfilSelecionado')

    // Não exibe mais os dois botões fixos antigos
    expect(fonte).not.toContain('Visualizar como Solicitante')
    expect(fonte).not.toContain('Visualizar como Atendente')
  })

  it('deve conter todos os 8 perfis operacionais no computed opcoesEmulacao', () => {
    const caminho = new URL('./AdminLayout.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("label: 'Administrador'")
    expect(fonte).toContain("label: 'Solicitante'")
    expect(fonte).toContain("label: 'Atendente'")
    expect(fonte).toContain("label: 'Atendente N1'")
    expect(fonte).toContain("label: 'Técnico N2'")
    expect(fonte).toContain("label: 'Coordenador Service Desk'")
    expect(fonte).toContain("label: 'Gestor TI'")
    expect(fonte).toContain("label: 'Auditor Governança'")
  })

  it('deve implementar a lógica de mudança de visão de homologação e transição de rotas', () => {
    const caminho = new URL('./AdminLayout.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('alterarVisaoHomologacao')
    expect(fonte).toContain('obterRotaPadraoPerfil')
    expect(fonte).toContain('authStore.iniciarEmulacaoPerfil')
  })

  it('deve mapear rotas iniciais dinâmicas e permitir saída da emulação na tela de acesso negado', async () => {
    const { obterRotaInicialParaPerfil } = await import('../stores/authStore')
    // Verificar rotas iniciais dinâmicas por perfil chamando a função centralizada
    expect(obterRotaInicialParaPerfil('Solicitante')).toBe('/portal/chamados')
    expect(obterRotaInicialParaPerfil('Atendente N1')).toBe('/admin/chamados')
    expect(obterRotaInicialParaPerfil('Técnico N2')).toBe('/admin/chamados')
    expect(obterRotaInicialParaPerfil('Gestor TI')).toBe('/admin/relatorios/avancados')
    expect(obterRotaInicialParaPerfil('Auditor Governança')).toBe('/admin/governanca/auditoria')
    expect(obterRotaInicialParaPerfil('Administrador')).toBe('/admin')
    expect(obterRotaInicialParaPerfil('Atendente')).toBe('/admin/chamados')
    expect(obterRotaInicialParaPerfil('Coordenador Service Desk')).toBe('/admin')

    const caminhoAdmin = new URL('./AdminLayout.vue', import.meta.url)
    const fonteAdmin = readFileSync(caminhoAdmin, 'utf-8')
    expect(fonteAdmin).toContain('await router.replace(authStore.rotaInicial)')

    // Verificar AcessoNegadoView.vue
    const caminhoAcesso = new URL('../views/AcessoNegadoView.vue', import.meta.url)
    const fonteAcesso = readFileSync(caminhoAcesso, 'utf-8')

    expect(fonteAcesso).toContain('emulando')
    expect(fonteAcesso).toContain('sairVisao')
    expect(fonteAcesso).toContain('authStore.encerrarEmulacao()')
    expect(fonteAcesso).toContain('Sair da visão de homologação')
    expect(fonteAcesso).toContain('Ir para meu painel')
  })
})
