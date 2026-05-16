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
  {
    id: 'historico-auditoria',
    titulo: 'Historico / Auditoria',
    categoria: 'Governança',
    resumo:
      'Consulta administrativa de auditoria com filtros, detalhe e indicadores de governanca.',
    atualizadoEm: '2026-05-14',
    tags: ['auditoria', 'governanca', 'rastreabilidade', 'historico', 'seguranca', 'itsm'],
    conteudo: `
# Historico / Auditoria

O modulo de auditoria do SGX registra eventos de governanca para garantir rastreabilidade real das acoes executadas no sistema e agora possui consulta administrativa dedicada.

## Diferenca entre log tecnico e auditoria

- Log tecnico (ILogger) apoia diagnostico operacional e depuracao.
- Auditoria de governanca registra quem fez, quando, de onde, em qual modulo e com qual efeito.

## O que as Sprints 1, 2 e 3 entregam

- Sprint 1: estrutura central com entidade \`EventoAuditoria\`, tabela \`eventos_auditoria\`, enums, service centralizado e contexto de requisicao.
- Sprint 2: aplicacao da auditoria nos modulos criticos com registro de eventos operacionais e administrativos.
- Sprint 3: consulta administrativa com endpoint de listagem paginada, endpoint de detalhe, dashboard de auditoria, filtros avancados e tela em \`/admin/governanca/auditoria\`.

## Como acessar

- Menu administrativo: Admin > Governanca > Auditoria.
- Rota principal: \`/admin/governanca/auditoria\`.
- Documentacao ITSM: \`/admin/gestao-itsm/documentacao\`.
- Permissao necessaria: \`Auditoria.Visualizar\` (Administrador possui acesso automaticamente).

## Modulos auditados na Sprint 2

- Chamados.
- Usuarios.
- Perfis e Permissoes.
- SLA administrativo (politicas, metas, alertas e calendarios).
- Autenticacao Corporativa (Microsoft Entra ID / Azure AD).
- Roadmap ITSM (item e checklist).
- Gestao ITSM / Documentacao: leitura nao auditada; edicao ainda nao existe no sistema.

## Eventos registrados por modulo

- Chamados: abertura, status, prioridade, categoria, atribuicao, assumir, comentario admin interno/publico, encerramento, reabertura e anexo adicionado.
- Usuarios: criacao, edicao, ativacao, inativacao, alteracao de perfis e eventos administrativos de senha local.
- Perfis e Permissoes: criacao/edicao/ativacao/inativacao de perfil e alteracao de permissoes.
- SLA: criacao/edicao/ativacao/inativacao de politica, alteracao de alerta, criacao/edicao/remoção logica de horario e excecao, criacao/edicao de calendario.
- Autenticacao Corporativa: sucesso e falha de login Microsoft por tenant/dominio/usuario interno.
- Roadmap ITSM: criacao/edicao/status de item, criacao/edicao/conclusao/reabertura/inativacao/reativacao/exclusao logica de checklist.

## Filtros da consulta administrativa

- periodo (\`dataInicio\` e \`dataFim\`);
- usuario (\`usuarioId\` e \`usuarioEmail\`);
- modulo, entidade e entidadeId;
- acao e nivel;
- sucesso/falha;
- IP de origem;
- correlacaoId;
- busca textual (\`texto\`) para descricao e contexto.

## Indicadores na tela

- total de eventos;
- total de eventos criticos;
- total de falhas e sucessos;
- agrupamentos por modulo, acao, usuario e dia;
- ultimos eventos criticos e ultimas falhas.

## EventoSla x EventoAuditoria

- \`EventoSla\` registra o ciclo operacional de prazo do chamado.
- \`EventoAuditoria\` registra governanca: quem alterou configuracoes, regras e dados auditaveis.
- Os dois podem coexistir sem duplicar o mesmo fato operacional.

## Dados armazenados por evento

- identificadores e data/hora do evento;
- usuario (id/nome/email/login) quando disponivel;
- origem (IP e User-Agent) quando houver HttpContext;
- modulo, entidade, entidadeId e acao;
- descricao funcional da acao;
- dadosAntes, dadosDepois e metadados em texto/json;
- nivel, sucesso/falha e mensagem de erro;
- correlacaoId para agrupar eventos da mesma operacao.

## Antes e depois

- Alteracoes relevantes registram \`dadosAntes\` e \`dadosDepois\`.
- Para entidades grandes, o registro prioriza campos alterados e metadados essenciais.
- Comentarios administrativos nao gravam o texto completo; gravam identificador e tamanho.

## Mascaramento de dados sensiveis

- \`AuditoriaDiffHelper\` mascara campos como senha, password, token, jwt, secret, clientSecret, refreshToken, accessToken, connectionString, chave, apiKey, authorization e bearer.
- Valor mascarado padrao: \`***\`.
- Auditoria nao persiste senha, hash de senha, JWT, refresh token, client secret ou connection string.

## Uso de correlacao

O \`correlacaoId\` permite associar os eventos da mesma requisicao e apoiar investigacao, rastreio de impacto e homologacao.

## Limitacoes atuais

- exportacao de auditoria (Excel/PDF) ainda nao implementada;
- sem tela de retencao configuravel de auditoria;
- sem assinatura/hash da trilha para evidencias criptograficas;
- sem dashboard avancado de seguranca e alertas proativos.

## Proximas sprints previstas

- exportacao de auditoria (Excel/PDF);
- retencao configuravel;
- assinatura/hash da trilha;
- alertas para eventos criticos;
- painel avancado de seguranca e integracao com SIEM/Log Analytics.
`,
  },
  {
    id: 'comentarios-anexos-atendimento',
    titulo: 'Comentarios e Anexos no Atendimento',
    categoria: 'Service Desk',
    resumo:
      'Entrega funcional do atendimento com comentarios e anexos por perfil, validacoes de seguranca e rastreabilidade.',
    atualizadoEm: '2026-05-15',
    tags: [
      'atendimento',
      'comentarios',
      'anexos',
      'service desk',
      'rastreabilidade',
      'seguranca',
      'itsm',
      'roadmap',
    ],
    conteudo: `
# Comentarios e Anexos no Atendimento

O modulo de Atendimento permite que usuarios autorizados interajam no chamado por comentarios e anexos, centralizando comunicacao e evidencias do atendimento.

## Area e status consolidado

- Area: Atendimento
- Item: Comentarios e anexos
- StatusImplementacao: Implementado funcionalmente
- StatusTecnico: Completo
- PercentualImplementacao: 100%
- Avaliacao: Aprovado
- Pendencia bloqueante: Nenhuma

## Perfis envolvidos

- Administrador
- Atendente
- Solicitante

## Regras de comentarios

- Administrador cria e visualiza comentarios publicos e internos.
- Atendente cria e visualiza comentarios publicos e internos.
- Solicitante cria apenas comentarios publicos.
- Solicitante visualiza apenas comentarios publicos.
- Solicitante nao pode criar comentario interno.
- Solicitante nao pode visualizar comentario interno.
- Comentario vazio e rejeitado.
- Limite maximo de 4000 caracteres.
- Ordenacao cronologica dos comentarios.

## Endpoints de comentarios

- \`GET /api/chamados/{chamadoId}/comentarios\`
- \`POST /api/chamados/{chamadoId}/comentarios\`

## Regras de anexos

- Administrador, Atendente e Solicitante podem anexar arquivos em chamados permitidos.
- Solicitante so pode anexar, listar e baixar anexos dos proprios chamados.
- Administrador pode listar e baixar anexos de qualquer chamado.
- Atendente pode listar e baixar anexos de chamados acessiveis para atendimento.
- Upload rejeita arquivo vazio.
- Upload valida tamanho maximo.
- Upload valida extensoes permitidas.
- Upload bloqueia extensoes perigosas.
- Download valida acesso ao chamado antes da abertura do arquivo.
- API nao expoe caminho fisico.
- API nao expoe nome fisico armazenado.
- Storage protegido contra path traversal.

## Regra obrigatoria de rastreabilidade

- Anexo salvo nao pode ser excluido por nenhum perfil.
- Nao existe endpoint DELETE de anexos.
- Nao existe botao de exclusao de anexo no frontend.
- Nao existe exclusao logica ou fisica de anexos.
- Justificativa: anexos compoem evidencia do atendimento.

## Endpoints de anexos

- \`GET /api/chamados/{chamadoId}/anexos\`
- \`POST /api/chamados/{chamadoId}/anexos\`
- \`GET /api/chamados/{chamadoId}/anexos/{anexoId}/download\`

## Extensoes permitidas

- \`.pdf\`
- \`.png\`
- \`.jpg\`
- \`.jpeg\`
- \`.doc\`
- \`.docx\`
- \`.xls\`
- \`.xlsx\`
- \`.txt\`
- \`.csv\`
- \`.zip\`

## Extensoes bloqueadas

- \`.exe\`
- \`.bat\`
- \`.cmd\`
- \`.ps1\`
- \`.sh\`
- \`.js\`
- \`.vbs\`
- \`.msi\`
- \`.dll\`
- \`.scr\`
- \`.com\`
- \`.jar\`
- \`.hta\`
- \`.reg\`

## Configuracao de arquivos

As regras de upload sao controladas por \`ArquivosOptions\` e \`appsettings\`.

\`\`\`json
"Arquivos": {
  "DiretorioBase": "storage/anexos",
  "TamanhoMaximoBytes": 10485760,
  "ExtensoesPermitidas": [ ".pdf", ".png", ".jpg", ".jpeg", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv", ".zip" ],
  "ExtensoesBloqueadas": [ ".exe", ".bat", ".cmd", ".ps1", ".sh", ".js", ".vbs", ".msi", ".dll", ".scr", ".com", ".jar", ".hta", ".reg" ]
}
\`\`\`

## Evidencias tecnicas

Comentarios:
- Migration: \`20260515154700_AddComentariosAtendimento\`
- Endpoints: \`GET /api/chamados/{chamadoId}/comentarios\`, \`POST /api/chamados/{chamadoId}/comentarios\`
- Testes backend aprovados
- Testes frontend aprovados
- Build frontend aprovado

Anexos:
- Migration: \`20260515161320_AddAnexosAtendimento\`
- Endpoints: \`GET /api/chamados/{chamadoId}/anexos\`, \`POST /api/chamados/{chamadoId}/anexos\`, \`GET /api/chamados/{chamadoId}/anexos/{anexoId}/download\`
- Testes backend aprovados
- Testes frontend aprovados
- Build frontend aprovado

Resultados de validacao:
- \`dotnet test\` aprovado
- \`npm.cmd run test:unit\` aprovado
- \`npm.cmd run build\` aprovado
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

