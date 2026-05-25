# Plano de Implantação - SGX Sistema de Chamados

## 1. Objetivo

Este documento define o plano inicial de implantação do SGX Sistema de Chamados em ambiente de homologação e, futuramente, em produção.

O objetivo é estabelecer requisitos mínimos de infraestrutura, segurança, configuração, publicação, backup, monitoramento e responsabilidades.

## 2. Modelos de implantação possíveis

### 2.1 Ambiente institucional / servidor do cliente

Neste modelo, o SGX é instalado em infraestrutura do cliente ou da organização.

Características:

- maior controle institucional;
- dados ficam no ambiente do cliente;
- depende da equipe interna para rede, DNS, SSL, backup e monitoramento;
- recomendado para órgãos públicos ou ambientes com maior exigência de controle.

### 2.2 VPS ou servidor dedicado gerenciado

Neste modelo, o SGX é publicado em uma VPS ou servidor dedicado sob gestão da equipe responsável pelo produto.

Características:

- maior controle técnico pelo fornecedor;
- implantação mais rápida;
- exige contrato claro de hospedagem, backup, segurança e suporte;
- adequado para MVP comercial ou pilotos controlados.

### 2.3 SaaS por subdomínio

Neste modelo, cada cliente acessa uma instância ou contexto pelo subdomínio.

Exemplo:

```txt
cliente.sgxsistemadechamados.com.br
```

Características:

- melhor modelo para escala comercial;
- exige estratégia de multi-tenant ou instâncias separadas;
- exige política forte de isolamento de dados;
- exige observabilidade, billing, suporte e governança de ambientes.

### 2.4 Ambiente híbrido

Modelo em que a aplicação fica hospedada em infraestrutura controlada, mas autentica com Microsoft Entra ID e integra com recursos do cliente.

Características:

- bom equilíbrio entre controle e praticidade;
- exige alinhamento com equipe de identidade, segurança e rede;
- adequado para pilotos institucionais.

## 3. Recomendação para a fase atual

Para transformar o MVP em produto, a recomendação é iniciar com:

**Ambiente de homologação controlado em VPS ou servidor institucional.**

Somente após homologação, definir se a primeira produção será:

- servidor do cliente;
- VPS gerenciada;
- SaaS por subdomínio;
- modelo híbrido.

## 4. Componentes da solução

O SGX possui os seguintes componentes principais:

- Frontend Web: Vue 3 + Quasar.
- API Backend: ASP.NET Core / .NET.
- Banco de Dados: PostgreSQL.
- Worker de E-mail: Worker Service para leitura/processamento IMAP.
- Integração de Identidade: Microsoft Entra ID.
- Armazenamento de anexos: diretório controlado ou estratégia futura de storage.
- Logs e auditoria: registros internos e eventos de auditoria.

## 5. Requisitos mínimos de ambiente de homologação

### Servidor

Requisitos sugeridos para homologação:

- 2 vCPU;
- 4 GB RAM;
- 40 GB SSD;
- Linux Server atualizado;
- acesso SSH restrito;
- firewall ativo;
- Nginx ou proxy reverso equivalente;
- certificado SSL válido;
- Docker/Docker Compose, se adotado no empacotamento.

### Banco de dados

- PostgreSQL dedicado ao ambiente;
- usuário específico da aplicação;
- senha forte;
- acesso externo bloqueado, salvo necessidade controlada;
- backup definido;
- rotina de restore testada antes de produção.

### Rede e DNS

- domínio ou subdomínio definido;
- certificado HTTPS;
- portas expostas somente quando necessário;
- API e frontend acessíveis via HTTPS;
- CORS configurado somente para origens permitidas.

## 6. Variáveis e configurações principais

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
- usuário da caixa;
- senha ou OAuth, conforme política institucional;
- pasta monitorada;
- intervalo de leitura;
- regras de categoria, prioridade e departamento padrão;
- limite de anexos;
- extensões permitidas e bloqueadas.

## 7. Integração Microsoft Entra ID

Para homologação real, a equipe responsável pelo Microsoft Entra ID deve validar:

- App Registration do frontend SPA;
- App Registration ou exposição de API;
- Redirect URI;
- Logout URI;
- escopos da API;
- Tenant ID;
- Client ID;
- Audience;
- permissões concedidas;
- Conditional Access;
- MFA;
- grupos/claims, se aplicável;
- domínios permitidos;
- comportamento para usuários externos.

Diretriz:

- Microsoft Entra ID autentica.
- SGX autoriza internamente por usuários, perfis e permissões.

## 8. Integração de e-mail

A abertura por e-mail deve ser validada em ambiente real ou equivalente.

Validar:

- caixa institucional;
- leitura IMAP;
- autenticação básica ou OAuth;
- criação de chamado por e-mail novo;
- deduplicação por MessageId;
- correlação de respostas;
- comentários por resposta;
- anexos permitidos;
- rejeição de anexos inválidos;
- logs administrativos;
- falhas de processamento;
- monitoramento do Worker.

## 9. Anexos e arquivos

Validar política de armazenamento de anexos:

- diretório de armazenamento;
- permissão de escrita apenas pela aplicação;
- bloqueio de extensões perigosas;
- limite de tamanho;
- nomes físicos seguros;
- não exposição de caminho interno;
- backup dos anexos;
- varredura antivírus como evolução recomendada.

## 10. Backup e restore

Antes de produção, definir:

- backup do banco;
- backup dos anexos;
- periodicidade;
- retenção;
- local seguro de armazenamento;
- responsável;
- procedimento de restore;
- teste de restore.

Checklist mínimo:

| Item | Status |
|---|---|
| Backup do banco definido | Pendente |
| Backup dos anexos definido | Pendente |
| Restore testado | Pendente |
| Responsável definido | Pendente |
| Retenção definida | Pendente |

## 11. Logs, monitoramento e observabilidade

Para homologação:

- logs da API;
- logs do Worker;
- logs de erro do frontend, quando aplicável;
- eventos de auditoria no banco;
- logs de integração de e-mail;
- health check básico.

Para produção:

- monitoramento de disponibilidade;
- alertas de indisponibilidade;
- alertas de erro recorrente no Worker;
- alertas de falha de autenticação/integração;
- métricas de performance;
- retenção de logs;
- integração futura com ferramenta de observabilidade.

## 12. Segurança mínima antes de produção

Antes de produção, validar:

- HTTPS obrigatório;
- segredos fora do código-fonte;
- senha inicial removida/rotacionada;
- login local de desenvolvimento desabilitado;
- CORS restrito;
- banco sem exposição pública desnecessária;
- permissões de arquivos restritas;
- usuário de banco com menor privilégio necessário;
- backup protegido;
- políticas de senha local, quando login local estiver habilitado;
- lockout;
- auditoria ativa;
- MFA via Microsoft Entra ID;
- revisão de endpoints administrativos.

## 13. Plano de publicação sugerido

### Etapa 1 - Preparação

- Definir ambiente.
- Definir URL.
- Configurar banco.
- Configurar variáveis.
- Configurar Entra ID.
- Configurar caixa de e-mail.
- Configurar SSL.

### Etapa 2 - Deploy técnico

- Publicar API.
- Publicar frontend.
- Publicar Worker.
- Aplicar migrations.
- Criar administrador inicial.
- Validar logs.
- Validar acesso.

### Etapa 3 - Validação técnica

- Testar login.
- Testar `GET /api/me`.
- Testar abertura de chamado.
- Testar atendimento.
- Testar anexos.
- Testar e-mail.
- Testar SLA.
- Testar dashboard.

### Etapa 4 - Homologação funcional

- Executar plano de homologação.
- Registrar evidências.
- Registrar pendências.
- Corrigir falhas críticas.
- Obter aceite formal.

### Etapa 5 - Preparação para produção

- Revisar segurança.
- Definir backup.
- Definir suporte.
- Definir monitoramento.
- Definir plano de rollback.
- Definir data de virada.

## 14. Plano de rollback

Antes de produção, documentar:

- como voltar para a ferramenta anterior;
- como preservar chamados criados no SGX durante piloto;
- como desativar Worker de e-mail;
- como bloquear novos acessos temporariamente;
- como restaurar banco e anexos;
- quem decide o rollback;
- tempo máximo aceitável de indisponibilidade.

## 15. Responsabilidades

| Área | Responsabilidade |
|---|---|
| Desenvolvimento SGX | Código, deploy da aplicação, ajustes e documentação técnica. |
| Infraestrutura | Servidor, rede, DNS, SSL, firewall e backup. |
| Identidade / Azure | App Registration, tenant, MFA, Conditional Access e permissões. |
| Service Desk / TI | Homologação funcional, critérios de aceite e operação. |
| Gestão de TI | Decisão de implantação, priorização e aceite. |
| Segurança | Revisão de riscos, acessos, logs e políticas. |

## 16. Critérios mínimos para produção

O SGX somente deve ir para produção se:

- homologação principal estiver aprovada;
- não houver falha crítica aberta;
- login e permissões estiverem validados;
- e-mail estiver validado ou formalmente fora do escopo inicial;
- backup e restore estiverem definidos;
- SSL e DNS estiverem configurados;
- logs estiverem acessíveis;
- responsáveis de suporte estiverem definidos;
- plano de rollback estiver aprovado.

## 17. Conclusão

A implantação do SGX deve ser conduzida como processo formal de produto, não apenas como publicação técnica.

A fase correta após o MVP avançado é a criação de um ambiente de homologação controlado, com validação de segurança, integrações, fluxos funcionais, evidências e aceite institucional.