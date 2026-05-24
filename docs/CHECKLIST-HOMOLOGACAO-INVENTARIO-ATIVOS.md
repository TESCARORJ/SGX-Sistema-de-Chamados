# Checklist de Homologacao - Inventario/Ativos

## Objetivo
Validar funcionalmente o modulo Inventario/Ativos antes da homologacao institucional.

## Cadastro de ativos
- [ ] Administrador cria ativo com dados obrigatorios.
- [ ] Administrador cria ativo sem numero de patrimonio.
- [ ] Administrador cria ativo sem numero de serie.
- [ ] Sistema impede codigo duplicado.
- [ ] Sistema impede patrimonio duplicado quando preenchido.
- [ ] Sistema impede numero de serie duplicado quando preenchido.
- [ ] Administrador edita ativo ativo.
- [ ] Sistema bloqueia edicao de ativo inativo.
- [ ] Administrador inativa ativo.
- [ ] Administrador reativa ativo.
- [ ] Sistema nao exclui fisicamente ativo.

## Tipos e filtros
- [ ] Listagem carrega tipos de ativo.
- [ ] Filtro por tipo funciona.
- [ ] Filtro por departamento funciona.
- [ ] Filtro por local/unidade funciona.
- [ ] Filtro por responsavel funciona.
- [ ] Filtro por status operacional funciona.
- [ ] Filtro por status patrimonial funciona.
- [ ] Filtro por criticidade funciona.
- [ ] Busca por codigo funciona.
- [ ] Busca por nome funciona.
- [ ] Busca por patrimonio/serie funciona.

## Historico e movimentacao
- [ ] Sistema registra historico na criacao.
- [ ] Sistema registra historico em edicao relevante.
- [ ] Sistema nao registra historico vazio.
- [ ] Administrador movimenta ativo entre departamentos.
- [ ] Administrador movimenta ativo entre locais.
- [ ] Administrador altera usuario responsavel.
- [ ] Administrador altera status operacional.
- [ ] Administrador altera status patrimonial.
- [ ] Sistema bloqueia movimentacao vazia.
- [ ] Sistema bloqueia movimentacao de ativo inativo.
- [ ] Historico exibe origem/destino e valores anteriores/novos.

## Vinculo com chamados
- [ ] Chamado pode ser aberto com ativo valido.
- [ ] Abertura sem ativo continua funcionando.
- [ ] Sistema bloqueia abertura com ativo inativo.
- [ ] Administrador vincula ativo a chamado existente.
- [ ] Administrador remove vinculo de ativo do chamado.
- [ ] Detalhe do chamado exibe ativo vinculado.
- [ ] Detalhe do ativo lista chamados relacionados.
- [ ] Historico do chamado registra vinculo/remocao.
- [ ] Historico do ativo registra vinculo/remocao.
- [ ] Chamados antigos continuam visiveis mesmo se ativo for inativado depois.

## Permissoes
- [ ] InventarioAtivos.Visualizar permite consulta.
- [ ] InventarioAtivos.Gerenciar permite criar/editar.
- [ ] InventarioAtivos.Inativar permite inativar/reativar.
- [ ] InventarioAtivos.Movimentar permite movimentar.
- [ ] InventarioAtivos.VincularChamado permite vincular/remover ativo em chamado.
- [ ] Usuario sem permissao nao acessa acoes restritas.

## Validacoes automatizadas de fechamento
- [ ] dotnet build -c Release
- [ ] dotnet test -c Release --no-build
- [ ] npm run test:unit
- [ ] npm run build

## Observacoes de homologacao
- Preencher evidencias reais em `docs/evidencias/inventario-ativos/README.md`.
- Nao anexar prints ficticios.
- Registrar perfil, dados e resultado de cada cenario.
