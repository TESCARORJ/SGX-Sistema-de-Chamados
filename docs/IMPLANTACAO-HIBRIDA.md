# Implantacao Hibrida - Autenticacao SGX

## Objetivo
Padronizar a implantacao da autenticacao do SGX quando a organizacao opera identidade cloud e on-premises simultaneamente.

## Arquitetura de referencia
- SGX hospedado em cloud ou datacenter neutro;
- Microsoft Entra ID para usuarios cloud;
- Active Directory para usuarios de rede local;
- `LocalSgx` como contingencia administrativa comum.

## Pre-requisitos
- conectividade segura SGX -> AD (VPN, private link ou peering);
- aplicacao registrada no Entra ID;
- definicao corporativa de prioridade entre provedores;
- equipe de identidade (cloud + on-prem) envolvida na homologacao.

## Estrategia operacional de autenticacao
- habilitar `MicrosoftEntraId`, `ActiveDirectory` e `LocalSgx`;
- definir provedor principal conforme estrategia corporativa;
- manter ordem de exibicao clara na tela de login;
- governar metodos em `Admin > Integracoes > Autenticacao > Metodos de login`.

## Configuracao base
- aplicar requisitos dos documentos:
  - `docs/IMPLANTACAO-CLOUD.md`
  - `docs/IMPLANTACAO-ON-PREMISES.md`
- validar que `LocalSgx` nunca seja desabilitado sem alternativa administrativa viavel.

## Sequencia de implantacao
1. Publicar SGX com segredos em cofre seguro.
2. Configurar e validar Microsoft Entra ID.
3. Configurar e validar Active Directory (preferencialmente LDAPS).
4. Executar bootstrap do Administrador Local da Instancia.
5. Ajustar metodos de login (habilitacao, principal e ordem).
6. Validar `GET /api/auth/provedores`.
7. Homologar fluxos:
   - login Microsoft;
   - login AD;
   - login local SGX;
   - auditoria de autenticacao.

## Riscos e mitigacoes
- indisponibilidade temporaria de conector externo:
  - manter `LocalSgx` ativo e testado;
- configuracao insegura de metodos:
  - usar bloqueios nativos que impedem estado sem acesso viavel;
- divergencia de perfil entre origens:
  - reforcar que autorizacao e sempre interna no SGX;
- provisionamento indevido de privilegio:
  - manter bloqueio de perfil `Administrador` no auto provisionamento.

## Validacao pos-implantacao
- executar os 20 itens de `docs/HOMOLOGACAO-AUTENTICACAO.md`;
- executar `docs/RUNBOOK-AUTENTICACAO.md`;
- emitir ata unica de homologacao hibrida com evidencias dos 3 fluxos.

## Criterio de aceite
- operacao comprovada em `MicrosoftEntraId`, `ActiveDirectory` e `LocalSgx`;
- auditoria rastreando sucesso/falha/bloqueio/negado;
- contingencia administrativa preservada.
