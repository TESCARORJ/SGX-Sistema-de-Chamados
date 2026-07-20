# Scripts legados (descontinuados)

Estes 22 scripts Python foram usados para editar `SeedData.cs`, migrations e
os documentos de roadmap (`docs/ROADMAP.md`, `docs/ROADMAP-ITSM.md`,
`docs/roadmap/*.md`) diretamente via regex/substituicao de texto, sem
revisao de diff estruturado por item de checklist.

Alem desses, ha mais 4 arquivos arquivados aqui de uma natureza um pouco
diferente:

- `fix_seed.csx` — script C# (nao Python) para edicao de seed.
- `seed_diff.txt`, `latest_diff.txt`, `seed_edits.txt` — artefatos de
  diff/anotacao de uma sessao especifica de correcao de seed, do commit
  `4174ad8` ("feat(notificacoes): integrar eventos ITSM e preparar
  homologacao da Sprint 6").

Esses 4 arquivos escaparam da varredura original de 15/07/2026 por serem
de extensao diferente (`.csx`/`.txt`, nao `.py`) e ficaram soltos na raiz
do repositorio. Mesma causa raiz e mesma politica dos 22 scripts Python:
edicao de seed fora de processo revisavel, descontinuada.

## Por que foram descontinuados

Esse padrao de edicao causou a divergencia da Sprint 7: os scripts
alteravam `Concluido`, `PercentualImplementacao` e `ProximaAcao` no seed
sem validar se a mudanca correspondia a uma entrega real (ou a ausencia
dela) comprovada por teste comportamental. O resultado foi um checklist
que nao refletia o estado real do codigo, alem de heranca de formatacao
corrompida em `docs/ROADMAP.md` decorrente de substituicoes de texto mal
segmentadas.

## Politica atual

Alteracoes em `SeedData.cs`, migrations e nos documentos de roadmap devem
ser feitas por edicao direta e revisada dos arquivos, citando o teste
comportamental que prova cada item marcado como concluido — nunca por
script solto de regex sobre o repositorio.

Os scripts foram mantidos aqui apenas para referencia historica. Nao
devem ser executados novamente.
