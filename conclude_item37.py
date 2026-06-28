import re
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
            if ordem == 37:
                lines[j] = re.sub(r'Concluido = (false|true)', 'Concluido = true', lines[j])

c = '\n'.join(lines)

def update_item18(text):
    parts = text.split('RoadmapItsmItem18Id,')
    if len(parts) > 1:
        sub_part = parts[1]
        sub_part = re.sub(
            r'PercentualImplementacao = 85',
            r'PercentualImplementacao = 87',
            sub_part,
            count=1
        )
        sub_part = re.sub(
            r'ProximaAcao = "Registrar homologacao funcional"',
            r'ProximaAcao = "Registrar passagem de conhecimento da governanca"',
            sub_part,
            count=1
        )
        parts[1] = sub_part
    return 'RoadmapItsmItem18Id,'.join(parts)

c = update_item18(c)

with open(SEED_FILE, 'w', encoding='utf-8-sig') as f:
    f.write(c)


# --- 2. Fix Test Array ---
with open(TEST_FILE, 'r', encoding='utf-8-sig') as f:
    tc = f.read()

tc = re.sub(
    r'Assert\.Equal\(33, checklistAtivo\.Count\(x => x\.Concluido\)\);',
    r'Assert.Equal(34, checklistAtivo.Count(x => x.Concluido));',
    tc
)

tc = re.sub(
    r'Assert\.Equal\(6, checklistAtivo\.Count\(x => !x\.Concluido\)\);',
    r'Assert.Equal(5, checklistAtivo.Count(x => !x.Concluido));',
    tc
)

tc = re.sub(
    r'Assert\.Equal\(33, detalhe\.QuantidadeChecklistConcluido\);',
    r'Assert.Equal(34, detalhe.QuantidadeChecklistConcluido);',
    tc
)
tc = re.sub(
    r'Assert\.Equal\(85, detalhe\.PercentualImplementacao\);',
    r'Assert.Equal(87, detalhe.PercentualImplementacao);',
    tc
)
tc = re.sub(
    r'Assert\.Equal\(85, item\.PercentualImplementacao\);',
    r'Assert.Equal(87, item.PercentualImplementacao);',
    tc
)
tc = re.sub(
    r'Assert\.Equal\(\s*"Registrar homologacao funcional",\s*RemoverAcentos\(item\.ProximaAcao\)\);',
    r'Assert.Equal(\n            "Registrar passagem de conhecimento da governanca",\n            RemoverAcentos(item.ProximaAcao));',
    tc
)
tc = re.sub(
    r'Assert\.Equal\(85, percentualEsperado\);',
    r'Assert.Equal(87, percentualEsperado);',
    tc
)
tc = re.sub(
    r'Math\.Round\(\(33 \* 100\.0\) \/ 39',
    r'Math.Round((34 * 100.0) / 39',
    tc
)

# Fix test array to remove 10, 13, 14, 38, 39
# Wait, the array in test was `1..36` excluding 10,13,14! Now it should be `1..37` excluding 10,13,14!
arr = [x for x in range(1, 38) if x not in [10, 13, 14]]
arr_str = 'new[] { ' + ', '.join(str(x) for x in arr) + ' }'
tc = re.sub(
    r'new\[\] \{ [0-9, ]+ \}',
    arr_str,
    tc,
    count=1
)

with open(TEST_FILE, 'w', encoding='utf-8-sig') as f:
    f.write(tc)


# --- 3. Fix Markdown Docs ---
with open(DOC_FILE, 'r', encoding='utf-8') as f:
    doc = f.read()

doc = re.sub(r'- \[ \] 37\.', r'- [x] 37.', doc)

doc = re.sub(r'85% \(', r'87% (', doc)
doc = re.sub(r'33/39 itens', r'34/39 itens', doc)
with open(DOC_FILE, 'w', encoding='utf-8') as f:
    f.write(doc)

with open(ROADMAP_MD, 'r', encoding='utf-8') as f:
    r1 = f.read()
r1 = re.sub(r'Sprint 7 - Gerenciamento de Requisicoes \(85%', r'Sprint 7 - Gerenciamento de Requisicoes (87%', r1)
with open(ROADMAP_MD, 'w', encoding='utf-8') as f:
    f.write(r1)

with open(ROADMAP_ITSM, 'r', encoding='utf-8') as f:
    r2 = f.read()
r2 = re.sub(r'Sprint 7 - Gerenciamento de Requisicoes \(85%', r'Sprint 7 - Gerenciamento de Requisicoes (87%', r2)
with open(ROADMAP_ITSM, 'w', encoding='utf-8') as f:
    f.write(r2)

