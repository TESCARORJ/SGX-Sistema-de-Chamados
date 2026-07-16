import re

# docs/ROADMAP.md
with open('docs/ROADMAP.md', 'r', encoding='utf-8') as f:
    c = f.read()
c = re.sub(r'7\. Sprint 7 - Gerenciamento de Requisicoes \(90% - Em desenvolvimento\)', '7. Sprint 7 - Gerenciamento de Requisicoes (92% - Em desenvolvimento)', c)
with open('docs/ROADMAP.md', 'w', encoding='utf-8') as f:
    f.write(c)

# docs/ROADMAP-ITSM.md
with open('docs/ROADMAP-ITSM.md', 'r', encoding='utf-8') as f:
    c = f.read()
c = re.sub(r'7\. Sprint 7 - Gerenciamento de Requisicoes \(90% - Em desenvolvimento\)', '7. Sprint 7 - Gerenciamento de Requisicoes (92% - Em desenvolvimento)', c)
c = re.sub(r'\| Sprint 7 - Gerenciamento de Requisicoes \| ITIL/ITSM \| Em desenvolvimento \| Parcial \| 90% \|', '| Sprint 7 - Gerenciamento de Requisicoes | ITIL/ITSM | Em desenvolvimento | Parcial | 92% |', c)
with open('docs/ROADMAP-ITSM.md', 'w', encoding='utf-8') as f:
    f.write(c)

# docs/roadmap/sprint-7-gerenciamento-requisicoes.md
with open('docs/roadmap/sprint-7-gerenciamento-requisicoes.md', 'r', encoding='utf-8') as f:
    c = f.read()
c = re.sub(r'Percentual recalculado: `90%`', 'Percentual recalculado: `92%`', c)
c = re.sub(r'Checklist concluido: `35`', 'Checklist concluido: `36`', c)
c = re.sub(r'(\n- \[ \] 35\. .*)', lambda m: m.group(1).replace('- [ ]', '- [x]'), c)
with open('docs/roadmap/sprint-7-gerenciamento-requisicoes.md', 'w', encoding='utf-8') as f:
    f.write(c)

# SeedData.cs
with open('src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs', 'r', encoding='utf-8-sig') as f:
    c = f.read()
c = c.replace(
    'PercentualImplementacao = 90, ProximaAcao = "Criar migration de dados ou checklist, se aplicavel"',
    'PercentualImplementacao = 92, ProximaAcao = "Registrar homologacao funcional"'
)
c = c.replace(
    'Titulo = "Registrar aceite formal somente com evidencia", Descricao = "Sprint 7 Gerenciamento de Requisicoes", Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 39, Concluido = false',
    'Titulo = "Registrar aceite formal somente com evidencia", Descricao = "Sprint 7 Gerenciamento de Requisicoes", Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 39, Concluido = true'
)
with open('src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs', 'w', encoding='utf-8-sig') as f:
    f.write(c)

# Tests
with open('tests/SGX.SistemaChamado.Tests/RoadmapSprint7GerenciamentoRequisicoesChecklistTests.cs', 'r', encoding='utf-8') as f:
    c = f.read()
c = c.replace('Assert.Equal(90, item.PercentualImplementacao);', 'Assert.Equal(92, item.PercentualImplementacao);')
c = c.replace('Assert.Equal(90, detalhe.PercentualImplementacao);', 'Assert.Equal(92, detalhe.PercentualImplementacao);')
c = c.replace('Assert.Equal(35, detalhe.QuantidadeChecklistConcluido);', 'Assert.Equal(36, detalhe.QuantidadeChecklistConcluido);')
c = c.replace('Assert.Equal(90, percentualEsperado);', 'Assert.Equal(92, percentualEsperado);')
c = c.replace('34, 35, 36, 37, 38', '34, 35, 36, 37, 38, 39')
c = c.replace('var percentualEsperado = (int)Math.Round((35 * 100.0) / 39, MidpointRounding.AwayFromZero);', 'var percentualEsperado = (int)Math.Round((36 * 100.0) / 39, MidpointRounding.AwayFromZero);')
c = c.replace('"Criar migration de dados ou checklist, se aplicavel"', '"Registrar homologacao funcional"')

with open('tests/SGX.SistemaChamado.Tests/RoadmapSprint7GerenciamentoRequisicoesChecklistTests.cs', 'w', encoding='utf-8') as f:
    f.write(c)
