# Evidencias de Homologacao - Base de Conhecimento

## Objetivo

Organizar o registro formal das evidencias de validacao funcional da Base de Conhecimento, sem obrigatoriedade imediata de imagens nesta etapa.

## Como usar

Para cada cenario validado, preencher os campos abaixo e anexar prints quando disponiveis.

Campos obrigatorios por cenario:
- Cenario
- Perfil utilizado
- Resultado esperado
- Resultado obtido
- Status (Aprovado / Aprovado com ressalvas / Reprovado)
- Observacoes
- Caminho da tela ou endpoint
- Data da validacao

## Template

```md
### Cenario: <nome do cenario>
- Perfil utilizado:
- Resultado esperado:
- Resultado obtido:
- Status:
- Observacoes:
- Caminho da tela ou endpoint:
- Data da validacao:
- Evidencias (prints/arquivos):
```

## Cenarios sugeridos para preenchimento

1. Administrador cria artigo.
2. Administrador edita artigo.
3. Administrador publica artigo.
4. Administrador arquiva artigo.
5. Administrador reativa artigo.
6. Solicitante visualiza artigo publicado no portal.
7. Solicitante nao visualiza artigo interno.
8. Atendente visualiza artigo permitido.
9. Administrador visualiza artigo administrativo.
10. Busca no portal por titulo e tags.
11. Busca administrativa por status e visibilidade.
12. Vinculo de artigo publicado ao chamado.
13. Bloqueio de vinculo de artigo arquivado/inativo.
14. Bloqueio de vinculo duplicado.
15. Exibicao de artigo vinculado no detalhe do chamado.
16. Remocao de vinculo.
17. Historico do chamado registra vinculo/remocao.
18. Auditoria registra acoes administrativas.
