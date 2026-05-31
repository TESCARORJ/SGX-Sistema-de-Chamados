import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('CadastroDetalheBaseView - Matriz de Permissões', () => {
  it('deve possuir o mapeamento exato de 12 agrupamentos na função mapModuloLabel', () => {
    const caminho = new URL('./CadastroDetalheBaseView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    // Verificações das 12 chaves de agrupamentos específicos do ITSM e ITIL
    expect(fonte).toContain("Dashboard: 'Chamados'")
    expect(fonte).toContain("Chamados: 'Chamados'")
    expect(fonte).toContain("AprovacaoChamados: 'Chamados'")
    expect(fonte).toContain("Problemas: 'Problemas'")
    expect(fonte).toContain("Mudancas: 'Mudanças'")
    expect(fonte).toContain("Tarefas: 'Tarefas'")
    expect(fonte).toContain("Sla: 'SLA'")
    expect(fonte).toContain("RelatoriosAvancados: 'Relatórios'")
    expect(fonte).toContain("Usuarios: 'Usuários'")
    expect(fonte).toContain("Perfis: 'Perfis'")
    expect(fonte).toContain("Cadastros: 'Cadastros'")
    expect(fonte).toContain("BaseConhecimento: 'Cadastros'")
    expect(fonte).toContain("CatalogoServicos: 'Cadastros'")
    expect(fonte).toContain("InventarioAtivos: 'Cadastros'")
    expect(fonte).toContain("Auditoria: 'Auditoria'")
    expect(fonte).toContain("AuditoriaAutenticacao: 'Auditoria'")
    expect(fonte).toContain("Parametros: 'Configurações'")
    expect(fonte).toContain("IntegracoesEmail: 'Configurações'")
    expect(fonte).toContain("IntegracoesMicrosoft: 'Configurações'")
    expect(fonte).toContain("IntegracoesActiveDirectory: 'Configurações'")
    expect(fonte).toContain("AutenticacaoProvedores: 'Configurações'")
    expect(fonte).toContain("Notificacoes: 'Configurações'")
    expect(fonte).toContain("Indicadores: 'Configurações'")
    expect(fonte).toContain("Roadmap: 'Roadmap'")
    expect(fonte).toContain("RoadmapImplementacoes: 'Roadmap'")
  })

  it('deve agrupar permissões diretamente por moduloLabel em modulosPermissoes para evitar acordeões duplicados', () => {
    const caminho = new URL('./CadastroDetalheBaseView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    // Deve agrupar pelo labelGrupo para evitar a duplicação
    expect(fonte).toContain("const labelGrupo = mapModuloLabel(chaveModulo)")
    expect(fonte).toContain("if (!grupos.has(labelGrupo))")
    expect(fonte).toContain("grupos.set(labelGrupo, [])")
    expect(fonte).toContain("moduloLabel,")
  })

  it('deve renderizar a chave do loop do expansion-item baseada no moduloLabel', () => {
    const caminho = new URL('./CadastroDetalheBaseView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    // Evita o key "modulo" que causaria loops incorretos com acordeões duplicados
    expect(fonte).toContain(':key="modulo.moduloLabel"')
  })

  it('deve vincular o checkbox à lista de codigosPermissoesSelecionadas', () => {
    const caminho = new URL('./CadastroDetalheBaseView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('v-model="codigosPermissoesSelecionadas"')
    expect(fonte).toContain(':val="permissao.codigo"')
  })

  it('deve invocar o salvamento via salvarPermissoesPerfil ao clicar no botão', () => {
    const caminho = new URL('./CadastroDetalheBaseView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('@click="salvarPermissoesPerfil"')
    expect(fonte).toContain('label="Salvar')
  })

  it('deve exibir o erro retornado da API ou do salvamento em um q-banner', () => {
    const caminho = new URL('./CadastroDetalheBaseView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('v-else-if="erroPermissoesPerfil"')
    expect(fonte).toContain(':mensagem="erroPermissoesPerfil"')
  })
})
