# Auditoria de Autenticacao - SGX Sistema de Chamados

## Objetivo
Registrar de forma persistida os eventos de autenticacao e governanca dos metodos de login.

## Base de persistencia
- entidade persistida: `EventoAuditoria` (auditoria corporativa unificada);
- modulo padrao para esta trilha: `Autenticacao`;
- entidade logica:
  - `EventoAutenticacao` (eventos de login e senha);
  - `MetodosLogin` (eventos administrativos de configuracao).

## Estrutura de classificacao
- enum `TipoEventoAutenticacao`;
- enum `ResultadoEventoAutenticacao`:
  - `Sucesso`
  - `Falha`
  - `Bloqueado`
  - `Negado`

## Eventos cobertos
- login local SGX bem-sucedido/negado;
- login Active Directory bem-sucedido/negado;
- login Microsoft Entra ID bem-sucedido (quando autenticacao Microsoft e efetiva);
- usuario inativo bloqueado;
- tentativa com provedor desabilitado;
- falha de configuracao de provedor (AD);
- falha por credencial invalida;
- auto provisionamento de usuario;
- troca obrigatoria de senha concluida;
- solicitacao/fluxo de recuperacao e redefinicao de senha local;
- alteracoes administrativas dos metodos de login:
  - habilitado/desabilitado
  - principal
  - ordem
  - auto provisionamento
  - perfil padrao
  - rotulo de exibicao
- tentativa negada de alteracao por falta de permissao;
- bloqueio de configuracao insegura pelo backend.

## Endpoint administrativo
- `GET /api/admin/auditoria/autenticacao`
- permissao requerida: `AuditoriaAutenticacao.Visualizar`

Observacao:
- o endpoint usa a trilha persistida e retorna eventos filtrados por modulo `Autenticacao`.
- o endpoint agora retorna campos estruturados para consulta administrativa frontend:
  - `provedor`
  - `tipoEvento`
  - `resultado`
  - `mensagem`
- filtros suportados para a consulta administrativa:
  - `dataInicio`
  - `dataFim`
  - `usuarioEmail`
  - `provedor`
  - `tipoEventoAutenticacao`
  - `resultadoAutenticacao`
  - `pagina`
  - `tamanhoPagina`

## Sprint 6 - Consulta administrativa no frontend
- tela administrativa criada em: `Admin > Governanca > Auditoria de autenticacao`
- rota: `/admin/governanca/auditoria-autenticacao`
- controle de acesso por permissao:
  - menu exibido somente com `AuditoriaAutenticacao.Visualizar`
  - rota protegida, redirecionando para `Acesso Negado` sem permissao
- recursos da tela:
  - filtros por periodo, provedor, resultado, tipo de evento e usuario/e-mail
  - listagem paginada
  - ordenacao por eventos mais recentes primeiro
  - estados de vazio e erro de carregamento
  - exibicao apenas de dados seguros (sem senha/token/hash/segredos)

## Regras de seguranca
- nao registrar senha, token, hash, segredo ou credencial sensivel;
- registrar contexto seguro: data/hora, executor (quando houver), usuario alvo (quando aplicavel), IP, user-agent e correlacao;
- falha de auditoria nao deve interromper login;
- falha de auditoria deve gerar log tecnico seguro no backend.
