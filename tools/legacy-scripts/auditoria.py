import re
import os
import math

SEED_FILE = 'src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs'
TEST_FILE = 'tests/SGX.SistemaChamado.Tests/RoadmapSprint7GerenciamentoRequisicoesChecklistTests.cs'
DOC_FILE = 'docs/roadmap/sprint-7-gerenciamento-requisicoes.md'
ROADMAP_MD = 'docs/ROADMAP.md'
ROADMAP_ITSM = 'docs/ROADMAP-ITSM.md'

# --- 1. Fix SeedData.cs ---
with open(SEED_FILE, 'r', encoding='utf-8-sig') as f:
    c = f.read()

lines = c.split('\n')
for j in range(len(lines)):
    if 'RoadmapItemId = RoadmapItsmItem18Id' in lines[j] and 'Ordem =' in lines[j]:
        m = re.search(r'Ordem = (\d+)', lines[j])
        if m:
            ordem = int(m.group(1))
            if ordem in [10, 13, 14, 37, 38, 39]:
                lines[j] = re.sub(r'Concluido = (false|true)', 'Concluido = false', lines[j])
            else:
                lines[j] = re.sub(r'Concluido = (false|true)', 'Concluido = true', lines[j])

c = '\n'.join(lines)

# And percentual to 85%
c = re.sub(
    r'PercentualImplementacao = \d+,\n            PendenciasTecnicas =',
    r'PercentualImplementacao = 85,\n            PendenciasTecnicas =',
    c
)

with open(SEED_FILE, 'w', encoding='utf-8-sig') as f:
    f.write(c)


# --- 2. Fix Test Array ---
with open(TEST_FILE, 'r', encoding='utf-8-sig') as f:
    tc = f.read()

# Quantities: 33 true, 6 false, total 39.
tc = re.sub(
    r'Assert\.Equal\(\d+, checklistAtivo\.Count\(x => x\.Concluido\)\);',
    r'Assert.Equal(33, checklistAtivo.Count(x => x.Concluido));',
    tc
)

tc = re.sub(
    r'Assert\.Equal\(\d+, checklistAtivo\.Count\(x => !x\.Concluido\)\);',
    r'Assert.Equal(6, checklistAtivo.Count(x => !x.Concluido));',
    tc
)

tc = re.sub(
    r'Assert\.Equal\(\d+, detalhe\.QuantidadeChecklistConcluido\);',
    r'Assert.Equal(33, detalhe.QuantidadeChecklistConcluido);',
    tc
)
tc = re.sub(
    r'Assert\.Equal\(\d+, detalhe\.PercentualImplementacao\);',
    r'Assert.Equal(85, detalhe.PercentualImplementacao);',
    tc
)

# Fix test array to remove 10, 13, 14
arr = [x for x in range(1, 37) if x not in [10, 13, 14]]
arr_str = 'new[] { ' + ', '.join(str(x) for x in arr) + ' }'
tc = re.sub(
    r'new\[\] \{ [0-9, ]+ \}',
    arr_str,
    tc
)

# And remove expected title lines if they exist, but the expected title lines don't matter as long as they correspond to 1..36.
# Wait, the expected titles are for specific items! Let's just leave the titles array as is unless the test fails. The titles correspond to the FIRST 7 items that are missing! 
# No, the titles check the expected output of `ProximaAcao`.
# Let's fix the percent expected in the test:
tc = re.sub(
    r'Assert\.Equal\(\d+, percentualEsperado\);',
    r'Assert.Equal(85, percentualEsperado);',
    tc
)
# And the manual calculation:
tc = re.sub(
    r'Math\.Round\(\(36 \* 100\.0\) \/ 39',
    r'Math.Round((33 * 100.0) / 39',
    tc
)

with open(TEST_FILE, 'w', encoding='utf-8-sig') as f:
    f.write(tc)

# --- 3. Fix Markdown Docs ---
with open(DOC_FILE, 'r', encoding='utf-8') as f:
    doc = f.read()

doc = re.sub(r'- \[x\] 10\.', r'- [ ] 10.', doc)
doc = re.sub(r'- \[x\] 13\.', r'- [ ] 13.', doc)
doc = re.sub(r'- \[x\] 14\.', r'- [ ] 14.', doc)

doc = re.sub(r'\d+% \(', r'85% (', doc)
doc = re.sub(r'\d+/\d+ itens', r'33/39 itens', doc)
doc = re.sub(r'36/39', r'33/39', doc)
with open(DOC_FILE, 'w', encoding='utf-8') as f:
    f.write(doc)

with open(ROADMAP_MD, 'r', encoding='utf-8') as f:
    r1 = f.read()
r1 = re.sub(r'Sprint 7 - Gerenciamento de Requisicoes \(\d+%', r'Sprint 7 - Gerenciamento de Requisicoes (85%', r1)
with open(ROADMAP_MD, 'w', encoding='utf-8') as f:
    f.write(r1)

with open(ROADMAP_ITSM, 'r', encoding='utf-8') as f:
    r2 = f.read()
r2 = re.sub(r'Sprint 7 - Gerenciamento de Requisicoes \(\d+%', r'Sprint 7 - Gerenciamento de Requisicoes (85%', r2)
with open(ROADMAP_ITSM, 'w', encoding='utf-8') as f:
    f.write(r2)

