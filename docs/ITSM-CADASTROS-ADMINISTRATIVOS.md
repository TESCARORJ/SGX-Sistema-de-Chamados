# Cadastros Administrativos no contexto ITSM

## Objetivo

O modulo de Cadastros Administrativos fornece a base de classificacao, organizacao, priorizacao, triagem, filtros e historico dos chamados no SGX Sistema de Chamados.

## Cadastros previstos

- Departamentos
- Categorias de chamado
- Subcategorias de chamado
- Prioridades
- Tipos de solicitação
- Locais / Unidades

## Relação com práticas ITSM

### Gestão de Incidentes

Categorias, subcategorias, prioridades e locais/unidades apoiam a classificacao inicial dos incidentes, direcionam a triagem para o atendimento correto, ajudam a medir impacto e melhoram a leitura analitica de relatorios operacionais.

### Gestão de Requisições de Serviço

Tipos de solicitacao permitem diferenciar incidente, solicitacao de servico, duvida, melhoria e problema recorrente, reduzindo ambiguidade na abertura e no atendimento.

### Apoio à Gestão de SLA

Prioridades com peso e cor podem servir como base para regras de SLA, tempo de primeira resposta, tempo de solucao e indicadores evolutivos de cumprimento.

### Apoio ao Catálogo de Serviços

Categorias e subcategorias podem evoluir para um catalogo de servicos mais estruturado, com padronizacao progressiva por dominio de atendimento.

### Apoio à Gestão de Conhecimento

A classificacao padronizada permite identificar recorrencias, criar artigos de base de conhecimento e melhorar a qualidade do atendimento ao longo do tempo.

### Apoio à Gestão por Unidade ou Localidade

Locais/unidades permitem analisar origem dos chamados, demanda por unidade e necessidade de atendimento presencial, apoiando priorizacao operacional.

## Regras operacionais

- Apenas registros ativos devem aparecer em novas operacoes.
- Registros inativos devem permanecer disponiveis para historico.
- Subcategorias devem pertencer a uma categoria.
- A subcategoria selecionada deve pertencer a categoria selecionada.
- Prioridades devem possuir nome, peso e, opcionalmente, cor.
- Exclusao de cadastro deve preferencialmente usar inativacao logica.
- Chamados antigos nao devem perder historico quando um cadastro for inativado.

## Fluxo operacional esperado

1. Administrador configura cadastros.
2. Solicitante abre chamado usando cadastros ativos.
3. Sistema valida categoria, subcategoria, prioridade, tipo de solicitacao e local/unidade.
4. Atendimento usa os dados para triagem e gestao.
5. Administracao usa filtros e relatorios.
6. Historico permanece preservado mesmo apos inativacao de cadastros.

## Endpoints administrativos esperados

Departamentos:
- GET /api/admin/departamentos
- GET /api/admin/departamentos/{id}
- POST /api/admin/departamentos
- PUT /api/admin/departamentos/{id}
- DELETE /api/admin/departamentos/{id}
- PATCH /api/admin/departamentos/{id}/ativar
- PATCH /api/admin/departamentos/{id}/inativar

Categorias:
- GET /api/admin/categorias
- GET /api/admin/categorias/{id}
- POST /api/admin/categorias
- PUT /api/admin/categorias/{id}
- DELETE /api/admin/categorias/{id}
- PATCH /api/admin/categorias/{id}/ativar
- PATCH /api/admin/categorias/{id}/inativar

Subcategorias:
- GET /api/admin/subcategorias
- GET /api/admin/subcategorias/{id}
- GET /api/admin/categorias/{categoriaId}/subcategorias
- POST /api/admin/subcategorias
- PUT /api/admin/subcategorias/{id}
- DELETE /api/admin/subcategorias/{id}
- PATCH /api/admin/subcategorias/{id}/ativar
- PATCH /api/admin/subcategorias/{id}/inativar

Prioridades:
- GET /api/admin/prioridades
- GET /api/admin/prioridades/{id}
- POST /api/admin/prioridades
- PUT /api/admin/prioridades/{id}
- DELETE /api/admin/prioridades/{id}
- PATCH /api/admin/prioridades/{id}/ativar
- PATCH /api/admin/prioridades/{id}/inativar

Tipos de Solicitacao:
- GET /api/admin/tipos-solicitacao
- GET /api/admin/tipos-solicitacao/{id}
- POST /api/admin/tipos-solicitacao
- PUT /api/admin/tipos-solicitacao/{id}
- DELETE /api/admin/tipos-solicitacao/{id}
- PATCH /api/admin/tipos-solicitacao/{id}/ativar
- PATCH /api/admin/tipos-solicitacao/{id}/inativar

Locais / Unidades:
- GET /api/admin/locais
- GET /api/admin/locais/{id}
- POST /api/admin/locais
- PUT /api/admin/locais/{id}
- DELETE /api/admin/locais/{id}
- PATCH /api/admin/locais/{id}/ativar
- PATCH /api/admin/locais/{id}/inativar

## Endpoints operacionais esperados

- GET /api/cadastros/departamentos/ativos
- GET /api/cadastros/categorias/ativas
- GET /api/cadastros/categorias/{categoriaId}/subcategorias/ativas
- GET /api/cadastros/prioridades/ativas
- GET /api/cadastros/tipos-solicitacao/ativos
- GET /api/cadastros/locais/ativos

## Telas administrativas esperadas

Menu esperado:

```text
Admin
 └── Cadastros
      ├── Departamentos
      ├── Categorias
      ├── Subcategorias
      ├── Prioridades
      ├── Tipos de Solicitacao
      └── Locais / Unidades
```

Cada tela deve possuir:

- Listagem
- Busca por nome
- Filtro por status
- Criar
- Editar
- Ativar
- Inativar
- Mensagens de sucesso e erro
- Tratamento de carregamento
- Tratamento de lista vazia

## Seed inicial sugerido

Prioridades:
- Baixa - Peso 1 - Cor #22C55E
- Media - Peso 2 - Cor #EAB308
- Alta - Peso 3 - Cor #F97316
- Critica - Peso 4 - Cor #EF4444

Tipos de Solicitacao:
- Incidente
- Solicitacao de Servico
- Duvida
- Melhoria
- Problema Recorrente

Categorias:
- Hardware
- Software
- Rede
- Sistema
- Acesso
- E-mail
- Impressora
- Telefonia
- Solicitacao Administrativa

Departamentos:
- Tecnologia da Informacao
- Recursos Humanos
- Financeiro
- Juridico
- Atendimento
- Infraestrutura

Locais / Unidades:
- Sede
- Filial
- Inspetoria
- Datacenter
- Almoxarifado
- Atendimento Externo

## Beneficios esperados

- Padronizacao da abertura de chamados.
- Melhor triagem.
- Reducao de chamados mal classificados.
- Melhoria dos filtros administrativos.
- Base para relatorios.
- Base para SLA.
- Base para catalogo de servicos.
- Base para gestao de conhecimento.
- Preservacao historica.

## Limites atuais

- Status de chamado nao faz parte deste modulo.
- Catalogo de servicos completo deve ser tratado em evolucao futura.
- SLA avancado por categoria/prioridade/tipo pode ser tratado em evolucao futura.
- Homologacao visual/manual deve ser realizada em ambiente institucional.
- Testes frontend E2E podem ser tratados em evolucao futura.

## Evolucoes futuras recomendadas

- Catalogo de servicos.
- SLA por prioridade, categoria e tipo de solicitacao.
- Dashboard ITSM.
- Relatorios por local/unidade.
- Relatorios por categoria/subcategoria.
- Base de conhecimento vinculada a categoria/subcategoria.
- Testes E2E automatizados.
- Seed institucional configuravel por ambiente.

## Status atual da trilha de cadastros (Item 8)

- Status da implementacao: Fluxo funcional validado.
- Status tecnico: Aguardando homologacao institucional.
- Percentual do item de roadmap: 90%.
- Checklist consolidado: 7/8 concluidos.
- Situacao atual: Modulo de Cadastros Administrativos implementado e validado funcionalmente em nivel tecnico. Backend, frontend administrativo, integracao com abertura/gestao de chamados, seed inicial e validacao funcional foram concluidos. A homologacao institucional/manual com evidencias formais permanece pendente.

Pendencias tecnicas atuais:
- Nao ha pendencias tecnicas bloqueantes identificadas para o modulo.
- Manter como evolucao futura a cobertura frontend E2E completa.
- Avaliar futuramente se status de chamado continuara como fluxo controlado ou se sera parametrizado em cadastro proprio.

Validacoes tecnicas confirmadas:
- abertura de chamado usa somente cadastros ativos;
- subcategoria e validada por categoria;
- detalhe portal e administrativo exibem nomes dos cadastros vinculados;
- filtros administrativos por categoria, subcategoria, prioridade, tipo, local e departamento estao operacionais;
- chamados antigos preservam visibilidade historica mesmo com inativacao posterior dos cadastros.
- seed inicial esta idempotente, sem duplicidade por variacao de acentuacao e sem reativacao automatica de registros inativados manualmente.
- validacoes administrativas de cadastro (obrigatoriedade, duplicidade, formato de cor e regras de ativo/inativo) estao cobertas por suite backend e testes de integracao HTTP.

## Homologacao institucional manual (pendente)

Situacao atual:
- homologacao institucional ainda nao registrada com evidencia manual real nesta etapa.

Roteiro formal definido:
- roteiro de 25 passos registrado em `docs/CHECKLIST-HOMOLOGACAO-CADASTROS.md`;
- validacao manual obrigatoria dos fluxos administrativos, abertura de chamado, filtros, ativo/inativo e preservacao historica.

Evidencias obrigatorias:
- prints das telas administrativas e do fluxo de chamado com cadastros;
- registro de responsavel, data, ambiente e resultado final (`Aprovado`, `Aprovado com ressalvas` ou `Reprovado`).

Regra de governanca:
- sem evidencias institucionais anexadas, o item deve permanecer em `90%` e `7/8`, sem promocao para homologado.
