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
