# Roteiro de Homologação da Sprint 5

## 1. Visão Geral
Este roteiro orienta a equipe de QA e o Product Owner (PO) na homologação das funcionalidades da **Sprint 5 - Regras de Fechamento e Ciclo de Vida do Chamado**. O objetivo é validar as novas regras de governança, o desmembramento do status Encerrado em **Resolvido** e **Fechado**, e a interação do solicitante na etapa de validação da solução técnica.

## 2. Pré-requisitos para Homologação
- Ambiente de staging atualizado com a última versão do banco de dados (Migrations da Sprint 5 aplicadas).
- Usuário "Analista/Atendente" (Membro do suporte).
- Usuário "Solicitante" (Usuário padrão criador de chamados).
- Administrador do Sistema (Para configurações).
- O Motor de Aprovação da Sprint 4 ativado e funcional para testes de integração.

---

## 3. Cenários de Teste Principais

### Cenário 1: Resolução de Chamado com Solução Técnica Obrigatória
**Perfil:** Analista
- **Passo 1:** O Analista abre um chamado com status "Em Atendimento".
- **Passo 2:** O Analista clica em "Resolver Chamado".
- **Passo 3:** O sistema deve **exigir** o preenchimento do campo "Solução Técnica". Tentar salvar vazio deve retornar erro.
- **Passo 4:** O Analista preenche a solução e confirma.
- **Validação:** O status do chamado muda para **Resolvido**. O tempo de SLA é suspenso. O chamado fica aguardando aceite. A solução técnica é visível no histórico e no ticket.

### Cenário 2: Cancelamento de Chamado com Motivo Obrigatório
**Perfil:** Analista
- **Passo 1:** O Analista seleciona um chamado "Aberto" ou "Em Atendimento".
- **Passo 2:** O Analista aciona o botão de "Cancelar".
- **Passo 3:** O sistema deve **exigir** o preenchimento de um Motivo de Cancelamento.
- **Passo 4:** O Analista preenche o motivo e confirma.
- **Validação:** O status muda para **Cancelado**. O motivo é registrado no histórico e na auditoria do chamado.

### Cenário 3: Aceite de Solução pelo Solicitante
**Perfil:** Solicitante
- **Passo 1:** O Solicitante acessa o Portal e abre um chamado no status **Resolvido**.
- **Passo 2:** A Solução Técnica descrita pelo analista deve estar claramente visível.
- **Passo 3:** O Solicitante clica no botão "Aceitar Solução".
- **Validação:** O status do chamado transita para **Fechado** (ou Encerrado). O chamado não pode mais ser editado. A data de aceite (`AceitoEm`) é registrada.

### Cenário 4: Rejeição de Solução pelo Solicitante e Retorno ao Atendimento
**Perfil:** Solicitante / Analista
- **Passo 1:** O Solicitante acessa um chamado no status **Resolvido**.
- **Passo 2:** O Solicitante clica no botão "Rejeitar Solução".
- **Passo 3:** O sistema **exige** a inclusão de um Motivo da Rejeição.
- **Passo 4:** Após preencher o motivo, a ação é confirmada.
- **Validação:** O status do chamado **retorna para Em Atendimento**. A data de resolução (`ResolvidoEm`) é limpa. O SLA retoma a contagem (se aplicável). O histórico registra a rejeição e o motivo justificado.

### Cenário 5: Fechamento Automático de Chamado Resolvido
**Perfil:** Sistema (Worker/Background)
- **Passo 1:** O Administrador configura o Prazo de Fechamento Automático (ex: 3 dias).
- **Passo 2:** Um chamado encontra-se no status **Resolvido** há 4 dias (sem interação do solicitante).
- **Passo 3:** O sistema (simulado via scheduler ou chamada direta no UseCase de automação) processa as pendências.
- **Validação:** O chamado é movido automaticamente de **Resolvido** para **Fechado**. A auditoria registra o fechamento por decurso de prazo.

### Cenário 6: Reabertura Controlada (Legado/Admin)
**Perfil:** Analista / Admin
- **Passo 1:** Um chamado está no status **Fechado**.
- **Passo 2:** Um Analista com permissão clica em "Reabrir Chamado".
- **Passo 3:** O sistema exige um Motivo de Reabertura.
- **Passo 4:** O usuário preenche e confirma.
- **Validação:** O status muda para **Reaberto** (retornando funcionalmente ao fluxo de Atendimento). A auditoria registra a ação. **Diferença para Rejeição:** A reabertura se aplica a chamados fechados/encerrados, não aos recém resolvidos.

### Cenário 7: Integração Bloqueante (Motor de Aprovações ITSM)
**Perfil:** Solicitante / Analista / Aprovador
- **Passo 1:** O Analista tenta "Resolver", ou o Solicitante tenta "Aceitar" um chamado.
- **Passo 2:** Este chamado possui uma aprovação ITSM pendente, com o campo `Bloqueante = true` (Sprint 4).
- **Validação:** A transição de ciclo final (Fechamento/Aceite definitivo) deve ser **impedida** pelo sistema, retornando uma notificação clara de que há uma aprovação bloqueante pendente.

---

## 4. Auditoria e Timeline
Para todos os testes acima, a interface do chamado ou a aba de histórico deve apresentar visualmente os novos eventos criados:
- `ChamadoResolvidoEvento` (exibe a solução)
- `SolucaoChamadoRejeitadaEvento` (exibe o motivo da recusa)
- `ChamadoAceitoEvento` (exibe a data/hora do de acordo)
- `ChamadoReabertoEvento` (exibe o motivo e o autor da reabertura)

## 5. Critérios de Sucesso
A Homologação será considerada bem sucedida quando:
1. Nenhum erro de infraestrutura for levantado ao realizar as transições.
2. A separação estrita entre o status **Resolvido** e o status **Fechado** estiver sendo respeitada visualmente e via regras lógicas no backend.
3. As exigências de preenchimentos obrigatórios (Motivo Cancelamento, Solução, Motivo Rejeição) impedirem os respectivos avanços de tela.
