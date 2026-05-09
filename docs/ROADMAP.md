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
- `Implementado funcionalmente` significa entrega tecnica feita, mas ainda nao implica homologacao final ou producao.
- Pendencias evolutivas devem ser registradas em `PendenciasTecnicas`, `PendenciasHomologacao` e `ProximaAcao`.

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
