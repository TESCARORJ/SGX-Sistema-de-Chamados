# Autenticação Corporativa - SGX Sistema de Chamados

## Objetivo
Padronizar a autenticação com múltiplos provedores no SGX Sistema de Chamados, mantendo a regra arquitetural principal:

- O provedor de identidade autentica.
- O SGX autoriza por perfis e permissões internos.

## Decisão arquitetural

- `MicrosoftEntraId`: autenticação corporativa via Microsoft Entra ID.
- `ActiveDirectory`: autenticação corporativa via Active Directory do cliente.
- `LocalSgx`: autenticação local SGX (e-mail e senha do próprio SGX).
- `LocalDevelopment`: fluxo técnico separado, exclusivo de `Development`.

Regras de governança:
- Provedor configurado não significa provedor habilitado.
- Provedor habilitado significa disponível na tela de login.
- O provedor autentica; o SGX autoriza por perfis e permissões internos.

## Status no Roadmap ITSM

- Item: `Autenticação corporativa` (Área Segurança, ordem 4).
- Status da implementação: `Implementado funcionalmente`.
- Status técnico: `Completo com pendências evolutivas`.
- Checklist ativo: `27` itens (`19` concluídos e `8` pendentes de homologação/governança), percentual calculado automaticamente (~`70%`).
- Homologação real ainda pendente com tenant institucional e usuários corporativos reais.

O login local de produção (`LocalSgx`) **não** substitui o login local de `Development`.

## Configurações obrigatórias

### Backend (`Authentication__*`)

- `Authentication__Provedores__Configurados`: lista de códigos configurados
- `Authentication__Provedores__Habilitados`: lista de códigos habilitados para login
- `Authentication__Provedores__Principal`: código principal para priorização na UI
- `Authentication__Provedores__Ordem__<Codigo>`: ordem de exibição na UI
- Códigos suportados: `MicrosoftEntraId`, `ActiveDirectory`, `LocalSgx`, `LocalDevelopment`
- `Authentication__LoginLocalHabilitado`: `true`/`false` (exigido para `LocalSgx`)
- `Authentication__ModoLocalHabilitado`: `true`/`false` (somente `Development` para `LocalDevelopment`)
- `Authentication__JwtLocalIssuer`
- `Authentication__JwtLocalAudience`
- `Authentication__JwtLocalChaveAssinatura` (mínimo de 32 caracteres)
- `Authentication__JwtLocalExpiracaoMinutos`
- `Authentication__PoliticaSenha__TamanhoMinimo`
- `Authentication__PoliticaSenha__ExigirMaiuscula`
- `Authentication__PoliticaSenha__ExigirMinuscula`
- `Authentication__PoliticaSenha__ExigirNumero`
- `Authentication__PoliticaSenha__ExigirEspecial`
- `Authentication__PoliticaSenha__BloquearSenhaAnterior`
- `Authentication__Lockout__TentativasMaximas`
- `Authentication__Lockout__MinutosBloqueio`
- `Authentication__RecuperacaoSenha__ExpiracaoMinutos`

### Microsoft Entra ID (`AzureAd__*`)

Obrigatório quando `MicrosoftEntraId` estiver habilitado para autenticação.

## Regras de exibição no login

A tela de login consulta `GET /api/auth/provedores`:

- O backend retorna somente provedores habilitados e válidos para o ambiente atual.
- O frontend renderiza somente os provedores retornados pela API.
- `LocalDevelopment` aparece somente em `Development`, mesmo que esteja configurado/habilitado fora desse ambiente.

Se Microsoft não estiver configurado e o login local SGX estiver habilitado, a tela permanece funcional sem erro técnico bloqueante.

## Microsoft Entra ID - Single Tenant

- Microsoft Entra ID autentica.
- SGX Sistema de Chamados autoriza internamente por perfis e permissões.
- Tenant configurado é obrigatório.
- Contas pessoais Microsoft não são aceitas.
- Tenants externos não são aceitos por padrão.
- `roles/groups` do Azure não viram perfil `Administrador`.
- O SGX decide acesso com base no usuário interno, perfis e permissões.

Mensagem para reunião:
O SGX Sistema de Chamados usa Microsoft Entra ID em modo Single Tenant. Isso significa que somente contas corporativas do tenant configurado podem autenticar. Contas pessoais Microsoft e tenants externos não são aceitos por padrão. A autorização continua sendo controlada dentro do SGX por perfis e permissões.

## Endpoints de autenticação

### `GET /api/auth/provedores`
Retorna os provedores habilitados para a UI.

Resposta:

```json
{
  "provedores": [
    {
      "codigo": "MicrosoftEntraId",
      "nome": "Microsoft Entra ID",
      "descricao": "Login corporativo federado pelo Microsoft Entra ID.",
      "habilitado": true,
      "principal": true,
      "ordem": 10
    }
  ]
}
```

### `POST /api/auth/local/login`
Autentica localmente no SGX e emite JWT assinado pela API.

Payload:

```json
{
  "email": "usuario@empresa.com",
  "senha": "SenhaForte"
}
```

Resposta:

```json
{
  "accessToken": "<jwt>",
  "tokenType": "Bearer",
  "expiresIn": 7200,
  "autenticadoPor": "LocalSgx",
  "deveAlterarSenha": false
}
```

## `/api/me` e origem da autenticação

`GET /api/me` mantém o contrato e retorna:

- `autenticadoPor=MicrosoftEntraId` no fluxo Microsoft.
- `autenticadoPor=LocalSgx` no fluxo local SGX.
- `autenticadoPor=LocalDevelopment` apenas no modo local de desenvolvimento.
- `deveAlterarSenha=true` quando o usuário local deve concluir troca obrigatória antes de navegar.

## Senhas e segurança

- Senha local SGX é armazenada em hash (`senha_hash_local`).
- Implementação usa `PasswordHasher<Usuario>`.
- Não há senha em texto puro no banco.
- Hash/salt não são retornados pela API.
- Não existe senha fixa de desenvolvimento no frontend para autenticação real.

## Regras de bloqueio

- Usuário inativo no SGX é bloqueado no login local SGX.
- Perfis e permissões continuam internos ao SGX.

## Diferença entre Local SGX e Local Development

### Local SGX (`LocalSgx`)

- Válido para produção/homologação quando habilitado.
- Usa `POST /api/auth/local/login`.
- Exige credenciais locais com hash seguro.
- Emite JWT local da API.

### Local Development (`LocalDevelopment`)

- Apenas em `Development`.
- Não representa autenticação corporativa nem login local de produção.
- Usa headers `X-Dev-*` para suporte técnico e emulação de perfis.

## Sprint Autenticação 7 - Administrador inicial seguro

Objetivo:
- Permitir a criação segura do primeiro Administrador em produção por variáveis de ambiente explícitas.

Variáveis obrigatórias:
- `SGX_ADMIN_INICIAL_EMAIL`
- `SGX_ADMIN_INICIAL_SENHA`
- `SGX_ADMIN_INICIAL_NOME`

Regras:
- Só cria o Administrador inicial quando as três variáveis estiverem preenchidas.
- Só cria se ainda não existir Administrador ativo.
- Senha sempre armazenada com hash (`PasswordHasher<Usuario>`).
- Senha nunca é registrada em log.
- Senhas fracas são rejeitadas (incluindo `Admin@123456`, `123456`, `password`, `senha`, `admin`).

Operação recomendada:
1. Definir as variáveis apenas para o primeiro ciclo de inicialização.
2. Confirmar criação do Administrador inicial.
3. Remover/rotacionar imediatamente as variáveis de ambiente.

Diferenças importantes:
- `LocalSgx`: login local SGX de produção/homologação.
- `LocalDevelopment`: login técnico somente em Development.
- Administrador inicial: mecanismo de bootstrap seguro para o primeiro acesso administrativo.

## Sprint 2 - Administrador Local da Instância

Objetivo:
- consolidar o Administrador Local da Instância como recurso oficial para primeiro acesso, contingência administrativa e configuração inicial do ambiente.

Status técnico:
- implementado funcionalmente.

Regras aplicadas:
- criação inicial por configuração explícita via `SGX_ADMIN_INICIAL_EMAIL`, `SGX_ADMIN_INICIAL_SENHA`, `SGX_ADMIN_INICIAL_NOME`;
- criação somente quando não existe Administrador ativo;
- senha sempre armazenada com hash seguro (`PasswordHasher<Usuario>`);
- senha nunca registrada em log;
- senha fraca rejeitada pela política centralizada;
- usuário inicial criado com `DeveAlterarSenha=true`;
- autenticação do administrador local via `LocalSgx`;
- `LocalDevelopment` permanece exclusivo de `Development` e não substitui o Administrador Local da Instância.

Fluxo esperado:
1. Operador define variáveis `SGX_ADMIN_INICIAL_*` e configuração de login local SGX.
2. API inicializa, cria o Administrador Local da Instância e associa perfil `Administrador`.
3. Primeiro login local retorna `deveAlterarSenha=true`.
4. Troca de senha obrigatória é concluída.
5. Próximo login local retorna `deveAlterarSenha=false`.

Detalhamento operacional:
- consultar `docs/ADMINISTRADOR-LOCAL-INSTANCIA.md`.

## Sprint 3 - Active Directory dedicado (LDAP/LDAPS)

Objetivo:
- permitir autenticação corporativa on-premises diretamente no Active Directory do cliente.

Regra central:
- Active Directory autentica.
- SGX autoriza por perfis e permissões internos.

Endpoint:
- `POST /api/auth/ad/login`

Payload:

```json
{
  "usuario": "thiago",
  "senha": "********",
  "dominio": "EMPRESA"
}
```

Resposta:

```json
{
  "accessToken": "<jwt>",
  "tokenType": "Bearer",
  "expiresIn": 7200,
  "autenticadoPor": "ActiveDirectory",
  "deveAlterarSenha": false
}
```

Regras aplicadas:
- endpoint AD só funciona com provedor `ActiveDirectory` habilitado;
- senha AD usada apenas para bind/validação LDAP/LDAPS;
- senha AD nunca é armazenada no SGX;
- senha AD nunca aparece em logs;
- LDAPS é preferencial; LDAP sem TLS exige configuração explícita;
- usuário inativo no SGX é bloqueado mesmo com credencial AD válida;
- auto provisionamento opcional por configuração;
- grupos AD não concedem `Administrador` automaticamente.

Detalhamento de configuração:
- consultar `docs/CONFIGURACAO-ACTIVE-DIRECTORY.md`.

## Sprint 4 - Gestao administrativa dos metodos de login

Objetivo:
- permitir que o administrador do SGX configure metodos de login sem depender apenas de appsettings/variaveis de ambiente.

Escopo entregue:
- endpoints administrativos:
  - `GET /api/admin/autenticacao/provedores`
  - `PUT /api/admin/autenticacao/provedores`
- permissões:
  - `AutenticacaoProvedores.Visualizar`
  - `AutenticacaoProvedores.Gerenciar`
- persistencia administrativa da configuracao de provedores;
- fallback seguro para configuracao tecnica quando nao houver configuracao administrativa salva;
- `GET /api/auth/provedores` passa a refletir a configuracao administrativa efetiva.

Regras de seguranca aplicadas:
- nao permite configuracao sem metodo de login viavel;
- nao permite remover ultimo metodo de acesso administrativo sem alternativa funcional;
- `LocalDevelopment` continua exclusivo de `Development`;
- auto provisionamento por AD/Microsoft nao pode atribuir `Administrador` automaticamente;
- o SGX continua autorizando internamente por perfis e permissoes.

Frontend:
- tela administrativa em `Administracao > Integracoes > Autenticacao > Metodos de login`;
- configuracao de habilitado, principal, ordem, auto provisionamento, perfil padrao e rotulo;
- login renderiza somente provedores retornados por `GET /api/auth/provedores`.

Detalhamento operacional:
- consultar `docs/METODOS-DE-LOGIN.md`.

## Sprint 5 - Auditoria de autenticacao e metodos de login

Objetivo:
- ampliar rastreabilidade e governanca com trilha persistida de autenticacao e administracao dos metodos de login.

Entregas:
- classificacao de eventos por `TipoEventoAutenticacao` e `ResultadoEventoAutenticacao`;
- eventos persistidos no banco via `EventoAuditoria` (modulo `Autenticacao`);
- cobertura dos fluxos:
  - login `LocalSgx`;
  - login `ActiveDirectory`;
  - login `MicrosoftEntraId` (ponto de sucesso no fluxo efetivo);
  - usuario inativo bloqueado;
  - auto provisionamento;
  - troca obrigatoria de senha e recuperacao/redefinicao local;
  - alteracoes administrativas dos metodos de login;
  - bloqueios de configuracao insegura;
  - tentativa negada por falta de permissao para alterar metodos de login.

Endpoint administrativo:
- `GET /api/admin/auditoria/autenticacao`
- permissao: `AuditoriaAutenticacao.Visualizar`

Regras de seguranca:
- sem registro de senha, token, hash, secret ou credencial sensivel;
- falha de auditoria nao quebra autenticacao;
- falhas de auditoria sao registradas em log tecnico seguro.

Detalhamento:
- consultar `docs/AUDITORIA-AUTENTICACAO.md`.

## Sprint Autenticação 8 - Recuperação de senha e hardening do login local SGX

Objetivo:
- Evoluir o login local SGX para uso de produção com recuperação de senha, troca obrigatória, política de senha, lockout e auditoria básica por logs estruturados.

Entregas:
- `POST /api/auth/local/alterar-senha` para troca de senha autenticada.
- `POST /api/auth/local/recuperar-senha/solicitar` com resposta genérica (sem enumeração de usuário).
- `POST /api/auth/local/recuperar-senha/redefinir` com token temporário, expiração e uso único.
- Política de senha centralizada e reutilizada no bootstrap, troca e redefinição.
- Lockout configurável por tentativas inválidas (`TentativasInvalidas`, `BloqueadoAte`).
- Registro de `UltimoLoginEm` em login local bem-sucedido.
- Suporte de troca obrigatória (`DeveAlterarSenha`) com redirecionamento para `/alterar-senha`.

Regras de segurança:
- Senha e token nunca são armazenados em texto puro.
- Banco armazena apenas `senha_hash_local` e `token_hash`.
- Senha e token não são registrados em logs.
- Resposta de solicitação de recuperação sempre usa mensagem genérica:
  - `Se o e-mail estiver cadastrado, enviaremos as instruções para redefinição de senha.`

Diferença entre os fluxos:
- `LocalSgx`: autenticação local de produção/homologação com hardening de senha.
- `LocalDevelopment`: suporte técnico exclusivo de Development.
- Administrador inicial seguro: bootstrap de primeiro acesso administrativo em produção, com `DeveAlterarSenha=true`.

Pendências para Sprint Autenticação 9:
- Entrega do envio transacional real de e-mail para recuperação de senha.
- Evolução para trilha dedicada de auditoria persistida em banco.
- Homologação formal em ambiente publicado dos fluxos de recuperação e lockout.

## Correções - Integração Microsoft e senha atribuída por Administrador

Entregas:
- Menu administrativo de Integrações com item `Microsoft Entra ID`.
- Tela administrativa em `/admin/integracoes/microsoft-entra-id`.
- Endpoints administrativos:
  - `GET /api/admin/integracoes/microsoft-entra-id`
  - `PUT /api/admin/integracoes/microsoft-entra-id`
- Login (`/api/auth/provedores`) passa a refletir configuração administrativa de provedores.
- Redefinição administrativa de senha:
  - `POST /api/admin/cadastros/usuarios/{id}/redefinir-senha`
  - valida política de senha, aplica hash seguro e permite `deveAlterarSenha`.

Permissões novas:
- `IntegracoesMicrosoft.Visualizar`
- `IntegracoesMicrosoft.Gerenciar`
- `Usuarios.RedefinirSenha`

Regras de segurança:
- Senha nunca é armazenada em texto puro.
- Hash de senha não é retornado pela API.
- Senha não é registrada em log.
- Não é permitido salvar configuração sem nenhum provedor de autenticação ativo.

## Correção urgente - usuários demonstrativos e validação Microsoft Entra ID

### Usuários demonstrativos oficiais (Development)

Manter ativos apenas os seis usuários abaixo:

- `admin@sgxdigital.com`
- `admin2@sgxdigital.com`
- `atendente.demo@sgxdigital.com`
- `atendente2.demo@sgxdigital.com`
- `solicitante.demo@sgxdigital.com`
- `solicitante2.demo@sgxdigital.com`

Regras:

- Usuários antigos de `seed`, `homol` e `local` devem ser inativados.
- Qualquer usuário demonstrativo legado no domínio `@sgx.local` deve ser inativado.
- Usuários demonstrativos legados não devem ser recriados automaticamente após inativação.
- Não excluir fisicamente usuários reais.
- Não inativar automaticamente o administrador inicial seguro quando for usuário real (`SGX_ADMIN_INICIAL_*`).

### Campos obrigatórios da integração Microsoft Entra ID

Quando o provedor `MicrosoftEntraId` estiver habilitado, são obrigatórios:

- `Tenant ID`
- `Client ID`
- `Audience`
- `Issuer`
- `Authority`
- `API Scope`
- `Redirect URI`

Validações:

- O backend rejeita `PUT /api/admin/integracoes/microsoft-entra-id` com qualquer campo obrigatório ausente.
- Em compatibilidade administrativa legada, `ProvedorPrincipal=Local` exige `LoginLocalHabilitado=true`.
- Em `ProvedorPrincipal=Local` com `LoginLocalHabilitado=false`, a API retorna a mensagem: `Login local SGX deve permanecer habilitado quando o modo Local estiver selecionado.`
- O sistema rejeita qualquer configuração sem provedor de autenticação ativo.
