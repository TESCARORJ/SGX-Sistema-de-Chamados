# Implantacao Cloud - Autenticacao SGX

## Objetivo
Padronizar a implantacao da autenticacao do SGX em cloud (IaaS/PaaS), com foco em seguranca operacional e contingencia administrativa.

## Arquitetura de referencia
- API SGX publicada em servico gerenciado;
- frontend SGX publicado em servico web/CDN;
- PostgreSQL gerenciado;
- segredos em cofre (secret manager/vault);
- Microsoft Entra ID como provedor corporativo principal;
- `LocalSgx` habilitado como contingencia administrativa.

## Pre-requisitos
- DNS, certificado TLS e politicas de rede definidos;
- banco provisionado com conectividade da API;
- aplicacao registrada no Entra ID com redirect URI e escopos corretos;
- conta administrativa para primeira validacao.

## Variaveis minimas recomendadas

### Core da autenticacao
- `Authentication__Provedores__Configurados`
- `Authentication__Provedores__Habilitados`
- `Authentication__Provedores__Principal`
- `Authentication__JwtLocalIssuer`
- `Authentication__JwtLocalAudience`
- `Authentication__JwtLocalChaveAssinatura`
- `Authentication__JwtLocalExpiracaoMinutos`
- `Authentication__Lockout__TentativasMaximas`
- `Authentication__Lockout__MinutosBloqueio`

### Integracao Microsoft Entra ID
- `AzureAd__TenantId`
- `AzureAd__ClientId`
- `AzureAd__Audience`
- `AzureAd__Issuer`
- `AzureAd__Authority`

### Bootstrap administrativo
- `SGX_ADMIN_INICIAL_EMAIL`
- `SGX_ADMIN_INICIAL_SENHA`
- `SGX_ADMIN_INICIAL_NOME`

## Sequencia de implantacao
1. Provisionar infraestrutura e cofres de segredo.
2. Publicar configuracoes/segredos sem versionar credenciais em arquivo.
3. Executar migrations do EF Core.
4. Publicar API e frontend.
5. Validar endpoint de saude da API e carregamento da tela de login.
6. Executar bootstrap do Administrador Local da Instancia.
7. Validar logins:
   - `MicrosoftEntraId`;
   - `LocalSgx`.
8. Validar `GET /api/auth/provedores` e auditoria de autenticacao.
9. Rotacionar/remover variaveis `SGX_ADMIN_INICIAL_*`.

## Hardening minimo obrigatorio
- TLS fim a fim;
- segredos fora do repositorio e com rotacao periodica;
- chave de assinatura JWT local com rotacao planejada;
- politicas de senha/lockout habilitadas;
- monitoramento de eventos de login negado e acesso negado;
- revisao periodica de perfis administrativos no SGX.

## Validacao pos-implantacao
- executar `docs/HOMOLOGACAO-AUTENTICACAO.md`;
- executar `docs/RUNBOOK-AUTENTICACAO.md`;
- anexar evidencias da rodada (prints e respostas anonimizadas).

## Criterio de aceite
- checklist de homologacao 100% concluido;
- contingencia `LocalSgx` comprovada;
- auditoria de autenticacao operacional e restrita por permissao.
