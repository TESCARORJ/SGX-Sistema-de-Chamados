# ACOES-POR-NATUREZA-ITSM

## 1. Objetivo da regra
Centralizar no backend a definicao de acoes administrativas disponiveis por chamado, considerando:
- natureza ITSM;
- status atual;
- estado atual do chamado;
- perfil/permissoes existentes do usuario autenticado.

## 2. Relacao entre natureza, status e acoes disponiveis
A regra central esta no servico `AcoesChamadoService` (`Application`) e usa:
- `NaturezaChamadoEnum`;
- `StatusChamadoEnum`;
- `IFluxoStatusChamadoService` (status permitidos por natureza);
- contexto do usuario (`UsuarioContextoAplicacao` com perfis/permissoes).

Comportamento inicial desta sprint:
- `Incidente`, `Requisicao`, `Mudanca`, `Problema`:
  - `Assumir`, `Atribuir`, `AlterarStatus`, `AlterarPrioridade`, `AlterarCategoria`, `Encerrar`, `Reabrir`, `Comentar`, `Anexar` (conforme estado/permissao).
- `EventoAlerta`, `TarefaOperacional`:
  - `Assumir`, `Atribuir`, `AlterarStatus`, `Encerrar`, `Reabrir`, `Comentar`, `Anexar` (conforme estado/permissao).
- chamado em status final (`Encerrado`/`Cancelado`):
  - bloqueia acoes operacionais de alteracao;
  - `Reabrir` permanece disponivel quando permitido.

## 3. Backend como fonte unica
- O DTO de detalhe admin (`ChamadoAdminDetalheResponse`) agora retorna `AcoesDisponiveisCodigos`.
- O frontend apenas consome essa lista para exibir/ocultar botoes.
- Validacao final continua no backend, inclusive para chamadas diretas de endpoint.

## 4. Acoes implementadas agora
- Identificadores controlados em `AcaoChamadoEnum`.
- Servico central:
  - `ObterAcoesDisponiveis`;
  - `AcaoEstaDisponivel`;
  - `ValidarAcaoDisponivel`.
- Integracao no detalhe admin:
  - `AcoesDisponiveisCodigos`.
- Validacao centralizada aplicada nesta sprint em:
  - `AlterarStatus`;
  - `AlterarPrioridade`;
  - `Encerrar`;
  - `Reabrir`.

## 5. Acoes futuras apenas planejadas
Os identificadores abaixo existem apenas para disponibilidade futura, sem fluxo funcional completo nesta sprint:
- `AprovarMudanca`;
- `ReprovarMudanca`;
- `ExecutarMudanca`;
- `RegistrarCausaRaiz`;
- `RegistrarSolucaoContorno`;
- `CorrelacionarEvento`;
- `TratarEvento`;
- `ConcluirTarefa`.

## 6. Limitacoes atuais
- Nem todos os endpoints administrativos aplicam validacao centralizada completa nesta etapa (adocao progressiva).
- Parte do refinamento de permissoes ainda depende de maior granularidade por role/perfil e por tipo de operacao.
- Acoes futuras ITIL estao apenas modeladas como identificadores.

## 7. Pendencias futuras
- aprovacao real de mudanca;
- causa raiz de problema;
- correlacao de evento/alerta;
- conclusao especifica de tarefa operacional;
- refinamento por perfil/role e possiveis politicas adicionais.
