# Runbook de Autenticacao - SGX Sistema de Chamados

## Objetivo
Fornecer procedimento operacional padrao para homologacao e entrada em operacao dos metodos de autenticacao do SGX.

## Escopo
- administrador local da instancia;
- login `LocalSgx`;
- login `ActiveDirectory`;
- login `MicrosoftEntraId`;
- metodos de login configuraveis;
- auditoria de autenticacao;
- contingencia e rollback.

## Equipe minima recomendada
- 1 administrador SGX com permissao de gestao de metodos;
- 1 analista de identidade (AD/Entra ID);
- 1 validador de governanca/auditoria.

## Janela de execucao recomendada
- homologacao: janela com suporte tecnico integral;
- producao: janela controlada com plano de rollback aprovado.

## 1. Preparacao da rodada
1. Validar disponibilidade de API, frontend e banco.
2. Confirmar segredo/configuracao tecnica:
   - `Authentication__*`
   - `ActiveDirectory__*`
   - `AzureAd__*`
   - `SGX_ADMIN_INICIAL_*` (somente bootstrap inicial)
3. Confirmar massa de teste:
   - usuario administrador;
   - usuario comum ativo;
   - usuario inativo;
   - usuario sem permissao `AuditoriaAutenticacao.Visualizar`.
4. Definir pasta de evidencias (prints, payloads anonimizados e ata de execucao).

## 2. Roteiro - Administrador Local da Instancia
1. Definir variaveis `SGX_ADMIN_INICIAL_EMAIL`, `SGX_ADMIN_INICIAL_SENHA`, `SGX_ADMIN_INICIAL_NOME`.
2. Subir aplicacao e validar criacao do administrador inicial.
3. Executar primeiro login e confirmar troca obrigatoria de senha.
4. Alterar senha e validar novo login.
5. Remover/rotacionar `SGX_ADMIN_INICIAL_*`.
6. Reiniciar aplicacao e validar que nao houve novo bootstrap indevido.

Resultado esperado:
- bootstrap ocorre uma unica vez e sem vazamento de segredo.

## 3. Roteiro - Login LocalSgx
1. Habilitar `LocalSgx` em `Admin > Integracoes > Autenticacao > Metodos de login`.
2. Validar exibicao no `/login`.
3. Executar login valido e confirmar emissao de JWT SGX.
4. Executar login invalido e confirmar resposta de negacao.
5. Validar bloqueio de usuario inativo.
6. Validar eventos de auditoria (sucesso e negado).

Resultado esperado:
- `LocalSgx` atende contingencia administrativa completa.

## 4. Roteiro - Login ActiveDirectory
1. Validar conectividade LDAP/LDAPS do ambiente SGX ate o AD.
2. Confirmar `ActiveDirectory` habilitado no SGX.
3. Executar `POST /api/auth/ad/login` com credencial valida.
4. Confirmar emissao de JWT SGX e acesso conforme permissao interna.
5. Validar bloqueio de usuario interno inativo.
6. Com auto provisionamento ligado, validar criacao com perfil padrao.
7. Com auto provisionamento desligado, validar exigencia de usuario pre-cadastrado.
8. Confirmar que nao ha atribuicao automatica de perfil `Administrador`.
9. Validar eventos de auditoria correspondentes.

Resultado esperado:
- AD autentica e SGX autoriza com seguranca institucional.

## 5. Roteiro - Login MicrosoftEntraId
1. Confirmar configuracao `AzureAd__*` no ambiente.
2. Habilitar `MicrosoftEntraId` nos metodos de login.
3. Validar exibicao do botao no `/login`.
4. Executar autenticacao com conta corporativa do tenant autorizado.
5. Confirmar autorizacao por perfis/permissoes internos do SGX.
6. Validar comportamento com conta nao autorizada (quando aplicavel).
7. Validar registro de auditoria.

Resultado esperado:
- fluxo corporativo Entra ID funcional e rastreavel.

## 6. Roteiro - Metodos de login configuraveis
1. Abrir `Admin > Integracoes > Autenticacao > Metodos de login`.
2. Validar habilitar/desabilitar provedores.
3. Validar provedor principal e ordem de exibicao.
4. Executar tentativas de configuracao insegura e confirmar bloqueio:
   - deixar sem metodo viavel de login;
   - remover ultimo acesso administrativo viavel.
5. Validar reflexo imediato em `GET /api/auth/provedores`.
6. Confirmar regra de `LocalDevelopment` restrita a `Development`.

Resultado esperado:
- governanca de metodos funcional sem quebrar acesso administrativo.

## 7. Roteiro - Auditoria de autenticacao
1. Acessar `Admin > Governanca > Auditoria de autenticacao`.
2. Validar filtros por periodo, provedor, resultado, tipo de evento e usuario/e-mail.
3. Confirmar exibicao de eventos:
   - login bem-sucedido;
   - login negado;
   - usuario inativo bloqueado;
   - alteracao de metodos de login.
4. Validar acesso com usuario sem permissao e confirmar bloqueio.

Resultado esperado:
- auditoria rastreavel e protegida por permissao.

## 8. Encerramento da homologacao
1. Atualizar checklist em `docs/HOMOLOGACAO-AUTENTICACAO.md`.
2. Consolidar evidencias em ata unica.
3. Registrar pendencias e plano de acao com prazo/responsavel.
4. Emitir parecer:
   - Aprovado
   - Aprovado com ressalvas
   - Reprovado

## 9. Contingencia e rollback
- indisponibilidade AD/Entra ID:
  - manter `LocalSgx` habilitado;
  - executar acesso administrativo local e tratar integracao externa.
- erro de configuracao de provedores:
  - reverter para conjunto seguro com metodo administrativo viavel;
  - revalidar `GET /api/auth/provedores` e login local.
- suspeita de comprometimento de credencial:
  - redefinir senha imediatamente;
  - revisar auditoria e acionar processo interno de seguranca.
