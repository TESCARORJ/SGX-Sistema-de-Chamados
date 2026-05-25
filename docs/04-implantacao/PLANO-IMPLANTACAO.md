# Plano de Implantacao - SGX Sistema de Chamados

## 1. Objetivo

Este documento define o plano inicial de implantacao do SGX Sistema de Chamados em ambiente de homologacao e, futuramente, em producao.

O objetivo e estabelecer requisitos minimos de infraestrutura, seguranca, configuracao, publicacao, backup, monitoramento e responsabilidades.

## 2. Modelos de implantacao possiveis

### 2.1 Ambiente institucional / servidor do cliente

Neste modelo, o SGX e instalado em infraestrutura do cliente ou da organizacao.

Caracteristicas:

- maior controle institucional;
- dados ficam no ambiente do cliente;
- depende da equipe interna para rede, DNS, SSL, backup e monitoramento;
- recomendado para orgaos publicos ou ambientes com maior exigencia de controle.

### 2.2 VPS ou servidor dedicado gerenciado

Neste modelo, o SGX e publicado em uma VPS ou servidor dedicado sob gestao da equipe responsavel pelo produto.

Caracteristicas:

- maior controle tecnico pelo fornecedor;
- implantacao mais rapida;
- exige contrato claro de hospedagem, backup, seguranca e suporte;
- adequado para MVP comercial ou pilotos controlados.

### 2.3 SaaS por subdominio

Neste modelo, cada cliente acessa uma instancia ou contexto pelo subdominio.

Exemplo:

```txt
cliente.sgxsistemadechamados.com.br
```

Caracteristicas:

- melhor modelo para escala comercial;
- exige estrategia de multi-tenant ou instancias separadas;
- exige politica forte de isolamento de dados;
- exige observabilidade, billing, suporte e governanca de ambientes.

### 2.4 Ambiente hibrido

Modelo em que a aplicacao fica hospedada em infraestrutura controlada, mas autentica com Microsoft Entra ID e integra com recursos do cliente.

Caracteristicas:

- bom equilibrio entre controle e praticidade;
- exige alinhamento com equipe de identidade, seguranca e rede;
- adequado para pilotos institucionais.

## 3. Recomendacao para a fase atual

Para transformar o MVP em produto, a recomendacao e iniciar com:

**Ambiente de homologacao controlado em VPS ou servidor institucional.**

Somente apos homologacao, definir se a primeira producao sera:

- servidor do cliente;
- VPS gerenciada;
- SaaS por subdominio;
- modelo hibrido.

## 4. Componentes da solucao

O SGX possui os seguintes componentes principais:

- Frontend Web: Vue 3 + Quasar.
- API Backend: ASP.NET Core / .NET.
- Banco de Dados: PostgreSQL.
- Worker de E-mail: Worker Service para leitura/processamento IMAP.
- Integracao de Identidade: Microsoft Entra ID.
- Armazenamento de anexos: diretorio controlado ou estrategia futura de storage.
- Logs e auditoria: registros internos e eventos de auditoria.

## 5. Requisitos minimos de ambiente de homologacao

### Servidor

Requisitos sugeridos para homologacao:

- 2 vCPU;
- 4 GB RAM;
- 40 GB SSD;
- Linux Server atualizado;
- acesso SSH restrito;
- firewall ativo;
- Nginx ou proxy reverso equivalente;
- certificado SSL valido;
- Docker/Docker Compose, se adotado no empacotamento.

### Banco de dados

- PostgreSQL dedicado ao ambiente;
- usuario especifico da aplicacao;
- senha forte;
- acesso externo bloqueado, salvo necessidade controlada;
- backup definido;
- rotina de restore testada antes de producao.

### Rede e DNS

- dominio ou subdominio definido;
- certificado HTTPS;
- portas expostas somente quando necessario;
- API e frontend acessiveis via HTTPS;
- CORS configurado somente para origens permitidas.

## 6. Variaveis e configuracoes principais

### Backend

- `ConnectionStrings__DefaultConnection`
- `Authentication__ProvedorPrincipal`
- `Authentication__LoginLocalHabilitado`
- `Authentication__ModoLocalHabilitado`
- `Authentication__JwtLocalIssuer`
- `Authentication__JwtLocalAudience`
- `Authentication__JwtLocalChaveAssinatura`
- `SGX_ADMIN_INICIAL_EMAIL`
- `SGX_ADMIN_INICIAL_SENHA`
- `SGX_ADMIN_INICIAL_NOME`
- `AzureAd__Instance`
- `AzureAd__TenantId`
- `AzureAd__ClientId`
- `AzureAd__Audience`
- `AzureAd__Issuer`

### Frontend

- `VITE_API_BASE_URL`
- `VITE_AZURE_CLIENT_ID`
- `VITE_AZURE_TENANT_ID`
- `VITE_AZURE_AUTHORITY`
- `VITE_AZURE_REDIRECT_URI`
- `VITE_AZURE_API_SCOPE`

### Worker de e-mail

- servidor IMAP;
- porta;
- SSL/TLS;
- usuario da caixa;
- senha ou OAuth, conforme politica institucional;
- pasta monitorada;
- intervalo de leitura;
- regras de categoria, prioridade e departamento padrao;
- limite de anexos;
- extensoes permitidas e bloqueadas.

## 7. Integracao Microsoft Entra ID

Para homologacao real, a equipe responsavel pelo Microsoft Entra ID deve validar:

- App Registration do frontend SPA;
- App Registration ou exposicao de API;
- Redirect URI;
- Logout URI;
- escopos da API;
- Tenant ID;
- Client ID;
- Audience;
- permissoes concedidas;
- Conditional Access;
- MFA;
- grupos/claims, se aplicavel;
- dominios permitidos;
- comportamento para usuarios externos.

Diretriz:

- Microsoft Entra ID autentica.
- SGX autoriza internamente por usuarios, perfis e permissoes.

## 8. Integracao de e-mail

A abertura por e-mail deve ser validada em ambiente real ou equivalente.

Validar:

- caixa institucional;
- leitura IMAP;
- autenticacao basica ou OAuth;
- criacao de chamado por e-mail novo;
- deduplicacao por MessageId;
- correlacao de respostas;
- comentarios por resposta;
- anexos permitidos;
- rejeicao de anexos invalidos;
- logs administrativos;
- falhas de processamento;
- monitoramento do Worker.

## 9. Anexos e arquivos

Validar politica de armazenamento de anexos:

- diretorio de armazenamento;
- permissao de escrita apenas pela aplicacao;
- bloqueio de extensoes perigosas;
- limite de tamanho;
- nomes fisicos seguros;
- nao exposicao de caminho interno;
- backup dos anexos;
- varredura antivirus como evolucao recomendada.

## 10. Backup e restore

Antes de producao, definir:

- backup do banco;
- backup dos anexos;
- periodicidade;
- retencao;
- local seguro de armazenamento;
- responsavel;
- procedimento de restore;
- teste de restore.

Checklist minimo:

| Item | Status |
|---|---|
| Backup do banco definido | Pendente |
| Backup dos anexos definido | Pendente |
| Restore testado | Pendente |
| Responsavel definido | Pendente |
| Retencao definida | Pendente |

## 11. Logs, monitoramento e observabilidade

Para homologacao:

- logs da API;
- logs do Worker;
- logs de erro do frontend, quando aplicavel;
- eventos de auditoria no banco;
- logs de integracao de e-mail;
- health check basico.

Para producao:

- monitoramento de disponibilidade;
- alertas de indisponibilidade;
- alertas de erro recorrente no Worker;
- alertas de falha de autenticacao/integracao;
- metricas de performance;
- retencao de logs;
- integracao futura com ferramenta de observabilidade.

## 12. Seguranca minima antes de producao

Antes de producao, validar:

- HTTPS obrigatorio;
- segredos fora do codigo-fonte;
- senha inicial removida/rotacionada;
- login local de desenvolvimento desabilitado;
- CORS restrito;
- banco sem exposicao publica desnecessaria;
- permissoes de arquivos restritas;
- usuario de banco com menor privilegio necessario;
- backup protegido;
- politicas de senha local, quando login local estiver habilitado;
- lockout;
- auditoria ativa;
- MFA via Microsoft Entra ID;
- revisao de endpoints administrativos.

## 13. Plano de publicacao sugerido

### Etapa 1 - Preparacao

- Definir ambiente.
- Definir URL.
- Configurar banco.
- Configurar variaveis.
- Configurar Entra ID.
- Configurar caixa de e-mail.
- Configurar SSL.

### Etapa 2 - Deploy tecnico

- Publicar API.
- Publicar frontend.
- Publicar Worker.
- Aplicar migrations.
- Criar administrador inicial.
- Validar logs.
- Validar acesso.

### Etapa 3 - Validacao tecnica

- Testar login.
- Testar `GET /api/me`.
- Testar abertura de chamado.
- Testar atendimento.
- Testar anexos.
- Testar e-mail.
- Testar SLA.
- Testar dashboard.

### Etapa 4 - Homologacao funcional

- Executar plano de homologacao.
- Registrar evidencias.
- Registrar pendencias.
- Corrigir falhas criticas.
- Obter aceite formal.

### Etapa 5 - Preparacao para producao

- Revisar seguranca.
- Definir backup.
- Definir suporte.
- Definir monitoramento.
- Definir plano de rollback.
- Definir data de virada.

## 14. Plano de rollback

Antes de producao, documentar:

- como voltar para a ferramenta anterior;
- como preservar chamados criados no SGX durante piloto;
- como desativar Worker de e-mail;
- como bloquear novos acessos temporariamente;
- como restaurar banco e anexos;
- quem decide o rollback;
- tempo maximo aceitavel de indisponibilidade.

## 15. Responsabilidades

| Area | Responsabilidade |
|---|---|
| Desenvolvimento SGX | Codigo, deploy da aplicacao, ajustes e documentacao tecnica. |
| Infraestrutura | Servidor, rede, DNS, SSL, firewall e backup. |
| Identidade / Azure | App Registration, tenant, MFA, Conditional Access e permissoes. |
| Service Desk / TI | Homologacao funcional, criterios de aceite e operacao. |
| Gestao de TI | Decisao de implantacao, priorizacao e aceite. |
| Seguranca | Revisao de riscos, acessos, logs e politicas. |

## 16. Criterios minimos para producao

O SGX somente deve ir para producao se:

- homologacao principal estiver aprovada;
- nao houver falha critica aberta;
- login e permissoes estiverem validados;
- e-mail estiver validado ou formalmente fora do escopo inicial;
- backup e restore estiverem definidos;
- SSL e DNS estiverem configurados;
- logs estiverem acessiveis;
- responsaveis de suporte estiverem definidos;
- plano de rollback estiver aprovado.

## 17. Conclusao

A implantacao do SGX deve ser conduzida como processo formal de produto, nao apenas como publicacao tecnica.

A fase correta apos o MVP avancado e a criacao de um ambiente de homologacao controlado, com validacao de seguranca, integracoes, fluxos funcionais, evidencias e aceite institucional.