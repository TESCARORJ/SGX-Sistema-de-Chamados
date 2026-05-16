# Atendimento - Comentarios, Anexos e Linha do Tempo

## Objetivo

Garantir comunicacao e evidencias do atendimento de chamados com comentarios e anexos, respeitando perfis e rastreabilidade.

## Perfis e regras

- `Administrador`
  - visualiza e envia comentarios/anexos em qualquer chamado permitido pelo sistema;
  - visualiza comentarios internos e publicos.
- `Atendente`
  - visualiza e envia comentarios/anexos em chamados acessiveis para atendimento;
  - visualiza comentarios internos e publicos.
- `Solicitante`
  - visualiza e envia comentarios/anexos apenas nos proprios chamados;
  - nao visualiza comentarios internos;
  - nao cria comentario interno.

## Comentarios

Regras:
- comentario pertence ao chamado;
- mensagem obrigatoria;
- limite de `4000` caracteres;
- ordenacao cronologica crescente;
- sem edicao e sem exclusao nesta fase.

Endpoints:
- `GET /api/chamados/{chamadoId}/comentarios`
- `POST /api/chamados/{chamadoId}/comentarios`

## Anexos

Regras:
- anexo pertence ao chamado;
- upload exige arquivo valido e nao vazio;
- validacao de tamanho maximo por configuracao `Arquivos.TamanhoMaximoBytes`;
- validacao de extensoes permitidas por `Arquivos.ExtensoesPermitidas`;
- bloqueio de extensoes perigosas por `Arquivos.ExtensoesBloqueadas`;
- nome fisico armazenado e gerado com identificador seguro, preservando apenas extensao;
- caminho fisico e nome armazenado nao sao expostos na API;
- download valida acesso ao chamado antes de abrir arquivo;
- se arquivo fisico nao existir, retorna `404` sem expor caminho interno.

Regra central de rastreabilidade:
- **apos salvo, anexo nao pode ser excluido por nenhum perfil**;
- **nao existe endpoint DELETE para anexos**;
- **nao existe botao de exclusao de anexo na interface**.

Endpoints:
- `GET /api/chamados/{chamadoId}/anexos`
- `POST /api/chamados/{chamadoId}/anexos` (`multipart/form-data`, campo `arquivo`)
- `GET /api/chamados/{chamadoId}/anexos/{anexoId}/download`

## Extensoes permitidas

- `.pdf`
- `.png`
- `.jpg`
- `.jpeg`
- `.doc`
- `.docx`
- `.xls`
- `.xlsx`
- `.txt`
- `.csv`
- `.zip`

## Extensoes bloqueadas

- `.exe`
- `.bat`
- `.cmd`
- `.ps1`
- `.sh`
- `.js`
- `.vbs`
- `.msi`
- `.dll`
- `.scr`
- `.com`
- `.jar`
- `.hta`
- `.reg`

## Exemplo de upload

Request:

```http
POST /api/chamados/{chamadoId}/anexos
Content-Type: multipart/form-data
```

Campo:
- `arquivo`

Response `200`:

```json
{
  "id": "f8d75ce2-0eb1-4a3e-af84-b3acff80e9d4",
  "nomeArquivo": "evidencia.pdf",
  "contentType": "application/pdf",
  "tamanhoBytes": 20480,
  "criadoEm": "2026-05-15T16:10:00Z",
  "usuarioId": "d7e714a8-1f02-44ea-bce6-06ecce2255d3",
  "usuario": "Solicitante SGX"
}
```

## Exemplo de listagem de anexos

Response `200`:

```json
[
  {
    "id": "f8d75ce2-0eb1-4a3e-af84-b3acff80e9d4",
    "nomeArquivo": "evidencia.pdf",
    "contentType": "application/pdf",
    "tamanhoBytes": 20480,
    "criadoEm": "2026-05-15T16:10:00Z",
    "usuarioId": "d7e714a8-1f02-44ea-bce6-06ecce2255d3",
    "usuario": "Solicitante SGX"
  }
]
```

## Criterios de aceite

- upload, listagem e download funcionando para usuarios autorizados;
- usuarios sem acesso ao chamado nao conseguem operar anexos;
- API nao expoe `Caminho` nem `NomeArquivoArmazenado`;
- extensoes perigosas bloqueadas;
- arquivo vazio bloqueado;
- limite de tamanho respeitado;
- interface de detalhe permite upload/download e nao mostra exclusao;
- nenhum endpoint DELETE de anexo exposto.

## Registro de implementacao consolidada

- Sprint Comentarios no Atendimento: concluida.
- Sprint Anexos no Atendimento: concluida.
- Migration de comentarios: `AddComentariosAtendimento`.
- Migration de anexos: `AddAnexosAtendimento`.
- Resultado consolidado: item de roadmap "Comentarios e anexos" atualizado para implementado funcionalmente, tecnico completo e 100%.

## Linha do Tempo do Atendimento

Objetivo:
- consolidar eventos relevantes do chamado em uma sequencia unica para rastreabilidade do ciclo de vida.

Perfis:
- `Administrador`: visualiza eventos publicos e internos.
- `Atendente`: visualiza eventos publicos e internos.
- `Solicitante`: visualiza apenas eventos publicos do proprio chamado.

Regras de visibilidade:
- `Solicitante` nao visualiza comentarios internos.
- `Solicitante` nao visualiza eventos tecnicos/internos (ex.: alteracoes administrativas sensiveis).
- `Solicitante` visualiza abertura, comentarios publicos, anexos, alteracao de status, encerramento e reabertura.
- anexos continuam sem exclusao por nenhum perfil apos upload.

Eventos exibidos:
- abertura;
- comentario publico/interno;
- anexo adicionado;
- alteracao de status;
- alteracao de responsavel;
- alteracao de prioridade;
- alteracao de categoria;
- encerramento;
- reabertura;
- historico tecnico elegivel por perfil.

Endpoint:
- `GET /api/chamados/{chamadoId}/linha-do-tempo`

Exemplo de retorno:

```json
{
  "chamadoId": "f8d75ce2-0eb1-4a3e-af84-b3acff80e9d4",
  "codigo": "CHM-2026-0001",
  "items": [
    {
      "id": "f8d75ce2-0eb1-4a3e-af84-b3acff80e9d4",
      "tipo": "abertura",
      "tipoDescricao": "Abertura",
      "dataHora": "2026-05-15T10:00:00Z",
      "usuarioId": "d7e714a8-1f02-44ea-bce6-06ecce2255d3",
      "usuario": "Solicitante SGX",
      "titulo": "Chamado aberto",
      "descricao": "Chamado CHM-2026-0001 aberto.",
      "interno": false,
      "referenciaId": "f8d75ce2-0eb1-4a3e-af84-b3acff80e9d4",
      "referenciaTipo": "chamado"
    },
    {
      "id": "7d8e7f17-629f-44a8-b402-cf404ecf6e95",
      "tipo": "anexo",
      "tipoDescricao": "Anexo",
      "dataHora": "2026-05-15T10:05:00Z",
      "usuarioId": "d7e714a8-1f02-44ea-bce6-06ecce2255d3",
      "usuario": "Solicitante SGX",
      "titulo": "Anexo adicionado",
      "descricao": "evidencia.pdf",
      "interno": false,
      "referenciaId": "7d8e7f17-629f-44a8-b402-cf404ecf6e95",
      "referenciaTipo": "anexo",
      "nomeArquivo": "evidencia.pdf",
      "contentType": "application/pdf",
      "tamanhoBytes": 20480
    }
  ]
}
```

## Comentarios e Anexos no Atendimento

Resumo:
- O modulo de Atendimento permite que usuarios autorizados interajam dentro do chamado por meio de comentarios e anexos.
- A funcionalidade centraliza a comunicacao, registra evidencias e melhora a rastreabilidade do ciclo de vida do chamado.

Perfis envolvidos:
- Administrador
- Atendente
- Solicitante

Status consolidado:
- StatusImplementacao: Implementado funcionalmente
- StatusTecnico: Completo
- PercentualImplementacao: 100%
- Avaliacao: Aprovado
- Pendencia bloqueante: nenhuma

Regras de comentarios:
- Administrador cria e visualiza comentarios publicos e internos.
- Atendente cria e visualiza comentarios publicos e internos.
- Solicitante cria apenas comentarios publicos.
- Solicitante visualiza apenas comentarios publicos.
- Solicitante nao pode criar comentario interno.
- Solicitante nao pode visualizar comentario interno.
- Comentario vazio e rejeitado.
- Comentario tem limite maximo de 4000 caracteres.
- Comentarios sao exibidos em ordem cronologica.

Endpoints de comentarios:
- `GET /api/chamados/{chamadoId}/comentarios`
- `POST /api/chamados/{chamadoId}/comentarios`

Regras de anexos:
- Administrador, Atendente e Solicitante podem anexar arquivos em chamados permitidos.
- Solicitante so pode anexar, listar e baixar anexos dos proprios chamados.
- Administrador pode listar e baixar anexos de qualquer chamado.
- Atendente pode listar e baixar anexos de chamados acessiveis para atendimento.
- Upload rejeita arquivo vazio.
- Upload valida tamanho maximo.
- Upload valida extensoes permitidas.
- Upload bloqueia extensoes perigosas.
- Download respeita permissao de acesso ao chamado.
- API nao expoe caminho fisico do arquivo.
- API nao expoe nome fisico armazenado.
- Storage possui protecao contra path traversal.
- Anexo salvo nao pode ser excluido por nenhum perfil.

Regra obrigatoria sobre exclusao de anexos:
- Apos upload, nenhum perfil pode excluir anexos.
- Nao existe endpoint DELETE de anexos.
- Nao existe botao de exclusao no frontend.
- Nao existe exclusao logica ou fisica de anexos.
- Justificativa: anexos fazem parte da rastreabilidade e podem servir como evidencia do atendimento.

Endpoints de anexos:
- `GET /api/chamados/{chamadoId}/anexos`
- `POST /api/chamados/{chamadoId}/anexos`
- `GET /api/chamados/{chamadoId}/anexos/{anexoId}/download`

Configuracao de arquivos:
- Regras de upload sao controladas por `ArquivosOptions` e `appsettings`.

```json
"Arquivos": {
  "DiretorioBase": "storage/anexos",
  "TamanhoMaximoBytes": 10485760,
  "ExtensoesPermitidas": [ ".pdf", ".png", ".jpg", ".jpeg", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv", ".zip" ],
  "ExtensoesBloqueadas": [ ".exe", ".bat", ".cmd", ".ps1", ".sh", ".js", ".vbs", ".msi", ".dll", ".scr", ".com", ".jar", ".hta", ".reg" ]
}
```

Evidencias tecnicas:
- Comentarios:
- Migration: `20260515154700_AddComentariosAtendimento`
- Endpoints: `GET /api/chamados/{chamadoId}/comentarios` e `POST /api/chamados/{chamadoId}/comentarios`
- Testes backend aprovados
- Testes frontend aprovados
- Build frontend aprovado
- Anexos:
- Migration: `20260515161320_AddAnexosAtendimento`
- Endpoints: `GET /api/chamados/{chamadoId}/anexos`, `POST /api/chamados/{chamadoId}/anexos`, `GET /api/chamados/{chamadoId}/anexos/{anexoId}/download`
- Testes backend aprovados
- Testes frontend aprovados
- Build frontend aprovado

Resultados de validacao:
- `dotnet test` aprovado
- `npm.cmd run test:unit` aprovado
- `npm.cmd run build` aprovado

Checklist da documentacao

Comentarios:
- [x] Endpoint de listagem de comentarios criado.
- [x] Endpoint de criacao de comentario criado.
- [x] Comentario publico implementado.
- [x] Comentario interno implementado.
- [x] Solicitante impedido de criar comentario interno.
- [x] Solicitante impedido de visualizar comentario interno.
- [x] Validacao de mensagem obrigatoria implementada.
- [x] Limite de 4000 caracteres implementado.
- [x] Ordenacao cronologica implementada.
- [x] Tela de detalhe do chamado atualizada.
- [x] Testes de comentarios aprovados.

Anexos:
- [x] Endpoint de listagem de anexos criado.
- [x] Endpoint de upload de anexos criado.
- [x] Endpoint de download de anexos criado.
- [x] Upload com validacao de arquivo vazio implementado.
- [x] Upload com validacao de tamanho implementado.
- [x] Upload com validacao de extensao implementado.
- [x] Bloqueio de extensoes perigosas implementado.
- [x] Storage seguro implementado.
- [x] Protecao contra path traversal implementada.
- [x] Download com permissao implementado.
- [x] API sem exposicao de caminho fisico.
- [x] API sem exposicao de nome fisico armazenado.
- [x] Exclusao de anexos bloqueada para todos os perfis.
- [x] Nenhum endpoint DELETE de anexos criado.
- [x] Nenhum botao de exclusao criado.
- [x] Testes de anexos aprovados.

Governanca:
- [x] docs/ATENDIMENTO.md atualizado.
- [x] docs/ROADMAP.md atualizado.
- [x] docs/ROADMAP-ITSM.md atualizado.
- [x] Documentacao ITSM atualizada.
- [x] Evidencias de testes registradas.
- [x] Status atualizado para Implementado funcionalmente.
- [x] Percentual atualizado para 100%.
- [x] Avaliacao atualizada para Aprovado.
