# Sprint 6 - Modelagem da Notificacao e Contrato de Eventos

## 1. Objetivo

Modelar o nucleo de dominio da notificacao persistente futura e o contrato interno minimo do evento candidato a gerar notificacoes, sem criar persistencia estrutural nesta etapa.

## 2. Estado anterior

- Sprint 6 estava em `2/16 - 13%`.
- Os itens `1` e `2` do checklist ja estavam concluidos.
- O diagnostico consolidado estava registrado em `docs/roadmap/sprint-6-diagnostico-notificacoes-itsm.md`.
- Ainda nao existia entidade persistente equivalente a `Notificacao`, nem contrato interno dedicado para eventos candidatos de notificacao.

## 3. Diagnostico reutilizado

- `CanalNotificacao` ja existia como enum com `Sistema` e `Email`.
- `HistoricoChamado`, `EventoAuditoria` e `EventoSla` continuam sendo fatos de origem e rastreabilidade, nao notificacoes materializadas.
- `Worker.Email` permanece exclusivamente inbound IMAP.
- O frontend possui estruturas locais de notificacoes, sem backend persistente correspondente.

## 4. Entidade criada

- `Notificacao` em `src/SGX.SistemaChamado.Domain/Entities/Notificacao.cs`.

## 5. Responsabilidade da entidade

`Notificacao` representa uma comunicacao concreta, ja materializada, destinada a um destinatario logico em um canal especifico.

Ela nao representa:

- template;
- preferencia;
- evento de auditoria;
- historico do chamado;
- fila;
- tentativa individual de envio;
- mensagem de e-mail recebida;
- configuracao administrativa.

## 6. Campos adotados

- `ChamadoId` opcional para correlacao futura com chamado.
- `TipoEvento` para classificar a origem estrutural da notificacao.
- `Canal` reutilizando `CanalNotificacao`.
- `Status` para o ciclo interno de processamento.
- `DestinatarioUsuarioId` opcional para usuario interno.
- `DestinatarioEndereco` opcional para endereco materializado.
- `Assunto` opcional.
- `Conteudo` obrigatorio e materializado.
- `ChaveCorrelacao` opcional.
- `ChaveIdempotencia` obrigatoria.
- `AgendadaEm`, `ProcessadaEm`, `EnviadaEm`, `FalhouEm` e `CanceladaEm`.
- `QuantidadeTentativas`.
- `UltimoErro`.
- `MotivoCancelamento`.
- `CriadoPorUsuarioId` e `AtualizadoPorUsuarioId` opcionais.

## 7. Campos avaliados e nao adotados

- nao foi criada colecao de destinatarios;
- nao foi criada entidade de tentativas individuais de envio;
- nao foi criada referencia obrigatoria a template;
- nao foi criado controle de lida/nao lida;
- nao foi criado estado de entrega detalhada por canal;
- nao foi criado payload generico de fila, outbox ou retry.

## 8. Canal reutilizado

Foi reutilizado `CanalNotificacao`, sem criar tabela ou enum adicional de canais.

## 9. Status criado ou reutilizado

Foi criado `StatusNotificacao` com o conjunto minimo:

- `Pendente`
- `Agendada`
- `EmProcessamento`
- `Enviada`
- `Falhou`
- `Cancelada`

## 10. Tipo de evento criado ou estrategia alternativa

Foi criado `TipoEventoNotificacao` com granularidade estrutural minima:

- `EventoChamado`
- `EventoAprovacao`
- `EventoSla`
- `EventoAdministrativo`

## 11. Destinatarios

A modelagem aceita:

- usuario interno por `DestinatarioUsuarioId`;
- endereco materializado por `DestinatarioEndereco`;
- ambos ao mesmo tempo quando houver necessidade de preservar usuario e endereco final.

Pelo menos uma forma valida de destinatario e obrigatoria.

## 12. Conteudo materializado

O conteudo da notificacao fica materializado na propria entidade por meio de `Assunto` e `Conteudo`, sem dependencia de template persistente nesta etapa.

## 13. Idempotencia

`ChaveIdempotencia` foi definida como obrigatoria, validada por conteudo nao vazio e limite de tamanho, preparando a futura unicidade estrutural do item 4.

## 14. Correlacao

`ChaveCorrelacao` foi mantida como opcional na entidade para permitir rastreabilidade sem confundir correlacao com idempotencia.

No contrato de evento, a correlacao foi mantida como obrigatoria para reforcar a rastreabilidade da origem.

## 15. Transicoes de estado

Metodos explicitamente modelados:

- `Agendar`
- `IniciarProcessamento`
- `RegistrarEnvio`
- `RegistrarFalha`
- `Cancelar`

Regras principais:

- criacao inicia em `Pendente`;
- apenas `Pendente` pode ser agendada;
- processamento pode iniciar de `Pendente`, `Agendada` ou `Falhou`;
- envio so pode ocorrer em `EmProcessamento`;
- falha so pode ser registrada em `EmProcessamento`;
- cancelamento nao e permitido para notificacao `Enviada`, `Cancelada` ou `EmProcessamento`.

## 16. Tratamento de falhas

- `UltimoErro` possui limite de tamanho;
- nao ha truncamento silencioso;
- nao ha stack trace irrestrita;
- nao ha retry automatico;
- a notificacao pode voltar a `EmProcessamento` manualmente a partir de `Falhou`.

## 17. Cancelamento

O cancelamento registra `CanceladaEm` e `MotivoCancelamento`, mas nao remove a entidade nem a transforma em historico, auditoria ou fila.

## 18. Contrato de evento

- `EventoCandidatoNotificacao` criado em `src/SGX.SistemaChamado.Application/DTOs/Notificacoes/EventoCandidatoNotificacao.cs`.

Campos adotados:

- `TipoEvento`
- `ChamadoId`
- `UsuarioOriginadorId`
- `OcorridoEm`
- `ChaveCorrelacao`
- `ChaveIdempotencia`
- `Metadados`

## 19. Metadados

- `Metadados` usa `IReadOnlyDictionary<string, string>`;
- a colecao e copiada e exposta como somente leitura;
- nao aceita chave vazia;
- continua sendo complementar, nao substituindo contrato tipado futuro.

## 20. Separacao entre entidade e evento

- `Notificacao` representa a comunicacao materializada para um destinatario.
- `EventoCandidatoNotificacao` representa apenas o fato interno que podera originar notificacoes no futuro.

## 21. Persistencia ainda nao criada

Nao foram criados:

- `DbSet<Notificacao>`;
- configuracao EF;
- migration estrutural;
- tabela;
- indice;
- constraint.

## 22. Compatibilidade com Worker.Email

A modelagem nao altera o `Worker.Email` e nao mistura o fluxo outbound de notificacoes com o processamento inbound IMAP existente.

## 23. Compatibilidade com historico

`HistoricoChamado` continua sendo trilha operacional do chamado e nao passa a representar notificacao entregue ou processada.

## 24. Compatibilidade com auditoria

`EventoAuditoria` continua sendo rastreabilidade administrativa e de governanca, sem substituir a notificacao materializada.

## 25. Impacto em abertura

O modelo prepara notificacoes para abertura futura por portal ou e-mail, mas nao integra nenhuma origem nesta etapa.

## 26. Impacto em atendimento

Atribuicao, transferencia, comentarios e mudancas de status continuam apenas como candidatos futuros de origem.

## 27. Impacto em aprovacao

Aprovacoes pendentes, aprovadas, rejeitadas ou canceladas continuam sem integracao funcional com notificacoes.

## 28. Impacto em SLA

Eventos de SLA seguem como historico operacional e origem candidata, sem emissao automatica de notificacao.

## 29. Impacto em fechamento e reabertura

Resolucao, aceite, fechamento, fechamento automatico e reabertura permanecem inalterados e apenas preparados conceitualmente para integracao posterior.

## 30. Riscos

- inflar cedo demais o enum de tipos de evento;
- confundir tentativa de processamento com tentativa de envio detalhada por canal;
- misturar notificacao materializada com template ou preferencia;
- antecipar persistencia antes de fechar configuracao EF, unicidade e indices.

## 31. Decisoes adiadas

- configuracao EF da entidade;
- unicidade estrutural da chave de idempotencia;
- modelagem de templates;
- preferencias por usuario/evento/canal;
- resolucao de destinatarios por grupo, perfil, aprovador e observador;
- outbox, fila, retry e adaptadores de envio;
- leitura/nao lida e API/frontend persistentes.

## 32. Criterios de aceite

- entidade `Notificacao` modelada no dominio;
- `CanalNotificacao` reutilizado;
- `StatusNotificacao` e `TipoEventoNotificacao` criados sem inflacao indevida;
- destinatario obrigatorio por usuario ou endereco;
- conteudo materializado e chave de idempotencia obrigatoria;
- transicoes de dominio protegidas;
- contrato `EventoCandidatoNotificacao` criado na Application;
- testes de dominio e contrato adicionados;
- nenhuma persistencia estrutural criada.

## 33. Proxima etapa

Criar configuracao EF e migration estrutural de notificacoes.
