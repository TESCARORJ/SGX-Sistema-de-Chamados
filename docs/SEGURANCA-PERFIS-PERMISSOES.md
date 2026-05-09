# Seguranca - Perfis e Permissoes

## Objetivo

O modulo de perfis e permissoes controla a autorizacao interna do SGX Sistema de Chamados.

## Autenticacao x autorizacao

- Microsoft Entra ID autentica a identidade.
- SGX Sistema de Chamados autoriza as acoes internas.
- Perfis e permissoes pertencem ao sistema.

## Perfis macro

Administrador:
- gerencia sistema
- gerencia usuarios
- gerencia perfis
- configura permissoes
- acessa administracao completa

Atendente:
- atua no atendimento
- acessa fila
- assume chamados
- interage com solicitantes
- consulta alguns cadastros conforme permissao

Solicitante:
- abre chamados
- acompanha chamados proprios
- comenta e anexa arquivos

## Permissoes granulares

Permissoes sao codigos estaveis por modulo e acao.

Exemplos:
- Dashboard.Visualizar
- Chamados.Assumir
- Chamados.Encerrar
- Usuarios.Gerenciar
- Perfis.AlterarPermissoes
- Parametros.Gerenciar
- IntegracoesEmail.Visualizar
- Notificacoes.Visualizar

## Matriz de permissoes

- Cada perfil tem permissoes agrupadas por modulo.
- Administrador com permissao adequada pode alterar.
- Atendente pode visualizar quando permitido.
- Permissoes criticas recebem destaque visual.

## GET /api/me

Formato de retorno:

```json
{
  "id": "...",
  "nome": "...",
  "email": "...",
  "perfis": ["Atendente"],
  "permissoes": ["Chamados.Visualizar", "Chamados.Assumir"]
}
```

Regras:
- Permissoes retornadas sao efetivas.
- Multiplos perfis sao unidos.
- Permissoes duplicadas sao removidas.
- Perfis/permissoes inativos sao ignorados.

## Backend

- `PermissionRequirement`
- `PermissionAuthorizationHandler`
- `PermissionPolicyProvider`
- Policies antigas mantidas por compatibilidade.
- Padrao de policy dinamica: `Permissao:{Codigo}`.

## Frontend

- `AuthStore` armazena permissoes.
- Helpers:
  - `possuiPermissao`
  - `possuiAlgumaPermissao`
  - `possuiTodasPermissoes`
- Frontend oculta/desabilita acoes conforme permissao.
- Backend continua sendo a fonte real de seguranca.

## Bloqueios e cuidados

- Nao deixar sistema sem administrador funcional.
- Cuidado com permissoes criticas.
- Futuras auditorias devem registrar alteracoes.
- Em producao, permissoes devem ser revisadas antes da homologacao.

## Como testar

1. Entrar como Administrador.
2. Abrir `/admin/cadastros/perfis`.
3. Abrir detalhe de um perfil.
4. Validar matriz de permissoes.
5. Alterar permissoes.
6. Salvar.
7. Emular Atendente.
8. Verificar acoes visiveis.
9. Emular Solicitante.
10. Confirmar bloqueio ao admin.
