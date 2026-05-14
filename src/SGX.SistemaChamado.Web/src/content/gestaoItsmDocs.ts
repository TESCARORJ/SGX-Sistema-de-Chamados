export type CategoriaDocumentoItsm =
  | 'Visão Geral'
  | 'Roadmap'
  | 'SLA'
  | 'Autenticação Corporativa'
  | 'Azure / Microsoft Entra ID'
  | 'Homologação'
  | 'Governança'
  | 'Service Desk'
  | 'Segurança'
  | 'Infraestrutura'

export type DocumentoItsm = {
  id: string
  titulo: string
  categoria: CategoriaDocumentoItsm
  resumo: string
  conteudo: string
  tags: string[]
  atualizadoEm?: string
}

export const categoriasDocumentosItsm: CategoriaDocumentoItsm[] = [
  'Visão Geral',
  'Roadmap',
  'SLA',
  'Autenticação Corporativa',
  'Azure / Microsoft Entra ID',
  'Homologação',
  'Governança',
  'Service Desk',
  'Segurança',
  'Infraestrutura',
]

export const documentosItsm: DocumentoItsm[] = [
  {
    id: 'visao-geral-sgx',
    titulo: 'Visão Geral do SGX Sistema de Chamados',
    categoria: 'Visão Geral',
    resumo:
      'Panorama funcional da plataforma SGX como solução de Service Desk/ITSM para operação interna.',
    atualizadoEm: '2026-05-14',
    tags: ['service desk', 'itsm', 'portal', 'administração', 'governança'],
    conteudo: `
# Visão Geral do SGX Sistema de Chamados

O SGX Sistema de Chamados é uma plataforma de Service Desk/ITSM para registro, acompanhamento, tratamento, controle e gestão de chamados internos.

## Capacidades principais

- Portal do solicitante para abertura e acompanhamento de chamados.
- Área administrativa para atendimento, cadastros, perfis, permissões e governança.
- Controle de SLA com políticas, metas por prioridade, alertas, eventos e painel.
- Autenticação corporativa com Microsoft Entra ID/Azure AD e autorização interna no SGX.
- Roadmap ITSM para acompanhar evolução, status técnico, pendências, critérios de aceite e evidências.
- Documentação integrada para consulta por gestores, equipe técnica e potenciais clientes.

## Objetivo operacional

Centralizar a operação de atendimento interno com rastreabilidade, indicadores, regras de acesso e documentação suficiente para homologação, apresentação e evolução contínua do sistema.
`,
  },
  {
    id: 'roadmap-geral',
    titulo: 'Roadmap Geral',
    categoria: 'Roadmap',
    resumo:
      'Resumo da evolução geral do SGX, entregas realizadas, itens em evolução e próximos passos recomendados.',
    atualizadoEm: '2026-05-14',
    tags: ['roadmap', 'evolução', 'entregas', 'próximas etapas'],
    conteudo: `
# Roadmap Geral

O roadmap geral consolida as entregas estruturais do SGX Sistema de Chamados e orienta as próximas etapas de evolução.

## Entregas consolidadas

- Backend .NET, PostgreSQL, EF Core e arquitetura por camadas.
- Portal do solicitante e área administrativa em Vue 3 + Quasar.
- Chamados, comentários, histórico, anexos e cadastros administrativos.
- Perfis e permissões internas com \`GET /api/me\`.
- Worker de e-mail, logs administrativos e integração com SLA.
- Documentação técnica inicial no repositório e agora também acessível no painel administrativo.

## Evolução recomendada

- Homologar com usuários reais.
- Validar integrações Microsoft Entra ID e IMAP em ambiente institucional.
- Evoluir relatórios, observabilidade, notificações oficiais e evidências formais.
`,
  },
  {
    id: 'roadmap-itsm',
    titulo: 'Roadmap ITSM',
    categoria: 'Roadmap',
    resumo:
      'Organização dos módulos, status, percentuais, pendências, evidências e critérios de aceite do SGX.',
    atualizadoEm: '2026-05-14',
    tags: ['roadmap itsm', 'status', 'checklist', 'evidências', 'critérios de aceite'],
    conteudo: `
# Roadmap ITSM

O Roadmap ITSM organiza os módulos, status, percentuais, pendências, evidências, critérios de aceite e próximas ações do SGX Sistema de Chamados.

## Como interpretar

- **Status da implementação** indica a maturidade funcional da entrega.
- **Status técnico** indica completude, pendências e riscos evolutivos.
- **Percentual** pode ser calculado por checklist ativo.
- **Pendências técnicas e de homologação** devem registrar o que ainda impede aceite formal ou produção.
- **Evidências** apontam telas, endpoints, testes e documentos que sustentam a entrega.
- **Critérios de aceite** descrevem como validar a funcionalidade com objetividade.

## Uso gerencial

A seção de roadmap ajuda a apresentar evolução para gestores, priorizar próximas ações, registrar decisões e manter rastreabilidade entre necessidade de negócio, implementação técnica e homologação.
`,
  },
  {
    id: 'sla',
    titulo: 'SLA',
    categoria: 'SLA',
    resumo:
      'Funcionamento das políticas, metas, aplicação no chamado, pausas, alertas, eventos e painel de SLA.',
    atualizadoEm: '2026-05-14',
    tags: ['sla', 'prioridade', 'primeira resposta', 'resolução', 'alertas', 'calendário'],
    conteudo: `
# SLA

O módulo de SLA define políticas de atendimento e metas por prioridade para acompanhar primeira resposta, resolução e cumprimento operacional dos chamados.

## Conceitos principais

- Políticas de SLA podem definir metas por prioridade.
- O SLA aplicado ao chamado é registrado na abertura quando há política/meta compatível.
- Primeira resposta pode ser registrada por comentário público de atendente ou mudança para atendimento.
- Resolução é registrada em status final ou status de resolução conforme o fluxo.
- O SLA pode pausar quando o chamado está aguardando solicitante.
- Alertas e eventos apoiam o acompanhamento de prazos próximos do vencimento ou vencidos.
- O painel de SLA exibe indicadores operacionais para gestão.

## Calendário e cálculo

O sistema suporta minutos corridos e horário comercial por calendário corporativo. Quando uma política usa horário comercial, prazos e minutos decorridos respeitam o calendário configurado.

## Pendências evolutivas

- Calendário específico por departamento/time.
- Importação automática de feriados.
- Exportação de relatórios.
- Evidências formais em homologação e ambiente publicado.
`,
  },
  {
    id: 'autenticacao-corporativa',
    titulo: 'Autenticação Corporativa',
    categoria: 'Segurança',
    resumo:
      'Decisão arquitetural: Microsoft Entra ID autentica e o SGX autoriza por perfis e permissões internas.',
    atualizadoEm: '2026-05-14',
    tags: ['segurança', 'microsoft entra id', 'azure ad', 'jwt', 'permissões', 'api me'],
    conteudo: `
# Autenticação Corporativa

A autenticação corporativa do SGX separa identidade e autorização.

## Decisão arquitetural

- Microsoft Entra ID/Azure AD autentica o usuário.
- SGX Sistema de Chamados autoriza internamente.
- Perfis e permissões ficam no SGX.
- \`groups\` e \`roles\` do Azure não concedem Administrador automaticamente.

## Fluxo resumido

1. Usuário acessa o frontend e entra com Microsoft Entra ID.
2. Frontend recebe token e chama a API.
3. API valida JWT, issuer, audience, tenant e domínio permitido quando configurado.
4. SGX identifica ou cria o usuário interno conforme regra configurada.
5. \`GET /api/me\` retorna perfis e permissões efetivas.
6. Frontend libera rotas, menus e ações com base no SGX.

## Pendência de homologação

O fluxo está implementado funcionalmente, mas exige homologação com tenant institucional real, usuários corporativos reais, MFA, Conditional Access, logout corporativo e evidências formais.
`,
  },
  {
    id: 'configuracao-azure-ad',
    titulo: 'Configuração Azure AD / Microsoft Entra ID',
    categoria: 'Azure / Microsoft Entra ID',
    resumo:
      'Orientações objetivas para App Registration, redirect URI, client id, tenant id, escopos e validação com equipe Azure.',
    atualizadoEm: '2026-05-14',
    tags: ['azure ad', 'entra id', 'app registration', 'client id', 'tenant id', 'escopos'],
    conteudo: `
# Configuração Azure AD / Microsoft Entra ID

A integração Microsoft depende de configuração coordenada entre SGX, frontend, API e equipe responsável pelo tenant Azure.

## Itens de configuração

- Registrar a aplicação no Microsoft Entra ID.
- Configurar Redirect URI da SPA.
- Registrar Client ID e Tenant ID no frontend.
- Configurar escopos da API e consentimento.
- Configurar issuer, audience e authority no backend.
- Validar domínio permitido quando aplicável.
- Validar MFA e Conditional Access com políticas institucionais.

## Validação necessária

A equipe Azure deve confirmar App Registration, permissões, escopos, consentimento, URLs publicadas e regras de segurança antes da homologação formal.
`,
  },
  {
    id: 'checklist-homologacao',
    titulo: 'Checklist de Homologação',
    categoria: 'Homologação',
    resumo:
      'Checklist funcional para validar login, perfis, abertura, atendimento, SLA, permissões e evidências.',
    atualizadoEm: '2026-05-14',
    tags: ['homologação', 'checklist', 'evidências', 'perfis', 'aceite'],
    conteudo: `
# Checklist de Homologação

A homologação deve validar o SGX com usuários, perfis e ambiente representativos.

## Fluxos mínimos

- Login com usuário real.
- Perfis Solicitante, Atendente e Administrador.
- Abertura de chamado pelo portal.
- Atendimento administrativo.
- Aplicação e acompanhamento de SLA.
- Alertas e eventos de SLA.
- Encerramento do chamado.
- Reabertura quando aplicável.
- Permissões permitidas e bloqueadas por perfil.

## Evidências esperadas

Cada evidência deve conter print, data, usuário, perfil, ambiente, resultado esperado e resultado observado. Pendências encontradas devem ser registradas no roadmap ou em plano de correção.
`,
  },
]

export function normalizarTextoDocumentoItsm(valor: string): string {
  return valor
    .normalize('NFD')
    .replace(/\p{Diacritic}/gu, '')
    .toLocaleLowerCase('pt-BR')
}

export function filtrarDocumentosItsm(
  documentos: DocumentoItsm[],
  termoBusca: string,
  categoria: CategoriaDocumentoItsm | 'Todas'
): DocumentoItsm[] {
  const termo = normalizarTextoDocumentoItsm(termoBusca.trim())

  return documentos.filter((documento) => {
    const correspondeCategoria = categoria === 'Todas' || documento.categoria === categoria
    if (!correspondeCategoria) {
      return false
    }

    if (!termo) {
      return true
    }

    const textoDocumento = normalizarTextoDocumentoItsm(
      [documento.titulo, documento.categoria, documento.resumo, documento.conteudo, documento.tags.join(' ')].join(' ')
    )

    return textoDocumento.includes(termo)
  })
}

function escaparHtml(valor: string): string {
  return valor
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;')
}

function renderizarInlineMarkdown(valor: string): string {
  return escaparHtml(valor)
    .replace(/`([^`]+)`/g, '<code>$1</code>')
    .replace(/\*\*([^*]+)\*\*/g, '<strong>$1</strong>')
}

export function markdownItsmParaHtml(markdown: string): string {
  const linhas = markdown.trim().split(/\r?\n/)
  const partes: string[] = []
  let listaAberta = false

  const fecharLista = () => {
    if (listaAberta) {
      partes.push('</ul>')
      listaAberta = false
    }
  }

  for (const linhaOriginal of linhas) {
    const linha = linhaOriginal.trim()

    if (!linha) {
      fecharLista()
      continue
    }

    if (linha.startsWith('## ')) {
      fecharLista()
      partes.push(`<h2>${renderizarInlineMarkdown(linha.slice(3))}</h2>`)
      continue
    }

    if (linha.startsWith('# ')) {
      fecharLista()
      partes.push(`<h1>${renderizarInlineMarkdown(linha.slice(2))}</h1>`)
      continue
    }

    if (linha.startsWith('- ')) {
      if (!listaAberta) {
        partes.push('<ul>')
        listaAberta = true
      }

      partes.push(`<li>${renderizarInlineMarkdown(linha.slice(2))}</li>`)
      continue
    }

    fecharLista()
    partes.push(`<p>${renderizarInlineMarkdown(linha)}</p>`)
  }

  fecharLista()

  return partes.join('\n')
}
