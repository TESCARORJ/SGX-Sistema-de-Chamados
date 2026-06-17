# Sprint 5 - Regra de Motivo Obrigatório ao Cancelar Chamado

## Objetivo
Criar e proteger a regra de negócio que exige motivo obrigatório ao cancelar um chamado, garantindo rastreabilidade, auditoria e compatibilidade com o ciclo ITSM, separando esse fluxo de resoluções e encerramentos convencionais.

## Diferença entre Cancelamento, Resolução e Fechamento
- **Resolvido**: Quando a equipe técnica encontrou uma solução, mas aguarda aceite.
- **Fechado**: Quando o ciclo de vida do chamado se encerra positivamente (com aceite ou via SLA).
- **Cancelado**: Encerramento administrativo antes da solução, requerendo um motivo.

## Fluxo Anterior
Não existia um fluxo limpo exclusivo para cancelar chamados no motor de use cases.

## Regra Implementada
1. **Domínio Seguro:** O método `Chamado.Cancelar(status, motivo)` exige preenchimento válido, não permitindo motivo nulo ou vazio e forçando a geração da exceção `ArgumentException`.
2. **DTO e Validação:** `CancelarChamadoRequest` com FluentValidation.
3. **Use Case Exclusivo:** `CancelarChamadoUseCase` coordena o encerramento SLA, geração de histórico (`TipoHistoricoChamado.Cancelado`), comentários de cancelamento e auditoria unificada.

## Comportamentos Esperados (Válido / Inválido)
- **Válido:** O motivo é preenchido, status muda para `Cancelado`, `EncerradoEm` recebe data/hora atual e o histórico registra o evento.
- **Inválido:** Motivo vazio, nulo ou preenchido só com espaços interrompem a ação no domínio (fail-fast), não gerando falso histórico, sem alterar SLA.

## Impactos nas Camadas
- **Domínio:** Criação do método `Cancelar` em `Chamado.cs` e `Cancelado` em `TipoHistoricoChamado.cs`.
- **API/UseCase:** Endpoint `POST /api/admin/chamados/{id}/cancelar` e `CancelarChamadoUseCase.cs`.
- **Auditoria:** Gravação unificada de motivo no log e comentários internos.
- **SLA e Aprovação:** Preservada validação de bloqueio administrativo/dependência ativa e congelamento no `EncerradoEm`.

## Testes Realizados
Criado pacote de testes `CancelarChamadoUseCaseTests.cs` com 100% de cobertura das restrições e regras.
