import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('CatalogoServicosFormPage - formulario administrativo', () => {
  it('deve integrar a secao de formulario do servico no detalhe administrativo', () => {
    const caminho = new URL('./CatalogoServicosFormPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('CatalogoServicoFormularioSection')
    expect(fonte).toContain(':catalogo-servico-id="servicoAtual.id"')
    expect(fonte).toContain('Salve o servico antes de configurar o formulario administrativo.')
  })

  it('deve expor a secao administrativa com criacao e edicao basica de formulario, versoes, campos e opcoes', () => {
    const caminho = new URL('../components/admin/CatalogoServicoFormularioSection.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Criar formulario')
    expect(fonte).toContain('Salvar formulario')
    expect(fonte).toContain('Nova versao')
    expect(fonte).toContain('Editar versao')
    expect(fonte).toContain('Novo campo')
    expect(fonte).toContain('Editar campo')
    expect(fonte).toContain('Nova opcao')
    expect(fonte).toContain('Editar opcao')
  })

  it('deve orquestrar a secao administrativa usando o service de formulario esperado', () => {
    const caminho = new URL('../components/admin/CatalogoServicoFormularioSection.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain("import { formularioServicosAdminService }")
    expect(fonte).toContain('formularioServicosAdminService.listarFormularios')
    expect(fonte).toContain('formularioServicosAdminService.obterFormulario')
    expect(fonte).toContain('formularioServicosAdminService.criarFormulario')
    expect(fonte).toContain('formularioServicosAdminService.atualizarFormulario')
    expect(fonte).toContain('formularioServicosAdminService.criarVersao')
    expect(fonte).toContain('formularioServicosAdminService.criarCampo')
    expect(fonte).toContain('formularioServicosAdminService.criarOpcao')
  })
})
