import { describe, expect, it } from 'vitest'
import {
  documentosItsm,
  filtrarDocumentosItsm,
  markdownItsmParaHtml,
} from './gestaoItsmDocs'

describe('gestaoItsmDocs', () => {
  it('deve conter os documentos iniciais obrigatórios', () => {
    const titulos = documentosItsm.map((documento) => documento.titulo)

    expect(titulos).toContain('Visão Geral do SGX Sistema de Chamados')
    expect(titulos).toContain('Roadmap ITSM')
    expect(titulos).toContain('SLA')
    expect(titulos).toContain('Autenticação Corporativa')
    expect(titulos).toContain('Configuração Azure AD / Microsoft Entra ID')
    expect(titulos).toContain('Checklist de Homologação')
    expect(titulos).toContain('Comentarios e Anexos no Atendimento')
  })

  it('deve filtrar documentos por busca ignorando acentos e caixa', () => {
    const resultado = filtrarDocumentosItsm(documentosItsm, 'autenticacao corporativa', 'Todas')

    expect(resultado.map((documento) => documento.id)).toContain('autenticacao-corporativa')
  })

  it('deve filtrar documentos por categoria', () => {
    const resultado = filtrarDocumentosItsm(documentosItsm, '', 'SLA')

    expect(resultado).toHaveLength(1)
    expect(resultado[0].titulo).toBe('SLA')
  })

  it('deve escapar html ao renderizar markdown interno', () => {
    const html = markdownItsmParaHtml('# Título\n\n<script>alert("xss")</script>')

    expect(html).toContain('<h1>Título</h1>')
    expect(html).toContain('&lt;script&gt;alert(&quot;xss&quot;)&lt;/script&gt;')
    expect(html).not.toContain('<script>')
  })
})
