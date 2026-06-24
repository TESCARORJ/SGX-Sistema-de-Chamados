# Sprint 6 - Checklist Operacional de Notificacoes

## Estado do ambiente

- SMTP outbound permanece desabilitado por padrao em desenvolvimento:
  - `EmailOutbound:Habilitado = false`
- Host padrao local:
  - `EmailOutbound:Host = localhost`
  - `EmailOutbound:Port = 1025`
- Remetente padrao:
  - `EmailOutbound:RemetenteEndereco = nao-responda@sgx.local`
  - `EmailOutbound:RemetenteNome = SGX Sistema de Chamados`

## Variaveis e configuracoes minimas

- `ConnectionStrings:DefaultConnection`
- `Authentication:ProvedorPrincipal`
- `Authentication:JwtLocalIssuer`
- `Authentication:JwtLocalAudience`
- `Authentication:JwtLocalChaveAssinatura`
- `EmailOutbound:Habilitado`
- `EmailOutbound:Host`
- `EmailOutbound:Port`
- `EmailOutbound:Usuario`
- `EmailOutbound:Senha`
- `EmailOutbound:UsarSsl`
- `EmailOutbound:RemetenteEndereco`
- `EmailOutbound:RemetenteNome`

## Dependencias minimas da Sprint 6

- tabelas estruturais de notificacao aplicadas;
- templates ativos para os eventos homologados;
- preferencias de notificacao acessiveis para os usuarios envolvidos;
- endpoints de inbox autenticada funcionando;
- processamento e entrega persistente disponiveis sem alteracao no `Worker.Email`.

## Eventos integrados nesta sprint

- abertura de chamado priorizada;
- atribuicao/assuncao priorizada;
- mudancas relevantes de status;
- encerramento priorizado;
- fluxos legados preservados sem acoplamento indevido.

## Checklist pre-homologacao

- confirmar banco atualizado com `dotnet ef database update`;
- confirmar `dotnet ef migrations has-pending-model-changes` sem divergencia;
- confirmar `dotnet test tests/SGX.SistemaChamado.Tests /p:UseSharedCompilation=false` com `Falhas = 0`;
- confirmar `npm.cmd run test:unit -- notificacoes`;
- confirmar `npx.cmd vue-tsc --noEmit`;
- confirmar `npm.cmd run build`;
- revisar se os templates exigidos para o ambiente estao ativos;
- revisar se as permissoes `Notificacoes.Visualizar` e `Notificacoes.Gerenciar` estao seedadas.

## Checklist de operacao

- validar inbox do solicitante em `/portal/notificacoes`;
- validar area administrativa em `/admin/notificacoes`;
- validar badge de nao lidas;
- validar cards, filtros, paginacao e estados de loading/vazio/erro;
- validar leitura e marcacao como nao lida;
- validar refresh sem duplicar notificacoes;
- validar ausencia de scroll horizontal nos breakpoints principais;
- validar que notificacoes `Falhou` sejam monitoradas e tratadas;
- validar que notificacoes `EmProcessamento` nao fiquem presas sem transicao;
- validar logs associados a processamento e entrega.

## Monitoramento operacional

- acompanhar volume de notificacoes por status;
- investigar acumulacao em `EmProcessamento`;
- investigar aumento de `Falhou`;
- verificar repeticao indevida por evento/destinatario/canal;
- verificar se os templates materializados correspondem ao evento gerador;
- verificar se a inbox do usuario autenticado nao expõe notificacoes de terceiros.

## Limpeza de dados de teste

- remover notificacoes geradas artificialmente para homologacao manual;
- remover templates temporarios usados apenas em validacao;
- validar que nao restaram preferencias de teste vinculadas a usuarios reais;
- preservar registros necessarios para auditoria tecnica da rodada.

## Troubleshooting

- se a inbox vier vazia, validar template ativo, destinatario resolvido e evento gerador;
- se a entrega ficar em `Falhou`, validar configuracoes SMTP, remetente, conectividade e credenciais;
- se a entrega ficar em `EmProcessamento`, verificar concorrencia, fixture ou bloqueio transacional;
- se houver duplicidade, revisar chave de idempotencia do evento e resolucao de destinatarios;
- se houver divergencia de roadmap/percentual, revisar `SeedData.cs`, snapshot e migrations de dados.

## Rollback

- rollback deve ocorrer por migration reversivel ou restauracao controlada do banco;
- nao editar manualmente o banco para "forcar" sincronismo;
- nao apagar migrations aplicadas;
- se a falha for apenas de template/configuracao, preferir rollback configuracional ao rollback estrutural.

## Pos-deploy

- repetir validacao dos fluxos principais em ambiente alvo;
- revisar configuracoes reais de SMTP e remetente;
- revisar usuarios/perfis com acesso administrativo;
- verificar se os eventos homologados continuam gerando notificacoes apos o deploy;
- registrar evidencias e data da homologacao manual.

## Seguranca

- nao expor credenciais SMTP em codigo ou documentos versionados;
- manter `EmailOutbound:Habilitado = false` quando o ambiente nao estiver pronto para envio real;
- validar autorizacao nas rotas administrativas e de portal;
- validar isolamento de notificacoes por usuario autenticado;
- revisar logs para nao vazar conteudo sensivel indevido.

## Resultado desta rodada

- checklist tecnico automatizado: concluido;
- checklist operacional manual: pendente de homologacao visual/funcional final;
- aceite formal: nao registrar enquanto a homologacao visual/manual nao for executada.
