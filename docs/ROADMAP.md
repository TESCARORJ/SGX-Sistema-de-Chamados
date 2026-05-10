# Roadmap SGX Sistema de Chamados

## Visao geral

O SGX Sistema de Chamados e uma solucao institucional para abertura, acompanhamento, atendimento, SLA, gestao administrativa e integracao de chamados por e-mail.

Diretriz de seguranca:
- Microsoft Entra ID autentica.
- SGX Sistema de Chamados autoriza internamente por perfis e permissoes.

## Entregue

- Migracao para .NET
- PostgreSQL + EF Core
- Estrutura Domain, Application, Infrastructure, Api, Worker.Email e Web
- Autenticacao Microsoft Entra ID
- Login local Development
- Autorizacao interna por perfis
- Perfis Administrador, Atendente e Solicitante
- Portal do solicitante
- Area administrativa
- Abertura e acompanhamento de chamados
- Comentarios, historico e anexos
- Cadastros administrativos
- SLA e dashboard
- Worker IMAP
- Logs de integracao de e-mail
- Frontend Vue 3 + Quasar
- Central de notificacoes frontend/local
- Perfis e permissoes granulares
- Matriz de permissoes por perfil
- GET /api/me com permissoes efetivas
- Controle visual por permissao

## Em evolucao

- Auditoria avancada de alteracoes de permissoes
- API real de notificacoes
- Relatorios exportaveis
- Testes automatizados frontend/e2e
- Homologacao com usuarios reais
- Integracao Microsoft Entra ID real em ambiente institucional
- Validacao IMAP real
- Deploy em VPS/Docker Compose
- Hardening final de producao
- Observabilidade, logs e monitoramento

## Proximas etapas recomendadas

1. Validar matriz de permissoes com usuarios-chave.
2. Validar fluxo real de abertura/atendimento de chamados.
3. Validar integracao Microsoft Entra ID real.
4. Validar leitura IMAP real.
5. Criar API persistente de notificacoes.
6. Criar auditoria detalhada de alteracao de permissoes.
7. Criar relatorios exportaveis.
8. Preparar ambiente de homologacao.
9. Preparar deploy em VPS.
10. Executar checklist final de seguranca.

## Pontos para reuniao

- Azure/Microsoft Entra ID autentica; SGX autoriza.
- Perfis atuais: Administrador, Atendente e Solicitante.
- Evolucao implementada: permissoes granulares configuraveis por modulo e acao.
- Beneficio: ajustar acessos sem alteracao de codigo.
- Beneficio: seguranca e rastreabilidade.
- Beneficio: aderencia a operacao institucional.
- Beneficio: possibilidade de criar perfis derivados no futuro, como Atendente N1, Atendente N2, Supervisor e Auditor.
- Beneficio: separacao clara entre autenticacao corporativa e regra interna do sistema.

## Status do item Perfis de Acesso

| Item | Status | Observacao |
|---|---|---|
| Perfis macro | Concluido | Administrador, Atendente e Solicitante |
| CRUD de perfis | Concluido | Area administrativa |
| Associacao usuario-perfil | Concluido | Usuario pode ter perfis internos |
| Permissoes granulares | Concluido | Catalogo por modulo e acao |
| Matriz de permissoes | Concluido | Perfil tem permissoes configuraveis |
| GET /api/me com permissoes | Concluido | Frontend recebe permissoes efetivas |
| Policies por permissao | Concluido | Backend suporta autorizacao granular |
| Controle visual por permissao | Concluido | Acoes sao exibidas conforme permissao |
| Auditoria detalhada | Proxima etapa | Historico avancado de alteracoes |

## Roadmap ITSM - status real

- `Status` permanece como classificacao geral/legado para compatibilidade.
- `StatusImplementacao` e a referencia principal para maturidade real da entrega.
- `Objetivo` descreve a finalidade de cada item e a necessidade de negocio/operacao que ele resolve.
- `Implementado funcionalmente` significa entrega tecnica feita, mas ainda nao implica homologacao final ou producao.
- Pendencias evolutivas devem ser registradas em `PendenciasTecnicas`, `PendenciasHomologacao` e `ProximaAcao`.

Exemplos de objetivo preenchido:
- Abertura de chamado pelo portal
- Abertura por e-mail
- Perfis de acesso

## Roadmap ITSM - futuras implementacoes

- Cada item do roadmap pode registrar evolucoes no CRUD de futuras implementacoes.
- O registro suporta backlog, planejamento, desenvolvimento, homologacao, conclusao, inativacao e reativacao.
- Objetivo: dar governanca ao que falta sem perder rastreabilidade do que ja foi entregue.

## Roadmap ITSM - governanca de categoria e checklist

- Categoria agora e cadastro controlado em tabela propria (`RoadmapCategoria`) com selecao por dropdown.
- Itens antigos continuam com fallback no campo legado `Categoria` para compatibilidade.
- Percentual de implementacao passa a ser calculado automaticamente pelo checklist ativo do item.
- Regra de calculo: concluidos ativos / ativos * 100.
- UI exibe labels amigaveis para status tecnico/implementacao, sem mostrar enums crus.

## Atualizacao Sprint Portal 3

Entregas consolidadas:
- upload de anexos na abertura (`/portal/chamados/novo`) com envio sequencial apos criacao
- tratamento de falha parcial de anexos sem perder abertura do chamado
- validacao visual de tipo/tamanho de anexo com base no contexto do portal
- listagem de chamados no portal validada com exibicao de chamados recem-criados
- detalhe do portal validado com historico, anexos, comentarios publicos e SLA
- integracao portal/admin validada em testes de listagem e detalhe administrativo

Pendencias evolutivas:
- homologacao manual com usuario real
- testes E2E do fluxo portal->admin

## Sprint Portal 4 - Fechamento tecnico

Item: Abertura de chamado pelo portal
- Situacao: Implementado funcionalmente
- Status tecnico: Completo com pendencias evolutivas

Entregas confirmadas:
- backend da abertura validado
- frontend /portal/chamados/novo validado
- anexos, listagem e detalhe do portal validados
- visibilidade na fila administrativa validada
- historico inicial validado

Pendencias evolutivas reais:
- homologacao manual com usuario real
- testes E2E frontend
- validacao de anexos em ambiente real

## Sprint Integracoes E-mail 2 - Atualizacao

Entregas tecnicas confirmadas:
- abertura automatica de chamado para e-mail novo;
- origem `Email` aplicada ao chamado;
- status inicial `Aberto` aplicado;
- historico inicial `Chamado criado a partir de e-mail`;
- deduplicacao por `MessageId` mantida;
- configuracoes de categoria/prioridade/departamento/dominios no `EmailWorker`.

Pendencias para evolucao:
- correlacao avancada de respostas;
- escopo completo de anexos por e-mail;
- validacao IMAP real com caixa institucional;
- homologacao com e-mails reais.

## Sprint Integracoes E-mail 3 - Atualizacao

Entregas tecnicas confirmadas:
- correlacao de resposta por codigo no assunto (`SGX`/`CHM`);
- correlacao por `InReplyTo` e `References` via `LogIntegracaoEmail`;
- resposta correlacionada gera comentario publico e historico de resposta;
- anexos validados por extensao, tamanho e MIME;
- bloqueio explicito de extensoes perigosas e tentativa de path traversal;
- sucesso parcial de anexos sem derrubar processamento do Worker.

Pendencias evolutivas:
- validacao IMAP real e homologacao com e-mails reais;
- OAuth para caixa Microsoft (se exigido pelo ambiente);
- retry/backoff, dead-letter e reprocessamento manual;
- varredura antivirus e sanitizacao HTML avancada;
- monitoramento operacional e E2E com IMAP real.

## Sprint Integracoes E-mail 4 - Atualizacao

Entregas tecnicas confirmadas:
- endpoints administrativos de logs de e-mail com listagem, detalhe, filtros, ordenacao e paginacao;
- tela administrativa `/admin/integracoes/email` com filtros, estado vazio/erro/loading e dialog tecnico;
- labels amigaveis de status (`Nao correlacionado` sem enum cru na UI);
- acao de abrir chamado vinculado a partir da tabela e do detalhe do log;
- bloqueio de acesso para `Solicitante` mantido em backend e frontend.

Pendencias evolutivas:
- validacao com caixa IMAP real e homologacao com e-mails reais;
- reprocessamento manual, retry/backoff e dead-letter;
- monitoramento operacional do Worker e E2E com IMAP real.

## Sprint Integracoes E-mail 5 - Atualizacao

Fechamento tecnico realizado:
- item `Abertura por e-mail` atualizado para `Implementado funcionalmente`;
- status tecnico atualizado para `Completo com pendencias evolutivas`;
- checklist tecnico/evolutivo vinculado ao roadmap com percentual automatico por itens ativos;
- documentacao consolidada de configuracao, processamento, correlacao, anexos e logs administrativos.

Pendencias reais mantidas:
- validacao IMAP real e homologacao com e-mails/anexos reais;
- OAuth Microsoft (se exigido), retry/backoff, dead-letter;
- monitoramento/metricas/alertas do Worker;
- reprocessamento manual de mensagens com erro;
- sanitizacao HTML avancada e antivirus de anexos.
