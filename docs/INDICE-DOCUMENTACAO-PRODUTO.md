# Indice da Documentacao de Produto - SGX Sistema de Chamados

## 1. Objetivo

Este indice organiza a documentacao criada para apoiar a transformacao do SGX Sistema de Chamados de MVP avancado para produto institucional de Service Desk / ITSM.

A documentacao esta dividida por publico e finalidade:

- diretoria e decisao executiva;
- visao de produto;
- aderencia ITIL / ITSM;
- homologacao institucional;
- implantacao tecnica.

## 2. Documentos executivos

### `docs/00-executivo/APRESENTACAO-DIRETORIA-TI.md`

Use este documento para apresentacao institucional para Diretoria de TI.

Ele responde:

- o que e o SGX;
- qual problema resolve;
- o que ja foi implementado;
- qual o status real do MVP;
- quais pendencias existem;
- qual roadmap executivo seguir;
- qual decisao se espera da diretoria.

### `docs/00-executivo/VISAO-PRODUTO.md`

Use este documento para explicar o SGX como produto.

Ele apresenta:

- posicionamento;
- publico-alvo;
- proposta de valor;
- modulos do produto;
- diferenciais;
- limites atuais;
- norte de evolucao.

## 3. Documentacao ITSM / ITIL

### `docs/01-itsm/MATRIZ-ADERENCIA-ITIL.md`

Use este documento em avaliacoes tecnicas com pessoas de ITIL / ITSM.

Ele mapeia:

- Gerenciamento de Incidentes;
- Gerenciamento de Requisicoes;
- Gerenciamento de Mudancas;
- Catalogo de Servicos;
- Base de Conhecimento;
- SLA;
- Inventario / Ativos;
- CMDB;
- MFA;
- Observadores;
- Grupos tecnicos;
- Regras de notificacao;
- Pesquisa de satisfacao;
- Regras de fechamento;
- Integracao Zabbix;
- Formularios dinamicos.

## 4. Homologacao

### `docs/02-homologacao/PLANO-HOMOLOGACAO-PRODUTO.md`

Use este documento para validar formalmente o SGX antes de producao.

Ele define:

- escopo de homologacao;
- perfis envolvidos;
- criterios de aceite;
- evidencias obrigatorias;
- roteiros por modulo;
- resultado consolidado;
- criterio para avancar para producao.

## 5. Implantacao

### `docs/04-implantacao/PLANO-IMPLANTACAO.md`

Use este documento para planejar ambiente de homologacao e producao.

Ele cobre:

- modelos de implantacao;
- requisitos minimos;
- variaveis de ambiente;
- Microsoft Entra ID;
- integracao de e-mail;
- anexos;
- backup e restore;
- logs e monitoramento;
- seguranca minima;
- plano de publicacao;
- rollback;
- responsabilidades.

## 6. Ordem recomendada de leitura

Para Diretoria de TI:

1. `docs/00-executivo/APRESENTACAO-DIRETORIA-TI.md`
2. `docs/00-executivo/VISAO-PRODUTO.md`
3. `docs/01-itsm/MATRIZ-ADERENCIA-ITIL.md`

Para equipe tecnica:

1. `docs/04-implantacao/PLANO-IMPLANTACAO.md`
2. `docs/02-homologacao/PLANO-HOMOLOGACAO-PRODUTO.md`
3. `docs/01-itsm/MATRIZ-ADERENCIA-ITIL.md`

Para homologacao:

1. `docs/02-homologacao/PLANO-HOMOLOGACAO-PRODUTO.md`
2. documentos especificos por modulo ja existentes em `docs/`
3. evidencias por modulo em `docs/02-homologacao/evidencias/`

## 7. Onde a documentacao deve ficar disponivel

### Fonte oficial

A fonte oficial da documentacao deve ser o proprio repositorio GitHub, na pasta `docs/`.

Motivos:

- versionamento;
- historico de alteracoes;
- rastreabilidade;
- revisao por commit ou pull request;
- alinhamento com o codigo-fonte.

### Consulta operacional

A consulta operacional deve ser feita dentro do SGX, no menu administrativo:

```txt
Admin > Gestao ITSM > Documentacao
Admin > Gestao ITSM > Roadmap
```

### Apresentacao executiva

Para reunioes com diretoria, gerar versoes derivadas em:

- PDF;
- PowerPoint;
- documento institucional.

Essas versoes devem ser derivadas da documentacao versionada no GitHub, evitando divergencia entre material apresentado e status real do produto.

## 8. Proximos documentos recomendados

Para completar a camada de produto, recomenda-se criar posteriormente:

- `docs/00-executivo/ROADMAP-EXECUTIVO.md`
- `docs/00-executivo/RESUMO-EXECUTIVO-UMA-PAGINA.md`
- `docs/02-homologacao/CHECKLIST-HOMOLOGACAO-GERAL.md`
- `docs/03-tecnico/SEGURANCA-PRODUTO.md`
- `docs/04-implantacao/AMBIENTE-HOMOLOGACAO.md`
- `docs/04-implantacao/AMBIENTE-PRODUCAO.md`

## 9. Conclusao

Esta estrutura separa claramente documentacao executiva, documentacao ITSM, homologacao e implantacao.

Com isso, o SGX passa a ter uma base documental adequada para avaliacao pela Diretoria de TI e para conduzir a evolucao do MVP para produto.