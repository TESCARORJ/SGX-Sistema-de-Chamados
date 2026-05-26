# Configuracao Active Directory - SGX Sistema de Chamados

## Objetivo
Habilitar autenticacao corporativa on-premises via LDAP/LDAPS com emissao de JWT interno do SGX.

## Regra central
- AD autentica.
- SGX autoriza por perfis e permissoes internos.

## Configuracoes

```ini
ActiveDirectory__Servidor=ldaps://dc01.empresa.local
ActiveDirectory__Porta=636
ActiveDirectory__UsarLdaps=true
ActiveDirectory__PermitirLdapSemTls=false
ActiveDirectory__Dominio=EMPRESA
ActiveDirectory__BaseDn=DC=empresa,DC=local
ActiveDirectory__UserSearchFilter=(&(objectClass=user)(sAMAccountName={0}))
ActiveDirectory__PermitirAutoProvisionamento=false
ActiveDirectory__PerfilPadrao=Solicitante
```

## Observacoes de seguranca
- preferir LDAPS em producao;
- LDAP sem TLS so deve ser usado com `ActiveDirectory__PermitirLdapSemTls=true`;
- senha de dominio e usada apenas no bind, sem persistencia;
- senha nunca deve ser registrada em log.

## Habilitacao do provedor
O endpoint `POST /api/auth/ad/login` exige `ActiveDirectory` habilitado em `Authentication__Provedores__Habilitados`.

Exemplo:

```ini
Authentication__Provedores__Configurados__0=ActiveDirectory
Authentication__Provedores__Habilitados__0=ActiveDirectory
Authentication__Provedores__Principal=ActiveDirectory
```

Na Sprint 4, a habilitacao efetiva pode ser sobreposta pela configuracao administrativa persistida em:
- `GET /api/admin/autenticacao/provedores`
- `PUT /api/admin/autenticacao/provedores`

Ou seja:
- configuracao tecnica define o baseline;
- configuracao administrativa define o estado efetivo para exibicao no login e para ativacao operacional do provedor.

## Endpoint de login AD
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

## Auto provisionamento
- quando `ActiveDirectory__PermitirAutoProvisionamento=true`, o SGX cria usuario interno com `ActiveDirectory__PerfilPadrao`;
- grupos AD nao promovem automaticamente para Administrador.

Com gestao administrativa de metodos de login ativa:
- o auto provisionamento AD pode ser ligado/desligado pela administracao;
- o perfil padrao pode ser configurado administrativamente;
- perfil `Administrador` continua proibido para atribuicao automatica.

## Contingencia
- login local SGX continua como contingencia administrativa se habilitado;
- `LocalDevelopment` continua exclusivo de `Development`.
