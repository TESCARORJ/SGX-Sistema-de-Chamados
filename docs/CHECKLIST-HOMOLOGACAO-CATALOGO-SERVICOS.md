# Checklist de Homologacao - Catalogo de Servicos

## Objetivo
Validar funcionalmente o modulo de Catalogo de Servicos do SGX Sistema de Chamados em cenarios administrativos, de portal e de abertura de chamado por servico.

Legenda sugerida para execucao manual:
- [ ] Nao executado
- [x] Validado
- [!] Bloqueado

## Administracao
- [ ] Administrador cria servico para TI.
- [ ] Administrador cria servico para RH.
- [ ] Administrador cria servico para Financeiro.
- [ ] Administrador cria servico para outro departamento.
- [ ] Administrador edita servico.
- [ ] Administrador publica servico.
- [ ] Administrador arquiva servico.
- [ ] Administrador reativa servico.
- [ ] Sistema impede servico sem departamento responsavel.
- [ ] Sistema gera slug unico.
- [ ] Sistema respeita permissoes CatalogoServicos.Visualizar.
- [ ] Sistema respeita permissoes CatalogoServicos.Gerenciar.
- [ ] Sistema respeita permissoes CatalogoServicos.Publicar.
- [ ] Sistema respeita permissoes CatalogoServicos.Arquivar.

## Portal
- [ ] Portal lista apenas servicos publicados.
- [ ] Portal nao lista servicos em rascunho.
- [ ] Portal nao lista servicos arquivados.
- [ ] Portal nao lista servicos inativos.
- [ ] Portal filtra por departamento responsavel.
- [ ] Portal filtra por categoria.
- [ ] Portal busca por termo.
- [ ] Portal abre detalhe por slug.
- [ ] Portal respeita visibilidade Solicitante.
- [ ] Portal respeita visibilidade Atendente.
- [ ] Portal respeita visibilidade Administrador.
- [ ] Portal respeita visibilidade Interno.

## Abertura de chamado
- [ ] Solicitante abre chamado a partir de servico valido.
- [ ] Chamado recebe CatalogoServicoId.
- [ ] Chamado recebe departamento responsavel do servico.
- [ ] Chamado recebe categoria do servico quando configurada.
- [ ] Chamado recebe subcategoria do servico quando configurada.
- [ ] Chamado recebe prioridade do servico quando configurada.
- [ ] Chamado recebe SLA/politica SLA do servico quando configurada.
- [ ] Historico registra abertura por catalogo.
- [ ] Abertura sem catalogo continua funcionando.
- [ ] Sistema bloqueia abertura por servico rascunho.
- [ ] Sistema bloqueia abertura por servico arquivado.
- [ ] Sistema bloqueia abertura por servico inativo.
- [ ] Sistema bloqueia abertura por servico sem visibilidade.
- [ ] Sistema bloqueia abertura por servico com PermiteAberturaChamado=false.
- [ ] Backend ignora/sobrescreve departamento/categoria/prioridade manipulados no frontend quando CatalogoServicoId e informado.

## Pendencias evolutivas relacionadas
- [ ] Homologacao institucional com usuarios reais.
- [ ] Evidencias com prints reais.
- [ ] Testes E2E completos.