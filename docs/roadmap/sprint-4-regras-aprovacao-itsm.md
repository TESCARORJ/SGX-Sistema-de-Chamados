# Sprint 4 - Regras de Aprovação ITSM

## 1. Objetivo das regras de aprovação

As regras de aprovação ITSM têm como objetivo central normatizar e automatizar a governança operacional sobre chamados críticos e sensíveis no **SGX Sistema de Chamados**. Elas determinam de forma dinâmica:
* **Quando** um chamado precisa passar por um fluxo de aprovação com base em critérios e contexto operacional (custo, risco, catálogo, natureza, criticidade, etc.).
* **Quem** deve ser o responsável ou grupo responsável por realizar a avaliação e decisão.
* **Se o chamado fica bloqueado** operacionalmente impedindo avanços para status finais ou movimentações críticas enquanto a aprovação estiver pendente.
* **Qual efeito operacional esperado** (permitir, sinalizar de forma informativa, exigir aprovação sem travar, exigir aprovação bloqueando avanços, ou requerer reavaliação).

Esse motor de regras substitui o modelo de aprovação manual e ad-hoc descentralizado por um mecanismo robusto, auditável e baseado em políticas formais da organização, sem, contudo, travar desnecessariamente o fluxo diário do Service Desk.

---

## 2. Diferença entre regra e instância

Para a governança do motor, é fundamental distinguir os modelos de parametrização e execução:
* **`ConfiguracaoRegraAprovacao` (Regra):** Representa a regra administrativa ou política de negócio cadastrada pelo gestor de ITSM (ex: *"Requisições com custo acima de R$ 5.000,00 exigem aprovação do Diretor de TI"*). É uma definição estática que contém vigência, critérios de seleção e estratégias de aprovador.
* **`InstanciaAprovacaoChamado` (Instância):** Representa a aplicação concreta e ativa de uma regra sobre um chamado específico. Quando um chamado satisfaz os critérios de uma regra, o sistema gera uma instância (um snapshot imutável da regra na data de geração), isolando o chamado de alterações futuras que a regra administrativa possa sofrer.
* **`EtapaAprovacaoChamado` (Etapa):** Representa os níveis avaliativos ou etapas de aprovação associadas à instância (ex: Nível 1 — Aprovação de Gestor Imediato; Nível 2 — Aprovação da Diretoria).
* **`DecisaoAprovacaoChamado` (Decisão):** Registra formalmente a decisão humana ou sistêmica tomada para uma instância ou etapa (como Aprovado, Rejeitado, Em Reavaliação, Cancelado ou Expirado).
* **`AprovacaoChamado` (Legado):** É o fluxo legado de aprovações do catálogo que permanece independente e ativo de forma paralela ao motor novo, garantindo retrocompatibilidade total.

---

## 3. Quando criar uma regra de aprovação

A criação de regras de aprovação deve ser feita de maneira estratégica para evitar sobrecarga operacional. Situações típicas recomendadas para criação de regras de aprovação incluem:
* **Serviço Sensível:** Serviços no catálogo que lidam com permissões de privilégio elevado, acesso a dados confidenciais ou LGPD (ex: Acesso de Administrador de Domínio).
* **Requisição com Custo:** Solicitações de compra de hardware, novos softwares ou contratação de licenças corporativas que ultrapassem alçadas financeiras padrão.
* **Mudança com Impacto Alto:** Alterações planejadas que afetam sistemas core do negócio e necessitam de validação do CAB (Comitê de Avaliação de Mudanças).
* **Incidente com Risco Elevado:** Incidentes graves que exigem alinhamento de segurança ou aprovação de plano de contingência antes da resolução final.
* **Solicitação que Envolve Segurança/Acesso:** Solicitações de acessos físicos ou lógicos a ambientes críticos de infraestrutura ou servidores de produção.
* **Solicitação que Exige Autorização do Gestor:** Abonos, alterações de escalas de trabalho ou solicitações de reembolsos.
* **Alteração em Infraestrutura Crítica:** Atualizações de patches de segurança ou desligamento programado de servidores.
* **Atendimento que Pode Gerar Indisponibilidade:** Testes em redes ou migrações de banco de dados.
* **Catálogo de Serviço que Exige Controle Formal:** Serviços regulados por auditorias ou conformidades regulatórias externas (como ISO 27001, SOX).

---

## 4. Critérios suportados

O motor avalia um conjunto amplo de critérios para determinar se uma regra se aplica ao contexto de um chamado:

### 4.1 Natureza do chamado
Atua sobre a classificação ITIL/ITSM mapeada no enum `NaturezaChamadoEnum`:
1. `Incidente`
2. `Requisicao`
3. `Mudanca`
4. `Problema`
5. `EventoAlerta`
6. `TarefaOperacional`

Permite, por exemplo, que se crie uma regra que exija aprovação automática para qualquer chamado do tipo `Mudanca`.

### 4.2 Tipo de solicitação
Filtra com base no identificador exclusivo do `TipoSolicitacaoId`. Útil quando regras de aprovação devem ser acionadas apenas para taxonomias operacionais específicas.

### 4.3 Catálogo de serviço
Critério principal para automações de portal. Vincula a regra a um item do catálogo (`CatalogoServicoId`), sendo ideal para serviços sensíveis que exigem governança logo na sua concepção.

### 4.4 Categoria e subcategoria
Mapeia regras a categorias (`CategoriaId`) ou subcategorias (`SubcategoriaId`) do chamado. Caso uma subcategoria seja fornecida, a categoria correspondente também deve ser definida obrigatoriamente para fins de integridade de dados.

### 4.5 Impacto mínimo
Filtra chamados onde o impacto declarado é igual ou superior ao piso definido em `ImpactoChamadoEnum` (ex: Médio, Alto, Muito Alto). O motor avalia se `contexto.Impacto >= regra.ImpactoMinimo`.

### 4.6 Urgência mínima
Filtra chamados por urgência igual ou superior ao configurado em `UrgenciaChamadoEnum` (ex: Alta, Crítica). Ajuda a filtrar chamados urgentes que necessitam de intervenção ou aviso de gerência.

### 4.7 Prioridade mínima
Avalia se a prioridade do chamado é igual ou superior ao definido em `PrioridadeChamadoEnum`. A prioridade funciona como a combinação operacional do impacto e urgência do ticket.

### 4.8 Custo mínimo
Avalia o campo numérico `CustoMinimo`. Regras ativadas por custo garantem que chamados com estimativa financeira acima do piso definido gerem aprovação obrigatória do gestor financeiro antes do atendimento/compra.

### 4.9 Nível de risco mínimo
Avalia o risco estimado (campo `NivelRiscoMinimo`). Permite isolar chamados que tragam riscos acentuados à continuidade de negócios, segurança ou compliance técnico.

### 4.10 Vigência
Controla o intervalo temporal de aplicação da regra através de `VigenteDe` e `VigenteAte`. O motor apenas seleciona a regra se a data atual estiver compreendida neste intervalo, permitindo criar regras sazonais (como congelamento de mudanças de fim de ano ou auditorias temporárias).

### 4.11 Regra ativa/inativa
Controla se a regra está disponível no motor. O estado **Inativo** permite que uma política seja desativada sem ser excluída do histórico do sistema, preservando a integridade referencial das instâncias de aprovação passadas.

---

## 5. Critérios de ordenação e desempate

Caso múltiplos critérios sejam compatíveis com o chamado, o motor do caso de uso `GerarAprovacaoObrigatoriaChamadoUseCase` seleciona a melhor regra através da seguinte cascata lógica e determinística:
1. **Regra Ativa e Vigente:** Apenas regras com status ativo e dentro da validade temporal são elegíveis.
2. **Critérios Compatíveis:** Confronto com as propriedades do chamado (todos os critérios configurados na regra devem ser atendidos).
3. **Prioridade da Regra (Decrescente):** Regras com o maior valor numérico de prioridade vencem.
4. **Especificidade (Decrescente):** Regras mais específicas vencem as genéricas. A especificidade é calculada pela soma de campos de critérios preenchidos (natureza, catálogo, impacto, custo, etc.). Quanto mais filtros a regra define, maior sua especificidade.
5. **Ordem (Crescente):** Se houver empate, vence a regra com menor valor configurado no campo `Ordem`.
6. **Versão (Decrescente):** Versões mais recentes de uma regra desempatam o processo.
7. **Nome (Alfabético):** Último critério de ordenação padrão para garantir determinismo.

> [!TIP]
> Sempre comece definindo regras muito específicas com maior prioridade operacional, deixando regras genéricas (com poucos filtros e menor prioridade) para atuar como fallback.

---

## 6. Tipos de regra

A parametrização do motor possui a propriedade `TipoRegraAprovacao` para categorizar o escopo da política corporativa no banco de dados. Os valores existentes no enum do projeto são:
* **`Geral` (1):** Regras amplas de governança que se aplicam a todo o ecossistema.
* **`NaturezaItsm` (2):** Regras associadas a um tipo de chamado ITIL (como Incidentes ou Mudanças).
* **`TipoSolicitacao` (3):** Focadas em tipos específicos de solicitação do negócio.
* **`CatalogoServico` (4):** Regras de itens de catálogo sensíveis abertos pelo portal.
* **`CategoriaSubcategoria` (5):** Voltadas a taxonomias operacionais da central de serviços.
* **`ImpactoUrgencia` (6):** Vinculadas a criticidades operacionais mínimas.
* **`CustoOuRiscoFuturo` (7):** Associadas a alçadas financeiras ou riscos declarados.
* **`Combinada` (8):** Regras complexas que agrupam múltiplos fatores.

> [!NOTE]
> Independente do tipo lógico da regra cadastrada, o comportamento de ser **obrigatória** (exigir decisão formal e poder bloquear o ticket) ou **informativa** é configurado de forma granular pelos flags `ExigeAprovacao` e `Bloqueante` e pelo enum `EfeitoOperacional`.

---

## 7. Regra bloqueante versus regra informativa

O comportamento operacional do ticket é controlado de acordo com o nível de rigidez da regra:
* **Regra Bloqueante (`Bloqueante = true` e `ExigeAprovacao = true`):**
  Impede que o analista operacional movimente o chamado para status finais (como Fechado ou Encerrado) ou realize certas transições críticas enquanto a `InstanciaAprovacaoChamado` estiver pendente de decisão.
  * *Exemplo Prático:* O analista tenta encerrar um chamado de Mudança crítica. O interceptador barra a transição exibindo: *"Não é possível encerrar o chamado pois há aprovações pendentes do comitê técnico"*.
* **Regra Informativa / Sinalizadora (`Bloqueante = false` ou `EfeitoOperacional = Sinalizar`):**
  Gera a instância de aprovação no histórico do chamado para dar visibilidade aos gestores e auditores, mas **não bloqueia** o andamento das tarefas pelo Service Desk.
  * *Exemplo Prático:* Uma requisição de troca de periférico padrão (baixo custo) gera uma instância sinalizada para o gestor da área apenas para acompanhamento de cota mensal, mas o analista pode continuar o atendimento e encerrar o ticket livremente.
* **`ExigeAprovacao = true`:** Define que a instância necessita obrigatoriamente de uma resposta formal (`DecisaoAprovacaoChamado`) para ser concluída, influenciando o status consolidado da instância.

---

## 8. Efeito operacional da regra

O enum `EfeitoOperacionalRegraAprovacao` dita as regras de validação interna no domínio da entidade `ConfiguracaoRegraAprovacao`:
* **`Permitir` (1):** Não exige aprovação e não marca bloqueio. Indica avanço direto sem controle formal.
* **`Sinalizar` (2):** Não exige aprovação e não marca bloqueio. Funciona estritamente para notificação e acompanhamento de auditoria.
* **`ExigirAprovacao` (3):** Exige decisão formal (`ExigeAprovacao = true`), mas não bloqueia o fluxo do chamado (`Bloqueante = false`). O avanço é permitido com ressalva.
* **`ExigirAprovacaoEBloquearAvanco` (4):** Exige decisão formal (`ExigeAprovacao = true`) e bloqueia transições do chamado (`Bloqueante = true`) enquanto pendente.
* **`RequerReavaliacao` (5):** Caso o chamado sofra alterações significativas de dados após ser aprovado, o motor pode exigir que o fluxo de aprovação retorne para revisão. Não marca bloqueio imediato na etapa de configuração da regra.

---

## 9. Estratégia de aprovador

O motor mapeia quem será o decisor final do chamado utilizando a propriedade `TipoResolucaoAprovadorRegraAprovacao`:
* **Aprovador específico (`AprovadorEspecifico`):** Um usuário fixo associado à regra (indicado pelo campo `AprovadorEspecificoUsuarioId`).
* **Aprovador padrão (`AprovadorPadrao`):** Um decisor alternativo configurado como fallback na regra (`AprovadorPadraoUsuarioId`).
* **Grupo aprovador futuro (`GrupoAprovadorFuturo`):** Estrutura reservada para roteamento difuso de aprovações a equipes ou perfis de aprovação.
* **Resolução dinâmica futura (`ResolucaoDinamicaFutura`):** Mecanismos dinâmicos como "Gestor do Solicitante" obtido em tempo de execução via Entra ID ou banco de dados.

> [!WARNING]
> **Limitação da Sprint 4:** As funcionalidades de roteamento de grupos de aprovação, quórum complexo e delegação de voto são conceitos estruturais modelados no banco e DTOs, mas **não estão totalmente operacionais** no fluxo automatizado desta fase da Sprint.

---

## 10. Fluxos de aprovação

O sistema oferece suporte a diferentes fluxogramas lógicos por meio de `TipoFluxoAprovacao`:
* **Simples:** Uma aprovação direta e única é suficiente para liberar o ticket.
* **Sequencial:** As etapas de aprovação devem ser resolvidas em série (ex: Nível 1 aprova, liberando o Nível 2).
* **Paralelo:** Vários aprovadores recebem a solicitação simultaneamente e o fluxo avança conforme regras de consolidação.
* **Multi-nível:** Cascata de etapas estrutural configurada por nível e prioridade.

> [!IMPORTANT]
> **Limitação da Sprint 4:** A consolidação de multi-nível e sequencial é puramente estrutural. O avanço de etapas de aprovação requer eventos disparados via Use Cases; não há um job assíncrono rodando em background para processar a fila de forma autônoma.

---

## 11. Exemplos de regras recomendadas

Abaixo constam exemplos pragmáticos recomendados para modelagem de políticas operacionais (sem inserção de seed funcional no banco):

### Exemplo 1 — Serviço sensível de acesso administrativo
* **Natureza:** Requisição
* **Catálogo:** Acesso a Banco de Dados de Produção (ou equivalente sensível)
* **Impacto mínimo:** Médio
* **Exige aprovação:** Sim
* **Bloqueante:** Sim
* **Aprovador:** Usuário específico (ex: Gerente de Segurança da Informação)
* **Justificativa:** Garantir conformidade de privilégio elevado e logs de auditoria para fins de compliance.

### Exemplo 2 — Mudança com alto impacto
* **Natureza:** Mudança
* **Impacto mínimo:** Alto
* **Risco mínimo:** Alto (nível 4 ou 5)
* **Exige aprovação:** Sim
* **Bloqueante:** Sim
* **Aprovador:** Aprovador padrão ou comitê (CAB)
* **Justificativa:** Mudanças em produção que podem gerar indisponibilidade geral da plataforma precisam de aprovação formal.

### Exemplo 3 — Solicitação de compra com custo
* **Natureza:** Requisição
* **Custo mínimo:** R$ 1.000,00
* **Exige aprovação:** Sim
* **Bloqueante:** Sim
* **Aprovador:** Usuário específico (ex: Diretor Financeiro)
* **Justificativa:** Alçada financeira que ultrapassa a autonomia do solicitante ou do time de suporte básico.

### Exemplo 4 — Serviço apenas informativo
* **Natureza:** Requisição
* **Catálogo:** Solicitação de Teclado ou Mouse Padrão
* **Exige aprovação:** Não
* **Bloqueante:** Não
* **Efeito operacional:** Sinalizar
* **Justificativa:** Itens de baixo custo e baixo risco que apenas necessitam de registro histórico para inventário, sem travar o Service Desk.

### Exemplo 5 — Regra genérica de fallback
* **Natureza:** Mudança
* **Impacto mínimo:** Baixo
* **Exige aprovação:** Sim
* **Bloqueante:** Não
* **Efeito operacional:** ExigirAprovacao
* **Justificativa:** Garantir que nenhuma mudança de produção passe sem avaliação humana mínima, operando de forma informativa sem causar deadlocks na equipe de infraestrutura.

---

## 12. Boas práticas de configuração

Para manter o ecossistema estável e responsivo:
* **Priorize especificidade:** Crie primeiro regras específicas de catálogo antes de criar regras genéricas baseadas apenas em categorias.
* **Evite regras bloqueantes globais:** Regras genéricas com `Bloqueante = true` geram gargalos de workflow (analistas não conseguem fechar chamados simples).
* **Utilize o campo Vigência:** Não delete regras que já operaram; altere a vigência final (`VigenteAte`) ou desative-as para manter a rastreabilidade histórica dos chamados.
* **Versione regras de aprovação:** Ao modificar critérios de uma política importante, crie uma nova versão para manter snapshots históricos íntegros.
* **Isolamento de Testes:** Teste regras complexas em chamados de homologação antes de ativá-las globalmente em produção.
* **Evite conflitos de prioridade:** Não cadastre regras com escopos idênticos na mesma prioridade e ordem, para não causar comportamentos de desempate imprevisíveis.
* **Nomes e Descrições claros:** Descreva detalhadamente a justificativa organizacional no cadastro da regra para facilitar futuras revisões por auditores.

---

## 13. Antipadrões

Evite os seguintes comportamentos ao gerir o motor de aprovação ITSM:
* **Saturação de aprovações:** Configurar o motor para que toda e qualquer solicitação exija aprovação (fazendo com que gestores ignorem e-mails por fadiga).
* **Bloqueio genérico:** Regra genérica de fallback marcada como bloqueante para incidentes (impede resoluções de emergência críticas do Service Desk).
* **Duplicação de escopo:** Criar regras com o mesmo catálogo e prioridade, gerando deadlock lógico.
* **Uso incorreto do efeito operacional:** Configurar o efeito de "Sinalizar" esperando que o chamado fique bloqueado operacionalmente.
* **Confiar em funcionalidades pendentes:** Desenhar políticas corporativas que dependem de roteamento complexo para grupos aprovadores ou quórum que ainda não estão funcionais no sistema.
* **Ignorar vigência:** Esquecer regras temporárias ativas após o término de períodos de congelamento, gerando bloqueios indesejados.
* **Falta de versionamento:** Alterar critérios fundamentais de uma regra em vigor em vez de inativá-la e gerar uma nova versão.

---

## 14. Compatibilidade com fluxo legado

* **Coexistência de dados:** O modelo `AprovacaoChamado` legado não foi depreciado. O motor de aprovações ITSM atua em paralelo e de forma independente.
* **Garantia de não duplicidade:** O caso de uso `GerarAprovacaoObrigatoriaChamadoUseCase` executa uma verificação preventiva (`ExisteAprovacaoLegadaEquivalenteAsync`). Caso já exista um fluxo legado pendente por catálogo associado ao chamado, a geração de uma nova instância pelo motor é evitada para não onerar o usuário.
* **Borda HTTP:** Os endpoints legados e do novo motor residem em controllers isolados, preservando integrações existentes de sistemas integrados ou portais paralelos.

---

## 15. Relação com abertura e atendimento

* **Abertura Assíncrona:** A validação e geração da instância de aprovação pelo motor não impede a abertura inicial do chamado. O chamado é registrado no sistema de forma íntegra e, posteriormente, a governança aplica a necessidade de aprovação.
* **Atendimento Colaborativo:** O chamado bloqueado por pendência de aprovação permanece acessível para triagem, inserção de comentários, anexos e ações consultivas dos analistas. Apenas transições finalísticas e resoluções são interrompidas.
* **Independência de SLA:** O motor não altera o cálculo ou prazos de SLA operacional do chamado de forma automatizada. Cabe à equipe operacional resolver a pendência de aprovação em tempo hábil para evitar o estouro de prazos contratuais.

---

## 16. Governança recomendada

Processo sugerido para ciclo de vida de uma regra de aprovação ITSM:
1. **Identificação do Contexto:** Identificar um catálogo, natureza ou cenário operacional sensível.
2. **Avaliação Financeira e de Risco:** Definir se a regra deve ser ativada por faixa de custos, criticidade ou risco.
3. **Mapeamento do Aprovador:** Determinar se será um aprovador nominal (específico) ou padrão.
4. **Efeito Operacional:** Definir se a regra deve apenas sinalizar para auditoria ou se deve travar transições do chamado.
5. **Vigência e Prioridades:** Estabelecer a prioridade de desempate do motor e as datas inicial e final de validade.
6. **Bateria de Testes:** Validar a regra em ambiente de homologação simulando chamados correspondentes.
7. **Publicação (Ativação):** Mudar o status para ativo e validar a entrada em vigor.
8. **Monitoramento:** Acompanhar chamados retidos pelo motor de aprovação para avaliar gargalos.
9. **Revisão Periódica:** Revisar o catálogo de regras inativas e vigências anualmente.

---

## 17. Checklist para criar regra

Ao cadastrar uma regra na interface administrativa, certifique-se de preencher e revisar os seguintes campos:
* [ ] **Nome claro** e contextualizado (ex: *"Aprovação de Acesso Admin — Produção"*).
* [ ] **Descrição corporativa** detalhando a política de negócio associada.
* [ ] **Tipo da regra** lógico conforme categorização.
* [ ] **Escopo da regra** (abertura, atendimento, encerramento, etc.).
* [ ] **Critérios de filtro** (Natureza, Catálogo, Prioridade Mínima, Risco Mínimo, Custo).
* [ ] **Prioridade e Ordem** configurados para correto ordenamento de desempate.
* [ ] **Versão inicial** setada como `1`.
* [ ] **Flag Exige Aprovação** marcado se for exigida tomada de decisão.
* [ ] **Flag Bloqueante** marcado se o chamado deve parar o andamento em caso de pendência.
* [ ] **Efeito operacional** em conformidade com os flags de bloqueio.
* [ ] **Tipo de Resolução de Aprovador** e ID do usuário aprovador específico ou padrão associado.
* [ ] **Prazo de Decisão (Horas)** preenchido se aplicável para controle de expiração.
* [ ] **Vigência** (`VigenteDe` e `VigenteAte`) definida.
* [ ] **Responsável pela política** e justificativa documentados para auditoria.

---

## 18. Limitações atuais

Principais restrições conhecidas na implementação atual da Sprint 4:
* **Grupo Aprovador:** Os conceitos de grupos de aprovadores e roteamento difuso de e-mails para múltiplos decisores são puramente estruturais no banco de dados, sem orquestração de distribuição ativa.
* **Quórum de Decisão:** Não há tratamento para aprovações fracionadas (ex: 2 de 3 votos necessários).
* **Delegação:** Não há suporte para direcionamento automático de decisões a suplentes em períodos de férias.
* **Avanço automático (Multi-nível):** O avanço entre as etapas de aprovação requer chamadas explícitas de casos de uso pelo sistema, não possuindo um orquestrador assíncrono em background (cronjob).
* **Reset de reavaliação:** O status `EmReavaliacao` indica necessidade de revisão de dados sensíveis, mas não recria instâncias ou reinicia etapas de aprovação de forma automatizada.
* **Expiração passiva:** O prazo de expiração é calculado, mas sua consolidação de cancelamento/expiração é reativa ao chamado do caso de uso, sem rotina autônoma de timeout ativo.
* **Ausência de automação de SLA:** O motor não suspende nem prorroga prazos de SLA de atendimento durante o período em que o chamado aguarda aprovação.

---

## 19. Testes relacionados

A estabilidade e regressão do motor ITSM são cobertas pelos seguintes testes da suíte:
* **Item 52 (Regra por Natureza):** Valida a geração de instâncias baseada em `NaturezaChamadoEnum`.
* **Item 53 (Serviço Sensível):** Garante que chamados criados para catálogos marcados como sensíveis disparem o motor corretamente.
* **Item 54 (Bloqueio por Pendência):** Testa o travamento de encerramento do chamado quando há instâncias de aprovação bloqueantes pendentes.
* **Item 55 (Aprovação/Liberação):** Valida a liberação do chamado após a aprovação formal (mudança do status da instância para aprovada e liberação de bloqueios).
* **Item 56 (Rejeição):** Valida o comportamento do chamado e encerramento de tarefas após decisão de rejeição da aprovação.
* **Item 57 (Grupo Aprovador):** Garante a corretude estrutural de regras parametrizadas com grupos decisores futuros.
* **Item 58 (Multi-nível):** Valida a integridade na geração de instâncias contendo etapas ordenadas por nível.
* **Item 59 (Regressão Fluxo de Aprovação):** Valida que as novas implementações não afetam o fluxo de decisão legada.
* **Item 60 (Regressão Abertura/Atendimento):** Garante que chamados sem regras aplicáveis possam ser abertos e atendidos sem nenhum tipo de bloqueio ou exceção de software.

---

## 20. Conclusão

As regras de aprovação ITSM encontram-se plenamente documentadas sob as perspectivas funcional, técnica e de governança. As diretrizes aqui dispostas capacitam sysadmins, gestores e analistas de ITSM do **SGX** a desenhar malhas de aprovação robustas e aderentes às políticas de conformidade da organização, com clareza total sobre o comportamento operacional do sistema, regras de desempate de concorrência e limitações técnicas temporárias mapeadas na Sprint 4.
