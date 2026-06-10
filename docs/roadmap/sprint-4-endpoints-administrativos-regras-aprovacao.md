# Sprint 4 - Endpoints Administrativos de Regras de Aprovação

## Objetivo
Criar a API administrativa para gerenciar as configurações das regras de aprovação. Estes endpoints permitem a manutenção completa (CRUD e ativação) das regras, mas não executam qualquer fluxo de aprovação ou operação de negócio em chamados.

## Limites desta etapa
Este item é estritamente de API administrativa. Não há implementação de interface gráfica (frontend), serviço de frontend ou endpoints de decisão e pendência operacional. Nenhuma aprovação de chamado ou bloqueio de movimentação ocorre nesta etapa.

## Contexto
Reutilizamos os contratos já desenvolvidos (`ListarConfiguracoesRegrasAprovacaoRequest`, `CriarConfiguracaoRegraAprovacaoRequest`, `AtualizarConfiguracaoRegraAprovacaoRequest`, `ConfiguracaoRegraAprovacaoResponse`, etc.) e a camada de serviço `IAdminConfiguracaoRegraAprovacaoUseCases`, que centraliza toda a lógica de negócio administrativa.

## Controller Criado
Foi criado o `AdminConfiguracoesRegrasAprovacaoController` seguindo o padrão de controllers administrativos do projeto.

## Rotas Criadas
- `GET /api/admin/regras-aprovacao`: Listagem de regras.
- `GET /api/admin/regras-aprovacao/{id}`: Consulta detalhada.
- `POST /api/admin/regras-aprovacao`: Criação de uma nova regra.
- `PUT /api/admin/regras-aprovacao/{id}`: Atualização completa.
- `PATCH /api/admin/regras-aprovacao/{id}/status`: Ativação/inativação de regra.
- `POST /api/admin/regras-aprovacao/validar`: Validação prévia de regra.
- `POST /api/admin/regras-aprovacao/candidatas`: Listagem de regras candidatas (simulação).
- `POST /api/admin/regras-aprovacao/avaliar`: Avaliação da melhor regra aplicável (simulação).

## Padrão de Autorização Aplicado
O controller utiliza a policy `AdminOuAtendente` em sua base. Endpoints de leitura utilizam `PermissionPolicies.AprovacaoChamadosVisualizar`. Endpoints de gravação exigem o perfil `Administrador` e a permissão `PermissionPolicies.AprovacaoChamadosGerenciar`.

## Padrão de Retorno e Tratamento de Erros
Foi aplicado o padrão `ExecutarAsync`, que padroniza os retornos:
- Sucesso com retorno `200 OK`.
- Exceções do tipo `UnauthorizedAccessException` mapeadas para `403 Forbid`.
- Exceções `KeyNotFoundException` mapeadas para `404 Not Found`.
- Exceções `ArgumentException` ou `InvalidOperationException` mapeadas para `400 Bad Request`.
- Exceções `ValidationException` (do FluentValidation) são interceptadas pelo `GlobalExceptionMiddleware` da API que padroniza o erro para `400 Bad Request`.

## O que a API Administrativa Permite
Manter e avaliar o conjunto de regras existentes para aprovação de chamados, diagnosticando como as regras se comportam perante diferentes cenários simulados.

## O que a API Administrativa Não Permite
- Criar instâncias de aprovação.
- Intervir no andamento de chamados.
- Aprovar ou rejeitar solicitações.
- Cancelar ou expirar instâncias de aprovação existentes.

## Relações Futuras
- **Item 38 (Geração obrigatória)**: A geração depende destas regras previamente configuradas.
- **Item 39 (Bloqueio)**: O bloqueio depende da identificação de aprovações pendentes provenientes destas regras.
- **Item 40 e 41 (Aprovação e Rejeição)**: Endpoints operacionais irão resolver pendências baseadas no setup configurado na API administrativa.
- **Item 42 (Reavaliação)**: A reavaliação se beneficia do endpoint de simulação para verificar mudanças de dados.
- **Item 44 (Endpoints de decisão)**: A próxima etapa criará os endpoints que executam de fato as aprovações.
- **Item 45 e 48 (Pendências e Frontend)**: Consultas de pendências para aprovadores e telas administrativas utilizarão as APIs criadas.

## Conclusão Técnica e Próxima Etapa
Os endpoints foram expostos de forma coesa, reutilizando a camada de aplicação sem poluir os controllers com regras de domínio ou detalhes de validação explícita desnecessários.
A próxima etapa recomendada é a execução do **Item 44: Criar endpoints de aprovação e rejeição**.
