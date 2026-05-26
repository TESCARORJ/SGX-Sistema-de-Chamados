# Implantacao On-Premises - Autenticacao SGX

## Objetivo
Padronizar a implantacao da autenticacao do SGX em datacenter do cliente com integracao ao Active Directory local.

## Arquitetura de referencia
- API SGX hospedada on-premises;
- frontend SGX interno ou DMZ corporativa;
- PostgreSQL local ou gerenciado pelo cliente;
- Active Directory via LDAP/LDAPS;
- `LocalSgx` habilitado como contingencia administrativa obrigatoria.

## Pre-requisitos
- conectividade de rede SGX -> controladores AD;
- portas LDAP/LDAPS liberadas conforme politica;
- certificado para LDAPS validado no host da API;
- massa de usuarios AD para homologacao.

## Variaveis minimas recomendadas

### Core da autenticacao
- `Authentication__Provedores__Configurados`
- `Authentication__Provedores__Habilitados`
- `Authentication__Provedores__Principal`
- `Authentication__JwtLocalIssuer`
- `Authentication__JwtLocalAudience`
- `Authentication__JwtLocalChaveAssinatura`
- `Authentication__Lockout__TentativasMaximas`
- `Authentication__Lockout__MinutosBloqueio`

### Active Directory
- `ActiveDirectory__Servidor`
- `ActiveDirectory__Porta`
- `ActiveDirectory__UsarLdaps`
- `ActiveDirectory__PermitirLdapSemTls`
- `ActiveDirectory__Dominio`
- `ActiveDirectory__BaseDn`
- `ActiveDirectory__UserSearchFilter`
- `ActiveDirectory__PermitirAutoProvisionamento`
- `ActiveDirectory__PerfilPadrao`

### Bootstrap administrativo
- `SGX_ADMIN_INICIAL_EMAIL`
- `SGX_ADMIN_INICIAL_SENHA`
- `SGX_ADMIN_INICIAL_NOME`

## Sequencia de implantacao
1. Validar conectividade SGX -> AD.
2. Priorizar `ActiveDirectory__UsarLdaps=true`.
3. Configurar `LocalSgx` como contingencia ativa.
4. Publicar configuracoes e aplicar migrations.
5. Publicar API/frontend.
6. Executar bootstrap do Administrador Local da Instancia.
7. Validar `POST /api/auth/ad/login` com usuario de dominio.
8. Validar cenarios de auto provisionamento ligado/desligado.
9. Validar auditoria de autenticacao e restricao por permissao.

## Hardening minimo obrigatorio
- evitar LDAP sem TLS (permitir apenas com aprovacao formal);
- nao persistir senha AD no SGX;
- bloquear provisionamento automatico com perfil `Administrador`;
- revisar usuarios internos inativos periodicamente;
- manter runbook de contingencia local testado.

## Validacao pos-implantacao
- executar checklist completo `docs/HOMOLOGACAO-AUTENTICACAO.md`;
- executar runbook `docs/RUNBOOK-AUTENTICACAO.md`;
- anexar evidencias institucionais de AD/contingencia/auditoria.

## Criterio de aceite
- itens 10 a 14 do checklist validados integralmente;
- `LocalSgx` funcional mesmo com AD indisponivel;
- eventos de autenticacao persistidos e consultaveis com permissao.
