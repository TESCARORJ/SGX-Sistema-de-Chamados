# Regra de Aceite do Solicitante (Item 9 - Sprint 5)

## Visão Geral
A funcionalidade de **Aceite do Solicitante** permite que o criador de um chamado informe formalmente que concorda com a solução técnica fornecida pela equipe de atendimento.
Esta ação representa o encerramento do ciclo técnico do chamado. O chamado transiciona do estado `Resolvido` para o estado `Fechado` definitivo, garantindo a rastreabilidade da aprovação do serviço prestado.

## O que Mudou na Arquitetura

### 1. Entidade Chamado
Adicionamos novas propriedades de controle ao `Chamado` para não misturarmos a ação de resolução com a ação de fechamento via aceite:
- `AceitoEm` (DateTime?): Marca o instante exato em que o aceite foi dado.
- `AceitoPorUsuarioId` (Guid?): Marca qual usuário realizou a ação (geralmente o solicitante).
- `ObservacaoAceite` (string): Opcional. Permite registrar comentários adicionais ao aceitar.

### 2. Status e Transições
- **Condição:** Um chamado **só pode ser aceito se o seu status atual for `Resolvido`**. Apenas preencher a resolução (Sprint 5 - Item 6/7) não finaliza o ciclo, é o aceite (ou o timeout de fechamento automático) que conclui de fato.
- **Transição:** Ao aceitar, o status muda automaticamente para `Fechado` (`EncerradoEm` é preenchido e o SLA encerra sua contabilização completa de ciclo).

### 3. Integrações de Negócio
- **Validação de Bloqueio:** Para respeitar o motor de governança (Sprint 4), não se pode aceitar a solução de um chamado caso ele possua uma aprovação obrigatória pendente que exija tratamento (embora um chamado com aprovação pendente bloqueante não devesse sequer chegar ao estado Resolvido sem aprovação, a segurança em dupla camada garante isso).
- **Endpoint Exclusivo:** O Portal ganha um endpoint exclusivo `POST /api/portal/chamados/{id}/aceitar-solucao` com autorização voltada ao dono do chamado.

### 4. Auditoria e Rastreabilidade
Para total conformidade com o módulo de Governança e Auditoria do SGX, a ação de Aceite provoca três eventos paralelos de registro:
1. **Comentário Público:** A observação preenchida pelo solicitante vira um comentário visível na timeline com o rótulo adequado.
2. **Histórico do Chamado:** Um registro de `TipoHistoricoChamado.SolucaoAceita` é adicionado para controle na linha do tempo.
3. **Log de Auditoria Institucional:** Toda a mudança de estado e os dados do aceite (quem, quando, comentários, IPs) vão para a tabela segura de `eventos_auditoria`.

## Próximos Passos no Roadmap
Esta entrega conclui a primeira de duas regras de avaliação do Solicitante. A próxima regra da **Sprint 5** será a **Rejeição da Solução**, que reabre o chamado (retirando o estado Resolvido) e o devolve ao atendimento com a justificativa de recusa.
