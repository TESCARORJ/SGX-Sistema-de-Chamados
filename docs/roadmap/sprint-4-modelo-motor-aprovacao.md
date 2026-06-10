# Sprint 4 - Modelo do Motor de Aprovação ITSM

## 1. Objetivo do motor
O motor de aprovação ITSM evolui a aprovação de chamado para um modelo robusto, auditável e configurável por regra, contexto ITSM (impacto, urgência), serviço/catálogo, natureza, criticidade e critérios futuros. Seu objetivo primordial é flexibilizar decisões, substituindo o bloqueio manual direto por regras pré-programadas e autônomas que preservam a compatibilidade com o fluxo legado de `AprovacaoChamado`.

## 2. Visão geral da arquitetura
A arquitetura baseia-se em separação de responsabilidades claras:
* **Configuração de regra:** Define critérios e alvos (Quem aprova, como aprova, por que aprova).
* **Avaliação de regra e geração de instância:** Intercepta gatilhos e instancia a cobrança de decisão (cria `InstanciaAprovacaoChamado`).
* **Etapas:** Modela workflow e níveis dentro de uma Instância (quem avalia primeiro, paralelismo estrutural).
* **Decisões:** Evento formal de avaliação de uma instância/etapa (`DecisaoAprovacaoChamado`).
* **Bloqueio operacional:** Pêndulo que tranca avanços do chamado até que instâncias bloqueantes atinjam consolidação.
* **Aprovação/Rejeição/Reavaliação:** Fluxos específicos de julgamento de ticket.
* **Compatibilidade com AprovacaoChamado legado:** Garantia de funcionamento simultâneo dos módulos clássicos da plataforma.

## 3. Entidades principais

### 3.1 ConfiguracaoRegraAprovacao
* **Finalidade:** Dicionário vivo de diretrizes organizacionais.
* **Principais campos:** TipoRegra, Escopo, EfeitoOperacional, ExigeAprovacao, Bloqueante, Fluxo.
* **Critérios suportados:** Natureza, Tipo Solicitação, Catalogo, Prioridade Mínima, SLA.
* **Critérios futuros:** Quórum refinado, escalonamento.
* **Efeito operacional:** Sinalizativo vs Obstrutivo.
* **Estratégia de resolução:** Quem responde? (Aprovador Padrão, Grupo Futuro, Solicitante, Específico).
* **Vigência e Versão:** Versão cronológica da regra para snapshot contínuo.
* **Limitações:** Ainda não rege autonomamente automações de fora do ecossistema.

### 3.2 InstanciaAprovacaoChamado
* **Finalidade:** O contrato de obrigação formal gerado para um Ticket (Chamado).
* **Vínculo com Chamado:** Relacionamento forte; isolado por ChamadoId.
* **Snapshots:** Captura Nome, Versão e Critério da `ConfiguracaoRegraAprovacao` na época do evento. Impede que a mudança da regra altere a validade da instância retroativa.
* **Campos chave:** Origem, Bloqueante, Efeito Operacional, Aprovador Resolvido.
* **Limitações:** Metadados como prazo/expiração atuam passivamente.

### 3.3 EtapaAprovacaoChamado
* **Finalidade:** Quebra a instância em fatias avaliativas (Multi-nível).
* **Campos chave:** Nivel, Ordem, Ramo, Obrigatoriedade, CriticidadeParaConsolidacao.
* **Limitações de orquestração:** A consolidação é estrutural; não há "worker" ativo movendo de Nível 1 para 2 de forma invisível. O avanço depende dos eventos de aprovação (use cases).

### 3.4 DecisaoAprovacaoChamado
* **Finalidade:** Trilha de auditoria das respostas humanas ou sistêmicas para instâncias ou etapas.
* **Campos chave:** Aprovação/Rejeição/Reavaliação, Parcial ou Final.
* **Membro de grupo:** Snapshots de `DecisorEhMembroGrupo`.
* **LiberaAvanco / MantemBloqueio:** Sinalizadores do impacto daquela decisão sobre a tranca operacional do ticket.
* **Limitações:** Não consolida quóruns fracionados, age integralmente sobre a etapa requisitada.

### 3.5 AprovacaoChamado legado
* **Finalidade:** Suporta as demandas clássicas, ad-hoc e manuais de operação de aprovação sem a máquina de regras complexa.
* **Coexistência:** Operam independentes na UI e API (`AdminAprovacaoChamadosController` vs `AdminAprovacoesMotorController`).
* **Integração:** `BloquearMovimentacaoAprovacaoPendenteUseCase` entende ambas.

## 4. Enums e conceitos operacionais
* `TipoRegraAprovacao`, `EscopoRegraAprovacao`, `EfeitoOperacionalRegraAprovacao` balizam as intenções corporativas.
* `TipoFluxoAprovacao` e `TipoResolucaoAprovadorRegraAprovacao` organizam como o RH e a operação encaram os requisitos avaliativos.
* `StatusInstanciaAprovacaoChamado`, `StatusEtapaAprovacaoChamado`, `TipoDecisaoAprovacaoChamado`, `ResultadoDecisaoAprovacaoChamado` consolidam estados em grafos fechados.

## 5. Critérios de seleção de regra
* **Suportados:** NaturezaChamado, TipoSolicitacaoId, CatalogoServicoId, CategoriaId, SubcategoriaId, Impacto/Urgencia/Prioridade Mínimas, Vigência Ativa.
* **Ordenação:** 1º Prioridade Decrescente; 2º Especificidade Decrescente; 3º Ordem Crescente; 4º Versão Decrescente.

## 6. Fluxo de geração obrigatória
* Gerenciado via `GerarAprovacaoObrigatoriaChamadoUseCase`. Intercepta gatilhos ou requisições e confronta com as Políticas Ativas. Evita duplicidade validando a ausência de instâncias análogas abertas.
* **O que não faz:** Não bloqueia status diretamente nem altera SLAs. Gera a obrigação que subsequentemente afeta as proteções do sistema.

## 7. Fluxo de bloqueio por aprovação pendente
* Avaliado sob demanda para ações finalísticas por meio de interceptadores (`ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase`).
* Restringe ações sensíveis (como `Encerrar`) sempre que esbarrar em Instância com flag `Bloqueante=true` e `ExigeAprovacao=true`.

## 8. Fluxos de Aprovação e Rejeição
* Responsáveis: `AprovarAprovacaoChamadoUseCase` e `ReprovarAprovacaoChamadoUseCase`.
* Realizam avaliações em instâncias completas ou etapas seccionadas, registrando `DecisaoAprovacaoChamado`.
* **O que não fazem:** Não alteram fechamentos de chamado nem encerram SLAs diretamente, apenas resolvem pendências.

## 10. Reavaliação por dados sensíveis
* `ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisUseCase` revisa aprovações já fornecidas se o ticket sofrer modificação radical (ex: aumento brusco de Custo/Impacto). Mantém estado em `EmReavaliacao`.

## 11. Grupo aprovador
* É puramente preparatório (`GrupoAprovadorFuturo`). Resolve via flags, não existindo tabelas e rotinas completas de escalonamento grupal neste marco da Sprint 4.

## 12. Multi-nível
* Estruturalmente funcional. As Etapas suportam e resolvem blocos lógicos simultâneos ou sequenciais. Não possui "Orquestração Ativa Autônoma" (cronjob worker).

## 13. Interface e endpoints
* `AdminConfiguracoesRegrasAprovacaoController` cuida de CRUD das leis corporativas.
* `AdminAprovacoesMotorController` resolve as requisições ativas.

## 14. Compatibilidade validada
* Natureza ITSM, Catálogos, Bloqueios, Rejeições, Snapshots, Regressão de `AprovacaoChamado`, Abertura e Triagens.

## 15. Limitações (O que não faz)
* Sem grupo aprovador real, sem delegação, sem quórum fracionário, orquestrador passivo, geração manual de automações complexas adicionais.

## 16. Riscos técnicos
* Duplicidade de configuração. Assunções falhas a respeito de Quórum e Multi-nível por interfaces desavisadas. Perda de dados sob migrações de rollback mal geridas.

## 17. Decisões adiadas
* Orquestrador de jobs, Quórum, Delegação formal e Relatórios Gerenciais robustos.

## 18. Arquivos relacionados
* `ConfiguracaoRegraAprovacao.cs`, `InstanciaAprovacaoChamado.cs`, `EtapaAprovacaoChamado.cs`, `DecisaoAprovacaoChamado.cs`, UseCases em `src/SGX.SistemaChamado.Application/UseCases/Chamados` e respectivos Testes na suíte de domínios.

## 19. Validação técnica
* Build `dotnet build SGX.SistemaChamado.sln`
* Testes via `dotnet test` apontando para a namespace dos Controllers/UseCases listados.

## 20. Conclusão
O modelo do motor está fundamentado, documentado e plenamente funcional no tocante ao seu núcleo administrativo e operacional básico, mantendo resiliência total ao legado pré-existente e com suas fundações preparadas para a próxima geração de regras avançadas.
