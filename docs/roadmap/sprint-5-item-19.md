# Sprint 5 - Item 19: Aceite e Rejeição pelo Solicitante na Interface

Este documento detalha o fluxo implementado no frontend (Portal do Solicitante) para permitir que os usuários aceitem ou rejeitem a solução proposta de um chamado, assim como as regras de negócio de integração aplicáveis.

## 1. Contexto de Negócio

Durante a "Sprint 5 - Regras de fechamento", o backend foi atualizado para introduzir conceitos de Resolução (solução funcional entregue) vs Encerramento Definitivo. 

O Item 19 refere-se à disponibilização dessa governança para o usuário final, o **Solicitante**, na interface do portal:
- Após um chamado assumir o status `Resolvido`, a solução deve ser formalmente aceita pelo solicitante para que o chamado seja fechado, ou rejeitada, o que devolve o chamado para o atendimento.

## 2. Fluxo da Interface (UI)

O fluxo de interface foi introduzido no componente `DetalheChamadoView.vue`:

### 2.1 Ações Disponíveis
Quando um chamado se encontra no status **"Resolvido"** e **ainda não está encerrado** (`!detalhe.encerradoEm`), dois novos botões são renderizados abaixo da linha do tempo:
- **Confirmar aceite da solução**: Botão principal (primary).
- **Rejeitar solução**: Botão secundário (outline negative).

### 2.2 Modal de Rejeição
- Ao clicar em "Rejeitar solução", a interface exibe um dialog (`q-dialog`) forçando o preenchimento de um motivo obrigatório.
- Sem o motivo, o botão de confirmação permanece desabilitado, protegendo a API de requisições inválidas de acordo com as regras de negócio da aplicação.

### 2.3 Tratamento de Respostas
Após a confirmação da operação na API, o fluxo obedece ao seguinte comportamento:
- **Sucesso**: Exibe uma notificação (`q-notify` positive) informando que o chamado foi encerrado ou que retornou ao atendimento e em seguida recarrega os dados do detalhe.
- **Falha**: Processa a resposta usando o método `extrairMensagemErro` para fornecer uma mensagem legível ao solicitante. 

## 3. Integração e Tratamentos de Erro da API

Os seguintes endpoints foram integrados via `portalService.ts` a partir dos métodos desenvolvidos nas fases anteriores da sprint:

### 3.1 Aceite da Solução
* **Endpoint:** `POST /api/portal/chamados/{id}/aceitar`
* **Payload:** `AceitarSolucaoChamadoRequest` (aceita uma `observacaoAceite` opcional)
* **Regra bloqueante tratada:** Se houver uma "aprovação pendente" no chamado (regra de integração da Sprint 4), o backend retornará `400 Bad Request` indicando o bloqueio. O `extrairMensagemErro` lê essa resposta e repassa ao solicitante com clareza.

### 3.2 Rejeição da Solução
* **Endpoint:** `POST /api/portal/chamados/{id}/rejeitar`
* **Payload:** `RejeitarSolucaoChamadoRequest` (`motivoRejeicao` obrigatório)
* **Comportamento no Backend:** Muda o status do chamado de volta para o status anterior em atendimento (ex: `Em Atendimento`), registra histórico e reinicia SLAs.
